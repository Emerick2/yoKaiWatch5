using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporteurAutomatique : MonoBehaviour{
    public string nameOfScene;
    public int IDTeleport;
    public bool autoTeleportPlayer = true;
    public bool inTheZonne = false;
    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
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

    void Update(){
        if (inTheZonne && Input.GetButtonDown("A")){
            Teleport();
        }
    }

    void Teleport(){
        GameObject TV = new GameObject("data of teleporteur");
        DataOfTeleport data = TV.AddComponent<DataOfTeleport>();
        data.IDTeleport = IDTeleport;
        data.nameOfSceneCible = nameOfScene;
        SceneManager.LoadScene(nameOfScene);
    }
}
