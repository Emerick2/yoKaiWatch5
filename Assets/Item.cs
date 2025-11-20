using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour{
    public static Item _instance;
    public List<EffetNouriture> listeDesEffetsNouritures;
    public List<Nouriture> listeDesNouritures;

    private void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }
}

/*
Nouriture
Objet
Annimal
Équipement
ObjetClef
Âme
*/

[System.Serializable]
public class Nouriture{
    public string nom;
    public Sprite image;
    [TextArea(3, 5)] public string description;
    public int prixAchat;
    public int prixVente;
    [Min(0)] public int IdEffetSurSiSurEnnemie;
    [Min(0)] public int IdEffetSiSurAmi;
}



[System.Serializable]
public class EffetNouriture{
    [TextArea(3, 5)] public string description;
}