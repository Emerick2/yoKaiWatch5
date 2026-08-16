using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class complétéLeJournal : MonoBehaviour{
    public Text nomDuJoueur;
    public Text tempsDeJeu;
    public Text Date;
    public Text lieu;
    public List<GameObject> médaille;
    public List<Image> icôneMédaille;
    private medaliumDictionnaire médailum;
    private Transform postitionJoueur;
    public GameObject bouton1;
    public GameObject bouton2;
    public bool takeAChouce = true;
    private int cible;
    private écrireLesPetitesValeursDuMenu menu;

    void Start(){
        Initialiser(null);
    }

    public void Initialiser(écrireLesPetitesValeursDuMenu m){
        if (m != null) menu = m;
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        nomDuJoueur.text = NewDataCenter.Instance.Lire("nomDuJoueur");
        if (nomDuJoueur.text == "0") nomDuJoueur.text = "";
        lieu.text = NewDataCenter.Instance.Lire("scenne");
        if (lieu.text == "0") lieu.text = "";
        Date.text = NewDataCenter.Instance.Lire("theTime");
        Debug.Log(NewDataCenter.Instance.Lire("theTime"));
        if (Date.text == "0") Date.text = "";

        int tempsEnMinutes = int.Parse(NewDataCenter.Instance.Lire("tempsDeJeu"));
        int heures = tempsEnMinutes / 60;
        int minutes = tempsEnMinutes % 60;
        string minutesSTR = minutes.ToString();
        if (minutes<10){
            minutesSTR="0"+minutesSTR;
        }
        tempsDeJeu.text = heures+":"+minutesSTR;
        List<int> yoKaiDansEquipe = MesYoKai.Instance.yoKaiDeMonEquipe;
        for (int i = 0; i < yoKaiDansEquipe.Count; i++){
            int ID = int.Parse(NewDataCenter.Instance.Lire(yoKaiDansEquipe[i]+"_ID"));
            médaille[i].SetActive(true);
            if (icôneMédaille.Count > i && ID > 0) {
                icôneMédaille[i].sprite = médailum.ServantYoKai(ID).médaillon;
            }
        }

        cible = 0;
        bouton1.transform.Find("fond-actif").gameObject.SetActive(cible==0);
        bouton1.transform.Find("fond-inactif").gameObject.SetActive(cible!=0);
        bouton2.transform.Find("fond-actif").gameObject.SetActive(cible==1);
        bouton2.transform.Find("fond-inactif").gameObject.SetActive(cible!=1);
        if (bouton1 != null) bouton1.SetActive(true);
        if (bouton2 != null) bouton2.SetActive(true);
        if (transform.Find("livre/Sauvegarder ?") != null) transform.Find("livre/Sauvegarder ?").gameObject.SetActive(true);
    }

    void Update() {
        if (takeAChouce){
            if (Input.GetButtonDown("left") || Input.GetButtonDown("right")){
                if (cible == 0){
                    cible = 1;
                } else {
                    cible = 0;
                }
                bouton1.transform.Find("fond-actif").gameObject.SetActive(cible==0);
                bouton1.transform.Find("fond-inactif").gameObject.SetActive(cible!=0);
                bouton2.transform.Find("fond-actif").gameObject.SetActive(cible==1);
                bouton2.transform.Find("fond-inactif").gameObject.SetActive(cible!=1);
            }
            if (Input.GetButtonDown("A")){
                bool disparitionMenu = true;
                if (cible==0){
                    NewDataCenter.Instance.SauvegarderSTR("theTime", TheTime());
                    disparitionMenu = NewDataCenter.Instance.Sauvgarder();                  
                }
                if (disparitionMenu){
                    if (bouton1 != null) bouton1.SetActive(false);
                    if (bouton2 != null) bouton2.SetActive(false);
                    if (transform.Find("livre/Sauvegarder ?") != null) transform.Find("livre/Sauvegarder ?").gameObject.SetActive(false);
                }
            }
        }
        if (Input.GetButtonDown("B")){
            Debug.Log("1");
            if (menu != null){
                Debug.Log("2");
                menu.RevenirAuMenu();
            }
        }
    }

    string TheTime(){
        return DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }
}
