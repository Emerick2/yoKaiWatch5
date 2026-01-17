using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderItem : MonoBehaviour
{ //fait que se programe fonctionne pour mettre des collision au objets qui n'on que un skined mesh renderer
    void Start()
    {
        AddColliders();
    }

    void AddColliders()
    {
        SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        int addedCount = 0;

        foreach (SkinnedMeshRenderer smr in skinnedMeshes)
        {
            if (smr.sharedMesh == null)
            {
                Debug.LogWarning($"Pas de mesh sur {smr.gameObject.name}, skip.");
                continue;
            }

            if (smr.GetComponent<MeshCollider>() == null)
            {
                MeshCollider mc = smr.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = smr.sharedMesh; // assigner le mesh correctement
                mc.convex = false; // ou true selon besoin
                addedCount++;
            }
        }

        //Debug.Log($"MeshColliders ajoutés sur {addedCount} objets avec SkinnedMeshRenderer.");
    }
}



// Input.GetKey(KeyCode.A);
// Input.GetButtonDown("A");

// //Les deux conditions doivent êtres vrais.
// if (joueur == true && Input.GetButtonDown("A")){
    
// }

// //Au moins une des deux condition doivent être vrais.
// if (joueur == true || Input.GetButtonDown("A")){
    
// }

// && ||




