using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RemplisLaRoue : MonoBehaviour{
    [Range(1,3)]public int numéroPageActuelle;
    public List<GameObject> lesMédaillon;
    private int cible = 0;
    public GameObject leParentDuModèle3DGénéré;
    private GameObject TV;
    public GameObject panneauDeInfo;
    public GameObject infoYokaiBandeau;

    public GameObject espaceBlocListeYokai;
    public GameObject exempleBlocListeYokai;

    private List<GameObject> lesBlocDansLaListeEspaceBlocListeYokai = new List<GameObject>();
    [Range(1,7)] private int positionVisibleDansLeBloc;

    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject blocInformatif;

    private int numéroDePageInfo = 1;
    private bool enCoursDeSélection = false;
    private int numéroEnCoursDeSélection;

    public Sprite spriteCercleSélection;
    public Sprite spriteCercleNonSélection;
    public Sprite spriteBoutonSélection;
    public Sprite spriteBoutonNonSélection;
    public Sprite spriteBoutonVerouiller;

    private int cible2;
    private int cibleMémoriser;
    private bool page3Facultatives;
    private bool page2Facultatives;
    private int numéroDeTrie;
    public écrireLesPetitesValeursDuMenu menu;
    private List<ListeDeMesYoKai> listeTriée = new List<ListeDeMesYoKai>();

    void Start(){
        enCoursDeSélection = false;
        numéroPageActuelle = 1;
        ChangerDePage();
    }

    void Update(){
        // la page 4 ces la page 2 quand on appuie sur A qui permet de choisir des options.
        // la page 5 ces la page 3 quand on appuie sur A qui permet de choisir des options.
        page2.gameObject.transform.Find("BLOC CHOIX").gameObject.SetActive(numéroPageActuelle==4);
        page3.gameObject.transform.Find("BLOC CHOIX").gameObject.SetActive(numéroPageActuelle==5);

        if (numéroPageActuelle == 2){
            if (Input.GetButtonDown("flèche droite")){
                cible++;
                NouvelleCiblePage2();
            } else if (Input.GetButtonDown("flèche gauche")){
                cible--;
                NouvelleCiblePage2();
            } else if (Input.GetButtonDown("X")){
                ChangementPageInfo();
            } else if (Input.GetButtonDown("Y")){
                switch (enCoursDeSélection){
                    case false:
                        bool autoriser = false;
                        for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                            if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue == cible){
                                autoriser = true;
                            }
                        }
                        if (autoriser){
                            enCoursDeSélection= true;
                            numéroEnCoursDeSélection=cible;
                            lesMédaillon[numéroEnCoursDeSélection].transform.Find("pointeur").gameObject.GetComponent<Image>().sprite = spriteCercleSélection;
                        }
                        break;
                    case true:
                        enCoursDeSélection= false;
                        InverserLesPositionDansLaRoue();
                        RemplireLaRouePage2();
                        NouvelleCiblePage2();
                        break;
                }
            } else if (Input.GetButtonDown("A")){
                if (!page2Facultatives){
                    numéroPageActuelle = 4;
                    cible2 = 0;
                    ChangerPositionCible2(page2,4);
                } else {
                    for (int j = 0; j < MesYoKai.Instance.yoKaiDeMonEquipe.Count; j++){
                        if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]].positionRoue == cible){
                            MesYoKai.Instance.yoKaiDeMonEquipe.RemoveAt(j);
                        }
                    }
                    MesYoKai.Instance.yoKaiDeMonEquipe.Add(listeTriée[cibleMémoriser].IDUnique);
                    MesYoKai.Instance.maListe[listeTriée[cibleMémoriser].IDUnique].positionRoue = cible;
                    numéroPageActuelle = 3;
                    page2Facultatives = false;
                    ChangerDePage();
                }
            } else if (Input.GetButtonDown("R")){
                for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                    MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue++;
                    MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue%=6;
                }
                MesYoKai.Instance.SauvegarderLesPositonDesYokai(); 
                RemplireLaRouePage2();
            } else if (Input.GetButtonDown("L")){
                for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                    MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue--;
                    if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue < 0){
                        MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue=6+MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue;
                    }
                }
                MesYoKai.Instance.SauvegarderLesPositonDesYokai(); 
                RemplireLaRouePage2();
            }
        }
        else if (numéroPageActuelle == 3) {
            if (Input.GetButtonDown("flèche bas")){
                cible++;
                NouvelleCiblePage3(1);
            } else if (Input.GetButtonDown("flèche haut")){
                cible--;
                NouvelleCiblePage3(-1);
            } else if (Input.GetButtonDown("X")){
                ChangementPageInfo();
            } else if (Input.GetButtonDown("A")){
                if (!page3Facultatives){
                    numéroPageActuelle = 5;
                    cible2=0;
                    ChangerPositionCible2(page3,4);
                } else {
                    for (int j = 0; j < MesYoKai.Instance.yoKaiDeMonEquipe.Count; j++){
                        if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]].positionRoue == cibleMémoriser){
                            MesYoKai.Instance.yoKaiDeMonEquipe.RemoveAt(j);
                        }
                    }
                    MesYoKai.Instance.yoKaiDeMonEquipe.Add(listeTriée[cible].IDUnique);
                    MesYoKai.Instance.maListe[listeTriée[cible].IDUnique].positionRoue = cibleMémoriser;
                    numéroPageActuelle = 2;
                    ChangerDePage();
                }
            } else if (Input.GetButtonDown("Y")){
                numéroDeTrie += 1;
                RemplireLaListeDesYoKai();
            }
        }
        else if (numéroPageActuelle == 4) {
             if (Input.GetButtonDown("flèche bas")){
                cible2++;
                ChangerPositionCible2(page2,4);
            } else if (Input.GetButtonDown("flèche haut")){
                cible2--;
                ChangerPositionCible2(page2,4);
            } else if (Input.GetButtonDown("A") && page2.gameObject.transform.Find("BLOC CHOIX/choix ("+(cible2+1).ToString()+")/texte").gameObject.transform.tag != "spriteBoutonVerouiller"){
                switch (cible2){
                    case 0:
                        cibleMémoriser = cible;
                        page3Facultatives = true;
                        numéroPageActuelle = 3;
                        ChangerDePage();
                        break;
                    case 3:
                        // renvoyer un yo-kai
                        if (MesYoKai.Instance.yoKaiDeMonEquipe.Count > 1){
                            for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                                if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue == cible){
                                    MesYoKai.Instance.yoKaiDeMonEquipe.RemoveAt(i);
                                    RemplireLaRouePage2();
                                }
                            }
                        }
                        numéroPageActuelle = 2;
                        break;
                }
            }
        } else if (numéroPageActuelle == 5) {
             if (Input.GetButtonDown("flèche bas")){
                cible2++;
                ChangerPositionCible2(page3,4);
            } else if (Input.GetButtonDown("flèche haut")){
                cible2--;
                ChangerPositionCible2(page3,4);
            } else if (Input.GetButtonDown("A") && page3.gameObject.transform.Find("BLOC CHOIX/choix ("+(cible2+1).ToString()+")/texte").gameObject.transform.tag != "spriteBoutonVerouiller"){
                switch (cible2){
                    case 0:
                        cibleMémoriser = cible;
                        page2Facultatives = true;
                        numéroPageActuelle = 2;
                        ChangerDePage();
                        break;
                    case 3:
                        // renvoyer un yo-kai
                        // if (MesYoKai.Instance.yoKaiDeMonEquipe.Count > 1){
                        //     for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                        //         if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue == cible){
                        //             MesYoKai.Instance.yoKaiDeMonEquipe.RemoveAt(i);
                        //             RemplireLaRouePage2();
                        //         }
                        //     }
                        // }
                        // numéroPageActuelle = 2;
                        break;
                }
            }
        }
        if (Input.GetButtonDown("B")){
            if (numéroPageActuelle == 4){
                numéroPageActuelle = 2;
            } else if (numéroPageActuelle == 5){
                numéroPageActuelle = 3;
            }else if (numéroPageActuelle != 1){
                if (numéroPageActuelle == 3 && page3Facultatives){
                    numéroPageActuelle = 2;
                    ChangerDePage();
                } else {
                    numéroPageActuelle = 1;
                    ChangerDePage();
                }
            } else if (numéroPageActuelle == 1){
                menu.RevenirAuMenu();
            }
        }
    }

    void ChangerPositionCible2(GameObject pageGameObject, int maximum){
        cible2 = cible2%maximum;
        if (cible2<0){
            cible2 = maximum+cible2;
        }
        for (int i = 0; i < maximum; i++){
            bool doitÊtreVerouiller = false;

            if (pageGameObject == page2){
                if (i==3 && MesYoKai.Instance.yoKaiDeMonEquipe.Count <= 1){
                    doitÊtreVerouiller = true;
                }
                bool estUnYoKai = false;
                for (int j = 0; j < MesYoKai.Instance.yoKaiDeMonEquipe.Count; j++){
                    if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]].positionRoue == cible){
                        estUnYoKai = true;
                    }
                }
                if (i != 0 && !estUnYoKai){
                    doitÊtreVerouiller = true;
                }
            } else if (pageGameObject == page3) {
                if (EstDansEquipe(listeTriée[cible].IDUnique)){
                    if (i == 0) doitÊtreVerouiller = true;
                    if (i == 3) doitÊtreVerouiller = true;
                }
            }
            if (doitÊtreVerouiller){
                pageGameObject.gameObject.transform.Find("BLOC CHOIX/choix ("+(i+1).ToString()+")").gameObject.GetComponent<Image>().sprite=spriteBoutonVerouiller;
                pageGameObject.gameObject.transform.Find("BLOC CHOIX/choix ("+(i+1).ToString()+")/texte").gameObject.transform.tag = "spriteBoutonVerouiller";
            } else {
                pageGameObject.gameObject.transform.Find("BLOC CHOIX/choix ("+(i+1).ToString()+")").gameObject.GetComponent<Image>().sprite=spriteBoutonNonSélection;
                pageGameObject.gameObject.transform.Find("BLOC CHOIX/choix ("+(i+1).ToString()+")/texte").gameObject.transform.tag = "spriteBoutonNonVerouiller";
            }  
            if (i == cible2){
                pageGameObject.gameObject.transform.Find("BLOC CHOIX/choix ("+(i+1).ToString()+")").gameObject.GetComponent<Image>().sprite=spriteBoutonSélection;
            }
        }
    }
    void InverserLesPositionDansLaRoue(){
        if (numéroEnCoursDeSélection != cible){
            ListeDeMesYoKai cibleInitiale = null; // numéroEnCoursDeSélection
            ListeDeMesYoKai cibleDestination = null; // cible

            List<int> yoKaiDeMonEquipe = MesYoKai.Instance.yoKaiDeMonEquipe;
            List<ListeDeMesYoKai> maListe = MesYoKai.Instance.maListe;
            for (int i = 0; i < yoKaiDeMonEquipe.Count; i++){
                if (maListe[yoKaiDeMonEquipe[i]].positionRoue == cible){
                    cibleDestination = maListe[yoKaiDeMonEquipe[i]];
                }
            }
            for (int i = 0; i < yoKaiDeMonEquipe.Count; i++){
                if (maListe[yoKaiDeMonEquipe[i]].positionRoue == numéroEnCoursDeSélection){
                    cibleInitiale = maListe[yoKaiDeMonEquipe[i]];
                }
            }
            if (cibleInitiale != null){
                if (cibleDestination != null){
                    cibleDestination.positionRoue = numéroEnCoursDeSélection;
                    cibleInitiale.positionRoue = cible;
                } else {
                    cibleInitiale.positionRoue = cible;
                }
            }
        }
        MesYoKai.Instance.SauvegarderLesPositonDesYokai();
    }

    IEnumerator Attend(){
        while (!MesYoKai.Instance.finDuChargement && !MesYoKai.Instance.yoKaiDeMonEquipeChargé){
            yield return new WaitForSeconds(0.1f);
        }
        Initialisation(false);
    }
    public void Initialisation(bool total){
        if (total){
            page2Facultatives = false;
            page3Facultatives = false;
            numéroDePageInfo = 1;
            enCoursDeSélection = false;
            numéroPageActuelle = 1;
        }
        RemplireLaRouePage2();
        RemplireLaListeDesYoKai();
        if (true){
            cible = 0;
            NouvelleCiblePage2();
        }
        positionVisibleDansLeBloc = 1;
    }

    void RemplireLaRouePage2(){
        enCoursDeSélection = false;
        for (int i = 0; i < lesMédaillon.Count; i++){
            lesMédaillon[i].SetActive(true);
            lesMédaillon[i].transform.Find("pointeur").gameObject.GetComponent<Image>().sprite = spriteCercleNonSélection;

            int positionCible = 0;
            bool estUnYoKai = false;
            if (6 > i){
                for (int j = 0; j < MesYoKai.Instance.yoKaiDeMonEquipe.Count; j++){
                    if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]].positionRoue == i){
                        positionCible = MesYoKai.Instance.yoKaiDeMonEquipe[j];
                        estUnYoKai = true;
                    }
                }
            }

            if (estUnYoKai){ 
                lesMédaillon[i].GetComponent<Image>().enabled = true;
                lesMédaillon[i].GetComponent<Image>().sprite = medaliumDictionnaire._instance.dictionnaire[MesYoKai.Instance.maListe[positionCible].ID].médaillon;
            } else {
                lesMédaillon[i].GetComponent<Image>().enabled = false;
            }
        }
        NouvelleCiblePage2();
    }

    public void ChangerDePage(){
        enCoursDeSélection = false;
        numéroDeTrie = 0;
        if (numéroPageActuelle == 1){
            page1.SetActive(true);
            page2.SetActive(false);
            page3.SetActive(false);
            blocInformatif.SetActive(false);
            menu.boutDroit.SetActive(true);
            menu.Initialisation();
        } else if (numéroPageActuelle == 2){
            page1.SetActive(false);
            page2.SetActive(true);
            page3.SetActive(false);
            blocInformatif.SetActive(true);
            menu.boutDroit.SetActive(false);
        } else if (numéroPageActuelle == 3){
            page1.SetActive(false);
            page2.SetActive(false);
            page3.SetActive(true);
            menu.boutDroit.SetActive(false);
            blocInformatif.SetActive(true);
        }
        if (MesYoKai.Instance != null){
            StartCoroutine(Attend());
        }
        if (page3Facultatives && numéroPageActuelle == 2){
            cible = cibleMémoriser;
            page3Facultatives = false;
        } else {
            cible = 0;
        }
        numéroDePageInfo = 0;
        ChangementPageInfo();
        if (numéroPageActuelle == 3){
            NouvelleCiblePage3(0);
        }
        if (numéroPageActuelle == 2){
            RemplireLaRouePage2();
            NouvelleCiblePage2();
        }
    }

    bool EstDansEquipe(int IDSpécifique){
        for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count ; i++) {
            if (MesYoKai.Instance.yoKaiDeMonEquipe[i] == IDSpécifique){
                return true;
            }
        }
        return false;
    }


    void ChangementPageInfo(){
        int nombreDePage = 3;
        numéroDePageInfo++;
        if (numéroDePageInfo > nombreDePage){
            numéroDePageInfo = 1;
        } else if (numéroDePageInfo < 1){
            numéroDePageInfo = nombreDePage;
        }
        for (int i = 1; i <= nombreDePage; i++){
            panneauDeInfo.transform.Find("page "+i.ToString()).gameObject.SetActive(false);
        }
        panneauDeInfo.transform.Find("page "+numéroDePageInfo.ToString()).gameObject.SetActive(true);
        panneauDeInfo.transform.Find("page/Numéro").gameObject.GetComponent<TextMeshProUGUI>().text = numéroDePageInfo.ToString()+" / "+nombreDePage.ToString();
    }

    void NouvelleCiblePage2(){
        if (cible < 0){
            cible = lesMédaillon.Count-1;
        } else if (cible >= lesMédaillon.Count){
            cible = 0;
        }
        int positionCible = cible;
        bool estUnYoKai = false;
        if (6 > cible){
            for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
                if (MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].positionRoue == cible){
                    positionCible = MesYoKai.Instance.yoKaiDeMonEquipe[i];
                    estUnYoKai = true;
                }
            }
        }
        for (int i = 0; i < lesMédaillon.Count; i++){
            lesMédaillon[i].transform.Find("pointeur").gameObject.SetActive(i == cible);
            if (lesMédaillon[i].transform.Find("pointeur").gameObject.GetComponent<Image>().sprite == spriteCercleSélection){
                lesMédaillon[i].transform.Find("pointeur").gameObject.SetActive(true);
            }
        }
        if (TV != null){
            Destroy(TV);
        }
        if (estUnYoKai){
            ListeDeMesYoKai informationSpécifique = MesYoKai.Instance.maListe[positionCible];
            MedaliumNonPersonel informationGénéral = medaliumDictionnaire._instance.dictionnaire[informationSpécifique.ID];
            MettreLesElementInfo(informationSpécifique,informationGénéral);
        } else {
            infoYokaiBandeau.SetActive(false);
            panneauDeInfo.SetActive(false);
        }
    }
    void AppliquerLayer(GameObject obj, int layer){
        obj.layer = layer;
        foreach (Transform enfant in obj.transform){
            AppliquerLayer(enfant.gameObject, layer);
        }
    }

    void MettreLesElementInfo(ListeDeMesYoKai informationSpécifique, MedaliumNonPersonel informationGénéral){
        if (informationSpécifique.ID <= 0){
            return;
        }
        TV = Instantiate(informationGénéral.yoKai);
            TV.transform.parent = leParentDuModèle3DGénéré.transform;
            TV.transform.localPosition = Vector3.zero;
            int layerUI = LayerMask.NameToLayer("CaméraUImodèle3D");
            AppliquerLayer(TV, layerUI);

            panneauDeInfo.SetActive(true);
            // numéroDePageInfo = 1;
            // panneauDeInfo.transform.Find("page 1").gameObject.SetActive(true);
            // panneauDeInfo.transform.Find("page 2").gameObject.SetActive(false);
            // panneauDeInfo.transform.Find("page 3").gameObject.SetActive(false);
            // panneauDeInfo.transform.Find("page/Numéro").gameObject.GetComponent<TextMeshProUGUI>().text = "1 / 3";
            
            panneauDeInfo.transform.Find("page 1/Force/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.force * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/Défance/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.vie * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/esprit/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.esprit * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/vitesse/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.vitesse * informationSpécifique.niveau).ToString();

            panneauDeInfo.transform.Find("page 1/Personalité/text").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.personnalité.ToString();
            panneauDeInfo.transform.Find("page 1/Talent/text").gameObject.GetComponent<TextMeshProUGUI>().text = informationGénéral.talent.ToString();
            panneauDeInfo.transform.Find("page 1/Talent/text (1)").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            
            infoYokaiBandeau.SetActive(true);
            infoYokaiBandeau.transform.Find("tribue").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.SpriteDeLaTribue(informationGénéral.tribue);
            infoYokaiBandeau.transform.Find("rang").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.RecevoirUnSpriteDeRang(informationGénéral.rang.ToString());
            infoYokaiBandeau.transform.Find("nom").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.surnom;
            infoYokaiBandeau.transform.Find("niveau").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.niveau.ToString();
    }

    void NouvelleCiblePage3(int direction){
        foreach (GameObject item in lesBlocDansLaListeEspaceBlocListeYokai){
            item.GetComponent<Image>().color = new Color(1f, 1f, 0.5f, 1f);
        }
        bool forcer = false;
        if (cible < 0){
            cible = lesBlocDansLaListeEspaceBlocListeYokai.Count-1;
            forcer = true;
        } else if (cible >= lesBlocDansLaListeEspaceBlocListeYokai.Count){
            positionVisibleDansLeBloc = 1;
            forcer = true;
            cible = 0;
        }
        if (TV != null){
            Destroy(TV);
        }
        lesBlocDansLaListeEspaceBlocListeYokai[cible].GetComponent<Image>().color = new Color(1f, 0.5f, 0.5f, 1f);
        ListeDeMesYoKai informationSpécifique = MesYoKai.Instance.maListe[int.Parse(lesBlocDansLaListeEspaceBlocListeYokai[cible].transform.name)];
        MedaliumNonPersonel informationGénéral = medaliumDictionnaire._instance.dictionnaire[informationSpécifique.ID];
        MettreLesElementInfo(informationSpécifique,informationGénéral);
        PositionDansLaListeDéfilementAutomatiquePage3(direction, forcer);
    }

    void PositionDansLaListeDéfilementAutomatiquePage3(int direction, bool forcer){
        //il y en à 7 de visible en tous
        //on vas en l'aisser 2 visible en haut et 2 visible en bas
        if ((positionVisibleDansLeBloc < 2 && direction < 0) || (positionVisibleDansLeBloc > 4 && direction > 0)||forcer){
            float height = 0;
            height += espaceBlocListeYokai.GetComponent<GridLayoutGroup>().padding.top;
            height += (espaceBlocListeYokai.GetComponent<GridLayoutGroup>().cellSize.y + espaceBlocListeYokai.GetComponent<GridLayoutGroup>().spacing.y)*(cible-positionVisibleDansLeBloc);
        
            RectTransform rt = espaceBlocListeYokai.GetComponent<RectTransform>();
            Vector2 anchored = rt.anchoredPosition;
            anchored.y = height;
            rt.anchoredPosition = anchored;
        } else {
            positionVisibleDansLeBloc += direction;
        }
    }

    void RemplireLaListeDesYoKai(){
        numéroDeTrie = numéroDeTrie%6;

        // 1 - Vider la liste précédante
        lesBlocDansLaListeEspaceBlocListeYokai.Clear();
        foreach (Transform child in espaceBlocListeYokai.transform){
            if (child.gameObject != exempleBlocListeYokai){
                Destroy(child.gameObject);
            }
        }
        exempleBlocListeYokai.SetActive(false);

        // 2 - Trié la liste
        listeTriée.Clear();
        listeTriée = MesYoKai.Instance.maListe;
        TextMeshProUGUI espaceTexte = page3.transform.Find("Trie/Nom").GetComponent<TextMeshProUGUI>();
        switch (numéroDeTrie){
            case 0 :
                espaceTexte.text = "Équipe";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yokai => !EstDansEquipe(yokai.IDUnique)).ToList();
                break;
            case 1 :
                espaceTexte.text = "Niveau";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yoKai => yoKai.niveau).ToList();
                break;
            case 2 :
                espaceTexte.text = "Rang";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yoKai => medaliumDictionnaire._instance.dictionnaire[yoKai.ID].rang.ToString()).ToList();
                break;
            case 3 :
                espaceTexte.text = "Tribu";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yoKai => medaliumDictionnaire._instance.dictionnaire[yoKai.ID].tribue.ToString()).ToList();
                break;
            case 4 :
                espaceTexte.text = "Nom";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yoKai => yoKai.surnom).ToList();
                break;
            case 5 :
                espaceTexte.text = "Nouveau";
                listeTriée = MesYoKai.Instance.maListe.OrderBy(yoKai => yoKai.IDUnique).ToList();
                break;
        }

        // 3 - Placer la liste
        for (int i = 0; i < listeTriée.Count; i++){
            if (page3Facultatives && EstDansEquipe(listeTriée[i].IDUnique)){
                listeTriée.RemoveAt(i);
                i--;
            } else {
                GameObject TV = Instantiate(exempleBlocListeYokai);
                TV.transform.parent = espaceBlocListeYokai.transform;
                TV.transform.localScale = new Vector3(1, 1, 1);
                TV.transform.Find("dans l'équipe").gameObject.SetActive(EstDansEquipe(listeTriée[i].IDUnique));
                TV.transform.Find("médaille").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.dictionnaire[listeTriée[i].ID].médaillon;
                TV.transform.Find("Nom").gameObject.GetComponent<TextMeshProUGUI>().text = listeTriée[i].surnom;
                TV.transform.Find("rang/Image rang").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.RecevoirUnSpriteDeRang(medaliumDictionnaire._instance.dictionnaire[listeTriée[i].ID].rang.ToString());
                TV.transform.Find("Niveau/nombre text").gameObject.GetComponent<TextMeshProUGUI>().text = listeTriée[i].niveau.ToString();
                TV.transform.name = listeTriée[i].IDUnique.ToString();
                TV.SetActive(true);
                lesBlocDansLaListeEspaceBlocListeYokai.Add(TV);
            }
        }
        float height = 0;
        height += espaceBlocListeYokai.GetComponent<GridLayoutGroup>().padding.top+espaceBlocListeYokai.GetComponent<GridLayoutGroup>().padding.bottom;
        height += listeTriée.Count * espaceBlocListeYokai.GetComponent<GridLayoutGroup>().cellSize.y;
        height += listeTriée.Count * espaceBlocListeYokai.GetComponent<GridLayoutGroup>().spacing.y;
        RectTransform rt = espaceBlocListeYokai.GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;
        size.y = height;
        rt.sizeDelta = size;
        NouvelleCiblePage3(0);
    }
}
