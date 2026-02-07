using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PetitePorte : MonoBehaviour{
    private bool dansLaZonne = false;
    public bool porteCoulisante;
    public bool reculeSurX;
    public bool avanceSurX;
    public bool reculeSurY;
    public bool avanceSurY;
    private float vitesse = 3f;
    private Vector3 ciblePosition;
    private bool ouvert = false;

    void Start(){
        ciblePosition = transform.position;
    }

    void Update(){
        if (dansLaZonne && Input.GetButtonDown("A") && !ouvert){
            ouvert = true;
            BoxCollider bc = gameObject.GetComponent<BoxCollider>();
            float largeurX = bc.bounds.size.x;
            float largeurY = bc.bounds.size.x;
            if (porteCoulisante) {
                if (reculeSurX) ciblePosition.x -= largeurX;
                if (avanceSurX) ciblePosition.x += largeurX;
                if (reculeSurY) ciblePosition.y -= largeurY;
                if (avanceSurY) ciblePosition.y += largeurY;
            }
            TriggerAutoCollider();
        }
        transform.position = Vector3.Lerp(transform.position, ciblePosition, vitesse * Time.deltaTime);
    }
    void OnTriggerEnter(Collider collision){
        if (collision.CompareTag("Player")){
            dansLaZonne = true;
        }
    }
    void OnTriggerExit(Collider collision){
        if (collision.CompareTag("Player")){
            dansLaZonne = false;
        }
    }

    private void TriggerAutoCollider(){
        AutoColliderGenerator generator = GetComponentInParent<AutoColliderGenerator>();
        if (generator != null){
            generator.AddColliders();
        } else {
            Debug.LogWarning("Aucun composant AutoColliderGenerator trouvé dans la hiérarchie parent.");
        }
    }
}
