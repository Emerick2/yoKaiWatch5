using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cible : MonoBehaviour{
    public List<GameObject> yoKaiGameObjects;
    public Sprite imageSélection;
    public Sprite imageNonSélection;

    void OnTriggerEnter(Collider other){
        GetComponent<SpriteRenderer>().sprite = imageSélection;
        Debug.Log("vu");
        if (yoKaiGameObjects.Contains(other.gameObject)){
            Debug.Log("trouvé !");
        }
    }
    void OnTriggerExit(Collider other){
        GetComponent<SpriteRenderer>().sprite = imageNonSélection;
        Debug.Log("vu");
        if (yoKaiGameObjects.Contains(other.gameObject)){
            Debug.Log("trouvé !");
        }
    }
}