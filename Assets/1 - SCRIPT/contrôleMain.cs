using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class contrôleMain : MonoBehaviour
{
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
    void Update(){
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)){ MainDroiteBas.SetActive(false);MainGaucheBas.SetActive(false); MainDroiteHaut.SetActive(false);MainGaucheHaut.SetActive(false); 
            if (ouSuisJe == 1) MainGaucheHaut.SetActive(true);//2
            else if (ouSuisJe == 2) MainDroiteHaut.SetActive(true);//1
            else if (ouSuisJe == 3) MainGaucheBas.SetActive(true);//4
            else if (ouSuisJe == 4) MainDroiteBas.SetActive(true);//3
                
            if (ouSuisJe==1){ouSuisJe=2;}
            else if (ouSuisJe==2) {ouSuisJe=1;}
            else if (ouSuisJe==3){ouSuisJe=4;}
            else {ouSuisJe=3;}

        }else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)){ MainDroiteBas.SetActive(false);MainGaucheBas.SetActive(false); MainDroiteHaut.SetActive(false);MainGaucheHaut.SetActive(false); 
            if (ouSuisJe == 1) MainDroiteBas.SetActive(true);//2
            else if (ouSuisJe == 3) MainDroiteHaut.SetActive(true);//4
            else if (ouSuisJe == 2) MainGaucheBas.SetActive(true);//1
            else if (ouSuisJe == 4) MainGaucheHaut.SetActive(true);//3
                
            if (ouSuisJe==1){ouSuisJe=3;}
            else if (ouSuisJe==3) {ouSuisJe=1;}
            else if (ouSuisJe==4){ouSuisJe=2;}
            else {ouSuisJe=4;}
        }else if(Input.GetKeyDown(KeyCode.Space)){
            if (ouSuisJe == 1) {MainDroiteHaut.GetComponent<Animator>().SetBool("clic",true); ClicCible();}//1
            else if (ouSuisJe == 2) {MainGaucheHaut.GetComponent<Animator>().SetBool("clic",true); ClicAmultime();}//2
            else if (ouSuisJe == 3) {MainDroiteBas.GetComponent<Animator>().SetBool("clic",true); ClicObjet();}//3
            else if (ouSuisJe == 4) {MainGaucheBas.GetComponent<Animator>().SetBool("clic",true); ClicPurification();}//4
            StartCoroutine(StopClic());
        } else if (Input.GetKeyDown(KeyCode.L) && finDeAttanteDeRotation){
            //Vector3 rotation = roue.transform.eulerAngles;
            //rotation.z += 60;
            //roue.transform.eulerAngles = rotation;
            finDeAttanteDeRotation=false;
            StartCoroutine(RotateSmoothly(-60f));
        } else if (Input.GetKeyDown(KeyCode.R) && finDeAttanteDeRotation){
            finDeAttanteDeRotation=false;
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
        eventRoue.SetActive(false); eventÂmultime.SetActive(true); eventCible.SetActive(false); eventPurifier.SetActive(false);  eventObjet.SetActive(false);
    }
    public void ClicObjet(){
        eventRoue.SetActive(false); eventÂmultime.SetActive(false); eventCible.SetActive(false); eventPurifier.SetActive(false);  eventObjet.SetActive(true);
    }
    public void ClicPurification(){
        eventRoue.SetActive(false); eventÂmultime.SetActive(false); eventCible.SetActive(false); eventPurifier.SetActive(true);  eventObjet.SetActive(false);
    }
}
