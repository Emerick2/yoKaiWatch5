using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GestionnaireCombat : MonoBehaviour{
    public List<Image> lesEspacesMédaillonDansLaRoue;
    [HideInInspector] public List<StructureYoKaiCombat> listeYoKaiAmiSurLeTerrain = new List<StructureYoKaiCombat>();
    [HideInInspector] public List<StructureYoKaiCombat> listeYoKaiEnnemieSurLeTerrain = new List<StructureYoKaiCombat>();
    private bool autorisationDeTournerLaRoue = true;
    [Range(0,5)]private int positionRotationRoue = 0;
    private int rotationDemander;//on demande la rotation. Elle sera réaliser à la fin du tours complets (quand chaque yo-kai aurras jouer)

    public List<Transform> troisPositionAmi;
    public List<GameObject> troisDisplateUIAmi;
    public List<GameObject> troisDisplateUIAmiDerrière;
    public List<Transform> troisPositionEnnemie;
    public List<GameObject> troisDisplateUIEnnemie;
    public List<StructureYoKaiCombat> ordreDePassage = new List<StructureYoKaiCombat>();

    public List<GameObject> lesOption;
    private int cibleOption = 0;
    public Image imageVitesseAccéléré;
    public List<Sprite> spritePourLaVitesse;
    private int numéroSpritePourLaVitesse = 0;
    private bool vitesseAccéléré = false;


    public DiminuerLaVieScript diminuerLaVieScript;

    public GameObject cible;
    // options :
    private bool utiliseLaCible = true;
    public GameObject blocDesBoutonGénéraux;

    void Start(){
        imageVitesseAccéléré.gameObject.SetActive(false);
        ChangerCibleOption();
        AjouterUnEnnemie(8,1);
        AjouterUnEnnemie(57,1);
        AjouterUnEnnemie(57,1);
        if (MesYoKai.Instance != null){
            StartCoroutine(CoroutineDeDémarrage());
        }
        utiliseLaCible = false;
        cible.SetActive(false);
        blocDesBoutonGénéraux.SetActive(true);
    }
    void AjouterUnEnnemie(int ID, int niveau){
        StructureYoKaiCombat nouv = new StructureYoKaiCombat{};
        nouv.gameObjectDeBase = medaliumDictionnaire._instance.dictionnaire[ID].yoKai;
        nouv.estUnAmi = false;
        nouv.ID = ID;
        nouv.nom = medaliumDictionnaire._instance.dictionnaire[ID].nom;
        nouv.imageMédaillon = medaliumDictionnaire._instance.dictionnaire[ID].médaillon;
        nouv.vie = medaliumDictionnaire._instance.dictionnaire[ID].vie;
        nouv.vieMaximum = nouv.vie;
        nouv.vitesse= medaliumDictionnaire._instance.dictionnaire[ID].vitesse;
        nouv.esprit= medaliumDictionnaire._instance.dictionnaire[ID].esprit;
        nouv.force= medaliumDictionnaire._instance.dictionnaire[ID].force;
        nouv.défance= medaliumDictionnaire._instance.dictionnaire[ID].défance;
        nouv.niveau = niveau;
        nouv.estDevant = true;
        listeYoKaiEnnemieSurLeTerrain.Add(nouv);
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
        foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
            PriseEnCompteObjet(item);
        }
        foreach (StructureYoKaiCombat item in listeYoKaiEnnemieSurLeTerrain){
            PriseEnCompteObjet(item);
        }
        for (int i = 0; i < listeYoKaiEnnemieSurLeTerrain.Count; i++){
            cible.GetComponent<Cible>().yoKaiGameObjects.Add(listeYoKaiEnnemieSurLeTerrain[i].gameObjectSurLaScène);
        }
        cible.GetComponent<Cible>().gestionnaireCombat = this;
        StartCoroutine(BoucleDuJeu());
    }

    void Update(){
        if (Input.GetButtonDown("L")){
            // TourneLaRoue(-1);
            rotationDemander=-1;
        } else if (Input.GetButtonDown("R")){
            // TourneLaRoue(1);
            rotationDemander=1;
        } else if (Input.GetButtonDown("flèche droite") && !utiliseLaCible){
            cibleOption++;
            ChangerCibleOption();
        } else if (Input.GetButtonDown("flèche gauche") && !utiliseLaCible){
            cibleOption--;
            ChangerCibleOption();
        } else if (Input.GetButtonDown("X")){
            if (vitesseAccéléré){
                vitesseAccéléré = false;
                imageVitesseAccéléré.gameObject.SetActive(false);
                Time.timeScale = 1;
            } else {
                imageVitesseAccéléré.gameObject.SetActive(true);
                Time.timeScale = 2;
                vitesseAccéléré = true;
                StartCoroutine(CoroutineVitesseAccéléré());
            }
        } else if (Input.GetButtonDown("A")){
            if (!utiliseLaCible){
                switch(cibleOption){
                    case 1:
                        utiliseLaCible = true;
                        cible.GetComponent<Cible>().Initialisation();
                        cible.SetActive(true);
                        blocDesBoutonGénéraux.SetActive(false);
                        break;
                }
            }
        } else if (Input.GetButtonDown("B")){
            if (utiliseLaCible){
                utiliseLaCible = false;
                cible.SetActive(false);
                blocDesBoutonGénéraux.SetActive(true);
            }
        }
    }

    void ChangerCibleOption(){
        cibleOption = ((cibleOption%lesOption.Count)+lesOption.Count)%lesOption.Count;
        for (int i = 0; i < lesOption.Count; i++){
            lesOption[i].transform.Find("cible").gameObject.SetActive(cibleOption==i);
            lesOption[i].transform.Find("main").gameObject.SetActive(cibleOption==i);
        }
    }

    IEnumerator CoroutineVitesseAccéléré(){
        while (vitesseAccéléré){
            imageVitesseAccéléré.gameObject.SetActive(true);
            imageVitesseAccéléré.sprite = spritePourLaVitesse[numéroSpritePourLaVitesse];
            numéroSpritePourLaVitesse = (numéroSpritePourLaVitesse+1)%spritePourLaVitesse.Count;
            yield return new WaitForSeconds(0.25f);
        }
        imageVitesseAccéléré.gameObject.SetActive(false);
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
            nouv.nom = MesYoKai.Instance.maListe[IDspécifique].surnom;
            nouv.imageMédaillon = medaliumDictionnaire._instance.dictionnaire[ID].médaillon;
            nouv.vie = medaliumDictionnaire._instance.dictionnaire[ID].vie;
            nouv.vieMaximum = nouv.vie;
            nouv.vitesse= medaliumDictionnaire._instance.dictionnaire[ID].vitesse;
            nouv.esprit= medaliumDictionnaire._instance.dictionnaire[ID].esprit;
            nouv.force= medaliumDictionnaire._instance.dictionnaire[ID].force;
            nouv.défance= medaliumDictionnaire._instance.dictionnaire[ID].défance;
            nouv.niveau = MesYoKai.Instance.maListe[IDspécifique].niveau;
            listeYoKaiAmiSurLeTerrain.Add(nouv);
        }
    }

    StructureYoKaiCombat RécupéréUnYoKaiParPositionSurLaRoue(int position){
        for (int j = 0; j < listeYoKaiAmiSurLeTerrain.Count; j++){
            if (listeYoKaiAmiSurLeTerrain[j].possitionRoue == position){
                return listeYoKaiAmiSurLeTerrain[j];
            }
        }
        return null;
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

    void PlacerLesModèles3DAmiSurLeTerrain(){
        for (int j = 0; j < listeYoKaiAmiSurLeTerrain.Count; j++){
            if (listeYoKaiAmiSurLeTerrain[j].gameObjectSurLaScène != null){
                Destroy(listeYoKaiAmiSurLeTerrain[j].gameObjectSurLaScène);
                listeYoKaiAmiSurLeTerrain[j].gameObjectSurLaScène = null;
            }
        }
        for (int i = 0; i < 3; i++){
            StructureYoKaiCombat yk = RécupéréUnYoKaiParPositionSurLaRoue(i);
            if (yk != null && yk.gameObjectDeBase !=null){
                GameObject TV = Instantiate(yk.gameObjectDeBase);
                TV.transform.position = troisPositionAmi[i].position;
                if (yk.estUnAmi){
                    TV.transform.Rotate(0, 180, 0);
                }
                yk.gameObjectSurLaScène = TV;
            }
        }
    }
    void PlacerLesModèles3DEnnemieSurLeTerrain(){
        for (int j = 0; j < listeYoKaiEnnemieSurLeTerrain.Count; j++){
            if (listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène != null){
                Destroy(listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène);
                listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène = null;
            }
        }
        for (int i = 0; i < listeYoKaiEnnemieSurLeTerrain.Count; i++){
            StructureYoKaiCombat yk = listeYoKaiEnnemieSurLeTerrain[i];
            if (yk != null && yk.gameObjectDeBase !=null){
                GameObject TV = Instantiate(yk.gameObjectDeBase);
                TV.transform.position = troisPositionEnnemie[i].position;
                if (yk.estUnAmi){
                    TV.transform.Rotate(0, 180, 0);
                }
                yk.gameObjectSurLaScène = TV;
            }
        }
    }

    void EcrireInformationUI(){
        //amis devant :
        for (int i = 0; i < troisDisplateUIAmi.Count; i++){
            StructureYoKaiCombat yk = RécupéréUnYoKaiParPositionSurLaRoue(i);
            if (yk != null){
                troisDisplateUIAmi[i].SetActive(true);
                troisDisplateUIAmi[i].transform.Find("médaillon").gameObject.GetComponent<Image>().sprite = yk.imageMédaillon;
                troisDisplateUIAmi[i].transform.Find("nom").gameObject.GetComponent<TextMeshProUGUI>().text = yk.nom;
                troisDisplateUIAmi[i].transform.Find("vie/slider").gameObject.GetComponent<Image>().fillAmount = DonnezUnPourcentage(yk.vieMaximum,yk.vie);
                troisDisplateUIAmi[i].transform.Find("amultime/slider").gameObject.GetComponent<Image>().fillAmount = (float)yk.amultime/100;
            } else {
                troisDisplateUIAmi[i].SetActive(false);
            }
        }
        //amis derrière :
        for (int i = 0; i < troisDisplateUIAmiDerrière.Count; i++){
            StructureYoKaiCombat yk = RécupéréUnYoKaiParPositionSurLaRoue(i+3);
            if (yk != null){
                troisDisplateUIAmiDerrière[i].SetActive(true);
                troisDisplateUIAmiDerrière[i].GetComponent<Image>().sprite = yk.imageMédaillon;
                troisDisplateUIAmiDerrière[i].transform.Find("vie/slider").gameObject.GetComponent<Image>().fillAmount = DonnezUnPourcentage(yk.vieMaximum,yk.vie);
                troisDisplateUIAmiDerrière[i].transform.Find("amultime/slider").gameObject.GetComponent<Image>().fillAmount = (float)yk.amultime/100;
            } else {
                troisDisplateUIAmiDerrière[i].SetActive(false);
            }
        }
        //ennemie :
        for (int i = 0; i < troisDisplateUIEnnemie.Count; i++){
            if (listeYoKaiEnnemieSurLeTerrain.Count > i){
                StructureYoKaiCombat yk = listeYoKaiEnnemieSurLeTerrain[i];
                troisDisplateUIEnnemie[i].SetActive(true);
                troisDisplateUIEnnemie[i].transform.Find("médaillon").gameObject.GetComponent<Image>().sprite = yk.imageMédaillon;
                troisDisplateUIEnnemie[i].transform.Find("nom").gameObject.GetComponent<TextMeshProUGUI>().text = yk.nom;
                troisDisplateUIEnnemie[i].transform.Find("vie/slider").gameObject.GetComponent<Image>().fillAmount = DonnezUnPourcentage(yk.vieMaximum,yk.vie);
                troisDisplateUIEnnemie[i].transform.Find("amultime/slider").gameObject.GetComponent<Image>().fillAmount = (float)yk.amultime/100;
            } else {
                troisDisplateUIEnnemie[i].SetActive(false);
            }
        }
    }
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
            PlacerLesModèles3DAmiSurLeTerrain();
            EcrireInformationUI();
        }
    }

    StructureYoKaiCombat YoKaiPrisAuHasard(List<StructureYoKaiCombat> liste){
        List<StructureYoKaiCombat> lesChoix = liste.Where(e => e.vie > 0 && e.estDevant).ToList();
        if (lesChoix.Count > 0) return lesChoix[Random.Range(0,lesChoix.Count)];
        else return null;
    }

    void JouerAnnimation(StructureYoKaiCombat yoKai, string nom){
        if (yoKai.gameObjectSurLaScène != null){
            if (yoKai.gameObjectSurLaScène.GetComponent<Animator>() != null){
                yoKai.gameObjectSurLaScène.GetComponent<Animator>().SetTrigger(nom);
            } else {
                Debug.Log("Le yo-kai "+yoKai.nom+" n'as pas de component Animator");
            }
        }
    }

    void PriseEnCompteObjet(StructureYoKaiCombat yoKai){
        if (yoKai.tienUnObjet && yoKai.numéroObjetTennu >= 0 && yoKai.numéroObjetTennu < Item._instance.listeDesÉquipements.Count){
            yoKai.vitesse += Item._instance.listeDesÉquipements[yoKai.numéroObjetTennu].effetSur_Vitesse;
            yoKai.force += Item._instance.listeDesÉquipements[yoKai.numéroObjetTennu].effetSur_Force;
            yoKai.esprit += Item._instance.listeDesÉquipements[yoKai.numéroObjetTennu].effetSur_Esprit;
            yoKai.défance += Item._instance.listeDesÉquipements[yoKai.numéroObjetTennu].effetSur_Défance;
        }      
    }

    IEnumerator VaFaireUneAttaqueSimple(StructureYoKaiCombat item){
        StructureYoKaiCombat cibleDuCoup = null;
        if (item.estUnAmi) cibleDuCoup = YoKaiPrisAuHasard(listeYoKaiEnnemieSurLeTerrain);
        else cibleDuCoup = YoKaiPrisAuHasard(listeYoKaiAmiSurLeTerrain);
        if (cibleDuCoup != null){
            JouerAnnimation(item, "attaque");
            yield return new WaitForSeconds(0.2f);
            diminuerLaVieScript.DiminueLaVie(item, cibleDuCoup,ordreDePassage);
            if (cibleDuCoup.vie>0){
                JouerAnnimation(cibleDuCoup, "coup");
            } else {
                JouerAnnimation(cibleDuCoup, "mort");
            }
            // Debug.Log(item.estUnAmi+"   "+cibleDuCoup.estUnAmi);
        } else {
            Debug.LogWarning("Attention, la cible du coup est null");
        }
    }

    float DonnezUnPourcentage(int total, int valeur){
        return (float)valeur / total;
    }

    IEnumerator BoucleDuJeu(){
        EcrireInformationUI();
        yield return new WaitForSeconds(0.5f);//temps avent le début du combat qui permet notament le chargement des animator
        while (true){
            if (rotationDemander != 0){
                TourneLaRoue(rotationDemander);
                rotationDemander = 0;
            }
            ordreDePassage = listeYoKaiAmiSurLeTerrain
                .Concat(listeYoKaiEnnemieSurLeTerrain)
                .Where(yokai => yokai.vie > 0)
                .OrderByDescending(yokai => yokai.vitesse)
                .ToList();
            foreach (StructureYoKaiCombat item in ordreDePassage){
                //Ici on va parcourire tous les yo-kai du plus rapide au plus lang
                //Les ennemie et amis sont mélanger
                bool vaFaireUneAttaqueSimple = true;//il y auras plusieurs sorte d'attaque plus tard
                if (vaFaireUneAttaqueSimple){
                    yield return StartCoroutine(VaFaireUneAttaqueSimple(item));
                }
                yield return new WaitForSeconds(0.5f);
                EcrireInformationUI();
                item.amultime+=10;
            }
            yield return null;
        }
    }
}

[System.Serializable]
public class StructureYoKaiCombat{
    public bool estUnAmi = false;
    public GameObject gameObjectDeBase;
    public GameObject gameObjectSurLaScène;
    public Sprite imageMédaillon;
    public string nom;
    public int possitionRoue;
    public bool estDevant;
    public int ID;
    public int IDListeYoKai;//permet de différancier deux Woko par exemple
    public int vie;
    public int vieMaximum;
    public int vitesse;
    public int esprit;
    public int force;
    public int défance;
    public int numéroObjetTennu;
    public bool tienUnObjet;
    public int amultime;//ces un pourcentage
    public int niveau;
    public float garde = 1; // 1/0.5/0.25
    public Rang rang;
    public Tribue tribue;
}
