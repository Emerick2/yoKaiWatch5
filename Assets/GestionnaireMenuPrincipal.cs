using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestionnaireMenuPrincipal : MonoBehaviour{
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;
    private int numeroPage = 1;
    private int cible = 0;
    // [Header("Page 1 :")]
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
    public GameObject windowQuestion;
    public Sprite spriteSelectQuestion;
    public Sprite spriteNotSelectQuestion;
    [Header("Données récupéré :")]
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
        if (Input.GetButtonDown("A")){
            if (numeroPage != 4){
                if (numeroPage == 2){
                    journalNumber = cible;
                } else if (numeroPage == 3){
                    thisIsAGirl = cible==1;
                } else if (numeroPage == 4){
                    name = espaceTexte.text;
                }
                ChangePage(numeroPage+1);
            }
        } else if (Input.GetButtonDown("B")){
            bool selectInputFild = numeroPage == 4 && inputFild.isFocused;
            if (numeroPage > 1 && !selectInputFild){
                ChangePage(numeroPage-1);
            }
        }
        switch (numeroPage){
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
        if (page4 != null) page4.SetActive(numeroNewPage == 4);
        if (windowQuestion != null) windowQuestion.SetActive(numeroNewPage == 5);

        transform.Find("Bande").gameObject.SetActive(numeroNewPage == 3 || numeroNewPage == 4 || numeroNewPage == 5);
        transform.Find("Bande/Sexe").gameObject.SetActive(numeroNewPage == 3);
        transform.Find("Bande/Nom").gameObject.SetActive(numeroNewPage == 4);
        transform.Find("Bande/Confirmer").gameObject.SetActive(numeroNewPage == 5);

        switch (numeroNewPage){
            case 2 :
                cible=journalNumber;
                ChangeSelectPlayerFolderPage2();
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
        page3.transform.Find("G bouton/1").gameObject.SetActive(cible!=0);
        page3.transform.Find("G bouton/2").gameObject.SetActive(cible==0);

        page3.transform.Find("F bouton/1").gameObject.SetActive(cible!=1);
        page3.transform.Find("F bouton/2").gameObject.SetActive(cible==1);

        if (cible == 0){
            Image image = page3.transform.Find("G").GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
            image = page3.transform.Find("F").GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
        } else {
            Image image = page3.transform.Find("G").GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.8f);
            image = page3.transform.Find("F").GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
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
}
