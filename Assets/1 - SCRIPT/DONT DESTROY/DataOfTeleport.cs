using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class DataOfTeleport : MonoBehaviour{
    public int IDTeleport;
    public string nameOfSceneCible;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    void Update(){
        if (nameOfSceneCible == SceneManager.GetActiveScene().name){
            Debug.Log("A fonctionner");
            GameObject player = GameObject.FindWithTag("Player");

            List<JustePoin> points = new List<JustePoin>(Object.FindObjectsByType<JustePoin>(FindObjectsSortMode.None));
            foreach (JustePoin item in points){
                if (item.ID == IDTeleport){
                    Vector3 positionTeleporteur = item.transform.position;
                    if (player != null) {
                        player.transform.position = positionTeleporteur;
                    }
                    Destroy(gameObject);
                }
            }
            Destroy(gameObject);
        }
    }
}