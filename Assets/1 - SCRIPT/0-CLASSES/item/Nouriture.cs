using UnityEngine;

[System.Serializable]
public class Nouriture{
    public string nom;
    public Sprite image;
    [TextArea(3, 5)] public string description;
    public float prixAchat;
    public float prixVente;
    [Min(0)] public int IdEffetSurSiSurEnnemie;
    [Min(0)] public int IdEffetSiSurAmi;
}