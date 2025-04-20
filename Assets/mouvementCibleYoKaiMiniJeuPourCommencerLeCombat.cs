using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mouvementCibleYoKaiMiniJeuPourCommencerLeCombat : MonoBehaviour{
    public float vitesse = 500f;
    
    private float minXPosition = -5.5f;
    private float maxXPosition = 5.5f;
    private float minZPosition = -2.5f;
    private float maxZPosition = 2.5f;
    private bool bloquer = false;
    private int étape = 0;
    public bool estViséCorrectement;
    public GameObject écranDeChargement;
    void Start(){
        transform.GetComponent<Rigidbody>().isKinematic = true;

        StartCoroutine(Regarde());
    }

    void Update(){
        if (!bloquer){
            Vector3 position = transform.position;

            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && position.x < maxXPosition){
                position.x += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) && position.x > minXPosition){
                position.x -= vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) && position.z < maxZPosition){
                position.z += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && position.z > minZPosition){
                position.z -= vitesse * Time.deltaTime;
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
        StartCoroutine(DéclancherUnCombat());
    }
    IEnumerator DéclancherUnCombat(){
        écranDeChargement.SetActive(true);
        GameObject newObject = new GameObject("livraison");
        livraison Livraison = newObject.AddComponent<livraison>();
        mouvementAléatoireYoKAi data = GameObject.Find("yo-kai").GetComponent<mouvementAléatoireYoKAi>();
        Livraison.ajouterUneValeur(data.monID,data.monNiveau);//ID puis niveau
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("combat");
    }
    void OnTriggerStay(){
        estViséCorrectement = true;
    }
    void OnTriggerExit(){
        estViséCorrectement = false;
    }
}
