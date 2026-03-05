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
    void Start(){
        livre = transform.Find("livre").gameObject;
        page1 = livre.transform.Find("PAGE-1").gameObject;
        page2 = livre.transform.Find("PAGE-2").gameObject;
        grid = blockListeOfQuest.GetComponent<GridLayoutGroup>();
        if (grid != null) left = grid.padding.left;
        Initialiser(null);
    }

    public void Initialiser(écrireLesPetitesValeursDuMenu m){
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
                if (TV.GetComponent<Image>() != null && listOfQuestMenuInformation[i].sprite != null) {
                    TV.GetComponent<Image>().sprite = listOfQuestMenuInformation[i].sprite;
                }
                TV.SetActive(true);
            }
            if (listOfQuestMenuInformation.Count > 0){
                Destroy(example);
            }
        }

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
        page1.SetActive(numberPage==0);
        page2.SetActive(numberPage==1);
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
}
