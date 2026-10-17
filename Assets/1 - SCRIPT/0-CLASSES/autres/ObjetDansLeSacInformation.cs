using UnityEngine;
using YKW.Core.Item.Class;
using YKW.Core.Item.Enum;

[System.Serializable]
public class ObjetDansLeSacInformation{
    public int quantitée;
    public string nom;
    public Sprite sprite;
    public Nouriture infoNouriture = null;
    public Annimal infoAnimal = null;
    public Équipement infoÉquipement = null;
}
