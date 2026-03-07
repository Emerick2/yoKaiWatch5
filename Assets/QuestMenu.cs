using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class QuestMenu : MonoBehaviour{
    public List<QuestMenuInformation> listOfQuestMenuInformation = new List<QuestMenuInformation>();
    private int cible=0;
    private int numberPage;
    private GameObject livre;
    private GameObject page1;
    private GameObject page2;

    private écrireLesPetitesValeursDuMenu menu;
    public GameObject blockListeOfQuest;

    public int maxProposition = 8;//on part de 1
    public int minProposition = 1;//on part de 1
    private GridLayoutGroup grid;
    private int left;
    public Sprite iconeDefault;
    void Start(){
        livre = transform.Find("livre").gameObject;
        page1 = livre.transform.Find("PAGE-1").gameObject;
        page2 = livre.transform.Find("PAGE-2").gameObject;
        grid = blockListeOfQuest.GetComponent<GridLayoutGroup>();
        if (grid != null) left = grid.padding.left;
        Initialiser(null);
    }


    public void Initialiser(écrireLesPetitesValeursDuMenu m){
        FindInformationInQuest();
        if (m != null) menu=m;
        numberPage = 0;
        ChangePage(false);
        cible = 0;
        GameObject example = null;
        foreach (Transform child in blockListeOfQuest.transform){
            if (example == null) example = child.gameObject;
            else Destroy(child.gameObject);
        }
        if (example != null){
            example.SetActive(false);
            for (int i = 0; i < listOfQuestMenuInformation.Count; i++){
                GameObject TV = Instantiate(example);
                TV.transform.parent = blockListeOfQuest.transform;
                TV.transform.localScale = new Vector3(1f, 1f, 1f);
                TV.transform.name = (i+1).ToString();
                if (TV.GetComponent<Image>() != null) {
                    if (listOfQuestMenuInformation[i].sprite != null){
                        TV.GetComponent<Image>().sprite = listOfQuestMenuInformation[i].sprite;
                    } else {
                        TV.GetComponent<Image>().sprite = iconeDefault;
                    }
                }
                TV.SetActive(true);
            }
            if (listOfQuestMenuInformation.Count > 0){
                Destroy(example);
            }
        }

        StartCoroutine(InitHighlightNextFrame());
    }

    IEnumerator InitHighlightNextFrame(){
        yield return null;
        ChangeQuest();
    }

    void ChangeQuest(){
        if (listOfQuestMenuInformation.Count == 0){
            return;
        }
        if (cible<0){
            cible = 0;
        } else if (cible >= listOfQuestMenuInformation.Count) {
            cible = listOfQuestMenuInformation.Count-1;
        }
        QuestMenuInformation quest = listOfQuestMenuInformation[cible];
        livre.transform.Find("BIEN-JOUER").gameObject.SetActive(quest.finish);
        
        livre.transform.Find("niveau/niveau texte").GetComponent<TextMeshProUGUI>().text = quest.level.ToString();
        livre.transform.Find("titre").GetComponent<TextMeshProUGUI>().text = quest.name;
        page1.transform.Find("description").GetComponent<TextMeshProUGUI>().text = quest.description;
        page1.transform.Find("image-quête").GetComponent<Image>().sprite = quest.sprite;
        page1.transform.Find("objectif").GetComponent<TextMeshProUGUI>().text = quest.objective;
        page2.transform.Find("nombreXP").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(quest.xp);

        int i = 0;
        foreach (Transform child in blockListeOfQuest.transform){
            float taux = 0.35f;
            if (i == cible) taux = 1;
            child.gameObject.GetComponent<Image>().color = new Color(1,1,1,taux);
            i++;
        }
        PousserPageQuest();
    }

    void Update(){
        if (Input.GetButtonDown("X")){
            ChangePage(true);
        }
        if (Input.GetButtonDown("B") && menu != null){
            menu.RevenirAuMenu();
        }
        if (Input.GetButtonDown("L")){
            cible++;
            ChangeQuest();
        } else if (Input.GetButtonDown("R")){
            cible--;
            ChangeQuest();
        }
    }

    void ChangePage(bool changeNumberPage){
        if (changeNumberPage){
            numberPage++;
            if (numberPage > 1){
                numberPage = 0;
            }
        }
        if (page1 != null) page1.SetActive(numberPage==0);
        if (page2 != null) page2.SetActive(numberPage==1);
    }

    void PousserPageQuest() {
        if (grid == null) return;

        float spacingX = grid.spacing.x;
        float cellWidth = grid.cellSize.x;

        if (cible - (minProposition - 1) >= maxProposition) {
            minProposition++;
        } 
        else if (cible < minProposition - 1) {
            minProposition--;
        }

        int newLeft = left+(int)((cellWidth + spacingX) * (minProposition - 1)) * -1;
        grid.padding.left = newLeft;

        LayoutRebuilder.ForceRebuildLayoutImmediate(grid.GetComponent<RectTransform>());
    }

    void FindInformationInQuest(){
        listOfQuestMenuInformation.Clear();
        NewDataCenter data = NewDataCenter.Instance;
        List<string> listOfQuest = data.ReadWithTag("Quête-");
        for (int i = 0; i < listOfQuest.Count; i++){
            listOfQuest[i] = listOfQuest[i].Replace("Quête-","");
        }
        foreach (string item in listOfQuest){
            if (data.Lire("Quête-"+item) != "0"){
                QuestMenuInformation q = new QuestMenuInformation();
                q.name = item;
                q.description = data.Lire("Quête_description%"+item);
                q.objective = data.Lire("Quête_objectif%"+item);
                q.level = int.Parse(data.Lire("Quête_niveau%"+item));
                q.xp = int.Parse(data.Lire("Quête_xp%"+item));
                q.finish = data.Lire("Quête-"+item) == "2";
                q.sprite = Resources.Load<Sprite>("image-quest/"+item);
                listOfQuestMenuInformation.Add(q);
            }
        }
    }
}


/*
Clef de quête :
Quête-le nom : 0,1,2
Quête_description%le nom : "blablaba"
Quête_objectif%le nom : "blabla"
Quête_niveau%le nom : 20
Quête_xp%le nom : 30
*/