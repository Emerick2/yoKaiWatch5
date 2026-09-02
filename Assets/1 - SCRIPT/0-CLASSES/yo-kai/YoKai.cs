using UnityEngine;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.YoKai.Class
{
    [System.Serializable]
    public class YoKai
    {
        [Header("Identité & Visuels")]
        public int ID;
        public int IDListeYoKai;//permet de différancier deux Woko par exemple
        public string nom;
        public Rang rang;
        public Tribue tribue;
        public Personalité personnalité;
        public Personalité personalité;
        public Sprite imageMédaillon;
        public Sprite imageFace;

    }
}
