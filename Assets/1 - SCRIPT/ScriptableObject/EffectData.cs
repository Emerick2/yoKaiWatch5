using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Item System/Effect Data")]
public class EffectData : ScriptableObject{
    public int ID;
    [TextArea(3, 5)] public string description;
}