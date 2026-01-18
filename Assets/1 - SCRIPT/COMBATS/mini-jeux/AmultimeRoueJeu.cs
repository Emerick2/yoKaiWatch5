using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmultimeRoueJeu : MonoBehaviour{
    private RectTransform rect;
    public float vitesseRotation = 200f;
    public List<GameObject> lesNuméros;
    private int cible;
    public int nombreDeToursMax=4;
    [HideInInspector] public int nombreDeTours = 0;
    [HideInInspector] public FonctionnementJeuAmultime fonctionnement;
    private bool victoireDéjàAppeler;
    
    public void Initialisation(FonctionnementJeuAmultime f){
        nombreDeTours = 0;
        fonctionnement = f;
        victoireDéjàAppeler = false;
    }
    void Start(){
        rect = gameObject.GetComponent<RectTransform>();
        NouveauTours(true);
    }
    void Update(){
        if (Input.GetButton("L")){
            Vector3 nouvelleRotation = rect.localEulerAngles;
            nouvelleRotation.z += vitesseRotation * Time.deltaTime; 
            rect.localEulerAngles = nouvelleRotation;        
        } else if (Input.GetButton("R")){
            Vector3 nouvelleRotation = rect.localEulerAngles;
            nouvelleRotation.z -= vitesseRotation * Time.deltaTime; 
            rect.localEulerAngles = nouvelleRotation;        
        } else{
            Vector3 nouvelleRotation = rect.localEulerAngles;
            int résulta = PositionSur360degDivPar12((int)nouvelleRotation.z);
            if (CibleSurResultat(cible, résulta)){
                NouveauTours(false);
            }
        }
    }

    bool CibleSurResultat(int cible, int résulta){
        int résulta1 = résulta+1;
        if (résulta1 > lesNuméros.Count){
            résulta1 = 1;
        }
        int résulta2 = résulta-1;
        if (résulta2 <= 0){
            résulta2 = lesNuméros.Count;
        }
        return cible == résulta || cible == résulta1 || cible == résulta2;

    }

    void NouveauTours(bool premierTours){
        if (premierTours == true){
            nombreDeTours = 0;
            fonctionnement.NouvelleValeurSliderImage(0);
        } else {
            nombreDeTours++;
            fonctionnement.NouvelleValeurSliderImage((float)nombreDeTours / nombreDeToursMax);
        } if (nombreDeTours >= nombreDeToursMax) {
            if (!victoireDéjàAppeler){
                fonctionnement.VictoireMiniJeu();
                victoireDéjàAppeler = true;
            }
        } else {
            foreach (GameObject item in lesNuméros){
                item.SetActive(false);
            }
            cible = Random.Range(0,lesNuméros.Count)+1;
            lesNuméros[cible-1].SetActive(true);
        }
    }

    int PositionSur360degDivPar12(int taux){
        int degré = 360;
        int n = lesNuméros.Count;
        int dif = degré/n;
        int total = lesNuméros.Count;
        while (taux > dif){
            taux-=dif;
            total--;
        }
        return total;
    }
}
