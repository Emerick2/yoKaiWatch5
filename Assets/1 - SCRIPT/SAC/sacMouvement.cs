using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sacMouvement : MonoBehaviour{
    private bool pause = false;
    public List<GameObject> sélectionnéHaut;
    public List<GameObject> sélectionnéBas;
    public List<GameObject> windows;
    private int indexHaut = 1;
    private int indexGauche = 0;
    private GameObject objetActuel;
    public GameObject boiteDeMenuDeJeu;
    [HideInInspector] public bool bloquer;
    void Update(){   
        if (!pause && !bloquer){
            if (Input.GetButtonDown("flèche droite")){
                indexGauche++;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetButtonDown("flèche gauche")){
                indexGauche--;
                if (indexGauche < 0){
                    indexGauche=sélectionnéHaut.Count-1;
                }else if (indexGauche > sélectionnéHaut.Count-1){
                    indexGauche=0;
                }
                changerDePosition();
            }
            if (Input.GetButtonDown("flèche haut")){
                if (indexHaut == 0){indexHaut=1;}
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetButtonDown("flèche bas")){
                if (indexHaut == 0){indexHaut=1;}
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetButtonDown("A")){
                StartCoroutine(Transition());
                int numéro=indexGauche;
                if (indexHaut==0){numéro+=sélectionnéHaut.Count;}
                if (windows[numéro] != null){
                    windows[numéro].SetActive(true);
                    pause=true;
                }
            }
            if (Input.GetButtonDown("B")){
                StartCoroutine(Transition());
                mouvementDuJoueur.droitDeMarcher = true;
                boiteDeMenuDeJeu.SetActive(true);
                gameObject.SetActive(false);
            }
        }else if (Input.GetButtonDown("B") && !bloquer){
            StartCoroutine(Transition());
            StartCoroutine(FermetureDeLaFenêtre());
        }
    }
    IEnumerator Transition(){
        //GameObject.FindWithTag("Fondu").GetComponent<Animator>().SetTrigger("fondu complet");
        yield return null;
    }
    IEnumerator FermetureDeLaFenêtre(){
        yield return new WaitForSeconds(0.01f);
        pause=false;    
        int numéro=indexGauche;
        if (indexHaut==0){numéro+=sélectionnéHaut.Count;}
        windows[numéro].SetActive(false);
    }
    void changerDePosition(){
        if (objetActuel!=null){
            objetActuel.SetActive(false);
        }else {
            sélectionnéHaut[0].SetActive(false);
        }
        if (indexHaut==1){
            objetActuel = sélectionnéHaut[indexGauche];
            objetActuel.SetActive(true);
        }
        if (indexHaut==0){
            objetActuel = sélectionnéBas[indexGauche];
            objetActuel.SetActive(true);
        }
    }
}
