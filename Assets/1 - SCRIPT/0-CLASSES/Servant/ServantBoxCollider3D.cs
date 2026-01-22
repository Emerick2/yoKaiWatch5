using UnityEngine;

[System.Serializable]
public class ServantBoxCollider3D {
    public void CreateBoxCollider(float bonus, GameObject objet) {
        Renderer[] renderers = objet.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) {
            Debug.LogWarning("Aucun enfant avec un Renderer n'a été trouvé.");
            return;
        }

        // 1. Calculer les bounds globales
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // 2. Récupérer ou ajouter le BoxCollider
        BoxCollider bc = objet.GetComponent<BoxCollider>();
        if (bc == null) {
            bc = objet.AddComponent<BoxCollider>();
        }

        // 3. Positionnement du centre (Conversion World -> Local)
        bc.center = objet.transform.InverseTransformPoint(bounds.center);

        // 4. Calcul de la taille locale
        // On divise la taille du monde par le scale global pour obtenir la taille locale
        Vector3 worldSize = bounds.size;
        Vector3 localScale = objet.transform.lossyScale;

        float localX = worldSize.x / localScale.x;
        float localY = worldSize.y / localScale.y;
        float localZ = worldSize.z / localScale.z;

        // 5. Application du bonus et assignation
        bc.size = new Vector3(localX + bonus, localY, localZ + bonus);
        
        bc.isTrigger = true;
    }
}

// using UnityEngine;

// [System.Serializable]
// public class ServantBoxCollider3D{
//     public void CrééUnBoxCollider(float bonus, GameObject objet){
//         Renderer[] renderers = objet.GetComponentsInChildren<Renderer>();

//         if (renderers.Length == 0){
//             Debug.LogWarning("Aucun enfant avec un Renderer n'a été trouvé.");
//             return;
//         }

//         Bounds bounds = renderers[0].bounds;

//         for (int i = 1; i < renderers.Length; i++){
//             bounds.Encapsulate(renderers[i].bounds);
//         }

//         BoxCollider bc = objet.GetComponent<BoxCollider>();
//         if (bc == null){
//             bc = objet.AddComponent<BoxCollider>();
//         }

//         bc.center = objet.transform.InverseTransformPoint(bounds.center);

//         Vector3 localSize = objet.transform.InverseTransformVector(bounds.size);

//         float extensionX = Mathf.Abs(localSize.x) + bonus;
//         float extensionZ = Mathf.Abs(localSize.z) + bonus;
        
//         bc.size = new Vector3(extensionX, localSize.y, extensionZ);
//         bc.isTrigger = true;
//         // Debug.Log($"Box Collider créé avec un bonus de {bonus} sur les côtés !");
//     }
// }