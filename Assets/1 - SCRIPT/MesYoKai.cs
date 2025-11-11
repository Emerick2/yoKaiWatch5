using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesYoKai : MonoBehaviour{
    private static MesYoKai _instance;

    public List<ListeDeMesYoKai> maListe = new List<ListeDeMesYoKai>();
    public List<int> yoKaiDeMonEquipe = new List<int>();
    /*[HideInInspector]*/ public bool yoKaiDeMonEquipeChargé = false;
    /*[HideInInspector]*/ public bool finDuChargement;
    
    // Singleton : la propriété Instance est du même type que _instance
    public static MesYoKai Instance {
        get {
            if (_instance == null) {
                GameObject obj = new GameObject("MesYoKai");
                _instance = obj.AddComponent<MesYoKai>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    } 
    private void Awake(){
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }
    public GameObject AvoirLeGameObject(){
        return gameObject;
    }

    void Start(){
        ChargerLesYoKai();
        ChargerMonEquipe();
    }
    public void ChargerMonEquipe(){
        string listeDesID = NewDataCenter.Instance.Lire("yoKaiDansEquipe", "équipe.json");
        yoKaiDeMonEquipe.Clear();
        string nombreTemp = "";

        for (int i = 0; i < listeDesID.Length; i++){
            char ch = listeDesID[i];

            if (char.IsDigit(ch)){
                nombreTemp += ch;
            } else {
                if (nombreTemp.Length > 0) {
                    yoKaiDeMonEquipe.Add(int.Parse(nombreTemp));
                    nombreTemp = "";
                }
            }
        }

        if (nombreTemp.Length > 0) {
            yoKaiDeMonEquipe.Add(int.Parse(nombreTemp));
        }
        finDuChargement = true;
    }
    public void AjouterUnYoKai(int ID2, int niveau2, string personalité2, string surnom2){
        int monIDActuel = int.Parse(NewDataCenter.Instance.Lire("IDdeMesYoKai"));
        NewDataCenter.Instance.SauvegarderINT("IDdeMesYoKai", 1);
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_ID",ID2.ToString());
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_niveau",niveau2.ToString());
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_personalité",personalité2);
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_surnom",surnom2);

        maListe.Add(new ListeDeMesYoKai());
        if(monIDActuel < maListe.Count){
            maListe[monIDActuel].IDUnique=monIDActuel;
            maListe[monIDActuel].ID=ID2;
            maListe[monIDActuel].niveau=niveau2;
            maListe[monIDActuel].personnalité=personalité2;
            maListe[monIDActuel].surnom=surnom2;
        }
    }
    private void ChargerLesYoKai(){
        int monIDActuel = int.Parse(NewDataCenter.Instance.Lire("IDdeMesYoKai"));
        for (int i = 0; i < monIDActuel; i++){
            maListe.Add(new ListeDeMesYoKai());
            maListe[i].IDUnique = i;
            maListe[i].ID  =  int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_ID"));// renvoie une erreur "out of range exeption"
            maListe[i].niveau=int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_niveau"));
            maListe[i].personnalité  =  NewDataCenter.Instance.Lire(i.ToString()+"_personalité");
            maListe[i].surnom      =    NewDataCenter.Instance.Lire(i.ToString()+"_surnom");
        }
        yoKaiDeMonEquipeChargé=true;
    }
}

[System.Serializable]
public class ListeDeMesYoKai{//maListe
    public int IDUnique;
    public int ID;
    public string surnom;
    public int niveau;
    public string personnalité;
}