using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeDisparaitPas : MonoBehaviour{
    public string aTransporter1;
    public string aTransporter2;
    public string aTransporter3;
    public string aTransporter4;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    public void SeDétruire(){
        Destroy(gameObject);
    }
}
