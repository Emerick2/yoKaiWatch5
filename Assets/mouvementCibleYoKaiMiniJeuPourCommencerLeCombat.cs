using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouvementCibleYoKaiMiniJeuPourCommencerLeCombat : MonoBehaviour{
    public float vitesse = 500f;
    
    private float minXPosition = -5.5f;
    private float maxXPosition = 5.5f;
    private float minYPosition = -2.5f;
    private float maxYPosition = 2.5f;
    private bool bloquer = false;
    private int étape = 0;
    public bool estViséCorrectement;
    void Start(){
        transform.GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(Regarde());
    }

    void Update(){
        if (!bloquer){
            Vector2 position = transform.position;

            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && position.x < maxXPosition){
                position.x += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) && position.x > minXPosition){
                position.x -= vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) && position.y < maxYPosition){
                position.y += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && position.y > minYPosition){
                position.y -= vitesse * Time.deltaTime;
            }
            transform.position = position;
        }
    }
    IEnumerator Regarde(){
        while (étape < 6){
            yield return new WaitForSeconds(0.5f);
            if (estViséCorrectement){
                étape++;
                transform.Find(étape.ToString()).gameObject.SetActive(true);
            }else if (étape>0){
                transform.Find(étape.ToString()).gameObject.SetActive(false);
                étape--;
            }
        }
        DéclancherUnCombat();
    }
    void DéclancherUnCombat(){

    }
    void OnTriggerEnter(Collider collision){
        estViséCorrectement = true;
    }
    void OnTriggerExit(Collider collision){
        estViséCorrectement = false;
    }
}
