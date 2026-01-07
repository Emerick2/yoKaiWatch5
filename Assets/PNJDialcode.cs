using UnityEngine;
using System.Collections.Generic;

public class PNJDialcode : MonoBehaviour{
    private DialcodePrinted dialcode;
    [Header("Configuration")]
    public string jsonPath;
    
    [Header("Données")]
    [HideInInspector] public List<FragmentDeDialogue> maListeDeFraguement = new List<FragmentDeDialogue>();
    public float taux = 0.01f;

    void Start(){
        dialcode = DialcodePrinted.Instance;
        ChargerDialogue();
        CrééUnBoxCollider(taux);
    }
    public void ChargerDialogue(){
        TextAsset targetFile = Resources.Load<TextAsset>("texte/" + jsonPath);
        if (targetFile != null){
            DialogueData wrapper = JsonUtility.FromJson<DialogueData>(targetFile.text);
            maListeDeFraguement = wrapper.fragments;
        }
    }
    void OnTriggerEnter(Collider collider){
        if (collider.tag == "Player"){
            dialcode.fragmentDeDialogue = maListeDeFraguement;
            dialcode.dansUneZoneDeDialogue = true;
        }
    }
    void OnTriggerExit(Collider collider){
        if (collider.tag == "Player"){
            dialcode.dansUneZoneDeDialogue = false;
        }
    }

    public void CrééUnBoxCollider(float bonus){
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0){
            Debug.LogWarning("Aucun enfant avec un Renderer n'a été trouvé.");
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++){
            bounds.Encapsulate(renderers[i].bounds);
        }

        BoxCollider bc = GetComponent<BoxCollider>();
        if (bc == null){
            bc = gameObject.AddComponent<BoxCollider>();
        }

        bc.center = transform.InverseTransformPoint(bounds.center);

        Vector3 localSize = transform.InverseTransformVector(bounds.size);

        float extensionX = Mathf.Abs(localSize.x) + bonus;
        float extensionZ = Mathf.Abs(localSize.z) + bonus;
        
        bc.size = new Vector3(extensionX, localSize.y, extensionZ);
        bc.isTrigger = true;
        Debug.Log($"Box Collider créé avec un bonus de {bonus} sur les côtés !");
    }
}

[System.Serializable]
public class DialogueData{
    public List<FragmentDeDialogue> fragments;
}