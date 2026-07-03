using UnityEngine;

[System.Serializable]

public class ServantBoxCollider3D {

    public void CreateBoxCollider(float bonus, GameObject objet, bool solide) {
        if (objet == null){
            Debug.LogWarning("Ont à essayer de créé un collider sur un gameObject vide !");
            return;
        }



        if (objet.transform.childCount == 1 && bonus > 0f) {
            Transform child = objet.transform.GetChild(0);
            BoxCollider childBc = child.GetComponent<BoxCollider>();
            if (childBc != null) {
                BoxCollider bc1 = objet.GetComponent<BoxCollider>();
                if (bc1 == null) bc1 = objet.AddComponent<BoxCollider>();
                Vector3 worldCenter = child.TransformPoint(childBc.center);
                bc1.center = objet.transform.InverseTransformPoint(worldCenter);

                Vector3 childWorldSize = Vector3.Scale(childBc.size, child.lossyScale);
                Vector3 parentLossy = objet.transform.lossyScale;

                float localX1 = parentLossy.x != 0f ? childWorldSize.x / parentLossy.x : childWorldSize.x;
                float localY1 = parentLossy.y != 0f ? childWorldSize.y / parentLossy.y : childWorldSize.y;
                float localZ1 = parentLossy.z != 0f ? childWorldSize.z / parentLossy.z : childWorldSize.z;

                bc1.size = new Vector3(localX1 + bonus, localY1, localZ1 + bonus);
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

        if (bc == null) {

            bc = objet.AddComponent<BoxCollider>();

        }



        bc.center = objet.transform.InverseTransformPoint(bounds.center);



        Vector3 worldSize = bounds.size;

        Vector3 localScale = objet.transform.lossyScale;



        float localX = localScale.x != 0f ? worldSize.x / localScale.x : worldSize.x;

        float localY = localScale.y != 0f ? worldSize.y / localScale.y : worldSize.y;

        float localZ = localScale.z != 0f ? worldSize.z / localScale.z : worldSize.z;



        bc.size = new Vector3(localX + bonus, localY, localZ + bonus);

       

        bc.isTrigger = !solide;

    }



    public void CreateACopieOfAColliderWithABonus(float bonus, GameObject objet, BoxCollider colliderToDuplicate, bool solide) {

        BoxCollider bc = objet.GetComponent<BoxCollider>();

        if (bc == null) bc = objet.AddComponent<BoxCollider>();



        if (colliderToDuplicate != null) {

            bc.center = colliderToDuplicate.center;

            Vector3 baseSize = colliderToDuplicate.size;

            bc.size = new Vector3(baseSize.x + bonus, baseSize.y, baseSize.z + bonus);

            bc.isTrigger = !solide;

        }

    }

} 

