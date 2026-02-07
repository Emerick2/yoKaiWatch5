using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporteurAutomatique : MonoBehaviour{
    public string nameOfScene;
    public int IDTeleport;
    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            GameObject TV = new GameObject("data of teleporteur");
            DataOfTeleport data = TV.AddComponent<DataOfTeleport>();
            data.IDTeleport = IDTeleport;
            data.nameOfSceneCible = nameOfScene;
            SceneManager.LoadScene(nameOfScene);
        }
    }
}
