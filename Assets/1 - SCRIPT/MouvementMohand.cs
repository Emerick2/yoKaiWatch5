using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvementMohand : MonoBehaviour
{
    public int vitesse=2;
    private Vector3 directionGauche = new Vector3(1, 0, 0);
    private Vector3 directionDroite = new Vector3(-1, 0, 0);
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("flèche gauche"))
        {
            transform.Translate(directionGauche * vitesse * Time.deltaTime);
        }
        
        if(Input.GetButton("flèche droite"))
        {
            transform.Translate(directionDroite * vitesse * Time.deltaTime);
        }
    }
}
