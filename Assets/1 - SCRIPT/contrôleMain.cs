using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class contrôleMain : MonoBehaviour{
    public GestionnaireDeCombas2 gestionnaireDeCombas;
    public GameObject MainDroiteHaut;//1
    public GameObject MainGaucheHaut;//2
    public GameObject MainDroiteBas;//3
    public GameObject MainGaucheBas;//4
    public RectTransform roue;
    private float rotationSpeed = 200f;
    private bool finDeAttanteDeRotation=true;
    private int ouSuisJe=1;
    public GameObject eventRoue;
    public GameObject eventÂmultime;
    public GameObject eventCible;
    public GameObject eventPurifier;
    public GameObject eventObjet;
    private bool attendChoieAmultime;
    void Update(){
        if(Input.GetButtonDown("flèche droite") || Input.GetButtonDown("flèche gauche")){
            MainDroiteBas.SetActive(false);MainGaucheBas.SetActive(false); MainDroiteHaut.SetActive(false);MainGaucheHaut.SetActive(false); 
            if (ouSuisJe == 1) MainGaucheHaut.SetActive(true);//2
            else if (ouSuisJe == 2) MainDroiteHaut.SetActive(true);//1
            else if (ouSuisJe == 3) MainGaucheBas.SetActive(true);//4
            else if (ouSuisJe == 4) MainDroiteBas.SetActive(true);//3
                
            if (ouSuisJe==1){ouSuisJe=2;}
            else if (ouSuisJe==2) {ouSuisJe=1;}
            else if (ouSuisJe==3){ouSuisJe=4;}
            else {ouSuisJe=3;}

        }else if(Input.GetButtonDown("flèche haut") || Input.GetButtonDown("flèche bas")){ MainDroiteBas.SetActive(false);MainGaucheBas.SetActive(false); MainDroiteHaut.SetActive(false);MainGaucheHaut.SetActive(false); 
            if (ouSuisJe == 1) MainDroiteBas.SetActive(true);//2
            else if (ouSuisJe == 3) MainDroiteHaut.SetActive(true);//4
            else if (ouSuisJe == 2) MainGaucheBas.SetActive(true);//1
            else if (ouSuisJe == 4) MainGaucheHaut.SetActive(true);//3
                
            if (ouSuisJe==1){ouSuisJe=3;}
            else if (ouSuisJe==3) {ouSuisJe=1;}
            else if (ouSuisJe==4){ouSuisJe=2;}
            else {ouSuisJe=4;}
        }else if(Input.GetButtonDown("A")){
            StartCoroutine(StopClic());
            if (ouSuisJe == 1) {MainDroiteHaut.GetComponent<Animator>().SetBool("clic",true); ClicCible();}//1
            else if (ouSuisJe == 2) {MainGaucheHaut.GetComponent<Animator>().SetBool("clic",true); ClicAmultime();}//2
            else if (ouSuisJe == 3) {MainDroiteBas.GetComponent<Animator>().SetBool("clic",true); ClicObjet();}//3
            else if (ouSuisJe == 4) {MainGaucheBas.GetComponent<Animator>().SetBool("clic",true); ClicPurification();}//4
        } else if (Input.GetButtonDown("flèche gauche") && finDeAttanteDeRotation){
            finDeAttanteDeRotation=false;
            gestionnaireDeCombas.RotationDeLaRoue(1);
            StartCoroutine(RotateSmoothly(-60f));
        } else if (Input.GetButtonDown("flèche droite") && finDeAttanteDeRotation){
            finDeAttanteDeRotation=false;
            gestionnaireDeCombas.RotationDeLaRoue(-1);
            StartCoroutine(RotateSmoothly(60f));
        }
    }
    private IEnumerator RotateSmoothly(float angle)
    {
        float startRotation = roue.eulerAngles.z; // Angle de départ
        float targetRotation = startRotation + angle; // Angle cible
        float elapsed = 0f;

        while (elapsed < Mathf.Abs(angle) / rotationSpeed)
        {
            elapsed += Time.deltaTime;
            float currentAngle = Mathf.Lerp(startRotation, targetRotation, elapsed * rotationSpeed / Mathf.Abs(angle));
            roue.eulerAngles = new Vector3(0, 0, currentAngle);
            yield return null; // Attend la prochaine frame
        }

        // Assure que la rotation est exactement de 60°
        roue.eulerAngles = new Vector3(0, 0, targetRotation);
        finDeAttanteDeRotation=true;
    }
    IEnumerator StopClic(){
        yield return new WaitForSeconds(0.1f);
        MainDroiteHaut.GetComponent<Animator>().SetBool("clic",false);//1
        MainGaucheHaut.GetComponent<Animator>().SetBool("clic",false);//2
        MainDroiteBas.GetComponent<Animator>().SetBool("clic",false);//3
        MainGaucheBas.GetComponent<Animator>().SetBool("clic",false);//4
    }
    public void ClicCible(){
        eventRoue.SetActive(false); eventÂmultime.SetActive(false); eventCible.SetActive(true); eventPurifier.SetActive(false);  eventObjet.SetActive(false);
    }
    public void ClicAmultime(){
        foreach (YoKaiPersonnage item in gestionnaireDeCombas.yoKaiJoueurEnVie){
            if (item.jaugeAmultime >=100 && item.estSurLaScène){
                attendChoieAmultime = true;
                gestionnaireDeCombas.imageDesMédaillonDansLaRoue[item.positionActuel].gameObject.transform.Find("sélection").gameObject.SetActive(true);
            }
        }        
        eventRoue.SetActive(false); eventÂmultime.SetActive(true); eventCible.SetActive(false); eventPurifier.SetActive(false);  eventObjet.SetActive(false);
    }
    public void AmultimeFinie(){
        eventÂmultime.GetComponent<jeuAmultime>().Initialisation();
    }
    public void ClicObjet(){
        eventRoue.SetActive(false); eventÂmultime.SetActive(false); eventCible.SetActive(false); eventPurifier.SetActive(false);  eventObjet.SetActive(true);
    }
    public void ClicPurification(){
        eventRoue.SetActive(false); eventÂmultime.SetActive(false); eventCible.SetActive(false); eventPurifier.SetActive(true);  eventObjet.SetActive(false);
    }

    public void ChoieUnYoKai(int position){
        Debug.Log(position);
        foreach (YoKaiPersonnage item in gestionnaireDeCombas.yoKaiJoueur){
            gestionnaireDeCombas.imageDesMédaillonDansLaRoue[item.positionActuel].gameObject.transform.Find("sélection").gameObject.SetActive(false);
        }
        //avent de faire sa, il faut allumer le mini-jeu d'amultime.
        if (attendChoieAmultime){
            gestionnaireDeCombas.UtiliseAmultime(position);
        }
    }
}
