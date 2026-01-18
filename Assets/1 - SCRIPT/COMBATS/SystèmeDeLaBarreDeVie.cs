using System.Collections; 
using UnityEngine;
using UnityEngine.UI;

public class SystèmeDeLaBarreDeVie : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private bool EnumeratorDéjatLancer = false; 

    public void initialisationDuGardientDeLaBarreDeVie(int nombreDePointDeVieQueJePossède){
        slider.maxValue = 1f;
        slider.value = nombreDePointDeVieQueJePossède / 100f;
        fill.color = gradient.Evaluate(slider.value);
    }
    public void retirerPointDeVieAvecUnSystèmeQuiFonctionne(int nombreARetirer){
        if (EnumeratorDéjatLancer == false){
            EnumeratorDéjatLancer = true;
            slider.value -= nombreARetirer / 100f;
            if (slider.value < 0f){
                slider.value = 0f;
            }
            fill.color = gradient.Evaluate(slider.value);
            StartCoroutine(attendPourNePasTropMettreDeDégat());
        }
    }
    IEnumerator attendPourNePasTropMettreDeDégat(){
        yield return new WaitForSeconds(0.2f);
        EnumeratorDéjatLancer = false;
    }
}
