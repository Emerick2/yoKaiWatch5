using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireDuJeuDeLaCible : MonoBehaviour{
    public int monID;
    public int monNiveau;
    public Vector3 positionPersonnage;
    public string nomDeLaScèneCible;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
