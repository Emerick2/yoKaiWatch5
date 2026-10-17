using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;
using YKW.Core.Commun;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.YoKai.Class
{
    /// <summary>Une instance possédée : niveau, XP, IV, EV, objet et roue.</summary>
    [Serializable]
    [MovedFrom(true, sourceClassName: "ListeDeMesYoKai")]
    public class YoKaiActif : YoKai, ISerializationCallbackReceiver
    {
        [Header("Identité de l'instance possédée")]
        public int IDUnique;
        public int IDListeYoKai;
        public string surnom;
        [Range(0, 5)] public int positionRoue;
        public bool tienUnObjet;
        public int numéroObjetTennu = -1;

        [Header("Objets")]
        public Item.Class.Item item;

        // Les statistiques de BASE sont héritées de YoKai : PV/Force/...
        // Seules les statistiques calculées et la progression sont propres
        // à cette instance.
        [Header("Statistiques actuelles (calculées)")]
        public Stats Stats;
        [Clamp(0, 99)] public int niveau;
        public int xp;

        [Header("Valeurs individuelles (IV)")]
        public float hp_iv;
        private float IVChecksomme;
        public Stats IVStats;

        [Header("Valeurs d'effort (EV)")]
        public int hp_ev;
        public Stats EVStats;

        // Ces deux anciens champs n'ont plus aucun rôle dans le modèle actif.
        // Ils ne servent qu'à restaurer les données sérialisées avant migration.
        [SerializeField, HideInInspector, FormerlySerializedAs("hp_base")]
        private int ancienHpBase;

        [SerializeField, HideInInspector, FormerlySerializedAs("BaseStats")]
        private Stats anciennesStatsBase;

        // Keep construction free of Unity APIs: Unity may create serialized
        // instances on a worker thread. Call InitialiserDepuis or AttribuerIV
        // explicitly when creating a playable instance.
        public YoKaiActif() { }

        /// <summary>
        /// Crée une instance depuis une fiche d'espèce YoKai.
        /// </summary>
        public void InitialiserDepuis(YoKai espece, int niveauInitial)
        {
            CopierEspeceDepuis(espece);
            niveau = Mathf.Clamp(niveauInitial, 1, 99);
            xp = 0;
            AttribuerIV();
            calcNewStats();
        }

        /// <summary>
        /// Copie une instance déjà possédée sans regénérer ses IV/EV/XP.
        /// Sert en particulier à construire un YoKaiCombat depuis l'équipe.
        /// </summary>
        public void CopierInstanceDepuis(YoKaiActif source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (ReferenceEquals(this, source)) return;

            CopierEspeceDepuis(source);
            IDUnique = source.IDUnique;
            IDListeYoKai = source.IDListeYoKai;
            surnom = source.surnom;
            positionRoue = source.positionRoue;
            tienUnObjet = source.tienUnObjet;
            numéroObjetTennu = source.numéroObjetTennu;
            item = source.item;
            Stats = source.Stats;
            niveau = source.niveau;
            xp = source.xp;
            hp_iv = source.hp_iv;
            IVChecksomme = source.IVChecksomme;
            IVStats = source.IVStats;
            hp_ev = source.hp_ev;
            EVStats = source.EVStats;
        }

        public void RecalculerStats() => calcNewStats();

        protected void calcNewStats()
        {
            Stats = new Stats(
                ((2 * Force + IVStats.Force + EVStats.Force / 2) * niveau) / 100 + 5,
                ((2 * Vitesse + IVStats.Vitesse + EVStats.Vitesse / 2) * niveau) / 100 + 5,
                ((2 * Défense + IVStats.Défence + EVStats.Défence / 2) * niveau) / 100 + 5,
                ((2 * Esprit + IVStats.Esprit + EVStats.Esprit / 2) * niveau) / 100 + 5
            );
        }

        public void AttribuerIV()
        {
            float pointsHP = 0;
            float pointsSTR = 0;
            float pointsSPR = 0;
            float pointsDEF = 0;
            float pointsSPD = 0;

            for (int i = 0; i < 40; i++)
            {
                switch (UnityEngine.Random.Range(0, 5))
                {
                    case 0: pointsHP++; break;
                    case 1: pointsSTR++; break;
                    case 2: pointsSPR++; break;
                    case 3: pointsDEF++; break;
                    case 4: pointsSPD++; break;
                }
            }

            // Constructeur Stats : Force, Vitesse, Défence, Esprit.
            IVStats = new Stats(pointsSTR, pointsSPD, pointsDEF, pointsSPR);
            hp_iv = pointsHP * 2;
            IVChecksomme = hp_iv / 2 + IVStats.Force + IVStats.Vitesse + IVStats.Défence + IVStats.Esprit;
        }

        public int NombreDePointXPAGagner(int niveauCible)
        {
            const int baseCalcule = 15;
            return Mathf.RoundToInt((baseCalcule + (int)rang * 12) * Mathf.Pow(niveauCible, 1.4f));
        }

        public void GagneXP(int nombre)
        {
            if (niveau >= 99 || nombre <= 0) return;
            xp += nombre;
            int ancienNiveau = niveau;
            int aGagner = NombreDePointXPAGagner(niveau + 1);
            while (xp >= aGagner && niveau < 99)
            {
                xp -= aGagner;
                niveau++;
                aGagner = NombreDePointXPAGagner(niveau + 1);
            }
            if (niveau != ancienNiveau) calcNewStats();
        }

        public void OnAfterDeserialize()
        {
            // Migrer seulement lorsqu'aucune nouvelle statistique d'espèce
            // n'a encore été renseignée. Ne jamais écraser les nouvelles.
            if (PV == 0 && ancienHpBase > 0) PV = ancienHpBase;

            if (Force == 0 && Vitesse == 0 && Défense == 0 && Esprit == 0 &&
                (anciennesStatsBase.Force != 0 || anciennesStatsBase.Vitesse != 0 ||
                 anciennesStatsBase.Défence != 0 || anciennesStatsBase.Esprit != 0))
            {
                Force = (int)anciennesStatsBase.Force;
                Vitesse = (int)anciennesStatsBase.Vitesse;
                Défense = (int)anciennesStatsBase.Défence;
                Esprit = (int)anciennesStatsBase.Esprit;
            }

            ancienHpBase = 0;
            anciennesStatsBase = default;
        }

        public void OnBeforeSerialize()
        {
            // Migration à sens unique, pas de copie miroir des statistiques.
        }
    }
}
