using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouvementSacObjet : MonoBehaviour
{
    //private int IDduPlusVisibleàGauche=5;
    private bool pause = false;
    /*[HideInInspector]*/ public List<GameObject> sélectionnéHaut;
    /*[HideInInspector]*/ public List<GameObject> sélectionnéMoyen;
    /*[HideInInspector]*/ public List<GameObject> sélectionnéBas;
    private int indexHaut = 2;
    private int indexGauche = 0;
    private GameObject objetActuel;
    private int positionDeDépart;
    private float valeur;
    private int dernierIndice;

    private int IDVisibleLaPlusBasse=0;//nombre de fois ou je suis décendu, se nombre pourait aller jusqua -5
    public GameObject cible;
    public GameObject cibleDeDépart;
    public UI_Déplacement monDéplacementScript;
    public RectTransform bloc;
    private int nombreMaximumDeArcticleAfficher = 5;
    void Start(){
        positionDeDépart = Mathf.RoundToInt(transform.position.x);
        valeur = positionDeDépart;
    }
    void Update(){   
        if (!pause){
            if (Input.GetButton("right")){
                indexGauche++;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetButton("left")){
                indexGauche--;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetButton("up")){
                if (indexHaut == 0){indexHaut=1;}
                else if (indexHaut == 1){indexHaut=2;}
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetButton("down")){
                if (indexHaut == 2){indexHaut=1;}
                else if (indexHaut == 1){indexHaut=0;}
                else {indexHaut=2;}
                changerDePosition();
            }
        }
    }
    void changerDePosition(){
        if (objetActuel!=null){
            objetActuel.SetActive(false);
        }else {
            sélectionnéHaut[0].SetActive(false);
        }
        if (indexHaut==2){
            objetActuel = sélectionnéHaut[indexGauche];
            objetActuel.SetActive(true);
        }
        if (indexHaut==1){
            objetActuel = sélectionnéMoyen[indexGauche];
            objetActuel.SetActive(true);
        }
        if (indexHaut==0){
            objetActuel = sélectionnéBas[indexGauche];
            objetActuel.SetActive(true);
        }/*
        if (IDduPlusVisibleàGauche<=indexGauche){
            print("droite");
            BougerLeBloc("droite");
            //IDduPlusVisibleàGauche++;
        }else if (IDduPlusVisibleàGauche/2>=indexGauche){
            print("gauche");
            BougerLeBloc("gauche");
            //IDduPlusVisibleàGauche--;
        }*/
        int temps = IDVisibleLaPlusBasse;
        IDVisibleLaPlusBasse = indexGauche-nombreMaximumDeArcticleAfficher;
        if (IDVisibleLaPlusBasse<0){IDVisibleLaPlusBasse=0;}
        if (IDVisibleLaPlusBasse>sélectionnéHaut.Count-1){IDVisibleLaPlusBasse=sélectionnéHaut.Count-nombreMaximumDeArcticleAfficher-3;}
        if (temps != IDVisibleLaPlusBasse){
            BougerLeBloc();
        }
    }
    void BougerLeBloc(){
        float décalageVersLeBas = sélectionnéHaut[0].transform.position.x - sélectionnéHaut[1].transform.position.x;
        float positionDeDépart = cibleDeDépart.transform.position.x;
        cible.transform.position = new Vector2((IDVisibleLaPlusBasse*décalageVersLeBas)+positionDeDépart, bloc.transform.position.y);
        monDéplacementScript.STOP = false;
    }
}
