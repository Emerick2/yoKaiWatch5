using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Coffre : MonoBehaviour{
    public List<string> objetPossible;
    public bool seCoffreNePeutÊtreOuvertQueUneSeulFois;
    private bool dansLaZonne;
    private bool déjaOuvert;
    private string nomDeMonID;
    void Start(){
        nomDeMonID = SceneManager.GetActiveScene().name;
        nomDeMonID += "_"+transform.position.x.ToString()+"_"+transform.position.z.ToString();
        if (NewDataCenter.Instance.Lire(nomDeMonID) == "1" && seCoffreNePeutÊtreOuvertQueUneSeulFois){
            déjaOuvert = true;
            transform.GetComponent<Animator>().SetBool("déjaOuvert",true);
        }
    }
    void Update(){
        if(dansLaZonne && Input.GetKeyDown(KeyCode.E) && déjaOuvert == false){
            transform.GetComponent<Animator>().SetBool("ouverture",true);
            string nom = objetPossible[Random.Range(0,objetPossible.Count)];
            NewDataCenter.Instance.SauvegarderINT(nom, 1);
            NewDataCenter.Instance.SauvegarderSTR(nomDeMonID, "1");
            déjaOuvert = true;
        }
    }
    void OnTriggerEnter(Collider collision){
        if(collision.CompareTag("Player")){
            dansLaZonne=true;
        }
    }
    void OnTriggerExit(Collider collision){
        if(collision.CompareTag("Player")){
            dansLaZonne=false;
        }
    }
}
