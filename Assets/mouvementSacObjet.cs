using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouvementSacObjet : MonoBehaviour
{
    public RectTransform bloc;
    private int IDduPlusVisibleàGauche=5;
    private bool pause = false;
    public List<GameObject> sélectionnéHaut;
    public List<GameObject> sélectionnéMoyen;
    public List<GameObject> sélectionnéBas;
    private int indexHaut = 2;
    private int indexGauche = 0;
    private GameObject objetActuel;
    public RectTransform cible;
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
        BougerLeBloc();
    }
    void BougerLeBloc(){
        if (indexGauche>IDduPlusVisibleàGauche || indexGauche<IDduPlusVisibleàGauche-IDduPlusVisibleàGauche){
            Vector2 direction;
            print("ici");
            while (Vector2.Distance(bloc.anchoredPosition, cible.anchoredPosition) < 5f){
                direction = (cible.anchoredPosition - bloc.anchoredPosition).normalized;
                bloc.anchoredPosition += direction * 1000 * Time.deltaTime;
            }
        }
    }
}
