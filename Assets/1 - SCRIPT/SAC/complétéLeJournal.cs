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

    void Start(){
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        nomDuJoueur.text = NewDataCenter.Instance.Lire("nomDuJoueur");
        if (nomDuJoueur.text == "0") nomDuJoueur.text = "";
        lieu.text = NewDataCenter.Instance.Lire("scenne");
        if (lieu.text == "0") lieu.text = "";
        Date.text = NewDataCenter.Instance.Lire("theTime");
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
            médaille[i].SetActive(true);
            icôneMédaille[i].sprite = médailum.ServantYoKai(int.Parse(NewDataCenter.Instance.Lire(yoKaiDansEquipe[i]+"_ID"))).médaillon;
        }
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
            if (Input.GetButtonDown("A") && cible==0){
                NewDataCenter.Instance.SauvegarderSTR("theTime", TheTime());
                if (NewDataCenter.Instance.Sauvgarder()){
                    if (bouton1 != null) bouton1.SetActive(false);
                    if (bouton2 != null) bouton2.SetActive(false);
                    if (transform.Find("Sauvegarder ?") != null) transform.Find("Sauvegarder ?").gameObject.SetActive(false);
                }
            }
        }
    }

    string TheTime(){
        DateTime maintenant = DateTime.Now;
        int mois = maintenant.Month;
        int jour = maintenant.Day;
        int année = maintenant.Year;
        int heure = maintenant.Hour;
        int minute = maintenant.Minute;
        string minuteSTR = minute.ToString();
        if (minute<10){
            minuteSTR="0"+minuteSTR;
        }
        return jour+"/"+mois+"/"+année+"  "+heure+":"+minuteSTR;
    }
}
