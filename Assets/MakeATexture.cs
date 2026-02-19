using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MakeATexture : MonoBehaviour{
    private int ID;
    private YoKaiData currentData;

    void Start(){
        ID = int.Parse(System.Text.RegularExpressions.Regex.Match(gameObject.name, @"\d+").Value);

        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/_3D/yokai_assets_mapping");

        if (targetFile != null){
            var allData = JsonConvert.DeserializeObject<Dictionary<string, YoKaiData>>(targetFile.text);
            string searchKey = ID.ToString();
            if (allData.ContainsKey(searchKey)){
                currentData = allData[searchKey];
                Debug.Log($"Modèle trouvé pour l'ID {ID} : {currentData.modelFile}");
            } else {
                Debug.LogError($"L'ID {ID} n'existe pas dans le JSON.");
            }
        }
        ApplyTexture();
    }

    void ApplyTexture(){
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < currentData.textureFiles.Length; i++){
            string path = "yo-kai-API/data/_3D/textures/" + currentData.textureFiles[i].Replace(".png", "");
            Texture2D loadedTexture = Resources.Load<Texture2D>(path);
            if (loadedTexture != null){
                foreach (Renderer rend in childRenderers) {
                    rend.material.mainTexture = loadedTexture;
                }
            } else {
                Debug.LogError("Texture introuvable au chemin : " + path);
            }
        }
    }
}

[System.Serializable]
public class YoKaiData
{
    public string modelFile;
    public string[] textureFiles;
}