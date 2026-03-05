using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestMenuInformation{
    public string name;
    public string description;
    public string objective;
    public int level;
    public Sprite sprite;
    public int xp;
    public List<int> idObjet;
    public bool finish;
}