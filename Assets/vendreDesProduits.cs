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
    private int IDVisibleLaPlusBasse=0;//nombre de fois ou je suis décendu, se nombre pourait aller jusqua -5
    public GameObject cible;
    public GameObject cibleDeDépart;
    public UI_Déplacement monDéplacementScript;
    public RectTransform bloc;
    private int nombreMaximumDeArcticleAfficher = 4;
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
            }else if (Input.GetKeyDown(KeyCode.B)){
                espaceDeVante[sélection].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                sélection=0;
                Choisir();
                mouvementDuJoueur.droitDeMarcher = true;
                gameObject.SetActive(false);
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
        int temps = IDVisibleLaPlusBasse;
        IDVisibleLaPlusBasse = sélection-nombreMaximumDeArcticleAfficher;
        if (IDVisibleLaPlusBasse<0){IDVisibleLaPlusBasse=0;}
        if (IDVisibleLaPlusBasse>espaceDeVante.Count-1){IDVisibleLaPlusBasse=espaceDeVante.Count-nombreMaximumDeArcticleAfficher-3;}
        if (temps != IDVisibleLaPlusBasse){
            mouvementDesArcticle();
        }
    }
    public void ReChargerMonArgent(){
        monArgent.text = NewDataCenter.Instance.Lire("€");
    }

    void mouvementDesArcticle(){
        int nombreMaximumDeArcticleAfficher = 4;
        float décalageVersLeBas = espaceDeVante[0].transform.position.y - espaceDeVante[1].transform.position.y;
        float positionDeDépart = cibleDeDépart.transform.position.y;
        cible.transform.position = new Vector2(bloc.transform.position.x, (IDVisibleLaPlusBasse*décalageVersLeBas)+positionDeDépart);
        monDéplacementScript.STOP = false;
    }
}
