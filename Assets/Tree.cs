using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tree : MonoBehaviour {
    private bool playerInTheZone = false;
    void Update() {
        if (playerInTheZone && Input.GetButtonDown("A") && mouvementDuJoueur.droitDeMarcher){
            PlayerPositionLocally.Instance.StoreThePlayerPositionLocally(true);
            StartCoroutine(PlayerPositionLocally.Instance.Teleport("attrape-insecte",true));
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            playerInTheZone = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")){
            playerInTheZone = false;
        }
    }
}
