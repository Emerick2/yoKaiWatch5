using UnityEngine;

public class MakeAShader : MonoBehaviour{
    [Header("Mode")]
    public bool choixForcer = false;
    public bool forcerToonShader = false;
    public bool forcerUnlitShader = false;

    [Header("Réglages de l'Outline")]
    public float outlineWidth = 1.0f;
    public Color outlineColor = Color.black;


    void Start(){
        if (choixForcer){
            if (forcerToonShader) {
                ApplyYokaiShader();
            } else if (forcerUnlitShader) {
                ApplyCharacterShader();
            }
        } else {
            string identifier = System.Text.RegularExpressions.Regex.Match(gameObject.name, @"\d+").Value;
            int id = int.TryParse(identifier, out int parsedId) ? parsedId : 0;

            if (id != 0 && gameObject.name.Contains("y")){
                ApplyYokaiShader();
            } else {
                ApplyCharacterShader();
            }
        }
    }

    public void ApplyYokaiShader() {
        Shader shader = Shader.Find("TSF/BaseOutline1");
        if (shader == null) {
            Debug.LogError("Yo-kai shader TSF/BaseOutline1 was not found.", this);
            return;
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true)) {
            Material[] materials = renderer.materials;
            foreach (Material material in materials) {
                if (material == null) continue;
                // URP imports use _BaseMap; TSF reads _MainTex. Preserve the
                // texture before switching shaders rather than relying on mainTexture.
                Texture texture = ReadMainTexture(material);
                material.shader = shader;
                if (texture != null) material.SetTexture("_MainTex", texture);
                material.SetFloat("_DetailTex", 1f);
                material.EnableKeyword("_TEX_ON");
                material.SetColor("_OutlineColor", outlineColor);
                material.SetFloat("_Outline", outlineWidth);
            }
            renderer.materials = materials;
        }
    }

    public void ApplyCharacterShader() {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) {
            Debug.LogError("Character shader Universal Render Pipeline/Unlit was not found.", this);
            return;
        }
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true)) {
            Material[] materials = renderer.materials;
            foreach (Material material in materials) {
                if (material == null) continue;
                Texture texture = ReadMainTexture(material);
                Color color = material.HasProperty("_BaseColor") ? material.GetColor("_BaseColor")
                    : material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
                material.shader = shader;
                if (texture != null) material.SetTexture("_BaseMap", texture);
                material.SetColor("_BaseColor", color);
            }
            renderer.materials = materials;
        }
    }

    private static Texture ReadMainTexture(Material material) {
        Texture texture = material.mainTexture;
        if (texture == null && material.HasProperty("_BaseMap")) texture = material.GetTexture("_BaseMap");
        if (texture == null && material.HasProperty("_MainTex")) texture = material.GetTexture("_MainTex");
        return texture;
    }
}
