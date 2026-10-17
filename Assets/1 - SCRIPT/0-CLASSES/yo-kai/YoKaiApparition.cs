using System;
using UnityEngine;
using YKW.Core.Commun;

namespace YKW.Core.YoKai.Class
{
    /// <summary>Configuration d'une apparition, pas une fiche d'espèce.</summary>
    [Serializable]
    public class YoKaiApparition
    {
        // Ne pas rétablir [MovedFrom(sourceClassName: "YoKai")] :
        // YoKai existe maintenant réellement sous ce nom et ce namespace.
        [PrefabOnlyInPath("Assets/Resources/tousLesYoKai")]
        public GameObject leYoKai;
        [Range(0, 100)] public int probabilitéDeApparition;
        [Range(1, 100)] public int niveauMinimum;
        [Range(1, 100)] public int niveauMaximum;
    }
}
