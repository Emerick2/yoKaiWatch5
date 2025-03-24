using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lesObjet : MonoBehaviour{
    public List<ITEM> item;
    public Transform contenu;
    public mouvementSacObjet sac;
    public int numéro = 3;
    void Start(){
        foreach (Transform child in contenu){
            Transform imageChild = child.Find("Image");
            Transform nombreChild = child.Find("nombre");
            Transform sélectionChild = child.Find("sélection");
            if (imageChild != null){
                imageChild.GetComponent<Image>().sprite = item[0].image;
            }
            if (nombreChild != null){
                nombreChild.GetComponent<Text>().text = "1";
            }
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
    }
}

[System.Serializable]
public class ITEM {
    public string nom;
    public Sprite image;
}
