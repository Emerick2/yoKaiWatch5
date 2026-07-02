using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
    private bool utiliseLaCible = false;
    private bool utiliseLeSac = false;
    private bool utiliseAmultime = false;
    private bool utilisePurification = false;

    public GameObject blocDesBoutonGénéraux;
    // sprite
    public Sprite spriteFlâne;
    public Sprite spriteDéfance;
    public Sprite spriteRaté;
    private int nouritureChoisie = 0;

    private bool combatEnCours = true;

    public GameObject panelVictory;
    public int stepPanelVictory = 0;
    private bool panelVictoryIsClose = true;
    public List<GameObject> aAfficherSeulementAvecLaCible;

    public int tauxPourRatéUneAttaque = 10;
    public bool gameInStop = false;

    void Start(){
        imageVitesseAccéléré.gameObject.SetActive(false);
        ChangerCibleOption();
        AjouterUnEnnemie(6,1);
        // AjouterUnEnnemie(57,1);
        // AjouterUnEnnemie(57,1);
        if (MesYoKai.Instance != null){
            StartCoroutine(CoroutineDeDémarrage());
        }
        utiliseLaCible = false;
        utiliseLeSac = false;
        utiliseAmultime = false;
        utilisePurification = false;
        cible.SetActive(false);
        blocDesBoutonGénéraux.SetActive(true);
        if (panelVictory != null) panelVictory.SetActive(false);
        foreach (GameObject gameObject in aAfficherSeulementAvecLaCible){
            gameObject.SetActive(false);
        }
    }
    void AjouterUnEnnemie(int ID, int niveau){
        StructureYoKaiCombat nouv = new StructureYoKaiCombat{};
        MedaliumNonPersonel y = medaliumDictionnaire._instance.ServantYoKai(ID);
        nouv.estUnAmi = false;
        nouv.ID = ID;
        nouv.nom = y.Nom;
        nouv.imageMédaillon = y.médaillon;
        nouv.imageFace = y.faceRondImage;
        nouv.vie = y.PV;
        nouv.vieMaximum = nouv.vie;
        nouv.vitesse= y.Vitesse;
        nouv.esprit= y.Esprit;
        nouv.force= y.Force;
        nouv.défance= y.Défense;
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
        if (combatEnCours) {
            if (Input.GetButtonDown("L")){
                // TourneLaRoue(-1);
                rotationDemander=-1;
            } else if (Input.GetButtonDown("R")){
                // TourneLaRoue(1);
                rotationDemander=1;
            } else if (Input.GetButtonDown("right") && !utiliseLaCible && !utiliseLeSac && !utiliseAmultime && !utilisePurification){
                cibleOption++;
                ChangerCibleOption();
            } else if (Input.GetButtonDown("left") && !utiliseLaCible && !utiliseLeSac && !utiliseAmultime && !utilisePurification){
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
                if (!utiliseLaCible && !utiliseLeSac && !utiliseAmultime && !utilisePurification){
                    Input.ResetInputAxes();
                    switch(cibleOption){
                        case 0:
                            utiliseAmultime = true;
                            GetComponent<ChoisieYoKaiAmi>().Initialiser();
                            blocDesBoutonGénéraux.SetActive(false);
                            break;
                        case 1:
                            utiliseLaCible = true;
                            foreach (GameObject gameObject in aAfficherSeulementAvecLaCible){
                                gameObject.SetActive(true);
                            }
                            cible.GetComponent<Cible>().Initialisation();
                            cible.SetActive(true);
                            blocDesBoutonGénéraux.SetActive(false);
                            break;
                        case 2:
                            utilisePurification = true;
                            GetComponent<ChoisieYoKaiAmi>().Initialiser();
                            blocDesBoutonGénéraux.SetActive(false);
                            break;
                        case 3:
                            gameInStop = true;
                            utiliseLeSac = true;
                            GetComponent<SacCombats>().Initialiser();
                            blocDesBoutonGénéraux.SetActive(false);
                            break;
                    }
                }
            } else if (Input.GetButtonDown("B")){
                if (utiliseLaCible){
                    foreach (GameObject gameObject in aAfficherSeulementAvecLaCible){
                        gameObject.SetActive(false);
                    }
                    utiliseLaCible = false;
                    cible.SetActive(false);
                    blocDesBoutonGénéraux.SetActive(true);
                }
                if (utiliseLeSac){
                    utiliseLeSac = false;
                    GetComponent<ChoisieYoKaiAmi>().Arrêter();
                    GetComponent<SacCombats>().Arrêter();
                    blocDesBoutonGénéraux.SetActive(true);
                }
                if (utiliseAmultime){
                    utiliseAmultime = false;
                    GetComponent<ChoisieYoKaiAmi>().Arrêter();
                    blocDesBoutonGénéraux.SetActive(true);
                }
                if (utilisePurification){
                    utilisePurification = false;
                    GetComponent<ChoisieYoKaiAmi>().Arrêter();
                    blocDesBoutonGénéraux.SetActive(true);
                }
            }
        }
        else {
            if (panelVictoryIsClose) {
                OpenPanelVictory();
                Transform panel = panelVictory.transform.Find("partie-"+stepPanelVictory.ToString());
                if (panel != null) panel.gameObject.SetActive(false);
                stepPanelVictory = 1;
                panel = panelVictory.transform.Find("partie-"+stepPanelVictory.ToString());
                if (panel != null) panel.gameObject.SetActive(true);
                if (stepPanelVictory == 1){
                    GameObject win = panel.transform.Find("win").gameObject;
                    GameObject lose = panel.transform.Find("lose").gameObject;
                    if (UnGroupeDeYoKaiEstMort(true)){
                        win.SetActive(true);
                        lose.SetActive(false);
                    } else {
                        win.SetActive(false);
                        lose.SetActive(true);
                    }
                }
            }
            if (Input.GetButtonDown("A")){
                if (stepPanelVictory < 2){
                    Transform panel = panelVictory.transform.Find("partie-"+stepPanelVictory.ToString());
                    if (panel != null) panel.gameObject.SetActive(false);
                    stepPanelVictory++;
                    panel = panelVictory.transform.Find("partie-"+stepPanelVictory.ToString());
                    if (panel != null) panel.gameObject.SetActive(true);
                } else {
                    PlayerPositionLocally._instance.ReturnToTheSaveScene();
                }
            }
        }
        combatEnCours = !UnGroupeDeYoKaiEstMort();
    }

    public void ChoisieAllimant(int ID){
        nouritureChoisie = ID;
        GetComponent<SacCombats>().Arrêter();
        GetComponent<ChoisieYoKaiAmi>().Initialiser();
    }

    public void ChoieDeAmiEffectuer(int cible){
        Debug.Log(cible+" à été sélectionner");
        utiliseAmultime = false;
        utilisePurification = false;
        if (utiliseLeSac){
            utiliseLeSac = false;
            Debug.Log("Consome la nouriture "+nouritureChoisie);
            string effetSurAmi = Item._instance.listeDesNouritures[nouritureChoisie].allyEffect;
            int donPV = 0;
            int donAme = 0;
            StructureYoKaiCombat yoKai = listeYoKaiAmiSurLeTerrain[cible];
            if (yoKai.amultime < 0){
                yoKai.amultime = 0;
            }
            if (yoKai.vie < 0){
                yoKai.vie=0;
            }

            switch (effetSurAmi) {
                case "Rend très peu de PV" :
                    donPV = 30;
                    break;
                case "Rend un peu de PV" :
                    donPV = 60;
                    break;
                case "Rend pas mal de PV" :
                    donPV = 80;
                    break;
                case "Rend beaucoup de PV" :
                    donPV = 130;
                    break;
                case "Rend énormément de PV" :
                    donPV = 180;
                    break;
                case "Rend tous les PV" :
                    donPV = yoKai.vieMaximum;
                    break;

                case "Rend un peu d'âme" :
                    donAme = 20;
                    break;
                case "Rend pas mal d'âme" :
                    donAme = 40;
                    break;
                case "Rend énormément d'âme" :
                    donAme = 70;
                    break;
                case "Rend toute l'âme" :
                    donAme = 100;
                    break;
                case "Récupère 50% de l'âme total" :
                    donAme = 50;
                    break;
                default :
                    Debug.LogError("Cas du " + effetSurAmi + " non traité !");
                    break;
            }

            yoKai.amultime += donAme;
            if (yoKai.amultime > 100) {
                yoKai.amultime = 100;
            } else if (yoKai.amultime < 0) {
                yoKai.amultime = 0;
            }

            yoKai.vie += donPV;
            if (yoKai.vie > yoKai.vieMaximum) {
                yoKai.vie = yoKai.vieMaximum;
            } else if (yoKai.vie < 0) {
                yoKai.vie = 0;
            }

            nouritureChoisie = 0;
        }
        GetComponent<ChoisieYoKaiAmi>().Arrêter();
        blocDesBoutonGénéraux.SetActive(true);
        gameInStop = false;
        EcrireInformationUI();
    }

    void ChangerCibleOption(){
        cibleOption = ((cibleOption%lesOption.Count)+lesOption.Count)%lesOption.Count;
        for (int i = 0; i < lesOption.Count; i++){
            lesOption[i].transform.Find("cible").gameObject.SetActive(cibleOption==i);
            lesOption[i].transform.Find("main").gameObject.SetActive(cibleOption==i);
        }
    }

    IEnumerator CoroutineVitesseAccéléré(){
        while (vitesseAccéléré && combatEnCours){
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
            MedaliumNonPersonel y = medaliumDictionnaire._instance.ServantYoKai(ID);
            nouv.possitionRoue = MesYoKai.Instance.maListe[IDspécifique].positionRoue;
            nouv.estUnAmi = true;
            nouv.ID = ID;
            nouv.IDListeYoKai = IDspécifique;//permet de différancier deux Woko par exemple
            nouv.nom = MesYoKai.Instance.maListe[IDspécifique].surnom;
            nouv.imageMédaillon = y.médaillon;
            nouv.imageFace = y.faceRondImage;
            nouv.vie = y.PV;
            nouv.vieMaximum = nouv.vie;
            nouv.vitesse= y.Vitesse;
            nouv.esprit= y.Esprit;
            nouv.force= y.Force;
            nouv.défance= y.Défense;
            nouv.niveau = MesYoKai.Instance.maListe[IDspécifique].niveau;
            nouv.personalité = MesYoKai.Instance.maListe[IDspécifique].personnalité;
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
            if (yk != null){
                GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(yk.ID.ToString());
                TV.AddComponent<AddAnimator>();
                TV.transform.position = troisPositionAmi[i].position;
                // TV.transform.localScale = new Vector3(2,2,2);
                TV.transform.localScale = troisPositionAmi[i].localScale;
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
            if (yk != null){
                GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(yk.ID.ToString());
                TV.AddComponent<AddAnimator>();
                TV.transform.position = troisPositionEnnemie[i].position;
                TV.transform.localScale = Vector3.one;
                TV.transform.localScale = troisPositionAmi[i].localScale;
                if (yk.estUnAmi){
                    TV.transform.Rotate(0, 180, 0);
                } else {
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
    void JouerAnnimationBOOL(StructureYoKaiCombat yoKai, string nom, bool état){
        if (yoKai.gameObjectSurLaScène != null){
            if (yoKai.gameObjectSurLaScène.GetComponent<Animator>() != null){
                yoKai.gameObjectSurLaScène.GetComponent<Animator>().SetBool(nom, état);
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
        bool raté = Random.Range(0,100)<tauxPourRatéUneAttaque;
        if (raté){
            EchecAttaqueCible(cibleDuCoup);
            cibleDuCoup = null;
        }
        if (cibleDuCoup != null){
            JouerAnnimation(item, "attaque");
            yield return new WaitForSeconds(0.2f);
            int dégâts = diminuerLaVieScript.DiminueLaVie(item, cibleDuCoup,ordreDePassage);
            if (cibleDuCoup.vie>0){
                JouerAnnimation(cibleDuCoup, "coup");
            } else {
                JouerAnnimation(cibleDuCoup, "mort");
            }
            // Debug.Log(item.estUnAmi+"   "+cibleDuCoup.estUnAmi);

            GameObject display = null;
            if(cibleDuCoup.possitionRoue<3) {
                if (cibleDuCoup.estUnAmi) display = troisDisplateUIAmi[cibleDuCoup.possitionRoue];
                else display = troisDisplateUIEnnemie[cibleDuCoup.possitionRoue];
            }
            if(display != null) {
                display.transform.Find("nombre").gameObject.GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(dégâts);
                display.transform.Find("nombre").gameObject.GetComponent<Animator>().SetTrigger("ouvert");
            } else {
                Debug.Log("display est null");
            }
        } else {
            Debug.LogWarning("Attention, la cible du coup est null");
        }
    }

    float DonnezUnPourcentage(int total, int valeur){
        return (float)valeur / total;
    }


    bool VaFlâner(Personalité personalité){
        float nombre = 0;
        // return Random.Range(0,100)<=70;
        switch (personalité){
            case Personalité.Sérieux:
            nombre = 0f;
            break;
            case Personalité.Ferme:
            nombre = 1.75f;
            break;
            case Personalité.Détendu:
            nombre = 3.5f;
            break;
            case Personalité.Frivole:
            nombre = 5f;
            break;
            case Personalité.Distrait:
            nombre = 9f;
            break;
            case Personalité.Maladroit:
            nombre = 12f;
            break;
            default:
            Debug.LogWarning("La personnalité n'as pas été défini ici.");
            nombre = 5f;
            break;
        }
        return Random.Range(0,100)<=nombre;
    }

    bool VaFaireUneGarde(Personalité personalité){
        return Random.Range(0,100)<=30;
    }


    void JouerAnnimationFlane(StructureYoKaiCombat item, bool leFait){
        if (item.gameObjectSurLaScène != null){
            JouerAnnimationBOOL(item, "Flâne", leFait);
            GameObject display = null;
            if(item.possitionRoue<3) {
                if (item.estUnAmi) display = troisDisplateUIAmi[item.possitionRoue];
                else display = troisDisplateUIEnnemie[item.possitionRoue];
            }
            if(display != null) {
                display.transform.Find("bulle").gameObject.SetActive(leFait);
            }
            if (item.estUnAmi){
                float currentX = item.gameObjectSurLaScène.transform.eulerAngles.x;
                float currentZ = item.gameObjectSurLaScène.transform.eulerAngles.z;
                if (leFait){    
                    item.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 180f, currentZ);
                } else {
                    item.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 0f, currentZ);
                }
            }
            if(display != null && leFait) {
                display.transform.Find("bulle/t").gameObject.GetComponent<TextMeshProUGUI>().text="Pfiou...";
                display.transform.Find("bulle").gameObject.GetComponent<Animator>().SetTrigger("ouvre");    
                display.transform.Find("bulle/Image").gameObject.GetComponent<Image>().sprite = spriteFlâne;                
            }
        }
    }

    void EchecAttaqueCible(StructureYoKaiCombat item){
        if (item == null) return;
        GameObject display = null;
        if(item.possitionRoue<3) {
            if (item.estUnAmi) display = troisDisplateUIAmi[item.possitionRoue];
            else display = troisDisplateUIEnnemie[item.possitionRoue];
        }
        if(display != null) {
            display.transform.Find("bulle").gameObject.SetActive(true);
        }
        if (display != null){
            display.transform.Find("bulle/t").gameObject.GetComponent<TextMeshProUGUI>().text="Au zut !";
            display.transform.Find("bulle").gameObject.GetComponent<Animator>().SetTrigger("ouvre");    
            display.transform.Find("bulle/Image").gameObject.GetComponent<Image>().sprite = spriteRaté;   
        }
    }
    
    void JouerAnnimationGarde(StructureYoKaiCombat item, bool leFait){
        if (item.gameObjectSurLaScène != null){
            JouerAnnimationBOOL(item, "garde", leFait);
            GameObject display = null;
            if(item.possitionRoue<3) {
                if (item.estUnAmi) display = troisDisplateUIAmi[item.possitionRoue];
                else display = troisDisplateUIEnnemie[item.possitionRoue];
            }
            if(display != null) {
                display.transform.Find("bulle").gameObject.SetActive(leFait);
            }
            if (item.estUnAmi){
                float currentX = item.gameObjectSurLaScène.transform.eulerAngles.x;
                float currentZ = item.gameObjectSurLaScène.transform.eulerAngles.z;
                if (leFait){    
                    item.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 180f, currentZ);
                } else {
                    item.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 0f, currentZ);
                }
            }
            if (leFait && display != null){
                display.transform.Find("bulle/t").gameObject.GetComponent<TextMeshProUGUI>().text="En garde !";
                display.transform.Find("bulle").gameObject.GetComponent<Animator>().SetTrigger("ouvre");    
                display.transform.Find("bulle/Image").gameObject.GetComponent<Image>().sprite = spriteDéfance;   
            }
        }
    }
    IEnumerator BoucleDuJeu(){
        EcrireInformationUI();
        foreach (GameObject item in troisDisplateUIAmi){
            if (item != null){
                item.transform.Find("bulle").gameObject.SetActive(false);
            }
        }
        foreach (GameObject item in troisDisplateUIEnnemie){
            if (item != null){
                item.transform.Find("bulle").gameObject.SetActive(false);
            }
        }
        yield return new WaitForSeconds(0.5f);//temps avent le début du combat qui permet notament le chargement des animator
        while (combatEnCours){
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
                while (gameInStop){
                    yield return null;
                }
                if (item.vie <= 0){
                    continue;
                }
                //Ici on va parcourire tous les yo-kai du plus rapide au plus lang
                //Les ennemie et amis sont mélanger
                bool vaFaireUneAttaqueSimple = true;//il y auras plusieurs sorte d'attaque plus tard
                bool vaFlâner = VaFlâner(item.personalité);
                bool vaFaireUneGarde = VaFaireUneGarde(item.personalité);
                // bool vaFlâner = true;
                if (!vaFlâner && item.estEnTrainDeFlâner){
                    item.estEnTrainDeFlâner = false;
                    JouerAnnimationFlane(item, false);
                }
                if (!vaFaireUneGarde && item.estEnTrainDeFaireUneGarde){
                    item.estEnTrainDeFaireUneGarde = false;
                    item.garde = 1;
                    JouerAnnimationGarde(item, false);
                }
                // ----------
                if (vaFlâner){
                    item.estEnTrainDeFlâner = true;
                    JouerAnnimationFlane(item, true);
                } else if (vaFaireUneGarde){
                    item.estEnTrainDeFaireUneGarde = true;
                    item.garde = 0.5f;
                    JouerAnnimationGarde(item, true);
                }else if (vaFaireUneAttaqueSimple){
                    yield return StartCoroutine(VaFaireUneAttaqueSimple(item));
                }
                yield return new WaitForSeconds(0.5f);
                EcrireInformationUI();
                item.amultime+=10;
            }
            yield return null;
        }
    }

    bool UnGroupeDeYoKaiEstMort(bool savoirSiLeJoueurAGagner = false){
        if (listeYoKaiAmiSurLeTerrain.Count == 0 || listeYoKaiEnnemieSurLeTerrain.Count == 0){
            return true;
        }
        bool unAmiOuPlusEstEnVie = false;
        bool unEnnemieOuPlusEstEnVie = false;

        foreach (StructureYoKaiCombat yoKai in listeYoKaiAmiSurLeTerrain){
            if (yoKai.vie > 0){
                unAmiOuPlusEstEnVie = true;
                break;
            }
        }
        if (!unAmiOuPlusEstEnVie){
            if (savoirSiLeJoueurAGagner) return false;
            return true;
        }

        foreach (StructureYoKaiCombat yoKai in listeYoKaiEnnemieSurLeTerrain){
            if (yoKai.vie > 0){
                unEnnemieOuPlusEstEnVie = true;
                break;
            }
        }
        if (!unEnnemieOuPlusEstEnVie){
            if (savoirSiLeJoueurAGagner) return true;
            return true;
        }

        return false;
    }

    void OpenPanelVictory(){
        panelVictoryIsClose = false;
        if (panelVictory){
            panelVictory.SetActive(true);
            for (int i = 0; i < 6; i++){
                GameObject bloque = panelVictory.transform.Find("partie-2/"+i.ToString()).gameObject;
                if (i < listeYoKaiAmiSurLeTerrain.Count){
                    bloque.SetActive(true);
                    bloque.transform.Find("unYoKai/bloc image/médaillon").GetComponent<Image>().sprite = listeYoKaiAmiSurLeTerrain[i].imageFace;
                    bloque.transform.Find("unYoKai/Niveau").GetComponent<TextMeshProUGUI>().text = listeYoKaiAmiSurLeTerrain[i].niveau.ToString();
                } else {
                    bloque.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class StructureYoKaiCombat{
    public bool estUnAmi = false;
    public GameObject gameObjectSurLaScène;
    public Sprite imageMédaillon;
    public Sprite imageFace;
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
    public Personalité personalité;
    public bool estEnTrainDeFlâner = false;
    public bool estEnTrainDeFaireUneGarde = false;
}
