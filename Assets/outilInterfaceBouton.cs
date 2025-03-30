using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outilInterfaceBouton : MonoBehaviour{
    public GameObject interfaceDuYoKaiPad;
    void Update(){
        if (Input.GetKeyDown(KeyCode.X)){
            mouvementDuJoueur.droitDeMarcher = false;
            interfaceDuYoKaiPad.SetActive(true);
        }
    }
}
