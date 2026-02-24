using UnityEngine;

public class MakeAShader : MonoBehaviour{
    [Header("Réglages de l'Outline")]
    public float outlineWidth = 1.0f;
    public Color outlineColor = Color.black;

    void Start(){
        ApplyYokaiShader();
    }

    [ContextMenu("Appliquer le Shader maintenant")]
    public void ApplyYokaiShader() {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) {
            // Utiliser renderer.sharedMaterials en mode Edition pour éviter les fuites de mémoire
            // ou renderer.materials en runtime.
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++) {
                Shader toonShader = Shader.Find("TSF/BaseOutline1");
                
                if (toonShader != null) {
                    materials[i].shader = toonShader;

                    // 1. Activer la texture de détail (si c'est ce que tu souhaites)
                    materials[i].SetFloat("_TEX_ON", 1.0f);
                    materials[i].EnableKeyword("_TEX_ON"); 

                    // 2. Configuration des couleurs et de l'outline
                    materials[i].SetColor("_OutlineColor", outlineColor);
                    
                    // CORRECTION : Utiliser "_Outline" au lieu de "_OutlineWidth"
                    materials[i].SetFloat("_Outline", outlineWidth);

                    Debug.Log($"Shader appliqué sur : {renderer.name} (Mat: {materials[i].name})");
                } else {
                    Debug.LogWarning("Shader 'TSF/BaseOutline1' non trouvé !");
                }
            }
            // Réassignation importante pour appliquer les changements
            renderer.materials = materials;
        }
    }
}