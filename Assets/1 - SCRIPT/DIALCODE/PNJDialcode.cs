using UnityEngine;
using System.Collections.Generic;

public class PNJDialcode : MonoBehaviour{
    private DialcodePrinted dialcode;
    [Header("Configuration")]
    public string jsonPath;
    
    [Header("Données")]
    /*[HideInInspector]*/ public List<FragmentDeDialogue> maListeDeFraguement = new List<FragmentDeDialogue>();
    public List<ConditionOfTheDialoguePage> conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>();
    public float taux = 0.01f;

    void Start(){
        dialcode = DialcodePrinted.Instance;
        LoadConditionOfTheDialoguePage();
        new ServantBoxCollider3D().CreateBoxCollider(taux, gameObject);
        if (TrouverBonDialogue()) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }
    public void ChargerDialogue(int ID){
        TextAsset targetFile = Resources.Load<TextAsset>("texte/" + jsonPath+"-page"+ID);
        if (targetFile != null){
            DialogueData wrapper = JsonUtility.FromJson<DialogueData>(targetFile.text);
            maListeDeFraguement = wrapper.fragments;
        }
    }

    public void LoadConditionOfTheDialoguePage(){
        TextAsset targetFile = Resources.Load<TextAsset>("texte/" + jsonPath+"-listePage");
        if (targetFile != null){
            ConditionOfTheDialoguePageData wrapper = JsonUtility.FromJson<ConditionOfTheDialoguePageData>(targetFile.text);
            conditionOfTheDialoguePage = wrapper.conditionOfTheDialoguePage;
        }
    }
    void OnTriggerEnter(Collider collider){
        if (collider.tag == "Player"){
            if (TrouverBonDialogue()) {
                dialcode.fragmentDeDialogue = maListeDeFraguement;
                dialcode.dansUneZoneDeDialogue = true;
                gameObject.SetActive(true);
            } else {
                gameObject.SetActive(false);
            }
        }
    }
    void OnTriggerExit(Collider collider){
        if (collider.tag == "Player"){
            dialcode.dansUneZoneDeDialogue = false;
        }
    }

    bool TrouverBonDialogue(){
        if (conditionOfTheDialoguePage.Count == 0){
            return true;
        }
        for (int i = conditionOfTheDialoguePage.Count-1; i >= 0; i--){
            if (conditionOfTheDialoguePage[i].listOfConditions.Count == 0 || CalculeCondition(conditionOfTheDialoguePage[i].listOfConditions)){
                if (conditionOfTheDialoguePage[i].visibleCharacter){
                    ChargerDialogue(i);
                    return true;
                } else {
                    return false;
                }
            }
        }
        return false;
    }

    bool CalculeCondition(List<Conditionnelle> conditionnelle){
        List<bool> valeurs = new List<bool>();
        foreach (Conditionnelle c in conditionnelle) {
            valeurs.Add(dialcode.EvaluerConditionSimple(c));
        }

        return dialcode.CalculerLogiqueComplexe(valeurs, conditionnelle);
    }
}

[System.Serializable]
public class DialogueData{
    public List<FragmentDeDialogue> fragments;
}

[System.Serializable]
public class ConditionOfTheDialoguePageData{
    public List<ConditionOfTheDialoguePage> conditionOfTheDialoguePage;
}