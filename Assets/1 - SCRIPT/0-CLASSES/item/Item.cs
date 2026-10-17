using UnityEngine;

namespace YKW.Core.Item.Class
{
    [System.Serializable]
    public class Item
    {
        public string nom;
        public string englishName;
        public Sprite image;
        [TextArea(3, 5)] public string description;
        public float prixAchat;
        public float prixVente;
    }
}