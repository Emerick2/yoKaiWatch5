using System;
using UnityEngine;
using UnityEngine.Serialization;
using YKW.Core.Commun;
using YKW.Core.Item.Class;
using YKW.Core.Item.Enum;
using YKW.Core.YoKai.Struct;
using YKW.Core.Events;

namespace YKW.Core.YoKai.Class
{
    /// <summary>Un Yo-kai actif auquel on ajoute uniquement l'état de combat.</summary>
    [Serializable]
    public class YoKaiCombat : YoKaiActif
    {
        [Header("État sur le terrain")]
        public bool estUnAmi;
        public GameObject gameObjectSurLaScène;
        public bool estDevant;
        public bool estEnTrainDeFlâner;
        public bool estEnTrainDeFaireUneGarde;
        public float multiplicateurGarde = 1;
        public bool aAttaque;
        public bool aEtePurifie;

        [Header("Jauges de combat")]
        [SerializeField, FormerlySerializedAs("hp_me")]
        private int hpMe;

        public int hp_me
        {
            get { return hpMe; }
            set
            {
                int clamped = Mathf.Clamp(value, 0, hp_max_me);
                if (hpMe == clamped) return;
                int previous = hpMe;
                hpMe = clamped;
                EventBus.Publish(new ValueChangedEvent<int>(this, nameof(hp_me), previous, hpMe));
            }
        }

        public int hp_max_me;
        [Clamp(0, 100)] public int amultime;

        [Header("États et envoûtements")]
        public Stats envoutements;

        /// <summary>
        /// Initialise correctement les PV après le choix de l'espèce et du niveau.
        /// Le constructeur sans argument, exécuté avant l'initialisation, ne suffit pas.
        /// </summary>
        public void InitialiserCombatDepuis(YoKai espece, int niveauInitial)
        {
            InitialiserDepuis(espece, niveauInitial);
            RecalculerPVMax(true);
        }

        /// <summary>
        /// Passage d'un Yo-kai de l'équipe au combat : conserve ses IV,
        /// ses EV, son niveau, son XP et son équipement.
        /// </summary>
        public void InitialiserCombatDepuis(YoKaiActif possede)
        {
            CopierInstanceDepuis(possede);
            RecalculerPVMax(true);
        }

        public void RecalculerPVMax(bool restaurerTousLesPV = false)
        {
            int ancienneValeurMax = hp_max_me;
            hp_max_me = Mathf.Max(1, (int)((2 * PV + hp_iv / 2 + hp_ev) * niveau / 100) + niveau + 10);
            hp_me = restaurerTousLesPV
                ? hp_max_me
                : Mathf.Clamp(hp_me + hp_max_me - ancienneValeurMax, 0, hp_max_me);
        }

        public void ModifierPV(int variation) => hp_me += variation;

        public static (int DonPV, int DonAme) EffetNourritureAllié(Nouriture nourriture, int HPMax)
        {
            int donPV = (int)nourriture.effetAlié;
            int donAme = (int)nourriture.effetAlié;

            switch (nourriture.effetAlié)
            {
                case EffetNourritureAllie.RestaurePVFixe:
                    donPV = global::Item.Instance.ObtenirValeurEffetAllie(nourriture);
                    break;
                case EffetNourritureAllie.RendTousLesPV:
                    donPV = HPMax;
                    break;
                case EffetNourritureAllie.RecuperePourcentageAme:
                    donAme = global::Item.Instance.ObtenirValeurEffetAllie(nourriture);
                    break;
                default:
                    Debug.LogError("Effet allié de nourriture non traité : " + global::Item.Instance.ObtenirValeurEffetAllie(nourriture));
                    break;
            }

            return (donPV, donAme);
        }
    }
}
