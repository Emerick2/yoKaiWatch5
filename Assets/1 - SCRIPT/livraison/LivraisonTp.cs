using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivraisonTp : MonoBehaviour{
    public Vector3 positionPersonnage;
    public string nomDeLaScèneCible;

    private bool aDéjàTéléporté = false;
    public bool cesUnAlléRetours;
    public string nomDeLaScèneDuRetours;
    public Vector3 positionDuRetours;

    void Start(){
        DontDestroyOnLoad(gameObject);
    }

    void Update(){
        if (!aDéjàTéléporté && SceneManager.GetActiveScene().name == nomDeLaScèneCible){
            GameObject joueur = GameObject.FindWithTag("Player");

            if (joueur != null){
                joueur.transform.position = positionPersonnage;
                aDéjàTéléporté = true;
                if (!cesUnAlléRetours) Destroy(gameObject);
            }
        }
    }

    public void InverserLaDirection(){
        positionPersonnage = positionDuRetours;
        nomDeLaScèneCible = nomDeLaScèneDuRetours;
        cesUnAlléRetours = false;
        aDéjàTéléporté = false;
        SceneManager.LoadScene(nomDeLaScèneCible);
    }
}
