using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                        PlacerIcone();
                        if (canevasBingoKai != null) canevasBingoKai.SetActive(true);
                        canevaBingoKaiOuvert = true;
                    }
                    break;
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
}
