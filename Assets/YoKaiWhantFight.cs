using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoKaiWhantFight : MonoBehaviour{
    void Start() {
        if (gameObject.GetComponent<BoxCollider>() == null){
            gameObject.AddComponent<BoxCollider>();
        }
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            StartCoroutine(PlayerPositionLocally._instance.Teleport("Combat2", true, gameObject));
            // Destroy(this);
        }
    }
}
