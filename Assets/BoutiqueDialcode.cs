using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoutiqueDialcode : MonoBehaviour{
    public List<int> produitAllimentaire;
    public GameObject bloc;
    private GameObject exemple;
    private List<GameObject> listeObjetsPosées = new List<GameObject>();
    private Item itemScript;
    private int cible;
    public Transform blocInformation;
    private int pointLePlusHautVisible;
    private int hauteurMaximal;
    [HideInInspector] public bool acheter;
    public AcheterUnProduit acheterUnProduitScript;
    [HideInInspector] public bool acheterUnProduitFenêtreOuverte = false;
    void Start(){
        StartCoroutine(Initialisation());
    }
    IEnumerator Initialisation(){
        cible = 0;
        pointLePlusHautVisible = 0;
        itemScript = Item._instance;
        while (!itemScript.chargementTerminé){
            yield return new WaitForSeconds(0.1f);
        }
        ViderLeBloc();
        foreach (int o in produitAllimentaire){
            if (o>=0 && o<itemScript.listeDesNouritures.Count){
                Nouriture elem = itemScript.listeDesNouritures[o];
                AjouterUnÉlémentAuBloc(elem.image, elem.nom, elem.prixAchat);
            }
        }
        ChangerDeCible(0);
        TailleLayoutGroup();
        ReChargerMonArgent();
        acheterUnProduitScript.gameObject.SetActive(false);
        acheterUnProduitFenêtreOuverte = false;
    }

    void ViderLeBloc(){
        foreach (Transform item in bloc.transform){
            if (exemple == null){
                exemple = item.gameObject;
                exemple.SetActive(false);
            } else {
                Destroy(item.gameObject);
            }
            listeObjetsPosées.Clear();
        }
    }
    void AjouterUnÉlémentAuBloc(Sprite image, string nom, float quantitée){
        GameObject TV = Instantiate(exemple);
        TV.transform.Find("img").GetComponent<Image>().sprite = image;
        TV.transform.Find("nom").GetComponent<TextMeshProUGUI>().text = nom;
        TV.transform.Find("prix").GetComponent<TextMeshProUGUI>().text = quantitée.ToString()+"€";
        TV.transform.parent = bloc.transform;
        TV.transform.localScale = new Vector3(1,1,1);
        TV.SetActive(true);
        listeObjetsPosées.Add(TV);
    }


    void Update(){
        if (!acheterUnProduitFenêtreOuverte){
            if (Input.GetButtonDown("flèche bas")){
                ChangerDeCible(1);
            } else if (Input.GetButtonDown("flèche haut")){
                ChangerDeCible(-1);
            } else if (Input.GetButtonDown("A")){
                acheterUnProduitScript.gameObject.SetActive(true);
                acheterUnProduitFenêtreOuverte = true;
                acheterUnProduitScript.Réinitialiser(itemScript.listeDesNouritures[produitAllimentaire[cible]].prixAchat, itemScript.listeDesNouritures[produitAllimentaire[cible]].nom);
            }
        } else if (Input.GetButtonDown("B")){
            acheterUnProduitScript.gameObject.SetActive(false);
            acheterUnProduitFenêtreOuverte = false;
        }
    }

    void ChangerDeCible(int augmentation){
        listeObjetsPosées[cible].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        int n = listeObjetsPosées.Count;
        cible = (((cible+augmentation)%n)+n)%n;
        listeObjetsPosées[cible].GetComponent<Image>().color = new Color(2f, 0.94875f, 0f, 1f);
        RemplirBlocInformation();
        ChangementHauteur(augmentation);
    }

    void RemplirBlocInformation(){
        Nouriture élément = itemScript.listeDesNouritures[produitAllimentaire[cible]];
        blocInformation.Find("icone/Image").GetComponent<Image>().sprite = élément.image;
        blocInformation.Find("Nom").GetComponent<TextMeshProUGUI>().text = élément.nom;
        blocInformation.Find("Description").GetComponent<TextMeshProUGUI>().text = élément.description;
        blocInformation.Find("Quantitée/nombre").GetComponent<TextMeshProUGUI>().text = NewDataCenter.Instance.Lire(élément.nom);

        blocInformation.Find("Spécial équipement").gameObject.SetActive(false);
        blocInformation.Find("Spécial alliment").gameObject.SetActive(true);
        blocInformation.Find("Spécial alliment/ami").GetComponent<TextMeshProUGUI>().text = Item._instance.listeDesEffetsNouritures[élément.IdEffetSiSurAmi].description;
        blocInformation.Find("Spécial alliment/Ennemie").GetComponent<TextMeshProUGUI>().text = Item._instance.listeDesEffetsNouritures[élément.IdEffetSurSiSurEnnemie].description;
    }

    void TailleLayoutGroup(){
        int n = listeObjetsPosées.Count;
        GridLayoutGroup glp = bloc.GetComponent<GridLayoutGroup>();
        float taille = glp.padding.top+glp.padding.bottom;
        taille += glp.cellSize.y*n;
        if (n > 0) taille += glp.spacing.y * (n - 1);
        RectTransform rt = bloc.GetComponent<RectTransform>();
        Vector2 newSize = rt.sizeDelta;
        newSize.y = taille;
        rt.sizeDelta = newSize;
        hauteurMaximal = (int)taille;
        // Debug.Log("Nouvelle hauteur : " + taille);
    }

    void ChangementHauteur(int changement) {
        if (changement != 0) {
            int nbElementsAffichables = 5;
            int marge = 1;
            int seuilHaut = pointLePlusHautVisible + marge;
            int seuilBas = pointLePlusHautVisible + (nbElementsAffichables - 1) - marge;

            bool estTropHaut = cible < seuilHaut;
            bool estTropBas = cible > seuilBas;

            if (estTropHaut || estTropBas) {
                int n = pointLePlusHautVisible;

                if (estTropHaut) {
                    n = cible - marge;
                } 
                else if (estTropBas) {
                    n = cible - (nbElementsAffichables - 1) + marge;
                }

                n = Mathf.Max(0, n);
                
                pointLePlusHautVisible = n;

                GridLayoutGroup glp = bloc.GetComponent<GridLayoutGroup>();
                RectTransform rt = bloc.GetComponent<RectTransform>();

                float nouvellePosition = glp.padding.top + (glp.cellSize.y * n) + (glp.spacing.y * n);

                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, nouvellePosition);
            }
        }
    }
    public void ReChargerMonArgent(){
        Debug.Log(NewDataCenter.Instance.LireMonaie());
    }
}