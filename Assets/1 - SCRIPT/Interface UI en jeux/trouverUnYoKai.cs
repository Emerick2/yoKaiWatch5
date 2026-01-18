using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trouverUnYoKai : MonoBehaviour{
    /*[HideInInspector]*/ public bool leLieuEstVide = false;
    /*[HideInInspector]*/ public int IDduYoKaiPrésent;
    /*[HideInInspector]*/ public int niveauDuYoKaiPrésent;
    /*[HideInInspector]*/ public Rang rang;

    public bool danLeLieu = false;

    void Update(){
        if (Input.GetButtonDown("A") && danLeLieu && mouvementDuJoueur.droitDeMarcher == true){
            if (!leLieuEstVide){
                GameObject newObject = new GameObject("livraison");
                BaladeDeCombat livraison = newObject.AddComponent<BaladeDeCombat>();
                livraison.Ajouter(IDduYoKaiPrésent, niveauDuYoKaiPrésent);
                livraison.positionJoueur = GameObject.FindWithTag("Player").transform.position; 
                livraison.scèneOriginale = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("visé le yo-kai");
            }
        }
    }

    private void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            danLeLieu = true;
        }
    }
    private void OnTriggerExit(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            danLeLieu = false;
        }
    }
}
