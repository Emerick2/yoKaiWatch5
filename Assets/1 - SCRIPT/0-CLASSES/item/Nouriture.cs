using UnityEngine;
using YKW.Core.Item.Enum;

namespace YKW.Core.Item.Class
{
    [System.Serializable]
    public class Nouriture : Item
    {
        public EffetNourritureAllie effetAlié;
        public EffetNourritureEnnemi effetEnemi;
    }
}
