using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lesObjetsQuiExistes : MonoBehaviour{
    public static lesObjetsQuiExistes Instance;
    public List<LesObjets> lesObjets;
    public List<Sprite> imageDesCatégorie;
    void Awake(){
        if (Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }
    public LesObjets TrouverUnObjetAvecSonNom(string nom){
        foreach (LesObjets item in lesObjets){
            if (item.nom == nom){
                return item;
            }
        }
        return null;
    }
    public LesObjets TrouverUnObjetAvecSonImage(Sprite image){
        foreach (LesObjets item in lesObjets){
            if (item.image == image){
                return item;
            }
        }
        return null;
    }
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