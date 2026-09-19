using UnityEngine;

[System.Serializable]
public class Équipement{
    public string nom;
    public Sprite sprite;
    [TextArea(3, 5)] public string description;
    public string catégorieDeYoKaiPorteur;
    public float prixAchat;
    public float prixVente;
    public int effetSur_Force;
    public int effetSur_Vitesse;
    public int effetSur_Défance;
    public int effetSur_Esprit;
    public string effetSur_ELSE;
    public string englishName;
}
