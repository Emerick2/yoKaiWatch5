using JetBrains.Annotations;

namespace YKW.Core.YoKai.Struct
{
    [System.Serializable]
    public struct Stats
    {
        public float Force;
        public float Vitesse;
        public float Défence;
        public float Esprit;
        [CanBeNull] public string modificateurAutre;
        
        public Stats(
            float force = 0,
            float vitesse = 0,
            float défense = 0,
            float esprit = 0,
            string modificateurAutre = null)
        {
            Force = force;
            Vitesse = vitesse;
            Défence = défense;
            Esprit = esprit;
            this.modificateurAutre = modificateurAutre;
        }

    }
}