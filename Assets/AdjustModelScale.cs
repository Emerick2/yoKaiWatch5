using UnityEngine;

public class AdjustModelScale : MonoBehaviour 
{
    public bool refresh;
    [Tooltip("La taille minimale en unités Unity en dessous de laquelle le modèle est considéré comme trop petit.")]
    private float minSizeThreshold = 0.3f;

    [SerializeField, HideInInspector] private string lastKnownName;

    void OnValidate() {
        if (string.IsNullOrEmpty(lastKnownName)) {
            lastKnownName = gameObject.name;
        }

        if (gameObject.name != lastKnownName) {
            Debug.Log($"[Rename] Le GameObject a été renommé dans OnValidate : '{lastKnownName}' -> '{gameObject.name}'");
            lastKnownName = gameObject.name;
        }

        ChargerTaille();
    }

    private void Start() {
        GererTexture(true);
    }

    private void GererTexture(bool forcer) {
        if (GetComponent<MakeATexture>() == null) {
            MakeATexture texture = gameObject.AddComponent<MakeATexture>();
            texture.ChargerStartTexture();
        } else if (forcer) {
            GetComponent<MakeATexture>().ChargerStartTexture();
        }
    }

    void ChargerTaille() {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        
        if (renderers.Length == 0) return;

        Bounds? totalBounds = null;

        foreach (var sr in renderers) {
            if (sr == null || sr.sharedMesh == null) continue;

            // Hors ligne (Éditeur), sr.bounds est souvent à 0. 
            // On prend donc la taille brute du mesh et on l'ajuste selon le scale de son GameObject.
            Vector3 meshSize = sr.sharedMesh.bounds.size;
            Vector3 localScale = sr.transform.lossyScale; // Scale global de l'enfant
            
            // On calcule les bounds réels de cet enfant
            Bounds worldBounds = new Bounds(
                sr.transform.position, 
                new Vector3(meshSize.x * localScale.x, meshSize.y * localScale.y, meshSize.z * localScale.z)
            );

            if (totalBounds == null) {
                totalBounds = worldBounds;
            } else {
                totalBounds.Value.Encapsulate(worldBounds);
            }
        }

        if (totalBounds.HasValue) {
            Vector3 modelSize = totalBounds.Value.size;

            if (modelSize.x < minSizeThreshold) {
                transform.localScale = new Vector3(100f, 100f, 100f);
            }
        }
    }
}
