using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SacCombats : MonoBehaviour{
    public GameObject blocAppSac;
    private Item itemScript;
    private GameObject exempleBloc;
    public GameObject conteneur;
    private List<GameObject> listeDeLaNouriturePlacé = new List<GameObject>();
    public Vector2 position;
    private int colonnes = 3;
    private int positionVisibleDansLeBloc;
    private bool utiliser = false;
    public TextMeshProUGUI texte;

    void Start() {
        blocAppSac.SetActive(false);
        itemScript = Item._instance;
    }

    public void Initialiser(){
        blocAppSac.SetActive(true);
        // positionVisibleDansLeBloc = 1;
        // position = new Vector2(0,0);
        StartCoroutine(InitialisationCoroutine());
    }

    IEnumerator InitialisationCoroutine(){
        while (!itemScript.chargementTerminé){
            yield return new WaitForSeconds(0.2f);
        }
        PlacerLesObjets();
        PlacerLaBonneLargeur();
        DefilemantAutomatique(0);
        ChangerDePosition(0,0);
        utiliser = true;
    }

    public void Arrêter(){
        blocAppSac.SetActive(false);
        utiliser = false;
    }

    void PlacerLaBonneLargeur() {
        if (conteneur == null) return;

        GridLayoutGroup grid = conteneur.GetComponent<GridLayoutGroup>();
        RectTransform rt = conteneur.GetComponent<RectTransform>();

        int nbElements = listeDeLaNouriturePlacé.Count;
        int nbColonnes = Mathf.CeilToInt((float)nbElements / 3f);

        float nouvelleLargeur = grid.padding.left + grid.padding.right;
        
        if (nbColonnes > 0) {
            nouvelleLargeur += (nbColonnes * grid.cellSize.x);
            nouvelleLargeur += (nbColonnes - 1) * grid.spacing.x;
        }

        rt.sizeDelta = new Vector2(nouvelleLargeur, rt.sizeDelta.y);
    }

    void DefilemantAutomatique(int direction){
        int cible = ConnaitreNombrePosition(position, true, false);
        bool forcer = false;
        // if (cible == 1){
        //     forcer = true;
        //     positionVisibleDansLeBloc = cible;
        // } else if (cible == listeDeLaNouriturePlacé.Count/colonnes){
        //     forcer = true;
        //     positionVisibleDansLeBloc = listeDeLaNouriturePlacé.Count/colonnes;
        // }

        if ((positionVisibleDansLeBloc < 2 && direction < 0) || (positionVisibleDansLeBloc > 4 && direction > 0)||forcer){
            int n = listeDeLaNouriturePlacé.Count/3;
            GridLayoutGroup grid = conteneur.GetComponent<GridLayoutGroup>();

            float width = 0;
            // width += grid.padding.left;
            width += (grid.cellSize.x + grid.spacing.x)*(cible-positionVisibleDansLeBloc);
            width *=-1;

            RectTransform rt = conteneur.GetComponent<RectTransform>();
            Vector2 anchored = rt.anchoredPosition;
            anchored.x = width;
            rt.anchoredPosition = anchored;
        } else {
            positionVisibleDansLeBloc += direction;
        }
    }

    void PlacerLesObjets(){
        foreach (Transform child in conteneur.transform){
            if (child.name == "exemple"){
                child.gameObject.SetActive(false);
                exempleBloc = child.gameObject;
            } else {
                Destroy(child.gameObject);
            }
        }
        listeDeLaNouriturePlacé.Clear();
        foreach (Nouriture item in itemScript.listeDesNouritures){
            GameObject TV = Instantiate(exempleBloc);
            TV.GetComponent<Image>().sprite = item.image;
            TV.transform.Find("nombre").gameObject.GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(int.Parse(NewDataCenter.Instance.Lire(item.nom)));
            TV.transform.Find("nombre").gameObject.GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi1 = false;
            TV.transform.Find("tient").gameObject.SetActive(false);
            TV.transform.parent = conteneur.transform;
            TV.transform.localScale = new Vector3(1,1,1);
            TV.transform.name = item.nom;
            listeDeLaNouriturePlacé.Add(TV);
            TV.SetActive(true);
        }
    }

    void Update() {
        if (utiliser) {
            if (Input.GetButtonDown("left")){
                ChangerDePosition(-1,0);
            } else if (Input.GetButtonDown("right")){
                ChangerDePosition(1,0);
            } else if (Input.GetButtonDown("up")){
                ChangerDePosition(0,-1);
            } else if (Input.GetButtonDown("down")){
                ChangerDePosition(0,1);
            } else if (Input.GetButtonDown("A")){
                Input.ResetInputAxes();
                GetComponent<GestionnaireCombat>().ChoisieAllimant(ConnaitreNombrePosition(position,false,false));
            }
        }
    }

    int ConnaitreNombrePosition(Vector2 laPosition, bool sansY, bool sansX){
        int nombreDeLigneHorizontale = listeDeLaNouriturePlacé.Count/3;
        int nombreDeLigneVerticale = 3;
        int x = (int)position.x+1;
        if (sansX) x=1;
        int y = (int)position.y+1;
        if (sansY) y=1;
        int p = ((int)position.y*nombreDeLigneHorizontale)+(int)position.x;
        p += (int)position.y;
        // p=((p%listeDeLaNouriturePlacé.Count)+listeDeLaNouriturePlacé.Count)%listeDeLaNouriturePlacé.Count;
        if (listeDeLaNouriturePlacé.Count<=p){
            p = listeDeLaNouriturePlacé.Count-1;
        }
        //Debug.Log(p);
        return p;
        
    }

    GameObject AvoirElementPosition(Vector2 laPosition){
        int p = ConnaitreNombrePosition(laPosition, false, false);
        if (listeDeLaNouriturePlacé.Count<=p){
            Debug.Log("Problème, l'élément : "+p+"est inférieur à la longueur de la liste qui est de "+listeDeLaNouriturePlacé.Count);
            return null;
        }
        return listeDeLaNouriturePlacé[p];
    }

    void ChangerDePosition(int x, int y){
        int nombreDeLignes = Mathf.CeilToInt((float)listeDeLaNouriturePlacé.Count / colonnes);
        GameObject obj = AvoirElementPosition(position);
        if (obj != null) obj.transform.Find("tient").gameObject.SetActive(false);
        position.x=((position.x+x%nombreDeLignes)+nombreDeLignes)%nombreDeLignes;
        position.y=((position.y+y%colonnes)+colonnes)%colonnes;
        obj = AvoirElementPosition(position);
        if (obj != null) obj.transform.Find("tient").gameObject.SetActive(true);
        DefilemantAutomatique(x);
        texte.text = "Utiliser "+obj.name+" ?";
    }
}
