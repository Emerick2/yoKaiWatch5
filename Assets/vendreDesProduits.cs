using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vendreDesProduits : MonoBehaviour{
    public lesObjetsQuiExistes scriptDesItem;
    public Text monArgent;
    public List<int> IDDesObjetAVendre = new List<int>();
    private List<GameObject> espaceDeVante = new List<GameObject>();
    public GameObject espaceDeVantePrefabe;
    private int sélection;
    public GameObject blocDeAffichage;
    public bool acheter = false;
    public GameObject blocDeAchat;
    public Transform conteneur;
    void Start(){
        monArgent.text = NewDataCenter.Instance.Lire("€");
        for (int i = 0; i < IDDesObjetAVendre.Count; i++){
            GameObject TV = Instantiate(espaceDeVantePrefabe);
            espaceDeVante.Add(TV);
            TV.transform.Find("Image").GetComponent<Image>().sprite = scriptDesItem.lesObjets[IDDesObjetAVendre[i]].image;
            TV.transform.Find("nom").GetComponent<Text>().text = scriptDesItem.lesObjets[IDDesObjetAVendre[i]].nom;
            TV.transform.Find("prix").GetComponent<Text>().text = scriptDesItem.lesObjets[IDDesObjetAVendre[i]].prix.ToString()+"€";
            TV.transform.parent = conteneur;
            TV.transform.localScale = new Vector3(1,1,1);
        }
        Choisir();
    }

    void Update(){
        if (!acheter){
            if (Input.GetKeyDown(KeyCode.UpArrow)){
                espaceDeVante[sélection].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                sélection--;
                Choisir();
            }else if (Input.GetKeyDown(KeyCode.DownArrow)){
                espaceDeVante[sélection].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                sélection++;
                Choisir();
            }else if (Input.GetKeyDown(KeyCode.E)){
                acheter=true;
                blocDeAchat.SetActive(true);
                blocDeAchat.GetComponent<AcheterUnProduit>().prixDeUnProduit = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].prix;
                blocDeAchat.GetComponent<AcheterUnProduit>().Réinitialiser();
                blocDeAchat.GetComponent<AcheterUnProduit>().nom = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].nom;
                blocDeAchat.GetComponent<AcheterUnProduit>().vdp = transform.GetComponent<vendreDesProduits>();
            }
        }else{
            if (Input.GetKeyDown(KeyCode.B)){
                acheter=false;
                blocDeAchat.SetActive(false);
            }
        }
    }

    void Choisir(){
        if (sélection<0){
            sélection = espaceDeVante.Count-1;
        }else if (sélection >= espaceDeVante.Count){
            sélection = 0;
        }
        espaceDeVante[sélection].GetComponent<Image>().color = new Color(0.986f, 1f, 0.617f, 1f);
        blocDeAffichage.transform.Find("Image").Find("Image").GetComponent<Image>().sprite = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].image;
        blocDeAffichage.transform.Find("Nom").GetComponent<Text>().text = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].nom;
        blocDeAffichage.transform.Find("Description").GetComponent<Text>().text = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].description;

        string va = scriptDesItem.lesObjets[IDDesObjetAVendre[sélection]].catégorie.ToString();
        if (va=="Nouriture"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[0]];
        }else if (va=="Objet"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[1]];
        }else if (va=="Animal"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[2]];
        }else if (va=="Équipement"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[3]];
        }else if (va=="Âme"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[4]];
        }else if (va=="ObjetClef"){
            blocDeAffichage.transform.Find("catégorie").GetComponent<Image>().sprite = scriptDesItem.imageDesCatégorie[IDDesObjetAVendre[5]];
        }
    }
    public void ReChargerMonArgent(){
        monArgent.text = NewDataCenter.Instance.Lire("€");
    }
}
