using UnityEngine;

[System.Serializable]
public class Annimal{
    public string nom;
    public Sprite image;
    [TextArea(3, 5)] public string description;
    public float prixAchat;
    public float prixVente;
    public bool rare;
    public GameObject modèle3D;
    public string englishName;
}
