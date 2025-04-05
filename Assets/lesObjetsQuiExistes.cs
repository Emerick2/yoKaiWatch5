using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lesObjetsQuiExistes : MonoBehaviour{
    public List<LesObjets> lesObjets;
    public List<Sprite> imageDesCatégorie;
}

[System.Serializable]
public class LesObjets {
    public Sprite image;
    public string nom;
    public string description;
    public float prix;
    public Catégorie catégorie;
    public string effetSurEnnemi;
    public string effetSurAmi;
}
public enum Catégorie{
    Nouriture,
    Objet,
    Animal,
    Équipement,
    Âme,
    ObjetClef
}