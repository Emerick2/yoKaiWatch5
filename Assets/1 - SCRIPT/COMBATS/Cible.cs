using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cible : MonoBehaviour{
    [HideInInspector] public GestionnaireCombat gestionnaireCombat;
    [HideInInspector] public List<GameObject> yoKaiGameObjects;
    public Sprite imageSélection;
    public Sprite imageNonSélection;
    public int cibleVisée;
    public SpriteRenderer imageEstUnAmi;
    public Sprite ouiIlEstUnAmi;
    public Sprite nonIlEstPasUnAmi;
    public TextMeshPro texteAlliment;

    public SpriteRenderer imageElement;

    public float vitesseCible;
    Vector2 directionHaut = new Vector2(0, 1);
    Vector2 directionBas = new Vector2(0, -1);
    Vector2 directionGauche = new Vector2(-1, 0);
    Vector2 directionDroit = new Vector2(1, 0);
    private Vector2 positionInitial = new Vector2(0, 0);

    void Start(){
        positionInitial = transform.position;
        Initialisation();
    }

    public void Initialisation(){
        imageElement.gameObject.SetActive(false);
        imageEstUnAmi.gameObject.SetActive(false);
        texteAlliment.gameObject.SetActive(false);
        transform.position = positionInitial;
    }

    void Update(){
        if (Input.GetButton("flèche droite")){
            transform.Translate(directionDroit * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("flèche gauche")){
            transform.Translate(directionGauche * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("flèche haut")){
            transform.Translate(directionHaut * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("flèche bas")){
            transform.Translate(directionBas * vitesseCible * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other){
        GameObject parent = EstValide(other.gameObject);
        if (parent != null){
            Debug.Log("ici");
            GetComponent<SpriteRenderer>().sprite = imageSélection;
            int IDTrouvé = 0;
            for (int i = 0; i < gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain.Count; i++){
                if (gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain[i].gameObjectSurLaScène == parent){
                    IDTrouvé = i;
                }
            }
            bool estAmi = false;
            for (int i = 0; i < MesYoKai.Instance.maListe.Count; i++){
                if (gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain[IDTrouvé].ID ==  MesYoKai.Instance.maListe[i].ID){
                    estAmi = true;
                    break;
                }
            }
            if (estAmi){
                imageEstUnAmi.sprite = ouiIlEstUnAmi;
            } else {
                imageEstUnAmi.sprite = nonIlEstPasUnAmi;
            }
            imageEstUnAmi.transform.localScale = new Vector3(1,1,1);
            texteAlliment.text = medaliumDictionnaire._instance.dictionnaire[gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain[IDTrouvé].ID].platPréféré.ToString();

            imageElement.sprite = new ServantImage().ReadImage_Element(medaliumDictionnaire._instance.dictionnaire[gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain[IDTrouvé].ID].monElement);
            imageElement.transform.localScale = new Vector3(1,1,1);

            imageElement.gameObject.SetActive(true);
            imageEstUnAmi.gameObject.SetActive(true);
            texteAlliment.gameObject.SetActive(true);
            // Debug.Log(IDTrouvé);
        }
    }
    void OnTriggerExit(Collider other){
        GameObject parent = EstValide(other.gameObject);
        if (parent != null){
            GetComponent<SpriteRenderer>().sprite = imageNonSélection;
            imageElement.gameObject.SetActive(false);
            imageEstUnAmi.gameObject.SetActive(false);
            texteAlliment.gameObject.SetActive(false);
        }
    }

    private GameObject EstValide(GameObject gameObjectAAnalyser){
        if (yoKaiGameObjects.Contains(gameObjectAAnalyser)){
            return gameObjectAAnalyser;
        }
        Transform parentTransform = gameObjectAAnalyser.transform.parent;
        
        while (parentTransform != null){
            if (yoKaiGameObjects.Contains(parentTransform.gameObject)){
                return parentTransform.gameObject;
            }
            parentTransform = parentTransform.parent;
        }
        return null;
    }
}