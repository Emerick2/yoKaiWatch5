using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using YKW.Core.Commun;
using YKW.Core.YoKai.Enums;
using YKW.Core.YoKai.Struct;

namespace YKW.Core.YoKai.Class
{
    /// <summary>
    /// Données propres à une espèce de Yo-kai. Source unique pour l'identité,
    /// les statistiques de base, les aptitudes, l'audio et le modèle 3D.
    /// </summary>
    [Serializable]
    public class YoKai
    {
        [Header("Identité & Visuels")]
        [ClampMin(0)] public int ID;

        public string nom;
        [FormerlySerializedAs("nom_anglais")] public string englishName;
        public string Description;
        public Rang rang;
        public Tribue tribue;
        public Personalité personnalité;

        public Sprite imageMédaillon;
        public Sprite imageFace;
        public Sprite faceRondImage;
        public bool rare;

        [Header("Audio")]
        public YoKaiSoundVoiceType Voice;
        public YoKaiSoundSFXType SFX;
        public AudioClip voiceClip;

        [Header("Gameplay de l'espèce")]
        public Talent talent;
        public Technique technique;
        public PlatPréféré platPréféré;
        public YKW.Core.YoKai.Enums.Type type;
        public Ammultime aptitudeAmultime;
        public StatutAmitié envoutement;

        [Header("Statistiques de base de l'espèce")]
        [Min(0)] public int Force;
        [Min(0)] public int Vitesse;
        [Min(0)] public int Esprit;
        [Min(0)] public int Défense;
        [Min(0)] public int PV;

        [Header("Éléments")]
        public Element elementFaiblesse;
        public Element elementResistance;
        public Element monElement;

        [Header("Modèle 3D")]
        public string nomDeMonSquellette;
        public YoKaiModele3D modele3D;
        public GameObject yoKai;

        // Unity ne sérialise PAS les Dictionary<> nativement ; cette API est
        // préservée pour le code existant, mais pas persistée dans les assets.
        public Dictionary<string, string> phrases_nourriture;

        /// <summary>
        /// Copie les données d'espèce vers une instance possédée / de combat.
        /// Le modèle 3D et le dictionnaire (données d'espèce) restent partagés.
        /// </summary>
        public void CopierEspeceDepuis(YoKai source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (ReferenceEquals(this, source)) return;

            ID = source.ID;
            nom = source.nom;
            englishName = source.englishName;
            Description = source.Description;
            rang = source.rang;
            tribue = source.tribue;
            personnalité = source.personnalité;
            imageMédaillon = source.imageMédaillon;
            imageFace = source.imageFace;
            faceRondImage = source.faceRondImage;
            rare = source.rare;
            Voice = source.Voice;
            SFX = source.SFX;
            voiceClip = source.voiceClip;

            talent = source.talent;
            technique = source.technique;
            platPréféré = source.platPréféré;
            type = source.type;
            aptitudeAmultime = source.aptitudeAmultime;
            envoutement = source.envoutement;
            Force = source.Force;
            Vitesse = source.Vitesse;
            Esprit = source.Esprit;
            Défense = source.Défense;
            PV = source.PV;
            elementFaiblesse = source.elementFaiblesse;
            elementResistance = source.elementResistance;
            monElement = source.monElement;
            nomDeMonSquellette = source.nomDeMonSquellette;
            modele3D = source.modele3D;
            yoKai = source.yoKai;
            phrases_nourriture = source.phrases_nourriture;
        }
    }
}
