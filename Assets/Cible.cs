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

    void Start(){
        imageElement.gameObject.SetActive(false);
        imageEstUnAmi.gameObject.SetActive(false);
        texteAlliment.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other){
        GameObject parent = EstValide(other.gameObject);
        if (parent != null){
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

            imageElement.sprite = medaliumDictionnaire._instance.ObtenirImageElement(medaliumDictionnaire._instance.dictionnaire[gestionnaireCombat.listeYoKaiEnnemieSurLeTerrain[IDTrouvé].ID].monElement);
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