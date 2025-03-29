using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouvementSacObjet : MonoBehaviour
{
    public GameObject cible;
    public UI_Déplacement monDéplacementScript;
    public RectTransform bloc;
    private int IDduPlusVisibleàGauche=5;
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
    void Start(){
        positionDeDépart = Mathf.RoundToInt(transform.position.x);
        valeur = positionDeDépart;
    }
    void Update(){   
        if (!pause){
            if (Input.GetKeyDown(KeyCode.RightArrow)){
                indexGauche++;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)){
                indexGauche--;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                if (indexHaut == 0){indexHaut=1;}
                else if (indexHaut == 1){indexHaut=2;}
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)){
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
        }
        if (IDduPlusVisibleàGauche<=indexGauche){
            print("droite");
            BougerLeBloc("droite");
            IDduPlusVisibleàGauche++;
        }else if (IDduPlusVisibleàGauche/2>=indexGauche){
            print("gauche");
            BougerLeBloc("gauche");
            IDduPlusVisibleàGauche--;
        }
    }
    void BougerLeBloc(string poisiton){
        float distanceEntreChaquePoint=80f;
        dernierIndice=indexGauche;
        if(poisiton=="gauche"){
            valeur+=distanceEntreChaquePoint;
        }else{
            valeur-=distanceEntreChaquePoint;
        }

        monDéplacementScript.STOP = false;
        cible.transform.position = new Vector2(valeur, cible.transform.position.y);
    }
}
