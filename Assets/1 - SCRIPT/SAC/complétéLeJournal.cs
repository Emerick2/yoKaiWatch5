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

    void Start(){
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        nomDuJoueur.text = NewDataCenter.Instance.Lire("nomDuJoueur");
        lieu.text = NewDataCenter.Instance.Lire("scenne","position");
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
        Date.text = jour+"/"+mois+"/"+année+"  "+heure+":"+minuteSTR;

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
}
