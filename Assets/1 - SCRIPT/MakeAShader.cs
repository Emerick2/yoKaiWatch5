using System.Data;
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
            int ID = int.Parse(System.Text.RegularExpressions.Regex.Match(gameObject.name, @"\d+").Value);

            if (ID != 0 && gameObject.name.Contains("y")){   
                ApplyYokaiShader();
            } else {
                ApplyCharacterShader();
            }
        }
    }

    public void ApplyYokaiShader() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++) {
                Shader toonShader = Shader.Find("TSF/BaseOutline1");
                
                if (toonShader != null) {
                    materials[i].shader = toonShader;
                    materials[i].SetFloat("_TEX_ON", 1.0f);
                    materials[i].EnableKeyword("_TEX_ON"); 
                    materials[i].SetColor("_OutlineColor", outlineColor);
                    materials[i].SetFloat("_Outline", outlineWidth);
                } else {
                    Debug.LogWarning("Shader 'TSF/BaseOutline1' non trouvé !");
                }
            }
            renderer.materials = materials;
        }
    }

    public void ApplyCharacterShader() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++) {
                Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit");
                
                if (unlitShader != null) {
                    Texture mainTexture = null;
                    if (materials[i].HasProperty("_MainTex")) {
                        mainTexture = materials[i].GetTexture("_MainTex");
                    } else if (materials[i].HasProperty("_BaseMap")) {
                        mainTexture = materials[i].GetTexture("_BaseMap");
                    }

                    Color baseColor = Color.white;
                    if (materials[i].HasProperty("_Color")) {
                        baseColor = materials[i].GetColor("_Color");
                    } else if (materials[i].HasProperty("_BaseColor")) {
                        baseColor = materials[i].GetColor("_BaseColor");
                    }
                    materials[i].shader = unlitShader;
                    if (mainTexture != null) {
                        materials[i].SetTexture("_BaseMap", mainTexture);
                    }
                    materials[i].SetColor("_BaseColor", baseColor);
                } else {
                    Debug.LogWarning("Shader 'Universal Render Pipeline/Unlit' non trouvé !");
                }
            }
            renderer.materials = materials;
        }
    }
}
