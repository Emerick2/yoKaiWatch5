using System;
using UnityEngine;
using UnityEngine.UI;

public class EnregistreParamètre : MonoBehaviour{
    public string clef;//2 true et 1 false
    void Start(){
        if (PlayerPrefs.GetInt(clef) == 2){
            GetComponent<Toggle>().isOn = true;
        } else if (PlayerPrefs.GetInt(clef) == 1){
            GetComponent<Toggle>().isOn = false;
        }
    }
    public void Sauvegard(){
        Debug.Log("ici");
        GetComponent<Toggle>().isOn = !GetComponent<Toggle>().isOn;
        int valeur = 1;
        if (GetComponent<Toggle>().isOn){
            valeur = 2;
        }
        PlayerPrefs.SetInt(clef,valeur);
    }
}
