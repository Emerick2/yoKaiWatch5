using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireDuJeuDeLaCible : MonoBehaviour{
    public int monID;
    public int monNiveau;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
}
