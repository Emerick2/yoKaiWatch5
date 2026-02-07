using UnityEngine;
using System.Collections.Generic;

public class AutoColliderGenerator : MonoBehaviour{
    public List<SkinnedMeshRenderer> listeOfSkinnedMeshesIgnore = new List<SkinnedMeshRenderer>();
    void Start()
    {
        AddColliders();
    }

    public void AddColliders()
    {
        SkinnedMeshRenderer[] skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        int addedCount = 0;

        foreach (SkinnedMeshRenderer smr in skinnedMeshes){
            if (listeOfSkinnedMeshesIgnore.Contains(smr)){
                continue;
            }
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
