using UnityEngine;

public class MakeAShader : MonoBehaviour{
    [Header("Réglages de l'Outline")]
    public float outlineWidth = 1.0f;
    public Color outlineColor = Color.black;

    void Start(){
        ApplyYokaiShader();
    }

    [ContextMenu("Appliquer le Shader maintenant")]
    public void ApplyYokaiShader(){
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers){
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++){
                Shader toonShader = Shader.Find("TSF/BaseOutline1");
                if (toonShader != null){
                    materials[i].shader = toonShader;

                    materials[i].SetFloat("_EnableDetail", 1.0f);
                    materials[i].EnableKeyword("_DETAIL_ON"); 

                    materials[i].SetColor("_OutlineColor", outlineColor);
                    materials[i].SetFloat("_OutlineWidth", outlineWidth);

                    Debug.Log($"Shader appliqué sur : {materials[i].name}");
                } else {
                    Debug.LogWarning("Shader 'TSF/BaseOutline1' non trouvé. Vérifie le nom exact !");
                }
            }

            renderer.materials = materials;
        }
    }
}