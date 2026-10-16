using System.Collections.Generic;
using YKW.Core.YoKai.Enums;
using UnityEngine;
using YKW.Core.YoKai.Class;

public class MesYoKai : MonoBehaviour{
    private static MesYoKai _instance;

    public List<YoKaiActif> maListe = new List<YoKaiActif>();
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
            if (transform.parent != null) transform.SetParent(null, true);
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
        string listeDesPosition = NewDataCenter.Instance.Lire("yoKaiDansEquipePosition", "équipe.json");

        yoKaiDeMonEquipe.Clear();
        // NewDataCenter returns "0" for a missing key. Treat that default as
        // an empty team; a real team containing roster index 0 is stored as "0%".
        yoKaiDeMonEquipe = string.IsNullOrWhiteSpace(listeDesID) || listeDesID.Trim() == "0"
            ? new List<int>()
            : TransformerUneListeStringEnUneListeInt(listeDesID);
        List<int> lesPositions = yoKaiDeMonEquipe.Count == 0
            ? new List<int>()
            : TransformerUneListeStringEnUneListeInt(listeDesPosition);
        //avent tous :
        // 1 - vérifier que le nombre est sufisant :
        if (yoKaiDeMonEquipe.Count != lesPositions.Count){
            Debug.Log("Problème de position lesPositions dans équipe");
            lesPositions.Clear();
            for (int i = 0; i < yoKaiDeMonEquipe.Count; i++){
                lesPositions.Add(i);
            }
        }
        // 2 - vérifier que les positions ne sont pas répété :
        // 2 - vérifier que les positions ne sont pas répété...
        int nombreDeYoKai = yoKaiDeMonEquipe.Count;
        List<int> positionsTrouvees = new List<int>();
        List<int> indicesDesDoublons = new List<int>();
        for (int i = 0; i < lesPositions.Count; i++){
            int positionActuelle = lesPositions[i];
            if (positionActuelle >= 0 && positionActuelle < 6 && !positionsTrouvees.Contains(positionActuelle)){
                positionsTrouvees.Add(positionActuelle);
            } else{
                indicesDesDoublons.Add(i);
            }
        }
        List<int> positionsManquantes = new List<int>();
        for (int i = 0; i < nombreDeYoKai; i++){
            if (!positionsTrouvees.Contains(i)){
                positionsManquantes.Add(i);
            }
        }

        for (int i = 0; i < indicesDesDoublons.Count; i++){
            int indexADupliquer = indicesDesDoublons[i];
            int nouvellePosition = positionsManquantes[i];
            lesPositions[indexADupliquer] = nouvellePosition;
        }

        // 3 - appliquer les positions au yo-kai :
        for (int i = 0; i < yoKaiDeMonEquipe.Count; i++){
            if (lesPositions.Count > i && maListe.Count > yoKaiDeMonEquipe[i]){
                maListe[yoKaiDeMonEquipe[i]].positionRoue = lesPositions[i];
            }
        }
        
        //List<int> yoKaiDeMonEquipePosition
        finDuChargement = true;
    }

    List<int> TransformerUneListeStringEnUneListeInt(string listeChainer){//"12%68%4%0%7" en [12,68,4,0,7]
        List<int> maListe2 = new List<int>();
        string nombreTemp = "";
        for (int i = 0; i < listeChainer.Length; i++){
            char ch = listeChainer[i];
            if (char.IsDigit(ch)){
                nombreTemp += ch;
            } else {
                if (nombreTemp.Length > 0) {
                    maListe2.Add(int.Parse(nombreTemp));
                    nombreTemp = "";
                }
            }
        }
        if (nombreTemp.Length > 0) {
            maListe2.Add(int.Parse(nombreTemp));
        }
        return maListe2;
    }
    public void AjouterUnYoKai(int ID2, int niveau2, string personalité2, string surnom2, int xp2){
        int monIDActuel = int.Parse(NewDataCenter.Instance.Lire("IDdeMesYoKai"));
        NewDataCenter.Instance.SauvegarderINT("IDdeMesYoKai", 1);
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_ID",ID2.ToString());
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_niveau",niveau2.ToString());
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_xp",xp2.ToString());
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_personalité",personalité2);
        NewDataCenter.Instance.SauvegarderSTR(monIDActuel.ToString()+"_surnom",surnom2);

        maListe.Add(new YoKaiActif());
        Debug.Log("ajoute un yo-kai");
        if(monIDActuel < maListe.Count){
            YoKaiActif instance = maListe[monIDActuel];
            instance.CopierEspeceDepuis(medaliumDictionnaire._instance.ServantYoKai(ID2));
            instance.AttribuerIV();
            instance.IDUnique = monIDActuel;
            instance.niveau = Mathf.Clamp(niveau2, 1, 99);
            instance.xp = xp2;
            instance.personnalité = TrouverMaPersonalité(personalité2);
            instance.surnom = surnom2;
            instance.RecalculerStats();
        }
    }
    private void ChargerLesYoKai(){
        maListe.Clear();
        int monIDActuel = int.Parse(NewDataCenter.Instance.Lire("IDdeMesYoKai"));
        for (int i = 0; i < monIDActuel; i++){
            if (int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_ID")) > 0){
                maListe.Add(new YoKaiActif());
                maListe[i].IDUnique = i;
                maListe[i].ID  =  int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_ID"));// renvoie une erreur "out of range exeption"
                maListe[i].niveau=int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_niveau"));
                maListe[i].xp=int.Parse(NewDataCenter.Instance.Lire(i.ToString()+"_xp"));
                maListe[i].CopierEspeceDepuis(medaliumDictionnaire._instance.ServantYoKai(maListe[i].ID));
                maListe[i].AttribuerIV();
                maListe[i].IDUnique = i;
                maListe[i].personnalité = TrouverMaPersonalité(NewDataCenter.Instance.Lire(i.ToString()+"_personalité"));
                maListe[i].surnom = NewDataCenter.Instance.Lire(i.ToString()+"_surnom");
                maListe[i].RecalculerStats();
            }
        }
        yoKaiDeMonEquipeChargé=true;
    }

    public void SauvegarderLesPositonDesYokai(){
        string texte = "";
        foreach (int item in yoKaiDeMonEquipe){
            texte += "%" + maListe[item].positionRoue + "%";
        }
        NewDataCenter.Instance.SauvegarderSTR("yoKaiDansEquipePosition",texte);
    }

    public Personalité TrouverMaPersonalité(string nom){
        switch (nom){
            case "Sérieux":
            return Personalité.Sérieux;
            case "Ferme":
            return Personalité.Ferme;
            case "Détendu":
            return Personalité.Détendu;
            case "Frivole":
            return Personalité.Frivole;
            case "Distrait":
            return Personalité.Distrait;
            case "Maladroit":
            return Personalité.Maladroit;
        }
        Debug.LogWarning("Personalité non trouvé dans l'enum : "+nom);
        return Personalité.Détendu;
    }
}

