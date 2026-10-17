using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using YKW.Core.YoKai.Class;

public class MakeATexture : MonoBehaviour{
    private int ID;
    private YoKaiModele3D currentData;
    private bool hasExplicitMapping;

    public void Initialize(YoKaiModele3D mapping){
        currentData = mapping;
        hasExplicitMapping = true;
        ApplyTexture();
    }

    void Start(){
        ChargerStartTexture();
    }

    public void ChargerStartTexture(){
        // Runtime spawners already resolved the species mapping. Keep it even
        // when the FBX model name differs from the species ID.
        if (hasExplicitMapping) return;
        System.Text.RegularExpressions.Match idMatch =
            System.Text.RegularExpressions.Regex.Match(gameObject.name, @"\d+");
        if (!idMatch.Success || !int.TryParse(idMatch.Value, out ID)) return;

        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/_3D/yokai_assets_mapping");
        if (targetFile == null) return;

        var allData = JsonConvert.DeserializeObject<Dictionary<string, YoKaiModele3D>>(targetFile.text);
        string searchKey = ID.ToString();
        if (allData == null || !allData.TryGetValue(searchKey, out currentData) || currentData == null)
            return;

        ApplyTexture();
        if (gameObject.GetComponent<MakeAShader>() == null) gameObject.AddComponent<MakeAShader>();
        if (gameObject.GetComponent<AddAnimator>() == null) gameObject.AddComponent<AddAnimator>();
    }

    void ApplyTexture(){
        if (currentData == null || currentData.textureFiles == null || currentData.textureFiles.Length == 0) return;
        var textures = new Texture2D[currentData.textureFiles.Length];
        for (int i = 0; i < textures.Length; i++) {
            string path = "yo-kai-API/data/_3D/textures/" +
                System.IO.Path.GetFileNameWithoutExtension(currentData.textureFiles[i]);
            textures[i] = Resources.Load<Texture2D>(path);
            if (textures[i] == null) {
                Sprite sprite = Resources.Load<Sprite>(path);
                if (sprite != null) textures[i] = sprite.texture;
            }
            if (textures[i] == null) Debug.LogError("Yo-kai texture was not found: " + path, this);
        }
        int materialIndex = 0;
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true)) {
            Material[] materials = renderer.materials;
            for (int slot = 0; slot < materials.Length; slot++, materialIndex++) {
                Material material = materials[slot];
                if (material == null) continue;
                int textureIndex = Mathf.Min(materialIndex, textures.Length - 1);
                for (int i = 0; i < textures.Length; i++) {
                    string textureName = System.IO.Path.GetFileNameWithoutExtension(currentData.textureFiles[i]);
                    if (material.name.IndexOf(textureName, System.StringComparison.OrdinalIgnoreCase) >= 0) {
                        textureIndex = i;
                        break;
                    }
                }
                Texture2D texture = textures[textureIndex];
                if (texture == null) continue;
                if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
                if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
                if (material.HasProperty("_DetailTex")) {
                    material.SetFloat("_DetailTex", 1f);
                    material.EnableKeyword("_TEX_ON");
                }
            }
            renderer.materials = materials;
        }
    }
}
