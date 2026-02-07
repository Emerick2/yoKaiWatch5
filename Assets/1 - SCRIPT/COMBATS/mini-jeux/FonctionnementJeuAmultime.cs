using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FonctionnementJeuAmultime : MonoBehaviour{
    public List<GameObject> listeDesMiniJeu;
    public AmultimeRoueJeu roueScript;
    public AmultimeTourneJeu tourneScript;
    public DessineJeuAmultime dessinScript;
    public int identifiantDansMesYoKaiDuJoueur;
    public bool ammultimePrête;
    public GameObject imageQuiCrieVictoire;
    public Transform positionCibleAmultime;
    public GameObject blocAmultimeAnnimation;
    public Image sliderImage;

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
        dessinScript.Initialisation(this);
        ammultimePrête = false;
        foreach (GameObject item in listeDesMiniJeu){
            item.SetActive(false);
        }
        sliderImage.fillAmount = 0;
    }

    public void VictoireMiniJeu(){
        imageQuiCrieVictoire.SetActive(true);
        ammultimePrête = true;
        UtilisationAmultime();
    }


    public void UtilisationAmultime(){
        MedaliumNonPersonel yoKai = medaliumDictionnaire._instance.ServantYoKai(MesYoKai.Instance.maListe[identifiantDansMesYoKaiDuJoueur].ID);
        GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(yoKai.ID.ToString());
        TV.transform.position = positionCibleAmultime.position;
        TV.transform.rotation = positionCibleAmultime.rotation;
        blocAmultimeAnnimation.SetActive(true);
        StartCoroutine(Annimation(TV));
        RéinitialiserMenuAmultime();
    }
    IEnumerator Annimation(GameObject TV){
        int STOP = 50;
        while (STOP > 0){
            STOP--;
            yield return new WaitForSeconds(0.2f);
            if (TV.GetComponent<Animator>() != null){
                TV.GetComponent<Animator>().SetTrigger("Amultime");
                StartCoroutine(FinAnnimation(TV));
                STOP = 0;
            }
        }
    }

    IEnumerator FinAnnimation(GameObject TV){
        yield return new WaitForSeconds(7f);
        blocAmultimeAnnimation.SetActive(false);
        Destroy(TV);
    }

    public void NouvelleValeurSliderImage(float valeur){
        sliderImage.fillAmount = valeur;
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