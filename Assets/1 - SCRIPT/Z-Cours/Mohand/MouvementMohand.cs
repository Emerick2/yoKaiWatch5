using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvementMohand : MonoBehaviour
{
    public int vitesse=2;
    public int vitesseDeCourse = 5;
    public int vitesseDeMarche = 2;
    public GameObject parents;
    private Vector3 directionGauche = new Vector3(1, 0, 0);
    private Vector3 directionDroite = new Vector3(-1, 0, 0);
    private Vector3 directionAvant = new Vector3(0, 0, -1);
    private Vector3 directionArriere = new Vector3(0, 0, 1);

    void Start()
    {        
        parents = new GameObject();
        parents.transform.position = transform.position;
        transform.parent = parents.transform;
        gameObject.AddComponent<Rigidbody>();
        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }



    void Update()
    {
        if(Input.GetButton("right"))
        {
            parents.transform.Translate(directionGauche * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f,90f,0f);
        }
        
        else if(Input.GetButton("left"))
        {
            parents.transform.Translate(directionDroite * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if(Input.GetButton("down"))
        {
            parents.transform.Translate(directionAvant * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if(Input.GetButton("up"))
        {
            parents.transform.Translate(directionArriere * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (Input.GetButton("B"))
        {
            vitesse = vitesseDeCourse;
        }
        else
        {
            vitesse = vitesseDeMarche;
        }
    }
}
