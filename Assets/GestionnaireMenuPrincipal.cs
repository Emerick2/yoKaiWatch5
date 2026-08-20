using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GestionnaireMenuPrincipal : MonoBehaviour{
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    private int numeroPage = 1;
    private int cible = 0;
    [Header("Page 1 :")]
    private bool playerMadeHisChoice;
    [Header("Page 2 :")]
    public List<GameObject> listOfPlayerFolder;
    public Sprite spriteSelect;
    public Sprite spriteNotSelect;
    [Header("Page 4 :")]
    public GameObject buttonChouse;
    private TextMeshProUGUI espaceTexte;
    private TMP_InputField inputFild;
    public Sprite spriteBoy;
    public Sprite spriteGirl;
    [Header("Page 5 (la '4²') :")]
    public List<Image> listeImagesFondInputFild;
    public GameObject windowQuestion;
    public Sprite spriteSelectQuestion;
    public Sprite spriteNotSelectQuestion;
    [Header("Données récupéré :")]
    public bool playerWantAReset;
    public int journalNumber;
    public bool thisIsAGirl;
    public string name;
    void Start(){
        transform.Find("fond").gameObject.SetActive(true);
        inputFild = page4.transform.Find("Espace nom/InputField (TMP)").GetComponent<TMP_InputField>();
        espaceTexte = page4.transform.Find("Espace nom/InputField (TMP)/Text Area/Text").GetComponent<TextMeshProUGUI>();
        ChangePage(1);
    }
    
    void Update(){
        bool selectInputFild = numeroPage == 4 && inputFild.isFocused;
        if (selectInputFild){
            for (int i = 0; i < listeImagesFondInputFild.Count; i++){
                float value = 0.85f;
                listeImagesFondInputFild[i].color = new Color(value,value,value,1f);
            }
        } else if (numeroPage == 4 || numeroPage == 5){
            for (int i = 0; i < listeImagesFondInputFild.Count; i++){
                listeImagesFondInputFild[i].color = new Color(1f,1f,1f,1f);
            }
        }
        if (Input.GetButtonDown("A")){
            bool changePage = true;
            if (numeroPage == 1){
                if (playerMadeHisChoice){
                    playerWantAReset = cible==1;
                    ChangePage(numeroPage+1);
                } else {
                    playerMadeHisChoice = true;
                    cible = 0;
                    page1.transform.Find("PanelChoice").gameObject.SetActive(true);
                    ChangePositionCiblePage1();
                }
            } else if (numeroPage != 4 || (!selectInputFild && (espaceTexte.text.Length >= 3 && espaceTexte.text.Length <= 20))){
                if (numeroPage == 2){
                    journalNumber = cible;
                    NewDataCenter.Instance.NewJournalNumber(journalNumber+1);
                    if (!playerWantAReset && NewDataCenter.Instance.NameOfPlayerInJournalNumber(journalNumber+1) != ""){
                        StartAGame(false);
                        return;
                    }
                } else if (numeroPage == 3){
                    thisIsAGirl = cible==1;
                } else if (numeroPage == 4){
                    name = espaceTexte.text;
                } else if (numeroPage == 5){
                    changePage = false;
                    if (cible == 1){
                        ChangePage(4);
                    } else {
                        StartAGame(true);
                    }
                }
                if (changePage){
                    ChangePage(numeroPage+1);
                }
            }
        } else if (Input.GetButtonDown("B")){
            if (numeroPage > 1 && !selectInputFild){
                ChangePage(numeroPage-1);
            }
            if (numeroPage==1){
                playerMadeHisChoice = false;
                page1.transform.Find("PanelChoice").gameObject.SetActive(false);
            }
        }
        switch (numeroPage){
            case 1 :
                if (Input.GetButtonDown("down")){
                    cible++;
                    ChangePositionCiblePage1();
                } else if (Input.GetButtonDown("up")){
                    cible--;
                    ChangePositionCiblePage1();
                }
                break;
            case 2 :
                if (Input.GetButtonDown("down")){
                    cible++;
                    ChangeSelectPlayerFolderPage2();
                } else if (Input.GetButtonDown("up")){
                    cible--;
                    ChangeSelectPlayerFolderPage2();
                }
                break;
            case 3 :
                if (Input.GetButtonDown("left")){
                    cible++;
                    ChangeSelectGenrePage3();
                } else if (Input.GetButtonDown("right")){
                    cible--;
                    ChangeSelectGenrePage3();
                }
                break;
            case 4 :
                buttonChouse.SetActive(espaceTexte.text.Length >= 3 && espaceTexte.text.Length <= 20);
                break;
            case 5 :
                if (Input.GetButtonDown("left")){
                    cible++;
                    ChangeSelectPage5();
                } else if (Input.GetButtonDown("right")){
                    cible--;
                    ChangeSelectPage5();
                }
                break;
        }
    }
    public void ChangePage(int numeroNewPage){
        name = espaceTexte.text;

        numeroPage = numeroNewPage;
        if (page1 != null) page1.SetActive(numeroNewPage == 1);
        if (page2 != null) page2.SetActive(numeroNewPage == 2);
        if (page3 != null) page3.SetActive(numeroNewPage == 3);
        if (page4 != null) page4.SetActive(numeroNewPage == 4 || numeroNewPage == 5);
        if (windowQuestion != null) windowQuestion.SetActive(numeroNewPage == 5);

        transform.Find("Bande").gameObject.SetActive(numeroNewPage == 3 || numeroNewPage == 4 || numeroNewPage == 5);
        transform.Find("Bande/Sexe").gameObject.SetActive(numeroNewPage == 3);
        transform.Find("Bande/Nom").gameObject.SetActive(numeroNewPage == 4);
        transform.Find("Bande/Confirmer").gameObject.SetActive(numeroNewPage == 5);
        page1.transform.Find("PanelChoice").gameObject.SetActive(false);
        switch (numeroNewPage){
            case 2 :
                cible=journalNumber;
                ChangeSelectPlayerFolderPage2();
                for (int i = 0; i < listOfPlayerFolder.Count; i++) {
                    string texte = NewDataCenter.Instance.NameOfPlayerInJournalNumber(i+1);
                    if (texte == "") texte = "Créer un nouveau fichier";
                    listOfPlayerFolder[i].transform.Find("Nom").GetComponent<TextMeshProUGUI>().text = texte;
                }
                page2.transform.Find("Titre/T1").gameObject.SetActive(!playerWantAReset);
                page2.transform.Find("Titre/T2").gameObject.SetActive(playerWantAReset);
                break;
            case 3 :
                cible=0;
                if (thisIsAGirl) cible=1;
                ChangeSelectGenrePage3();
                break;
            case 4 :
                if (!thisIsAGirl) page4.transform.Find("G").GetComponent<Image>().sprite = spriteBoy;
                else page4.transform.Find("G").GetComponent<Image>().sprite = spriteGirl;
                break;
            case 5 :
                cible=0;
                string genre = "Garçon";
                if (thisIsAGirl) genre = "Fille";
                windowQuestion.transform.Find("Texte").GetComponent<TextMeshProUGUI>().text = $"Cela te convient-il ?\n<color=#4F6C4C>{name}</color>\n({genre})";
                ChangeSelectPage5();
                break;
        }    
    }
    // page 1 :
    void ChangePositionCiblePage1(){
        int n = 2;
        cible = ((cible%n)+n)%n;
        page1.transform.Find("PanelChoice/bouton (1)/1").gameObject.SetActive(cible!=0);
        page1.transform.Find("PanelChoice/bouton (1)/2").gameObject.SetActive(cible==0);
        page1.transform.Find("PanelChoice/bouton (2)/1").gameObject.SetActive(cible!=1);
        page1.transform.Find("PanelChoice/bouton (2)/2").gameObject.SetActive(cible==1);
    }
    // page 2 :
    void ChangeSelectPlayerFolderPage2(){
        int n = listOfPlayerFolder.Count;
        cible = ((cible%n)+n)%n;
        for (int i = 0; i < n; i++){
            listOfPlayerFolder[i].gameObject.transform.Find("1").gameObject.SetActive(cible!=i);
            listOfPlayerFolder[i].gameObject.transform.Find("2").gameObject.SetActive(cible==i);
        }
    }

    // page 3 :
    void ChangeSelectGenrePage3(){
        int n = 2;
        cible = ((cible%n)+n)%n;
        bool boutonSexeMasquer = true;
        if (!boutonSexeMasquer){
            page3.transform.Find("G bouton/1").gameObject.SetActive(cible!=0);
            page3.transform.Find("G bouton/2").gameObject.SetActive(cible==0);

            page3.transform.Find("F bouton/1").gameObject.SetActive(cible!=1);
            page3.transform.Find("F bouton/2").gameObject.SetActive(cible==1);
        }

        page3.transform.Find("G").gameObject.SetActive(cible == 0);
        page3.transform.Find("F").gameObject.SetActive(cible != 0);
    }
    void ChangeSelectPage5(){
        int n = 2;
        cible = ((cible%n)+n)%n;
        if (cible == 0){
            windowQuestion.transform.Find("Q1").GetComponent<Image>().sprite = spriteSelectQuestion;
            windowQuestion.transform.Find("Q2").GetComponent<Image>().sprite = spriteNotSelectQuestion;
        } else {
            windowQuestion.transform.Find("Q1").GetComponent<Image>().sprite = spriteNotSelectQuestion;
            windowQuestion.transform.Find("Q2").GetComponent<Image>().sprite = spriteSelectQuestion;
        }
    }

    void StartAGame(bool newPlayer){
        if (!newPlayer){
            string scene = NewDataCenter.Instance.NameOfPlayerInJournalNumber(journalNumber+1, "scenne");
            if (scene == ""){
                SceneManager.LoadScene("chambre");
            } else {
                GameObject newObject = new GameObject("livraison début de partie");
                newObject.AddComponent<LivraisonDébutUnePartie>();
                SceneManager.LoadScene(scene);
            }
        } else {
            NewDataCenter.Instance.ResetJournal(journalNumber+1);
            NewDataCenter.Instance.SauvegarderSTR("nomDuJoueur", name);
            NewDataCenter.Instance.SauvegarderSTR("thisIsAGirl", thisIsAGirl.ToString());
            SceneManager.LoadScene("chambre");
        }
    }
}
