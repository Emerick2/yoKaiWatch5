using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GestionnaireCombat : MonoBehaviour{
    public List<Image> lesEspacesMédaillonDansLaRoue;
    public List<StructureYoKaiCombat> listeYoKaiAmiSurLeTerrain = new List<StructureYoKaiCombat>();
    public List<StructureYoKaiCombat> listeYoKaiEnnemieSurLeTerrain = new List<StructureYoKaiCombat>();
    private bool autorisationDeTournerLaRoue = true;
    [Range(0,5)]public int positionRotationRoue = 0;
    public int rotationDemander;//on demande la rotation. Elle sera réaliser à la fin du tours complets (quand chaque yo-kai aurras jouer)

    void Start(){
        if (MesYoKai.Instance != null){
            StartCoroutine(CoroutineDeDémarrage());
        }
    }

    IEnumerator CoroutineDeDémarrage(){
        while (!MesYoKai.Instance.finDuChargement || !MesYoKai.Instance.yoKaiDeMonEquipeChargé){
            yield return new WaitForSeconds(0.2f);
        }
        RemplireLaStructureDesYoKaiCombats();
        PlacerLesMédaillonSurLaRoue();
        PlacerLesModèles3DAmiSurLeTerrain();
        PlacerLesModèles3DEnnemieSurLeTerrain();
        SavoirQuiEstDevant();
    }

    void Update(){
        if (Input.GetButtonDown("L")){
            // TourneLaRoue(-1);
            rotationDemander=-1;
        } else if (Input.GetButtonDown("R")){
            // TourneLaRoue(1);
            rotationDemander=1;
        }
    }

    void RemplireLaStructureDesYoKaiCombats(){
        listeYoKaiAmiSurLeTerrain.Clear();
        for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
            int IDspécifique = MesYoKai.Instance.yoKaiDeMonEquipe[i];
            int ID = MesYoKai.Instance.maListe[IDspécifique].ID;
            StructureYoKaiCombat nouv = new StructureYoKaiCombat{};
            nouv.gameObjectDeBase = medaliumDictionnaire._instance.dictionnaire[ID].yoKai;
            nouv.possitionRoue = MesYoKai.Instance.maListe[IDspécifique].positionRoue;
            nouv.estUnAmi = true;
            nouv.ID = ID;
            nouv.IDListeYoKai = IDspécifique;//permet de différancier deux Woko par exemple
            nouv.imageMédaillon = medaliumDictionnaire._instance.dictionnaire[ID].médaillon;
            nouv.vie = medaliumDictionnaire._instance.dictionnaire[ID].vie;
            nouv.vitesse= medaliumDictionnaire._instance.dictionnaire[ID].vitesse;
            nouv.esprit= medaliumDictionnaire._instance.dictionnaire[ID].esprit;
            nouv.force= medaliumDictionnaire._instance.dictionnaire[ID].force;
            nouv.défance= medaliumDictionnaire._instance.dictionnaire[ID].défance;
            listeYoKaiAmiSurLeTerrain.Add(nouv);
        }
    }

    void PlacerLesMédaillonSurLaRoue(){
        foreach (Image item in lesEspacesMédaillonDansLaRoue){
            item.GetComponent<Image>().enabled = false;
        }
        foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
            lesEspacesMédaillonDansLaRoue[item.possitionRoue].enabled = true;
            lesEspacesMédaillonDansLaRoue[item.possitionRoue].GetComponent<Image>().sprite = item.imageMédaillon;
        }
    }

    void PlacerLesModèles3DAmiSurLeTerrain(){}
    void PlacerLesModèles3DEnnemieSurLeTerrain(){}

    void SavoirQuiEstDevant(){
        foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
            item.estDevant = item.possitionRoue == (positionRotationRoue)%6 || item.possitionRoue == (positionRotationRoue+1)%6 || item.possitionRoue == (positionRotationRoue+2)%6;
        }
    }

    void TourneLaRoue(int sense){
        if (autorisationDeTournerLaRoue){
            foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
                item.possitionRoue+=sense;
                if (item.possitionRoue<0){
                    item.possitionRoue = 6+item.possitionRoue;
                }
                item.possitionRoue%=6;
                if (item.possitionRoue<0){
                    item.possitionRoue = 5;//Normalement on ne passeras jamais ici.
                    Debug.LogWarning("La position de la roue à reçus une valeur incorect. Il faut corrigé se problème au plus vite !");
                }
            }
            PlacerLesMédaillonSurLaRoue();
            SavoirQuiEstDevant();
        }
    }

    IEnumerator BoucleDuJeu(){
        while (true){
            if (rotationDemander != 0){
                TourneLaRoue(rotationDemander);
                rotationDemander = 0;
            }
            List<StructureYoKaiCombat> ordreDePassage = new List<StructureYoKaiCombat>();
            foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
                if (item.vie>0) ordreDePassage.Add(item);
            }
            foreach (StructureYoKaiCombat item in listeYoKaiEnnemieSurLeTerrain){
                if (item.vie>0) ordreDePassage.Add(item);
            }
            ordreDePassage = ordreDePassage.OrderByDescending(yokai => yokai.vitesse).ToList();
            foreach (var item in ordreDePassage){
                
            }
        }
    }
}

[System.Serializable]
public class StructureYoKaiCombat{
    public bool estUnAmi = false;
    public GameObject gameObjectDeBase;
    public GameObject gameObjectSurLaScène;
    public Sprite imageMédaillon;
    public int possitionRoue;
    public bool estDevant;
    public int ID;
    public int IDListeYoKai;//permet de différancier deux Woko par exemple
    public int vie;
    public int vitesse;
    public int esprit;
    public int force;
    public int défance;
    public int numéroObjetTennu;
}
