using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoKaiWhantFight : MonoBehaviour{
    [Min(0)] public int idYokai1 = 0;
    [Range(1,99)] public int niveauYokai1 = 10;
    [Min(0)] public int idYokai2 = 0;
    [Range(1,99)] public int niveauYokai2 = 10;
    [Min(0)] public int idYokai3 = 0;
    [Range(1,99)] public int niveauYokai3 = 10;
    private bool misseAJoutFaitDansInspecteur = false;

    void OnValidate(){
        if (!misseAJoutFaitDansInspecteur){
            misseAJoutFaitDansInspecteur = true;
            string match = System.Text.RegularExpressions.Regex.Match(gameObject.name, @"\d+").Value;
            if (!string.IsNullOrEmpty(match)) {
                if (int.TryParse(match, out int idTrouve)) {
                    idYokai1 = idTrouve;
                }
            }
        }
    }

    void Start() {
        if (gameObject.GetComponent<BoxCollider>() == null){
            gameObject.AddComponent<BoxCollider>();
        }
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Player")){
            PlayerPositionLocally._instance.idYokai1 = idYokai1;
            PlayerPositionLocally._instance.niveauYokai1 = niveauYokai1;
            PlayerPositionLocally._instance.idYokai2 = idYokai2;
            PlayerPositionLocally._instance.niveauYokai2 = niveauYokai2;
            PlayerPositionLocally._instance.idYokai3 = idYokai3;
            PlayerPositionLocally._instance.niveauYokai3 = niveauYokai3;

            PlayerPositionLocally._instance.StoreThePlayerPositionLocally(false);
            StartCoroutine(PlayerPositionLocally._instance.Teleport("Combat2", true, gameObject));
        }
    }
}
