using UnityEngine;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.Item.Enum
{
    [System.Serializable]
    public class Équipement : YKW.Core.Item.Class.Item
    {
        public string catégorieDeYoKaiPorteur;
        public CatégorieEquipement categorieEquipement;
        public Stats effet;
    }
}