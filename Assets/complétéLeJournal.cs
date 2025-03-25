using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class complétéLeJournal : MonoBehaviour{
    public Text nomDuJoueur;
    public Text tempsDeJeu;
    public Text Date;

    void Start(){
        nomDuJoueur.text = NewDataCenter.Instance.Lire("nomDuJoueur");

        DateTime maintenant = DateTime.Now;
        int mois = maintenant.Month;
        int jour = maintenant.Day;
        int année = maintenant.Year;
        int heure = maintenant.Hour;
        int minute = maintenant.Minute;
        Date.text = jour+"/"+mois+"/"+année+"  "+heure+":"+minute;

        int tempsEnMinutes = int.Parse(NewDataCenter.Instance.Lire("tempsDeJeu"));
        int heures = tempsEnMinutes / 60;
        int minutes = tempsEnMinutes % 60;
        tempsDeJeu.text = heures+":"+minutes;
    }
}
