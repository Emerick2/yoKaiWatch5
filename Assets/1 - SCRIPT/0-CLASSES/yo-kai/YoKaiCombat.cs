using UnityEngine;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.YoKai.Class
{
    public class YoKaiCombat : YoKai
    {
        [Header("État sur le Terrain")] 
        public bool estUnAmi;
        public GameObject gameObjectSurLaScène;
        public int positionRoue;
        public bool estDevant;
        public bool estEnTrainDeFlâner;
        public bool estEnTrainDeFaireUneGarde;
        public float multiplicateurGarde = 1; // 1/0.5/0.25
        public bool aAttaque;
        public bool aEtePurifie;
        
        [Header("Jauges de Combat")]
        public int hp_me; // PV
        public int hp_max_me; // PV MAX
        public int amultime;//ces un pourcentage

        [Header("États et Envoûtements")] public Stats envoutements;

        public YoKaiCombat()
        {
            hp_max_me = (int)((2 * hp_base + (hp_iv / 2) + hp_ev) * niveau) / 100 + niveau + 10;
            hp_me = hp_max_me; 
        }
    }
}