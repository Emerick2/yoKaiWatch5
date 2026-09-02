using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace YKW.Core.YoKai.Class
{
    [System.Serializable]
    [MovedFrom(true, sourceClassName: "YoKai")]
    public class YoKaiApparition
    {
        [PrefabOnlyInPath("Assets/Resources/tousLesYoKai")]
        public GameObject leYoKai;
        [Range(0, 100)] public int probabilitéDeApparition;
        [Range(1, 100)] public int niveauMinimum;
        [Range(1, 100)] public int niveauMaximum;
    }
}
