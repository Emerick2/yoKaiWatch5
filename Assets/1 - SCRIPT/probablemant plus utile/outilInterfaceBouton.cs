using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outilInterfaceBouton : MonoBehaviour{
    public GameObject interfaceDuYoKaiPad;
    public GameObject boiteDeMenuDeJeu;
    void Update(){
        if (Input.GetButtonDown("X")){
            mouvementDuJoueur.droitDeMarcher = false;
            interfaceDuYoKaiPad.SetActive(true);
            boiteDeMenuDeJeu.SetActive(false);
        }
    }
}
