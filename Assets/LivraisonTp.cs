using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivraisonTp : MonoBehaviour{
    public Vector3 positionPersonnage;
    public string nomDeLaScèneCible;

    private bool aDéjàTéléporté = false;

    void Start(){
        DontDestroyOnLoad(gameObject);
    }

    void Update(){
        if (!aDéjàTéléporté && SceneManager.GetActiveScene().name == nomDeLaScèneCible){
            GameObject joueur = GameObject.FindWithTag("Player");

            if (joueur != null){
                joueur.transform.position = positionPersonnage;
                aDéjàTéléporté = true;
            }
        }
    }
}
