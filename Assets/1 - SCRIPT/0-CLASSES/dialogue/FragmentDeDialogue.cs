using UnityEngine;
using System.Collections.Generic;
using YKW.Core.Item.Enum;

[System.Serializable]
public class FragmentDeDialogue{
    public int auteurConditionnelle;
    public int IdÉlément;
    public TypeDialogue type;
    public string phrase;
    public QuiJeSuis quiJeSuis;
    public string nom;
    [Min(1)]public int quantitée = 1;//si ces un cadeau, il y a une quantitée
    public CatégorieItem typeObjet;
    [Header("Opération")]
    public List<Conditionnelle> conditionnelle = new List<Conditionnelle>();
    public bool cesUneExclamation;
    public List<int> listeProduitBoutique = new List<int>();
    public CatégorieQuête categorieQuete;
    public bool endQuest;
    public QuestMenuInformation infoQuest = new QuestMenuInformation();

    public bool variableAActivée;
    public string imagePath;
    public string imageDePresentationPath;
    [System.NonSerialized] private Sprite _imageRuntime;
    [System.NonSerialized] private Sprite _imageDePresentationRuntime;
    public bool ilDevientNotreAmi = true;
    public string surnomYoKai = "";

    public Sprite image {
        get {
            if (_imageRuntime != null) return _imageRuntime;
            if (string.IsNullOrEmpty(imagePath)) return null;
            return Resources.Load<Sprite>(imagePath);
        }
        set {
            _imageRuntime = value;
        }
    }

    public Sprite imageDePrésentation {
        get {
            if (_imageDePresentationRuntime != null) return _imageDePresentationRuntime;
            if (string.IsNullOrEmpty(imageDePresentationPath)) return null;
            return Resources.Load<Sprite>(imageDePresentationPath);
        }
        set {
            _imageDePresentationRuntime = value;
        }
    }
}

