using UnityEngine;
using UnityEngine.UI;

public class AmultimeTourneJeu : MonoBehaviour{
    private RectTransform rect;
    public float vitesseInitiale;
    public float vitesseMaximum;
    [HideInInspector] public float vitesse;
    private bool cesR;
    [HideInInspector] public FonctionnementJeuAmultime fonctionnement;
    private bool victoireDéjàAppeler;
    
    public void Initialisation(FonctionnementJeuAmultime f){
        fonctionnement = f;
        vitesse = 0;
        victoireDéjàAppeler = false;
    }

    void Start(){
        rect = gameObject.GetComponent<RectTransform>();
    }

    void Update(){
        if (vitesse != vitesseMaximum){
            if (Input.GetButton("L")){
                cesR = false;
                Vector3 nouvelleRotation = rect.localEulerAngles;
                nouvelleRotation.z += vitesse * Time.deltaTime; 
                rect.localEulerAngles = nouvelleRotation;
                vitesse += 2;        
            } else if (Input.GetButton("R")){
                cesR = true;
                Vector3 nouvelleRotation = rect.localEulerAngles;
                nouvelleRotation.z -= vitesse * Time.deltaTime; 
                rect.localEulerAngles = nouvelleRotation;        
                vitesse += 2;
            }
            else{
                vitesse = vitesseInitiale;
            }
            fonctionnement.NouvelleValeurSliderImage(vitesse / vitesseMaximum);
        } else {
            Vector3 nouvelleRotation = rect.localEulerAngles;
            if (cesR){
                nouvelleRotation.z -= vitesse * Time.deltaTime; 
            } else {
                nouvelleRotation.z += vitesse * Time.deltaTime;    
            }
            rect.localEulerAngles = nouvelleRotation;    
            if (!victoireDéjàAppeler){
                fonctionnement.VictoireMiniJeu();
                victoireDéjàAppeler = true;
            }  
        }
    }
}

