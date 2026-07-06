using UnityEngine;

[System.Serializable]
public class ServantBoxCollider3D {

    public void CreateBoxCollider(float bonus, GameObject objet, bool solide) {
        if (objet == null){
            Debug.LogWarning("On a essayé de créer un collider sur un gameObject vide !");
            return;
        }

        Vector3 objectScale = objet.transform.localScale;
        if (objectScale.x == 0f) objectScale.x = 1f;
        if (objectScale.y == 0f) objectScale.y = 1f;
        if (objectScale.z == 0f) objectScale.z = 1f;

        if (objet.transform.childCount == 1 && bonus > 0f) {
            Transform child = objet.transform.GetChild(0);
            BoxCollider childBc = child.GetComponent<BoxCollider>();
            if (childBc != null) {
                BoxCollider bc1 = objet.GetComponent<BoxCollider>();
                if (bc1 == null) bc1 = objet.AddComponent<BoxCollider>();
                
                Vector3 worldCenter = child.TransformPoint(childBc.center);
                bc1.center = objet.transform.InverseTransformPoint(worldCenter);

                Vector3 childWorldSize = Vector3.Scale(childBc.size, child.lossyScale);
                
                float targetWorldX = childWorldSize.x + bonus;
                float targetWorldY = childWorldSize.y;
                float targetWorldZ = childWorldSize.z + bonus;

                Vector3 parentLossy = objet.transform.lossyScale;
                bc1.size = new Vector3(
                    parentLossy.x != 0f ? targetWorldX / parentLossy.x : targetWorldX,
                    parentLossy.y != 0f ? targetWorldY / parentLossy.y : targetWorldY,
                    parentLossy.z != 0f ? targetWorldZ / parentLossy.z : targetWorldZ
                );

                bc1.isTrigger = !solide;
                return;
            }
        }

        Renderer[] renderers = objet.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0) {
            Debug.LogWarning("Aucun enfant avec un Renderer n'a été trouvé.");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) {
            bounds.Encapsulate(renderers[i].bounds);
        }

        BoxCollider bc = objet.GetComponent<BoxCollider>();
        if (bc == null) bc = objet.AddComponent<BoxCollider>();

        bc.center = objet.transform.InverseTransformPoint(bounds.center);

        Vector3 worldSize = bounds.size;

        float totalWorldX = worldSize.x + bonus;
        float totalWorldY = worldSize.y;
        float totalWorldZ = worldSize.z + bonus;

        Vector3 currentLossy = objet.transform.lossyScale;
        
        bc.size = new Vector3(
            currentLossy.x != 0f ? totalWorldX / currentLossy.x : totalWorldX,
            currentLossy.y != 0f ? totalWorldY / currentLossy.y : totalWorldY,
            currentLossy.z != 0f ? totalWorldZ / currentLossy.z : totalWorldZ
        );

        bc.isTrigger = !solide;
    }

    public void CreateACopieOfAColliderWithABonus(float bonus, GameObject objet, BoxCollider colliderToDuplicate, bool solide) {
        BoxCollider bc = objet.GetComponent<BoxCollider>();
        if (bc == null) bc = objet.AddComponent<BoxCollider>();

        if (colliderToDuplicate != null) {
            bc.center = colliderToDuplicate.center;
            
            Vector3 baseSize = colliderToDuplicate.size;
            Vector3 localScale = objet.transform.localScale;

            float bonusX = localScale.x != 0f ? bonus / localScale.x : bonus;
            float bonusZ = localScale.z != 0f ? bonus / localScale.z : bonus;

            bc.size = new Vector3(baseSize.x + bonusX, baseSize.y, baseSize.z + bonusZ);
            bc.isTrigger = !solide;
        }
    }
}