using UnityEngine;
using YKW.Core.YoKai.Struct;
using UnityEngine.Scripting.APIUpdating;
using YKW.Core.Commun;

namespace YKW.Core.YoKai.Class
{
    [MovedFrom(true, sourceClassName: "ListeDeMesYoKai")]
    public class YoKaiActif : YoKai
    {
        [Header("Identité de l'instance possédée")]
        public int IDUnique;
        public string surnom;
        [Range(0, 5)] public int positionRoue;
        public bool tienUnObjet;
        public int numéroObjetTennu = -1;
        [Header("Objets")]
        public Item.Class.Item item;

        [Header("Statistiques de Base de l'Espèce")]
        public int hp_base; // PV
        public Stats BaseStats;

        [Header("Statistiques Actuelles (Calculées)")]
        public Stats Stats;
        [Clamp(0,99)]
        public int niveau;
        public int xp;

        [Header("Valeurs Individuelles (IV)")]
        public float hp_iv; // PV
        private float IVChecksomme;
        //TODO: Ajoute de Hash du IVChecksomme et method de verification pour éviter la tiche
        public Stats IVStats;

        [Header("Valeurs d'Effort (EV)")]
        public int hp_ev; // PV
        public Stats EVStats;

        public YoKaiActif() {
            AttribuerIV();
            calcNewStats();
        }

        protected void calcNewStats()
        {
            // Formule : ((2 * Base + IV + (EV / 2)) * Niveau) / 100 + 5
            Stats = new Stats(
                ((2 * BaseStats.Force + IVStats.Force + EVStats.Force / 2) * niveau) / 100 + 5,
                ((2 * BaseStats.Vitesse + IVStats.Vitesse + EVStats.Vitesse / 2) * niveau) / 100 + 5,
                ((2 * BaseStats.Défence + IVStats.Défence + EVStats.Défence / 2) * niveau) / 100 + 5,
                ((2 * BaseStats.Esprit + IVStats.Esprit + EVStats.Esprit / 2) * niveau) / 100 + 5
            );
        }
        
        public void AttribuerIV() {
            float pointsHP = 0;
            float pointsSTR = 0;
            float pointsSPR = 0;
            float pointsDEF = 0;
            float pointsSPD = 0;

            for (int i = 0; i < 40; i++) {
                int choixStat = Random.Range(0, 5);

                switch (choixStat) {
                    case 0: pointsHP++; break;
                    case 1: pointsSTR++; break;
                    case 2: pointsSPR++; break;
                    case 3: pointsDEF++; break;
                    case 4: pointsSPD++; break;
                }
            }

            IVStats = new Stats(
                pointsSTR,
                pointsSPR,
                pointsDEF,
                pointsSPD
            );

            hp_iv = pointsHP * 2;

            IVChecksomme = (hp_iv / 2) + IVStats.Force + IVStats.Vitesse + IVStats.Défence + IVStats.Esprit;
            // Debug.Log($"[Génération IV] {nom} généré ! Somme vérifiée : {sommeVerification}/40. (PV IV stocké : {hp_iv})");
        }

        public int NombreDePointXPAGagner(int niveau) {
            int baseCalcule = 15;

            int xpRequis = Mathf.RoundToInt((baseCalcule + ((int)rang * 12)) * Mathf.Pow(niveau, 1.4f));

            return xpRequis;
        }

        public void GagneXP(int nombre){
            if (niveau < 99){
                xp += nombre;
                int aGagner = NombreDePointXPAGagner(niveau+1);
                while (xp > aGagner && niveau < 99){
                    Debug.Log("gagne un niveau !");
                    xp-=aGagner;
                    niveau += 1;
                    aGagner = NombreDePointXPAGagner(niveau+1);
                }
            }
        }
    }
}
