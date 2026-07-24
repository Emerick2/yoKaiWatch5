using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private bool afficherTexteYokaiSortie;
    private int étapeAfficherTexteYokaiSortie;
    private GameObject gameObjectYoKaiBingoKaiSupr;
    private bool attenteDuChoixDeLaPièce = false;

    void Start(){
        dialcode = DialcodePrinted.Instance;
        if (canevasBingoKai != null) canevasBingoKai.SetActive(false);
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
                ChangerLaCible(cible, ((cible+1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            } else if (Input.GetButtonDown("left")){
                ChangerLaCible(cible, ((cible-1)%listeObjets.Count+listeObjets.Count)%listeObjets.Count);
            }
            if (Input.GetButtonDown("B")){
                if (canevasBingoKai != null) canevasBingoKai.SetActive(false);
                canevaBingoKaiOuvert = false;
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
        yokaiQuiVasSortir = 95;
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
        
        videoPlayer.loopPointReached += ApparitionYoKai;
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
        étapeAfficherTexteYokaiSortie = 1;                
        Input.ResetInputAxes();
    }

    void ChangerLayerRecursif(GameObject gameObject, int layer){
        gameObject.layer = layer;
        foreach (Transform item in gameObject.transform){
            ChangerLayerRecursif(item.gameObject, layer);
        }
    }
}
