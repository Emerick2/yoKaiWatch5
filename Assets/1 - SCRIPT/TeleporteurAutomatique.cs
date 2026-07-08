using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporteurAutomatique : MonoBehaviour{
    public string nameOfScene;
    public int IDTeleport;
    public bool autoTeleportPlayer = true;
    public GameObject theInterface;
    public bool inTheZonne = false;
    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            inTheZonne = true;
            if (autoTeleportPlayer){
                Teleport();
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")){
            inTheZonne = true;
            if (autoTeleportPlayer){
                Teleport();
            }
        }
    }

    void OnTriggerExit(Collider other){
        if (other.CompareTag("Player")){
            inTheZonne = false;
        }
    }

    void OnCollisionExit(Collision other){
        if (other.gameObject.CompareTag("Player")){
            inTheZonne = false;
        }
    }

    void Update(){
        if (inTheZonne && Input.GetButtonDown("A")){
            Teleport();
        }
    }

    void Teleport(){
        bool lancer = false;
        theInterface = GameObject.FindWithTag("interfasse");
        if (theInterface != null){
            theInterface = theInterface.transform.Find("fondu").gameObject;
            if (theInterface != null && theInterface.GetComponent<Animator>() != null){
                theInterface.GetComponent<Animator>().SetBool("fonduActif",true);
                StartCoroutine(CoroutineTeleport(true));
                lancer = true;
            }
        }
        if (!lancer) StartCoroutine(CoroutineTeleport(false));
    }
    IEnumerator CoroutineTeleport(bool attante){
        if (attante) yield return new WaitForSeconds(1f);
        GameObject TV = new GameObject("data of teleporteur");
        DataOfTeleport data = TV.AddComponent<DataOfTeleport>();
        data.IDTeleport = IDTeleport;
        data.nameOfSceneCible = nameOfScene;
        SceneManager.LoadScene(nameOfScene);
    }
}
