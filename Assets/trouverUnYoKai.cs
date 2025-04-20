using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trouverUnYoKai : MonoBehaviour{
    private bool leLieuEstVide = false;
    private int IDduYoKaiPrésent;
    private int niveauDuYoKaiPrésent;
    private string rang;

    public bool danLeLieu = false;

    void Update(){
        if (Input.GetKeyDown(KeyCode.E) && danLeLieu){
            if (!leLieuEstVide){
                GameObject newObject = new GameObject("livraison");
                GestionnaireDuJeuDeLaCible livraison = newObject.AddComponent<GestionnaireDuJeuDeLaCible>();
                //revenir
                livraison.monID = IDduYoKaiPrésent;
                livraison.monNiveau = niveauDuYoKaiPrésent;
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
