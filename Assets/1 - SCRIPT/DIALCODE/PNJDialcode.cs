using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PNJDialcode : MonoBehaviour{
    private DialcodePrinted dialcode;
    [Header("Configuration")]
    public string jsonPath;
    
    [Header("Données")]
    /*[HideInInspector]*/ public List<FragmentDeDialogue> maListeDeFraguement = new List<FragmentDeDialogue>();
    public List<ConditionOfTheDialoguePage> conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>();
    private float taux = 0f;
    private bool inTheZonne = false;
    private bool characterVisibel = true;
    private bool doitDisparètreALaFinDuDialogue = false;

    void Awake() {
        dialcode = DialcodePrinted.Instance;
    }

    void Start(){
        ServantBoxCollider3D servantBC = new ServantBoxCollider3D();
        servantBC.CreateBoxCollider(1,gameObject,false);
        foreach (Transform item in gameObject.transform){
            servantBC.CreateBoxCollider(0,item.gameObject,true);
        }   
        StartCoroutine(TheStart());
    }

    IEnumerator TheStart(){
        while (DialcodePrinted.Instance == null){
            yield return new WaitForSeconds(0.1f);
        }
        dialcode = DialcodePrinted.Instance;
        LoadConditionOfTheDialoguePage();
        Refresh();
    }
    public void ChargerDialogue(int ID){
        TextAsset targetFile = Resources.Load<TextAsset>("texte/" + jsonPath+"-page"+ID);
        if (targetFile != null){
            DialogueData wrapper = JsonUtility.FromJson<DialogueData>(targetFile.text);
            if (wrapper == null || wrapper.fragments == null){
                maListeDeFraguement = new List<FragmentDeDialogue>();
            } else {
                maListeDeFraguement = wrapper.fragments;
            }
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
        if (collider.tag == "Player" && mouvementDuJoueur.droitDeMarcher){
            inTheZonne = true;
            if (Refresh() && dialcode != null){
                // dialcode.fragmentDeDialogue = maListeDeFraguement;
                dialcode.dansUneZoneDeDialogue = true;
                dialcode.pnjTalk = this;
            }
        }
    }

    public bool Refresh(){
        if (TrouverBonDialogue()) {
            characterVisibel = true;
            gameObject.SetActive(true);
            // if (inTheZonne){
            //     StartCoroutine(UpdateDialcodePrinted());
            // }
            doitDisparètreALaFinDuDialogue = false;
            return true;
        } else {
            if (inTheZonne && dialcode!=null){
                doitDisparètreALaFinDuDialogue = true;
            } else {
                characterVisibel = false;
                gameObject.SetActive(false);
            }
            return false;
        }
    }

    public void RetoursDeDiscution(){
        if (doitDisparètreALaFinDuDialogue) {
            dialcode.dansUneZoneDeDialogue = false;
            // dialcode.Fermeture(false);
            characterVisibel = false;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider collider){
        if (collider.tag == "Player" && dialcode != null){
            dialcode.dansUneZoneDeDialogue = false;
            inTheZonne = false;
            if (dialcode.pnjTalk == this) dialcode.pnjTalk = null;
        }
    }

    public bool TrouverBonDialogue(){
        if (conditionOfTheDialoguePage.Count == 0) {
            ChargerDialogue(0); 
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
        if (dialcode == null) dialcode = DialcodePrinted.Instance;
        if (dialcode == null) {
            Debug.Log("DialcodePrinted.Instance est introuvable dans la scène !");
            return false;
        }
        List<bool> valeurs = new List<bool>();
        foreach (Conditionnelle c in conditionnelle) {
            valeurs.Add(dialcode.EvaluerConditionSimple(c));
        }

        return dialcode.CalculerLogiqueComplexe(valeurs, conditionnelle);
    }

    // IEnumerator UpdateDialcodePrinted(){
    //     while(DialcodePrinted.Instance.discussionInProgress){
    //         yield return new WaitForSeconds(0.2f);
    //     }
    //     if (inTheZonne){
    //         // dialcode.fragmentDeDialogue = maListeDeFraguement;
    //     }
    // }
}

[System.Serializable]
public class DialogueData{
    public List<FragmentDeDialogue> fragments;
}

[System.Serializable]
public class ConditionOfTheDialoguePageData{
    public List<ConditionOfTheDialoguePage> conditionOfTheDialoguePage;
}