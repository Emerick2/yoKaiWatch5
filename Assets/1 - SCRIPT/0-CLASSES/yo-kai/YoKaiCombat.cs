using System;
using UnityEngine;
using YKW.Core.Commun;
using YKW.Core.Item.Class;
using YKW.Core.Item.Enum;
using YKW.Core.YoKai.Struct;


namespace YKW.Core.YoKai.Class
{
    public class YoKaiCombat : YoKaiActif
    {
        [SerializeField]
        [Header("État sur le Terrain")] 
        public bool estUnAmi;
        public GameObject gameObjectSurLaScène;
        public bool estDevant;
        public bool estEnTrainDeFlâner;
        public bool estEnTrainDeFaireUneGarde;
        public float multiplicateurGarde = 1; // 1/0.5/0.25
        public bool aAttaque;
        public bool aEtePurifie;
        
        [Header("Jauges de Combat")]
        [Clamp(0, nameof(hp_max_me))]
        public int hp_me; // PV
        public int hp_max_me; // PV MAX

        [Clamp(0, 100)] 
        public int amultime;

        [Header("États et Envoûtements")] public Stats envoutements;

        public YoKaiCombat()
        {
            hp_max_me = (int)((2 * hp_base + (hp_iv / 2) + hp_ev) * niveau) / 100 + niveau + 10;
            hp_me = hp_max_me;
        }

        static public (int DonPV, int DonAme) EffetNourritureAllié(Nouriture nourriture, int HPMax)
        {
            int donPV = (int)nourriture.effetAlié;
            int donAme = (int)nourriture.effetAlié;
            
            switch (nourriture.effetAlié) {
                case EffetNourritureAllie.RestaurePVFixe:
                    donPV = global::Item.Instance.ObtenirValeurEffetAllie(nourriture);
                    break;
                case EffetNourritureAllie.RendTousLesPV:
                    donPV = HPMax;
                    break;
                case EffetNourritureAllie.RecuperePourcentageAme:
                    donAme = global::Item.Instance.ObtenirValeurEffetAllie(nourriture);
                    break;
                default :
                    Debug.LogError("Effet allié de nourriture non traité : " + global::Item.Instance.ObtenirValeurEffetAllie(nourriture));
                    break;
            }
            
            return (donPV, donAme);
        }
    }
}
