using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouvementCigalopin : MonoBehaviour{

    private Vector3 positionDepart;
    private Vector3 positionCible;
    private float duree = 2f; // Durée du déplacement
    private float tempsEcoule = 0f;

    int valeur = 2;

    public Transform P1;
    public Transform P2;
    public Transform P3;
    public int position=3;
    public int vitesse = 1;

    void Start(){
        positionDepart = transform.position;//(x,y,z)
        positionCible = new Vector3(0, 0,1);
    }

    void Update(){
        transform.Translate(positionCible * vitesse * Time.deltaTime);
        if (Input.GetButtonDown("flèche gauche")){
            if (position == 3){
                Vector3 nouvellePosition = new Vector3(P2.position.x, transform.position.y, transform.position.z);
                transform.position = nouvellePosition;
            }
        }
    }
}
