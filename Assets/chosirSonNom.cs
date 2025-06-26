using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class chosirSonNom : MonoBehaviour
{
    public GameObject boutonContinuer;
    public Text texteEcrit;
    private bool leBoutonDoitÊtreAfficher;
    string choie;
    void Update(){
        choie = texteEcrit.text;
        leBoutonDoitÊtreAfficher = false;
        if (choie.Length > 2 && choie.Length <= 16){
            bool espaceValidé = false;
            for (int i = 0; i < choie.Length; i++){
                if (choie[i] != ' '){
                    espaceValidé = true;
                }
            }
            if (espaceValidé){
                leBoutonDoitÊtreAfficher = true;
            }
        }
        if (leBoutonDoitÊtreAfficher){
            boutonContinuer.SetActive(true);
        }else{
            boutonContinuer.SetActive(false);
        }/*
        if ((Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.E)) && leBoutonDoitÊtreAfficher){
            EnregistrerLeNom();
        }*/
    }
    public void EnregistrerLeNom(){
        NewDataCenter.Instance.SauvegarderSTR("nomDuJoueur", choie);
        print("enregistrer");
        StartCoroutine(ChangementScène("maison du héro"));
    }
    
    IEnumerator ChangementScène(string scène) {        
        GameObject.FindWithTag("Fondu").GetComponent<Animator>().SetTrigger("faireLeFondu");
        yield return new WaitForSeconds(0.40f);
        SceneManager.LoadScene(scène);
    }
}
