using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lesObjet : MonoBehaviour{
    private lesObjetsQuiExistes objet;
    //public List<ITEM> item;
    private List<Transform> emplacementPourItem = new List<Transform>();
    public Transform contenu;
    public mouvementSacObjet sac;
    private int numéro = 3;
    private int indiceDeMaPosition = 0;
    void Start(){
        objet = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<lesObjetsQuiExistes>();
        foreach (Transform child in contenu){
            Transform sélectionChild = child.Find("sélection");
            emplacementPourItem.Add(child);
            if (sélectionChild != null){
                if (numéro == 3){
                    sac.sélectionnéHaut.Add(sélectionChild.gameObject);
                    numéro--;
                }else if (numéro == 2){
                    sac.sélectionnéMoyen.Add(sélectionChild.gameObject);
                    numéro--;
                }else {
                    sac.sélectionnéBas.Add(sélectionChild.gameObject);
                    numéro=3;
                }
            }
        }
        for (int i = 0; i < objet.lesObjets.Count; i++){
            if (NewDataCenter.Instance.Lire(objet.lesObjets[i].nom) != "0"){
                Transform imageChild = emplacementPourItem[indiceDeMaPosition].Find("Image");
                Transform nombreChild = emplacementPourItem[indiceDeMaPosition].Find("nombre");
                Transform sélectionChild = emplacementPourItem[indiceDeMaPosition].Find("sélection");
                if (imageChild != null){
                    imageChild.GetComponent<Image>().sprite = objet.lesObjets[i].image;
                }
                if (nombreChild != null){
                    nombreChild.GetComponent<Text>().text = NewDataCenter.Instance.Lire(objet.lesObjets[i].nom);
                }
                indiceDeMaPosition++;
            }
        }
    }
}
