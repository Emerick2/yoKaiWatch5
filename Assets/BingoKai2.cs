using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YKW.Core.Item.Class;
using YKW.Core.YoKai.Class;
using YKW.Core.BingoKai.Data;
using YKW.Core.Item.Enum;

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

    BingoKaiCoinData lesDonnéesBingoKai;
    public int tauxPremierPrix = 10;
    public int tauxSecondPrix = 45;
    public int tauxTroisièmePrix = 80;
    private int _yokaiDiscussion = 0;

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
    }

    void InitialisationBingoKai(){
        cible = 0;
        AfficherLesPrixDuBingoKai();
        ChangerLaCible(cible, cible);
        if (panelGain != null) panelGain.SetActive(false);
        if (imageObjet != null) imageObjet.gameObject.SetActive(false);

        if (gameObjectYoKaiBingoKaiSupr != null) {
            Destroy(gameObjectYoKaiBingoKaiSupr);
        }
        attenteDuChoixDeLaPièce = true;
        canevaBingoKaiOuvert = false;
    }

    void Update() {
        if (_yokaiDiscussion != 0){
            if (Input.GetButtonDown("A")){
                YoKai yokai = medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir);
                if (_yokaiDiscussion == 1) {
                    // Devient ton ami
                    // texte de "je veux être ton ami"
                    string friendSentence = "Merci de m'avoir libéré, devenons ami !";
                    if (yokai.phrasesNourriture != null && yokai.phrasesNourriture.TryGetValue("Devient ton ami", out string customPhrase) && !string.IsNullOrEmpty(customPhrase)) {
                        friendSentence = yokai.phrasesNourriture["Devient ton ami"];
                    }
                    AfficherTexte(friendSentence, yokai.nom);
                    _yokaiDiscussion = 2;
                } else if (_yokaiDiscussion == 2) {
                    // texte qui montre qu'il devient ami
                    dialcode.RecevoirUnCadeau(TypeDialogue.amiYoKai, 1, CatégorieItem.YoKai, yokaiQuiVasSortir, null, yokai.nom, false, yokai.nom, true);
                    _yokaiDiscussion = 3;
                } else {
                    // texte : "Le Yo-kai x a été ajouté au Médallium des Yo-kai !"
                    // <color=#3462C2></color>
                    AfficherTexte("Le Yo-kai "+yokai.nom+" a été ajouté au Médallium des Yo-kai !", "");
                    _yokaiDiscussion = 0;
                }
            }
            return;
        }

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
                ChangerLaCible(cible, ((cible+1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            } else if (Input.GetButtonDown("left")){
                ChangerLaCible(cible, ((cible-1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            }
            if (Input.GetButtonDown("B") && attenteDuChoixDeLaPièce){
                Exit();
            }
            if (Input.GetButtonDown("A")){
                if (afficherTexteYokaiSortie){
                    if (étapeAfficherTexteYokaiSortie == 1){
                        afficherTexteYokaiSortie = false;
                        étapeAfficherTexteYokaiSortie = 0;

                        dialcode.Fermeture(false);
                        if (panelGain != null) panelGain.SetActive(false);
                        if (imageObjet != null) imageObjet.gameObject.SetActive(false);
                        if (videoPlayer != null) {
                            videoPlayer.Stop();
                            videoPlayer.gameObject.SetActive(false);
                        }

                        if (gameObjectYoKaiBingoKaiSupr != null) {
                            Destroy(gameObjectYoKaiBingoKaiSupr);
                        }

                        attenteDuChoixDeLaPièce = true;
                        Input.ResetInputAxes();
                        Exit();
                    }
                }
                else if (attenteDuChoixDeLaPièce){
                    attenteDuChoixDeLaPièce = false;
                    UtilisationDeLaPièce(cible);
                }
            }
        } else {
            if (inTheZone && Input.GetButtonDown("A")){
                switch (étape){
                    case 0 :
                        if (mouvementDuJoueur.droitDeMarcher) {
                            mouvementDuJoueur.droitDeMarcher = false;
                            dialcode.Ouverture();
                            AfficherTexte("Utilise des pièces pour obtenir des Yo-kai ou des objets au Bingo-kai !", "");  
                        }
                        break;

                    case 1 :
                        AfficherTexte("Activer la manivelle au Bingo-kai ?", "");
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
                            InitialisationBingoKai();
                            attenteDuChoixDeLaPièce = true;
                            Input.ResetInputAxes();
                            PlacerIcone();
                            if (canevasBingoKai != null) canevasBingoKai.SetActive(true);
                            canevaBingoKaiOuvert = true;
                        }
                        break;
                }                
            }
        }
    }

    void AfficherTexte(string texte, string nom){
        dialcode.OuvertureFenêtre(dialcode.bulle);
        dialcode.name = nom;
        if (coroutineApparitionLettre == null){
            coroutineApparitionLettre = StartCoroutine(dialcode.PrintSentence(texte, FinCoroutineAppartionLettre));
        } else {
            StopCoroutine(coroutineApparitionLettre);
            étape++;
            coroutineApparitionLettre = null;
            if (dialcode.espaceDialogue != null) {
                dialcode.espaceDialogue.text = texte;
            }
        }   
    }

    public void FinCoroutineAppartionLettre(){
        étape++;
        coroutineApparitionLettre = null;
    }

    public void Exit(){
        if (canevasBingoKai != null) canevasBingoKai.SetActive(false);
        canevaBingoKaiOuvert = false;
        imageObjet.gameObject.SetActive(false);
        mouvementDuJoueur.droitDeMarcher = true;
        étape = 0;
    }

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
        listeObjets.Clear();
        exemple = null;
        foreach (Transform child in contentViewport.transform) {
            if (exemple == null){
                exemple = child.gameObject;
                exemple.SetActive(false);
            } else {
                Destroy(child.gameObject);
            }
        }
        foreach (PièceBingoKai piece in listePièceBingoKai) {
            GameObject nouvellePièce = Instantiate(exemple);
            nouvellePièce.SetActive(true);
            nouvellePièce.transform.parent = contentViewport.transform;
            
            Vector3 objectScale = Vector3.one;
            nouvellePièce.transform.localScale = objectScale;

            Image imgComp = nouvellePièce.GetComponent<Image>();
            if (imgComp != null && piece.sprite != null){
                imgComp.sprite = piece.sprite;
            }

            listeObjets.Add(nouvellePièce);
        }
    }


    void ChangerLaCible(int ancienneCible, int nouvelleCible){
        cible = nouvelleCible;
        if (listeObjets.Count > ancienneCible){
            listeObjets[ancienneCible].transform.Find("cible").gameObject.SetActive(false);
        }
        if (listeObjets.Count > nouvelleCible){
            listeObjets[nouvelleCible].transform.Find("cible").gameObject.SetActive(true);
        }

        AfficherLesPrixDuBingoKai();
    }

    void UtilisationDeLaPièce(int numéroPièce) {
        yokaiQuiVasSortir = 0;
        nomObjetQuiVasSortir = "";
        
        if (lesDonnéesBingoKai == null) return;
        int roll = Random.Range(1, 101);
        BingoKaiPrizeData prize = roll <= tauxPremierPrix ? lesDonnéesBingoKai.firstPrize
            : roll <= tauxSecondPrix ? lesDonnéesBingoKai.secondPrize
            : roll <= tauxTroisièmePrix ? lesDonnéesBingoKai.thirdPrize : lesDonnéesBingoKai.defaultPrize;
        if (prize == null || prize.Count == 0) prize = lesDonnéesBingoKai.defaultPrize;
        if (prize == null || prize.Count == 0) return;
        int rewardIndex = Random.Range(0, prize.Count);
        if (rewardIndex < prize.yokaiIds.Count) yokaiQuiVasSortir = prize.yokaiIds[rewardIndex];
        else nomObjetQuiVasSortir = ResolveItemName(prize.itemNames[rewardIndex - prize.yokaiIds.Count]);

        videoPlayer.gameObject.SetActive(true);

        videoPlayer.loopPointReached -= FaireVideoCapsule;
        videoPlayer.loopPointReached += FaireVideoCapsule;
        
        videoPlayer.clip = videoBingoKai;
        videoPlayer.Play();

        // Lancement de la vidéo...
    }

    void FaireVideoCapsule(UnityEngine.Video.VideoPlayer source) {
        // La vidéo est terminée !
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
        videoPlayer.loopPointReached -= ApparitionYoKai;
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
        AfficherTexte("Le Yo-kai " + medaliumDictionnaire._instance.ServantYoKai(yokaiQuiVasSortir).nom + " est sorti !", "");
        _yokaiDiscussion = 1;

        étapeAfficherTexteYokaiSortie = 1;
        Input.ResetInputAxes();
    }

    void ApparitionObjet(UnityEngine.Video.VideoPlayer source){
        videoPlayer.loopPointReached -= ApparitionObjet;
        if (gameObjectYoKaiBingoKaiSupr != null) {
            Destroy(gameObjectYoKaiBingoKaiSupr);
        }
        Sprite objet = Item.Instance?.GetSpriteByName(nomObjetQuiVasSortir);
        if (objet == null){
            Debug.Log("L'objet est null !");
        } else {
            if (imageObjet == null){
                Debug.LogWarning("La variable 'imageObjet' est null");
            } else {
                imageObjet.sprite = objet;
                imageObjet.gameObject.SetActive(true);
            }
        }
        if (panelGain == null) {
            Debug.LogWarning("La variable 'panelGain' est null");
        }
        panelGain.SetActive(true);
        afficherTexteYokaiSortie = true;

        dialcode.Ouverture();
        AfficherTexte("Tu as obtenu : " + nomObjetQuiVasSortir + " !", "");

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
                if (lesDonnéesBingoKai.firstPrize.yokaiIds.Count > i){
                    listeImagePremierPrix[i].gameObject.SetActive(true);
                    int speciesId = lesDonnéesBingoKai.firstPrize.yokaiIds[i];
                    YoKai yokai = medaliumDictionnaire._instance.ServantYoKai(speciesId);
                    Sprite image = yokai != null ? yokai.imageMédaillon : null;
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
                if (lesDonnéesBingoKai.secondPrize.yokaiIds.Count > i){
                    listeImageDeuxièmePrix[i].gameObject.SetActive(true);
                    int speciesId = lesDonnéesBingoKai.secondPrize.yokaiIds[i];
                    YoKai yokai = medaliumDictionnaire._instance.ServantYoKai(speciesId);
                    Sprite image = yokai != null ? yokai.imageMédaillon : null;
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
                if (lesDonnéesBingoKai.thirdPrize.yokaiIds.Count > i){
                    listeImageTroisièmePrix[i].gameObject.SetActive(true);
                    int speciesId = lesDonnéesBingoKai.thirdPrize.yokaiIds[i];
                    YoKai yokai = medaliumDictionnaire._instance.ServantYoKai(speciesId);
                    Sprite image = yokai != null ? yokai.imageMédaillon : null;
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
                if (lesDonnéesBingoKai.defaultPrize.yokaiIds.Count > i){
                    listeImageDernierPrix[i].gameObject.SetActive(true);
                    int speciesId = lesDonnéesBingoKai.defaultPrize.yokaiIds[i];
                    YoKai yokai = medaliumDictionnaire._instance.ServantYoKai(speciesId);
                    Sprite image = yokai != null ? yokai.imageMédaillon : null;
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

    private static string ResolveItemName(string englishName)
    {
        if (Item.Instance == null) return englishName;
        return Item.Instance.FindFoodWithEnglishName(englishName)?.nom
            ?? Item.Instance.FindEquipmentWithEnglishName(englishName)?.nom
            ?? Item.Instance.FindAnimalWithEnglishName(englishName)?.nom
            ?? englishName;
    }

    BingoKaiCoinData ChargerLesDonnéesBingoKai(int position)
    {
        if (position < 0 || position >= listePièceBingoKai.Count || medaliumDictionnaire._instance == null) return null;
        TextAsset jsonFile = Resources.Load<TextAsset>("bingo-kai/yokaiWatch3_bingoKai");
        if (jsonFile == null) return null;
        return BingoKaiCoinData.FromJson(jsonFile.text, listePièceBingoKai[position].nom_anglais, "Granval",
            englishName => medaliumDictionnaire._instance.TrouverUnYoKaiParSonNomAnglais(englishName)?.ID);
    }

}


[System.Serializable]
public class PièceBingoKai{
    public int id;
    public string nom_anglais;
    public Sprite sprite;
    public bool posséder;
}