using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using YKW.Core.YoKai.Class;

namespace YKW.Core.YoKai.Class
{
    [System.Serializable]
    [MovedFrom(true, sourceClassName: "MedaliumNonPersonel")]
public class MedaliumYoKai
{
    public int ID;
    public string Nom;
    public string Description;
    public bool rare;
    public Sprite médaillon;
    public Sprite faceImage;
    public Sprite faceRondImage;
    public GameObject yoKai;
    public Rang rang;
    public Tribue tribue;
    public Personalité personnalité;
    public Personalité personalité;
    public Talent talent;
    public Technique technique;
    public PlatPréféré platPréféré;
    public Type type;
    public Ammultime amultime;
    public StatutAmitié envoutement;
    public int Force;
    public int Vitesse;
    public int Esprit;
    public int Défense;
    public int PV;
    public Element elementFaiblesse;
    public Element elementResistance;
    public Element monElement;
    public AudioClip Bruit;
    public string nomDeMonSquellette;
    public Dictionary<string, string> phrasesNourriture = new Dictionary<string, string>();
    public string Tribue;
    public string Rang;
    public YoKaiModele3D modele3D;
}
}
