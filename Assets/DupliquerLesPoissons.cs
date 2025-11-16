using System.Collections.Generic;
using UnityEngine;

public class DupliquerLesPoissons : MonoBehaviour{
    public GameObject poisson;
    public Transform cible;
    public List<Transform> listeCible;
    public Sprite sprite;
    public Vector3 vecteur;

    void Start(){
        GameObject cube = new GameObject("mon cube");
        cube.transform.position = cible.position;
        cube.AddComponent<SpriteRenderer>().sprite = sprite;


        Vector3 vecteur = cible.position;

    }

    void Update(){
        if (Input.GetButtonDown("A")){
            Apparition();
        }
        poisson.transform.rotation = Quaternion.Euler(0f, poisson.transform.rotation.y+0.5f, 0f);

    }

    void Apparition(){
        GameObject nouveauPoisson = Instantiate(poisson);
        // partie 1 :
        // if (cible != null){
        //     nouveauPoisson.transform.position = cible.position;
        // }
        // partie 2 :
        if (listeCible.Count > 0){
            cible = listeCible[Random.Range(0,listeCible.Count)];
            nouveauPoisson.transform.position = cible.position;
        }
    }
}

/*
dupliquer : instantiate

créé : create new GameObject

-position
-scale
-parent

*/