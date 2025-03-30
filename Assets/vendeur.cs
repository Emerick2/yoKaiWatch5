using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vendeur : MonoBehaviour{
    public GameObject blocDeBoutique;
    private bool dansLaZonne;
    void Update(){
        if(dansLaZonne && Input.GetKeyDown(KeyCode.E)){
            mouvementDuJoueur.droitDeMarcher = false;
            blocDeBoutique.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider collision){
        if(collision.CompareTag("Player")){
            dansLaZonne=true;
        }
    }
    void OnTriggerExit(Collider collision){
        if(collision.CompareTag("Player")){
            dansLaZonne=false;
        }
    }
}
