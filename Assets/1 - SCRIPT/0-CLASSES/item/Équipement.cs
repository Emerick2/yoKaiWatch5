using UnityEngine;
using YKW.Core.Item.Enum;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.Item.Class
{
    [System.Serializable]
    public class Équipement : Item
    {
        public string catégorieDeYoKaiPorteur;
        public CatégorieEquipement categorieEquipement;
        public Stats effet;
    }
}
