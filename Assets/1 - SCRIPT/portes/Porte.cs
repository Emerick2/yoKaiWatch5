using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Porte : MonoBehaviour{
    public string scèneCible;
    public Vector3 positionCible;
    public bool PréciserUnePosition;
    public bool jeSuisDedans;
    public bool seSeraUnAlléRetours;

    void Update(){
        if (jeSuisDedans && Input.GetButton("A")&& mouvementDuJoueur.droitDeMarcher == true){
            InfoUI._instance.ChangerTexteInfoUI("");
            GameObject TV = new GameObject();
            LivraisonTp TP = TV.AddComponent<LivraisonTp>();
            TP.nomDeLaScèneCible = scèneCible;
            TP.positionPersonnage = positionCible;
            TP.cesUnAlléRetours = seSeraUnAlléRetours;
            TP.nomDeLaScèneDuRetours = SceneManager.GetActiveScene().name;
            TP.positionDuRetours = GameObject.FindWithTag("Player").transform.position;
            SceneManager.LoadScene(scèneCible);
        }
    }
    private void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            jeSuisDedans = true;
            InfoUI._instance.ChangerTexteInfoUI("Appuie sur 'A' pour ouvrir la porte");
        }
    }
    private void OnTriggerExit(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            jeSuisDedans=false;
            InfoUI._instance.ChangerTexteInfoUI("");
        }
    }
}
