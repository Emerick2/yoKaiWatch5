using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FonctionnementJeuAmultime : MonoBehaviour{
    public List<GameObject> listeDesMiniJeu;
    public AmultimeRoueJeu roueScript;
    public AmultimeTourneJeu tourneScript;
    public int identifiantDansMesYoKaiDuJoueur;
    public bool ammultimePrête;
    public GameObject imageQuiCrieVictoire;

    void Start(){
        RéinitialiserMenuAmultime();
        DéclangerLeJeuAmultime(2);
    }

    public void DéclangerLeJeuAmultime(int identifiantDansMesYoKaiDuJoueur1){
        RéinitialiserMenuAmultime();
        identifiantDansMesYoKaiDuJoueur = identifiantDansMesYoKaiDuJoueur1;
        int n = Random.Range(0,listeDesMiniJeu.Count);
        listeDesMiniJeu[n].SetActive(true);
    }

    void RéinitialiserMenuAmultime(){
        imageQuiCrieVictoire.SetActive(false);
        roueScript.Initialisation(this);
        tourneScript.Initialisation(this);
        ammultimePrête = false;
        foreach (GameObject item in listeDesMiniJeu){
            item.SetActive(false);
        }
    }

    public void VictoireMiniJeu(){
        imageQuiCrieVictoire.SetActive(true);
        ammultimePrête = true;
    }


    public void UtilisationAmultime(){
        RéinitialiserMenuAmultime();
    }
}



/*
*1 - créé un mini-jeu amultime
*2 - créé le code qui lance un mini-jeu amultime au hasard
*3 - écran de victoire mini-jeu amultime
4 - faire une fonction qui permet de créé l'action amultime
5 - Action amultime commence par un déplacement de la caméra et une mise en pause du combat
6 - Les paramètre du panneau amultime son réinitialiser
7 - le yo-kai ne fait pas l'annimation attaque mais l'annimation amultime
8 - les déga infligé double

9 - quand on revient en arrière réinitialiser les paramètre d'amultime
10 - empêcher le retours en arière quand l'amultime est charger

*/