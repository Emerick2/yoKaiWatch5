using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FragmentDeDialogue{
    public int conditionalAuthor;
    public int idElement;
    public TypeDialogue type;
    public string sentence;
    public QuiJeSuis whoIAm;
    public string name;
    [Min(1)]public int quantity = 1;//si ces un cadeau, il y a une quantitée
    public CatégorieItem objectType;
    [Header("Opération")]
    public List<Conditionnelle> conditional = new List<Conditionnelle>();
    public bool theseAreAnExclamations;
    public List<int> storeProductList = new List<int>();
    public CatégorieQuête questCategory;
    public bool endQuest;
    public QuestMenuInformation infoQuest = new QuestMenuInformation();

    public bool variableAEnabled;
    public string imagePath;
    public string presentationImagePath;
    [System.NonSerialized] private Sprite _imageRuntime;
    [System.NonSerialized] private Sprite _imageDePresentationRuntime;
    public bool heBecomesOurFriend = true;

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
            if (string.IsNullOrEmpty(presentationImagePath)) return null;
            return Resources.Load<Sprite>(presentationImagePath);
        }
        set {
            _imageDePresentationRuntime = value;
        }
    }
}

