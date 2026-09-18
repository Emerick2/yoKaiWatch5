using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BingoKai2 : MonoBehaviour {
    private DialcodePrinted dialcode;
    private Coroutine coroutineApparitionLettre;
    private bool inTheZone = false;
    private int étape = 0;
    public GameObject canevasBingoKai;
    public GameObject contentViewport;
    private GameObject exemple;
    private List<GameObject> listeObjets = new List<GameObject>();
    private bool canevaBingoKaiOuvert = false;
    public int cible = 0;
    public VideoPlayer videoPlayer;
    public VideoClip videoBingoKai;
    public VideoClip videoCapsuleGrise;
    public VideoClip videoCapsuleBleue;
    public VideoClip videoCapsuleRouge;
    public VideoClip videoCapsuleDorée;
    public GameObject positionYoKaiCaméra;
    public GameObject panelGain;
    private int yokaiQuiVasSortir = 0;
    private string nomObjetQuiVasSortir;
    private bool afficherTexteYokaiSortie;
    private int étapeAfficherTexteYokaiSortie;
    private GameObject gameObjectYoKaiBingoKaiSupr;
    private bool attenteDuChoixDeLaPièce = false;
    public Image imageObjet;
    public List<Image> listeImagePremierPrix;
    public List<Image> listeImageDeuxièmePrix;
    public List<Image> listeImageTroisièmePrix;
    public List<Image> listeImageDernierPrix;

    BingoKaiCityData lesDonnéesBingoKai;
    public int tauxPremierPrix = 10;
    public int tauxSecondPrix = 45;
    public int tauxTroisièmePrix = 80;

    public List<PièceBingoKai> listePièceBingoKai = new List<PièceBingoKai>{
        new PièceBingoKai{ id = 0, nom_anglais = "Red_Coin" },
        new PièceBingoKai{ id = 1, nom_anglais = "Yellow_Coin" },
        new PièceBingoKai{ id = 2, nom_anglais = "Orange_Coin" },
        new PièceBingoKai{ id = 3, nom_anglais = "Pink_Coin" },
        new PièceBingoKai{ id = 4, nom_anglais = "Green_Coin" },
        new PièceBingoKai{ id = 5, nom_anglais = "Blue_Coin" },
        new PièceBingoKai{ id = 6, nom_anglais = "Purple_Coin" },
        new PièceBingoKai{ id = 7, nom_anglais = "One_Star_Coin" },
        new PièceBingoKai{ id = 8, nom_anglais = "Five_Star_Coin" },
        new PièceBingoKai{ id = 9, nom_anglais = "Special_Coin" },
        new PièceBingoKai{ id = 10, nom_anglais = "Yo_Mystery_Coin" },
        new PièceBingoKai{ id = 11, nom_anglais = "Kai_Mystery_Coin" },
        new PièceBingoKai{ id = 12, nom_anglais = "Soul_Mystery_Coin" },
        new PièceBingoKai{ id = 13, nom_anglais = "Sushi_Coin" },
        new PièceBingoKai{ id = 14, nom_anglais = "Tempura_Coin" },
        new PièceBingoKai{ id = 15, nom_anglais = "Whirlwind_Coin" },
        new PièceBingoKai{ id = 16, nom_anglais = "Wanderer_Coin" },
        new PièceBingoKai{ id = 17, nom_anglais = "Rusted_Coin" },
        new PièceBingoKai{ id = 18, nom_anglais = "Flower_Excite_Coin" },
        new PièceBingoKai{ id = 19, nom_anglais = "Bird_Excite_Coin" },
        new PièceBingoKai{ id = 20, nom_anglais = "Wind_Excite_Coin" },
        new PièceBingoKai{ id = 21, nom_anglais = "Moon_Excite_Coin" },
        new PièceBingoKai{ id = 22, nom_anglais = "Mellow_Coin" },
        new PièceBingoKai{ id = 23, nom_anglais = "Brave_Dream_Coin" },
        new PièceBingoKai{ id = 24, nom_anglais = "Light_Dream_Coin" },
        new PièceBingoKai{ id = 25, nom_anglais = "Platin_Dream_Coin" },
        new PièceBingoKai{ id = 26, nom_anglais = "Fortun_Dream_Coin" },
        new PièceBingoKai{ id = 27, nom_anglais = "Princess_Dream_Coin" },
        new PièceBingoKai{ id = 28, nom_anglais = "Wisdom_Dream_Coin" },
        new PièceBingoKai{ id = 29, nom_anglais = "Chivalry_Dream_Coin" },
        new PièceBingoKai{ id = 30, nom_anglais = "_2nd_Helping_Coin" },
        new PièceBingoKai{ id = 31, nom_anglais = "Belly_Buster_Coin" },
        new PièceBingoKai{ id = 32, nom_anglais = "L_Souvenir_Coin" },
        new PièceBingoKai{ id = 33, nom_anglais = "F_Souvenir_Coin" },
        new PièceBingoKai{ id = 34, nom_anglais = "S_Souvenir_Coin" },
        new PièceBingoKai{ id = 35, nom_anglais = "K_Souvenir_Coin" },
        new PièceBingoKai{ id = 36, nom_anglais = "Joyful_Coin" },
        new PièceBingoKai{ id = 37, nom_anglais = "Excellent_Coin" },
        new PièceBingoKai{ id = 38, nom_anglais = "Beaming_Coin" },
        new PièceBingoKai{ id = 39, nom_anglais = "Iron_Pot_Coin" },
        new PièceBingoKai{ id = 40, nom_anglais = "Fancy_That_Coin" },
        new PièceBingoKai{ id = 41, nom_anglais = "Black_Coin" },
        new PièceBingoKai{ id = 42, nom_anglais = "Demonic_Coin" },
        new PièceBingoKai{ id = 43, nom_anglais = "Underworld_Coin" },
        new PièceBingoKai{ id = 44, nom_anglais = "Sealed_Coin" },
        new PièceBingoKai{ id = 45, nom_anglais = "Emblem_Coin" },
        new PièceBingoKai{ id = 46, nom_anglais = "Treasure_Coin" },
        new PièceBingoKai{ id = 47, nom_anglais = "Legend_Coin" },
        new PièceBingoKai{ id = 48, nom_anglais = "Super_Red_Coin" },
        new PièceBingoKai{ id = 49, nom_anglais = "Super_Yellow_Coin" },
        new PièceBingoKai{ id = 50, nom_anglais = "Super_Orange_Coin" },
        new PièceBingoKai{ id = 51, nom_anglais = "Super_Pink_Coin" },
        new PièceBingoKai{ id = 52, nom_anglais = "Super_Green_Coin" },
        new PièceBingoKai{ id = 53, nom_anglais = "Super_Blue_Coin" },
        new PièceBingoKai{ id = 54, nom_anglais = "Super_Purple_Coin" },
        new PièceBingoKai{ id = 55, nom_anglais = "Super_Light_Blue_Coin" },
        new PièceBingoKai{ id = 56, nom_anglais = "Ultra_Red_Coin" },
        new PièceBingoKai{ id = 57, nom_anglais = "Ultra_Yellow_Coin" },
        new PièceBingoKai{ id = 58, nom_anglais = "Ultra_Orange_Coin" },
        new PièceBingoKai{ id = 59, nom_anglais = "Ultra_Pink_Coin" },
        new PièceBingoKai{ id = 60, nom_anglais = "Ultra_Green_Coin" },
        new PièceBingoKai{ id = 61, nom_anglais = "Ultra_Blue_Coin" },
        new PièceBingoKai{ id = 62, nom_anglais = "Ultra_Purple_Coin" },
        new PièceBingoKai{ id = 63, nom_anglais = "Ultra_Light_Blue_Coin" },
        new PièceBingoKai{ id = 64, nom_anglais = "_3DS_Coin" }
    };

    void Start(){
        dialcode = DialcodePrinted.Instance;
        if (canevasBingoKai != null) canevasBingoKai.SetActive(false);
        AfficherLesPrixDuBingoKai();
    }

    void Update() {
        if (inTheZone && Input.GetButtonDown("up") && étape == 3){
            dialcode.positionChoix = (dialcode.positionChoix+1)%2;
            dialcode.ChangerPositionChoix();
        }

        if (inTheZone && Input.GetButtonDown("down") && étape == 3){
            dialcode.positionChoix--;
            if (dialcode.positionChoix<0){
                dialcode.positionChoix = 1;
            }
            dialcode.ChangerPositionChoix();
        }


        if (canevaBingoKaiOuvert == true){
            if (Input.GetButtonDown("right")){
    
    void ComplétéListePièce(){
        GameObject exemple = null;
        foreach (GameObject item in listeObjets){
            if (item != null){
                if (exemple != null){
                    exemple = item;
                } else {
                    Destroy(item);
                }
            }
        }
        listePièceBingoKai.Clear();

        foreach (PièceBingoKai item in listePièceBingoKai) {
            GameObject nouvellePièce = Instantiate(exemple);
            if (nouvellePièce.GetComponent<Image>() != null){
                nouvellePièce.GetComponent<Image>().sprite = item.sprite;
            }
            nouvellePièce.transform.parent = exemple.transform.parent;

            listeObjets.Add(nouvellePièce);
        }

        Destroy(exemple);
    }

    ChangerLaCible(cible, ((cible+1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            } else if (Input.GetButtonDown("left")){
                ChangerLaCible(cible, ((cible-1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            }
            if (Input.GetButtonDown("B")){
                if (canevasBingoKai != null) canevasBingoKai.SetActive(false);
                canevaBingoKaiOuvert = false;
                imageObjet.gameObject.SetActive(false);
                mouvementDuJoueur.droitDeMarcher = true;
            }
            if (Input.GetButtonDown("A")){
                if (afficherTexteYokaiSortie){
                    if (étapeAfficherTexteYokaiSortie == 1){
                        afficherTexteYokaiSortie = false;
                        étapeAfficherTexteYokaiSortie = 0;
                        dialcode.Fermeture(false);
                        canevaBingoKaiOuvert = false;
                        canevasBingoKai.SetActive(false);
                        if (gameObjectYoKaiBingoKaiSupr != null) {
                            Destroy(gameObjectYoKaiBingoKaiSupr);
                        }
                        mouvementDuJoueur.droitDeMarcher = true;
                    }
                }
                if (attenteDuChoixDeLaPièce){
                    attenteDuChoixDeLaPièce = false;
                    UtilisationDeLaPièce(cible);
                }
            }
        } else {
            if (inTheZone && Input.GetButtonDown("A")){
                switch (étape){
                    case 0 :
                        dialcode.Ouverture();
                        AfficherTexte("Utilise des pièces pour obtenir des Yo-kai ou des objets au Bingo-kai !");
                        étape++;    
                        break;

                    case 1 :
                        AfficherTexte("Activer la manivelle au Bingo-kai ?");
                        étape++;    
                        break;

                    case 2 :
                        dialcode.OuvertureFenêtre(dialcode.choix);
                        dialcode.positionChoix = 0;
                        dialcode.ChangerPositionChoix();
                        étape++; 
                        break;

                    case 3 :
                        dialcode.Fermeture(false);
                        étape = 0;
                        if (dialcode.positionChoix == 0){
                            attenteDuChoixDeLaPièce = true;
                            Input.ResetInputAxes();
                            PlacerIcone();
                            if (canevasBingoKai != null) canevasBingoKai.SetActive(true);
                            canevaBingoKaiOuvert = true;
                            InitialisationBingoKai();
                            mouvementDuJoueur.droitDeMarcher = false;
                        }
                        break;
                }                
            }
        }
    }

    void AfficherTexte(string texte){
        dialcode.OuvertureFenêtre(dialcode.bulle);
        if (coroutineApparitionLettre == null){
            coroutineApparitionLettre = StartCoroutine(dialcode.PrintSentence(texte));
        } else {
            StopCoroutine(coroutineApparitionLettre);
            coroutineApparitionLettre = null;
            if (dialcode.espaceDialogue != null) {
                dialcode.espaceDialogue.text = texte;
            }
        }   
    }

    void OnCollisionEnter(Collision other) {
        if (other.transform.CompareTag("Player")){
            inTheZone = true;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.transform.CompareTag("Player")){
            inTheZone = false;
        }
    }

    void PlacerIcone(){
        foreach (Transform child in contentViewport.transform) {
            if (exemple == null){
                exemple = child.gameObject;
                exemple.SetActive(false);
            } else {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < 4; i++) {
            GameObject TV = Instantiate(exemple);
            TV.SetActive(true);
            TV.transform.parent = contentViewport.transform;
            Vector3 objectScale = TV.transform.localScale;
            objectScale.x = 1f;
            objectScale.y = 1f;
            objectScale.z = 1f;
            TV.transform.localScale = objectScale;

            listeObjets.Add(TV);
        }
    }

    void InitialisationBingoKai(){
        cible = 0;
        ChangerLaCible(cible, cible);
    }

    void ChangerLaCible(int ancienneCible, int nouvelleCible){
        cible = nouvelleCible;
        if (listeObjets.Count > ancienneCible){
            listeObjets[ancienneCible].transform.Find("cible").gameObject.SetActive(false);
        }
        if (listeObjets.Count > nouvelleCible){
            listeObjets[nouvelleCible].transform.Find("cible").gameObject.SetActive(true);
        }
    }

    void UtilisationDeLaPièce(int numéroPièce) {
        // if (numéroPièce == 0){
        //     yokaiQuiVasSortir = 95;
        //     nomObjetQuiVasSortir = "";
        // } else {
        //     yokaiQuiVasSortir = 0;
        //     nomObjetQuiVasSortir = "Lait merveilleux";
        // }
        if (lesDonnéesBingoKai != null) {
            int posibilitéeTotal_1 = lesDonnéesBingoKai.First_prize.yokais.Count + lesDonnéesBingoKai.First_prize.articles.Count;
            int posibilitéeTotal_2 = lesDonnéesBingoKai.Second_prize.yokais.Count + lesDonnéesBingoKai.Second_prize.articles.Count;
            int posibilitéeTotal_3 = lesDonnéesBingoKai.Third_prize.yokais.Count + lesDonnéesBingoKai.Third_prize.articles.Count;
            int posibilitéeTotal_4 = lesDonnéesBingoKai.Default_prize.yokais.Count + lesDonnéesBingoKai.Default_prize.articles.Count;

            int randomNumber = Random.Range(1, 101);

            // -------- PREMIER PRIX --------
            if (randomNumber <= tauxPremierPrix && posibilitéeTotal_1 > 0) {
                BingoKaiPriceData listePrix = lesDonnéesBingoKai.First_prize;

                randomNumber = Random.Range(0, posibilitéeTotal_1);
                if (randomNumber > listePrix.yokais.Count){
                    randomNumber -= listePrix.yokais.Count;
                    yokaiQuiVasSortir = 0;
                    nomObjetQuiVasSortir = listePrix.articles[randomNumber];
                } else {
                    int identifiantYoKai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(listePrix.yokais[randomNumber]).ID;
                    yokaiQuiVasSortir = identifiantYoKai;
                    nomObjetQuiVasSortir = "";  
                } 
            }
            // -------- DEUXIÈME PRIX --------
            if (randomNumber <= tauxSecondPrix && posibilitéeTotal_2 > 0) {
                BingoKaiPriceData listePrix = lesDonnéesBingoKai.Second_prize;

                randomNumber = Random.Range(0, posibilitéeTotal_2);
                if (randomNumber > listePrix.yokais.Count){
                    randomNumber -= listePrix.yokais.Count;
                    yokaiQuiVasSortir = 0;
                    nomObjetQuiVasSortir = listePrix.articles[randomNumber];
                } else {
                    int identifiantYoKai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(listePrix.yokais[randomNumber]).ID;
                    yokaiQuiVasSortir = identifiantYoKai;
                    nomObjetQuiVasSortir = "";  
                } 
            }
            // -------- TROISIÈME PRIX --------
            if (randomNumber <= tauxTroisièmePrix && posibilitéeTotal_3 > 0) {
                BingoKaiPriceData listePrix = lesDonnéesBingoKai.Third_prize;

                randomNumber = Random.Range(0, posibilitéeTotal_3);
                if (randomNumber > listePrix.yokais.Count){
                    randomNumber -= listePrix.yokais.Count;
                    yokaiQuiVasSortir = 0;
                    nomObjetQuiVasSortir = listePrix.articles[randomNumber];
                } else {
                    int identifiantYoKai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(listePrix.yokais[randomNumber]).ID;
                    yokaiQuiVasSortir = identifiantYoKai;
                    nomObjetQuiVasSortir = "";  
                } 
            }
            // -------- DERNIER PRIX --------
            else if (posibilitéeTotal_4 > 0) {
                BingoKaiPriceData listePrix = lesDonnéesBingoKai.Default_prize;

                randomNumber = Random.Range(0, posibilitéeTotal_4);
                if (randomNumber > listePrix.yokais.Count){
                    randomNumber -= listePrix.yokais.Count;
                    yokaiQuiVasSortir = 0;
                    nomObjetQuiVasSortir = listePrix.articles[randomNumber];
                } else {
                    int identifiantYoKai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(listePrix.yokais[randomNumber]).ID;
                    yokaiQuiVasSortir = identifiantYoKai;
                    nomObjetQuiVasSortir = "";  
                } 
            }
        } else {
            yokaiQuiVasSortir = 0;
            nomObjetQuiVasSortir = "Lait merveilleux";
        }


        videoPlayer.gameObject.SetActive(true);

        videoPlayer.loopPointReached -= FaireVideoCapsule;
        videoPlayer.loopPointReached += FaireVideoCapsule;
        
        videoPlayer.clip = videoBingoKai;
        videoPlayer.Play();

        Debug.Log("Lancement de la vidéo...");
    }

    void FaireVideoCapsule(UnityEngine.Video.VideoPlayer source) {
        Debug.Log("La vidéo est terminée !");
        videoPlayer.loopPointReached -= FaireVideoCapsule;
        
        if (yokaiQuiVasSortir > 0){
            videoPlayer.loopPointReached += ApparitionYoKai;
        } else {
            // nomObjetQuiVasSortir
            videoPlayer.loopPointReached += ApparitionObjet;
        }
        videoPlayer.clip = videoCapsuleRouge;
        videoPlayer.Play();   
    }

    void ApparitionYoKai(UnityEngine.Video.VideoPlayer source){
        if (gameObjectYoKaiBingoKaiSupr != null) {
            Destroy(gameObjectYoKaiBingoKaiSupr);
        }
        gameObjectYoKaiBingoKaiSupr = medaliumDictionnaire._instance.InstantiateYoKai(yokaiQuiVasSortir.ToString());
        gameObjectYoKaiBingoKaiSupr.transform.position = positionYoKaiCaméra.transform.position;
        gameObjectYoKaiBingoKaiSupr.transform.rotation = positionYoKaiCaméra.transform.rotation;
        ChangerLayerRecursif(gameObjectYoKaiBingoKaiSupr, 10);
        panelGain.SetActive(true);
        afficherTexteYokaiSortie = true;

        dialcode.Ouverture();
        AfficherTexte("Le Yo-kai " + medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir).Nom + " est sorti !");
        // Debug.Log(medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir).phrasesNourriture["Devient ton ami"]);
        // Debug.Log(medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir).keys);
        var keys = medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir).phrasesNourriture;
        Debug.Log(keys);
        Debug.Log(keys.Count);

        étapeAfficherTexteYokaiSortie = 1;
        Input.ResetInputAxes();
    }

    void ApparitionObjet(UnityEngine.Video.VideoPlayer source){
        if (gameObjectYoKaiBingoKaiSupr != null) {
            Destroy(gameObjectYoKaiBingoKaiSupr);
        }
        Sprite objet = Item._instance.GetSpriteByName(nomObjetQuiVasSortir);
        if (objet == null){
            Debug.Log("L'objet est null !");
        } else {
            imageObjet.sprite = objet;
            imageObjet.gameObject.SetActive(true);
        }
        panelGain.SetActive(true);
        afficherTexteYokaiSortie = true;

        dialcode.Ouverture();
        AfficherTexte("Tu as obtenu : " + nomObjetQuiVasSortir + " !");

        étapeAfficherTexteYokaiSortie = 1;
        Input.ResetInputAxes();
    }

    void ChangerLayerRecursif(GameObject gameObject, int layer){
        gameObject.layer = layer;
        foreach (Transform item in gameObject.transform){
            ChangerLayerRecursif(item.gameObject, layer);
        }
    }


    void AfficherLesPrixDuBingoKai(){
        lesDonnéesBingoKai = ChargerLesDonnéesBingoKai(cible);

        if (lesDonnéesBingoKai != null) {
            // -------- PREMIER PRIX --------
            for (int i = 0; i < listeImagePremierPrix.Count; i++) {
                if (lesDonnéesBingoKai.First_prize.yokais.Count > i){
                    listeImagePremierPrix[i].gameObject.SetActive(true);
                    string nomEn = lesDonnéesBingoKai.First_prize.yokais[i];
                    MedaliumNonPersonel yokai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(nomEn);
                    Sprite image = yokai.médaillon;
                    if (image != null) {
                        listeImagePremierPrix[i].sprite = image;
                    } else {
                        listeImagePremierPrix[i].gameObject.SetActive(false);
                    }
                } else {
                    listeImagePremierPrix[i].gameObject.SetActive(false);
                }
            }
            // -------- DEUXIÈME PRIX --------
            for (int i = 0; i < listeImageDeuxièmePrix.Count; i++) {
                if (lesDonnéesBingoKai.Second_prize.yokais.Count > i){
                    listeImageDeuxièmePrix[i].gameObject.SetActive(true);
                    string nomEn = lesDonnéesBingoKai.Second_prize.yokais[i];
                    MedaliumNonPersonel yokai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(nomEn);
                    Sprite image = yokai.médaillon;
                    if (image != null) {
                        listeImageDeuxièmePrix[i].sprite = image;
                    } else {
                        listeImageDeuxièmePrix[i].gameObject.SetActive(false);
                    }
                } else {
                    listeImageDeuxièmePrix[i].gameObject.SetActive(false);
                }
            }
            // -------- TROISIÈME PRIX --------
            for (int i = 0; i < listeImageTroisièmePrix.Count; i++) {
                if (lesDonnéesBingoKai.Third_prize.yokais.Count > i){
                    listeImageTroisièmePrix[i].gameObject.SetActive(true);
                    string nomEn = lesDonnéesBingoKai.Third_prize.yokais[i];
                    MedaliumNonPersonel yokai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(nomEn);
                    Sprite image = yokai.médaillon;
                    if (image != null) {
                        listeImageTroisièmePrix[i].sprite = image;
                    } else {
                        listeImageTroisièmePrix[i].gameObject.SetActive(false);
                    }
                } else {
                    listeImageTroisièmePrix[i].gameObject.SetActive(false);
                }
            }
            // -------- DERNIER PRIX --------
            for (int i = 0; i < listeImageDernierPrix.Count; i++) {
                if (lesDonnéesBingoKai.Default_prize.yokais.Count > i){
                    listeImageDernierPrix[i].gameObject.SetActive(true);
                    string nomEn = lesDonnéesBingoKai.Default_prize.yokais[i];
                    MedaliumNonPersonel yokai = medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(nomEn);
                    Sprite image = yokai.médaillon;
                    if (image != null) {
                        listeImageDernierPrix[i].sprite = image;
                    } else {
                        listeImageDernierPrix[i].gameObject.SetActive(false);
                    }
                } else {
                    listeImageDernierPrix[i].gameObject.SetActive(false);
                }
            }
            // ----------------

        }
    }

    BingoKaiCityData ChargerLesDonnéesBingoKai(int position){
        TextAsset jsonFile = Resources.Load<TextAsset>("bingo-kai/yokaiWatch3_bingoKai");

        if (jsonFile == null) {
            Debug.LogError("Impossible de trouver le fichier JSON dans le dossier Resources !");
            return null;
        }

        string jsonString = jsonFile.text;

        if (jsonString.TrimStart().StartsWith("[")) {
            jsonString = "{\"items\":" + jsonString + "}";
        }

        BingoKaiBigData data = JsonUtility.FromJson<BingoKaiBigData>(jsonString);


        string variable = listePièceBingoKai[position].nom_anglais;
        BingoKaiData piece = variable switch {
            "Red_Coin" => data.Red_Coin,
            "Yellow_Coin" => data.Yellow_Coin,
            "Orange_Coin" => data.Orange_Coin,
            "Pink_Coin" => data.Pink_Coin,
            "Green_Coin" => data.Green_Coin,
            "Blue_Coin" => data.Blue_Coin,
            "Purple_Coin" => data.Purple_Coin,
            "One_Star_Coin" => data.One_Star_Coin,
            "Five_Star_Coin" => data.Five_Star_Coin,
            "Special_Coin" => data.Special_Coin,
            "Yo_Mystery_Coin" => data.Yo_Mystery_Coin,
            "Kai_Mystery_Coin" => data.Kai_Mystery_Coin,
            "Soul_Mystery_Coin" => data.Soul_Mystery_Coin,
            "Sushi_Coin" => data.Sushi_Coin,
            "Tempura_Coin" => data.Tempura_Coin,
            "Whirlwind_Coin" => data.Whirlwind_Coin,
            "Wanderer_Coin" => data.Wanderer_Coin,
            "Rusted_Coin" => data.Rusted_Coin,
            "Flower_Excite_Coin" => data.Flower_Excite_Coin,
            "Bird_Excite_Coin" => data.Bird_Excite_Coin,
            "Wind_Excite_Coin" => data.Wind_Excite_Coin,
            "Moon_Excite_Coin" => data.Moon_Excite_Coin,
            "Mellow_Coin" => data.Mellow_Coin,
            "Brave_Dream_Coin" => data.Brave_Dream_Coin,
            "Light_Dream_Coin" => data.Light_Dream_Coin,
            "Platin_Dream_Coin" => data.Platin_Dream_Coin,
            "Fortun_Dream_Coin" => data.Fortun_Dream_Coin,
            "Princess_Dream_Coin" => data.Princess_Dream_Coin,
            "Wisdom_Dream_Coin" => data.Wisdom_Dream_Coin,
            "Chivalry_Dream_Coin" => data.Chivalry_Dream_Coin,
            "_2nd_Helping_Coin" => data._2nd_Helping_Coin,
            "Belly_Buster_Coin" => data.Belly_Buster_Coin,
            "L_Souvenir_Coin" => data.L_Souvenir_Coin,
            "F_Souvenir_Coin" => data.F_Souvenir_Coin,
            "S_Souvenir_Coin" => data.S_Souvenir_Coin,
            "K_Souvenir_Coin" => data.K_Souvenir_Coin,
            "Joyful_Coin" => data.Joyful_Coin,
            "Excellent_Coin" => data.Excellent_Coin,
            "Beaming_Coin" => data.Beaming_Coin,
            "Iron_Pot_Coin" => data.Iron_Pot_Coin,
            "Fancy_That_Coin" => data.Fancy_That_Coin,
            "Black_Coin" => data.Black_Coin,
            "Demonic_Coin" => data.Demonic_Coin,
            "Underworld_Coin" => data.Underworld_Coin,
            "Sealed_Coin" => data.Sealed_Coin,
            "Emblem_Coin" => data.Emblem_Coin,
            "Treasure_Coin" => data.Treasure_Coin,
            "Legend_Coin" => data.Legend_Coin,
            "Super_Red_Coin" => data.Super_Red_Coin,
            "Super_Yellow_Coin" => data.Super_Yellow_Coin,
            "Super_Orange_Coin" => data.Super_Orange_Coin,
            "Super_Pink_Coin" => data.Super_Pink_Coin,
            "Super_Green_Coin" => data.Super_Green_Coin,
            "Super_Blue_Coin" => data.Super_Blue_Coin,
            "Super_Purple_Coin" => data.Super_Purple_Coin,
            "Super_Light_Blue_Coin" => data.Super_Light_Blue_Coin,
            "Ultra_Red_Coin" => data.Ultra_Red_Coin,
            "Ultra_Yellow_Coin" => data.Ultra_Yellow_Coin,
            "Ultra_Orange_Coin" => data.Ultra_Orange_Coin,
            "Ultra_Pink_Coin" => data.Ultra_Pink_Coin,
            "Ultra_Green_Coin" => data.Ultra_Green_Coin,
            "Ultra_Blue_Coin" => data.Ultra_Blue_Coin,
            "Ultra_Purple_Coin" => data.Ultra_Purple_Coin,
            "Ultra_Light_Blue_Coin" => data.Ultra_Light_Blue_Coin,
            "_3DS_Coin" => data._3DS_Coin,
            _ => null
        };

        return piece?.Granval;

    }
}


[System.Serializable]
public class PièceBingoKai{
    public int id;
    public string nom_anglais;
    public Sprite sprite;
    public bool posséder;
}