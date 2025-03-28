using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sacMouvement : MonoBehaviour
{
    private bool pause = false;
    public List<GameObject> sélectionnéHaut;
    public List<GameObject> sélectionnéBas;
    public List<GameObject> windows;
    private int indexHaut = 1;
    private int indexGauche = 0;
    private GameObject objetActuel;
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
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)){
                if (indexHaut == 0){indexHaut=1;}
                else {indexHaut=0;}
                changerDePosition();
            }
            if (Input.GetKeyDown(KeyCode.E)){
                int numéro=indexGauche;
                if (indexHaut==0){numéro+=sélectionnéHaut.Count-1;}
                if (windows[numéro] != null){
                    windows[numéro].SetActive(true);
                    pause=true;
                }
            }
            if (Input.GetKeyDown(KeyCode.B)){
                gameObject.SetActive(false);
            }
        }else if (Input.GetKeyDown(KeyCode.B)){
            pause=false;    
            int numéro=indexGauche;
            if (indexHaut==0){numéro+=sélectionnéHaut.Count-1;}
            windows[numéro].SetActive(false);
        }
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
