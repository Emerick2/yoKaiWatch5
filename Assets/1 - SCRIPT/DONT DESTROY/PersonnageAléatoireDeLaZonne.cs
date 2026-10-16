using System.Collections;
using System.Collections.Generic;
using YKW.Core.YoKai.Enums;
using UnityEngine;
using YKW.Core.YoKai.Class;
//using System.ComponentModel;


public class PersonnageAléatoireDeLaZonne : MonoBehaviour{
    public List<YoKaiApparition> yoKai;
    private medaliumDictionnaire médalium;
    [Range(0,100)]public int probabilitéDeAparitionYoKai = 70;

    void Start(){
        médalium = medaliumDictionnaire._instance.AvoirLeGameObject().GetComponent<medaliumDictionnaire>();

        trouverUnYoKai[] composantsTrouvés = FindObjectsOfType<trouverUnYoKai>();
        foreach (trouverUnYoKai composant in composantsTrouvés) {
            composant.leLieuEstVide = true;
            if (probabilitéDeAparitionYoKai <= Random.Range(0,100)){
                bool STOP = false;
                foreach (YoKaiApparition yoKaii in yoKai){
                    if (Random.Range(0, 100) >= yoKaii.probabilitéDeApparition && STOP == false){
                        STOP = true;
                        composant.leLieuEstVide = false;
                        ComplétéUnYoKai(composant, yoKaii);
                    }
                }
            }
        }
    }
    void ComplétéUnYoKai(trouverUnYoKai composant, YoKaiApparition yoKaiRandom){
        if (yoKaiRandom == null || yoKaiRandom.leYoKai == null){
            Debug.LogError("yoKaiRandom ou yoKaiRandom.leYoKai est null !");
            return;
        }
        string pasID = yoKaiRandom.leYoKai.name;
        pasID = pasID.Replace("²", "");
        pasID = pasID.Replace("!", "");
        int ID;
        if (!int.TryParse(pasID, out ID)) {
            Debug.LogError("Le nom du prefab n'est pas un nombre valide : " + pasID);
            return;
        }
        ID--;
        composant.IDduYoKaiPrésent = ID;
        if (médalium == null){
            Debug.LogError("médalium ou médalium.dictionnaire est null !");
            return;
        }
        if (ID < 0 || ID >= médalium.MAX) {
            Debug.LogError("ID hors limites : " + ID);
            return;
        }
        Rang r = médalium.ServantYoKai(ID).rang;
        composant.rang = r;
        composant.niveauDuYoKaiPrésent = Random.Range(yoKaiRandom.niveauMinimum, yoKaiRandom.niveauMaximum);
    }
}
