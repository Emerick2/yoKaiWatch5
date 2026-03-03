using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestMenu : MonoBehaviour{
    public string name;
    public string description;
    public string objective;
    public int level;
    public Sprite sprite;
    public int xp;
    public List<int> idObjet;
    public bool finish;
    private int numberPage;
    private GameObject livre;
    private GameObject page1;
    private GameObject page2;

    private écrireLesPetitesValeursDuMenu menu;
    void Start(){
        livre = transform.Find("livre").gameObject;
        page1 = livre.transform.Find("PAGE-1").gameObject;
        page2 = livre.transform.Find("PAGE-2").gameObject;
        Initialiser(null);
    }

    public void Initialiser(écrireLesPetitesValeursDuMenu m){
        if (m != null) menu=m;
        numberPage = 0;
        ChangePage(false);
        livre.transform.Find("BIEN-JOUER").gameObject.SetActive(finish);
        
        livre.transform.Find("niveau/niveau texte").GetComponent<TextMeshProUGUI>().text = level.ToString();
        livre.transform.Find("titre").GetComponent<TextMeshProUGUI>().text = name;
        page1.transform.Find("description").GetComponent<TextMeshProUGUI>().text = description;
        page1.transform.Find("image-quête").GetComponent<Image>().sprite = sprite;
        page1.transform.Find("objectif").GetComponent<TextMeshProUGUI>().text = objective;
        page2.transform.Find("nombreXP").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(xp);
    }

    void Update(){
        if (Input.GetButtonDown("X")){
            ChangePage(true);
        }
        if (Input.GetButtonDown("B") && menu != null){
            menu.RevenirAuMenu();
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
}
