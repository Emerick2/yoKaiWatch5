using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.ComponentModel;


public class PersonnageAléatoireDeLaZonne : MonoBehaviour{
    public List<YoKai> yoKai;
    public medaliumDictionnaire médalium;

    void Awake(){
        trouverUnYoKai[] composantsTrouvés = FindObjectsOfType<trouverUnYoKai>();
        foreach (trouverUnYoKai composant in composantsTrouvés) {
            if (Random.Range(0,100) > 70){
                bool STOP = false;
                foreach (YoKai yoKaii in yoKai){
                    if (Random.Range (0,100)>=yoKaii.probabilitéDeApparition && STOP==false){
                        STOP = true;
                        ComplétéUnYoKai(composant, yoKaii);
                    }
                }
            }else{
                composant.leLieuEstVide = true;
            }
        }
    }
    void ComplétéUnYoKai(trouverUnYoKai composant, YoKai yoKaiRandom){
        string pasID = yoKaiRandom.leYoKai.name;
        pasID = pasID.Replace("²", "");
        pasID = pasID.Replace("!", "");
        int ID = int.Parse(pasID);
        ID --;
        composant.IDduYoKaiPrésent = ID;
        if (médalium.dictionnaire.Count > ID){
            Rang r = médalium.dictionnaire[ID].rang;
            composant.rang = r.ToString();
            composant.niveauDuYoKaiPrésent = Random.Range(yoKaiRandom.niveauMinimum,yoKaiRandom.niveauMaximum);
        }
    }
}

[System.Serializable]
public class YoKai{
    [PrefabOnlyInPath("Assets/Resources/tousLesYoKai")]
    public GameObject leYoKai;
    [Range(0,100)] public int probabilitéDeApparition;
    [Range(1,100)] public int niveauMinimum;
    [Range(1,100)] public int niveauMaximum;
}
