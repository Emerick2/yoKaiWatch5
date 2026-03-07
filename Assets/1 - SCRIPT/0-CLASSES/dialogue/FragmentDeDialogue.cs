using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FragmentDeDialogue{
    public int auteurConditionnelle;
    public int IdÉlément;
    public TypeDialogue type;
    public string phrase;
    public QuiJeSuis quiJeSuis;
    public string nom;
    public Sprite image;
    [Min(1)]public int quantitée = 1;//si ces un cadeau, il y a une quantitée
    public CatégorieItem typeObjet;
    [Header("Opération")]
    public List<Conditionnelle> conditionnelle = new List<Conditionnelle>();
    public bool cesUneExclamation;
    public Sprite imageDePrésentation;
    public List<int> listeProduitBoutique = new List<int>();
    public CatégorieQuête categorieQuete;
    public bool endQuest;
    public QuestMenuInformation infoQuest = new QuestMenuInformation();
}

