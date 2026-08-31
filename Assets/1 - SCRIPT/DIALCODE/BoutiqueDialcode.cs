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

    public bool acheterUser = true;

    private void AssignerContexteAchat(){
        if (acheterUnProduitScript != null){
            acheterUnProduitScript.vdp = this;
        }
    }

    void Start(){
        AssignerContexteAchat();
        StartCoroutine(Initialisation());
    }
    public IEnumerator Initialisation(bool initialisationTotal = true){
        if (initialisationTotal){
            cible = 0;
            pointLePlusHautVisible = 0;
            itemScript = Item.Instance;

            if (itemScript == null){
                yield break;
            }

            List<int> produitsValides = new List<int>();
            foreach (int itemId in produitAllimentaire){
                if (itemId < 0 || itemId >= itemScript.listeDesNouritures.Count){
                    continue;
                }

                if (!acheterUser){
                    if (int.Parse(NewDataCenter.Instance.Lire(itemScript.listeDesNouritures[itemId].nom)) <= 0){
                        continue;
                    }
                }

                produitsValides.Add(itemId);
            }
            produitAllimentaire = produitsValides;
        }
        ReChargerMonArgent();
        if (produitAllimentaire == null || produitAllimentaire.Count == 0) {
            cible = 0;
        } else if (cible < 0 || cible >= produitAllimentaire.Count) {
            cible = 0;
        }

        while (!itemScript.chargementTerminé){
            yield return new WaitForSeconds(0.1f);
        }
        ViderLeBloc();
        foreach (int o in produitAllimentaire){
            if (o>=0 && o<itemScript.listeDesNouritures.Count){
                Nouriture elem = itemScript.listeDesNouritures[o];
                if (acheterUser) AjouterUnÉlémentAuBloc(elem.image, elem.nom, elem.prixAchat);
                else AjouterUnÉlémentAuBloc(elem.image, elem.nom, elem.prixVente);
            }
        }
        ChangerDeCible(0);
        TailleLayoutGroup();
        AssignerContexteAchat();
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
        }
        listeObjetsPosées.Clear();
    }
    void AjouterUnÉlémentAuBloc(Sprite image, string nom, float quantitée){
        GameObject TV = Instantiate(exemple);
        TV.GetComponent<Image>().enabled=true;
        TV.transform.Find("img").GetComponent<Image>().sprite = image;
        TV.transform.Find("img").GetComponent<Image>().enabled = true;
        TV.transform.Find("nom").GetComponent<TextMeshProUGUI>().text = nom;
        TV.transform.Find("nom").GetComponent<TextMeshProUGUI>().enabled = true;
        TV.transform.Find("prix").GetComponent<TextMeshProUGUI>().text = quantitée.ToString()+"€";
        TV.transform.Find("prix").GetComponent<TextMeshProUGUI>().enabled = true;
        TV.transform.parent = bloc.transform;
        TV.transform.localScale = new Vector3(1,1,1);
        TV.SetActive(true);
        listeObjetsPosées.Add(TV);
    }


    void Update(){
        if (!acheterUnProduitFenêtreOuverte){
            if (Input.GetButtonDown("down")){
                ChangerDeCible(1);
            } else if (Input.GetButtonDown("up")){
                ChangerDeCible(-1);
            } else if (Input.GetButtonDown("A")){
                if (itemScript == null || produitAllimentaire == null || produitAllimentaire.Count == 0 || cible < 0 || cible >= produitAllimentaire.Count){
                    return;
                }

                AssignerContexteAchat();
                acheterUnProduitScript.gameObject.SetActive(true);
                acheterUnProduitFenêtreOuverte = true;
                int itemId = produitAllimentaire[cible];
                if (itemId < 0 || itemId >= itemScript.listeDesNouritures.Count){
                    return;
                }

                Nouriture nourriture = itemScript.listeDesNouritures[itemId];
                if (acheterUser) acheterUnProduitScript.Réinitialiser(nourriture.prixAchat, nourriture.nom);
                else acheterUnProduitScript.Réinitialiser(nourriture.prixVente, nourriture.nom);
            }
        } else if (Input.GetButtonDown("B")){
            acheterUnProduitScript.gameObject.SetActive(false);
            acheterUnProduitFenêtreOuverte = false;
        }
    }

    void ChangerDeCible(int augmentation){
        if (produitAllimentaire == null || produitAllimentaire.Count == 0 || listeObjetsPosées.Count == 0){
            cible = 0;
            if (blocInformation != null) blocInformation.gameObject.SetActive(false);
            return;
        }

        if (cible < 0 || cible >= produitAllimentaire.Count){
            cible = 0;
        }

        foreach (GameObject item in listeObjetsPosées){
            item.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }

        int n = Mathf.Min(listeObjetsPosées.Count, produitAllimentaire.Count);
        if (n <= 0) return;

        cible = (((cible + augmentation) % n) + n) % n;
        if (cible < listeObjetsPosées.Count){
            listeObjetsPosées[cible].GetComponent<Image>().color = new Color(2f, 0.94875f, 0f, 1f);
        }
        RemplirBlocInformation();
        ChangementHauteur(augmentation);
    }

    public void RemplirBlocInformation(){
        if (itemScript == null || produitAllimentaire == null || produitAllimentaire.Count == 0 || blocInformation == null){
            cible = 0;
            if (blocInformation != null) blocInformation.gameObject.SetActive(false);
            return;
        }

        if (cible < 0 || cible >= produitAllimentaire.Count){
            cible = 0;
        }

        if (cible < 0 || cible >= produitAllimentaire.Count){
            blocInformation.gameObject.SetActive(false);
            return;
        }

        int itemId = produitAllimentaire[cible];
        if (itemId < 0 || itemId >= itemScript.listeDesNouritures.Count){
            blocInformation.gameObject.SetActive(false);
            return;
        }

        blocInformation.gameObject.SetActive(true);

        Nouriture élément = itemScript.listeDesNouritures[itemId];
        blocInformation.Find("icone/Image").GetComponent<Image>().sprite = élément.image;
        blocInformation.Find("Nom").GetComponent<TextMeshProUGUI>().text = élément.nom;
        blocInformation.Find("Description").GetComponent<TextMeshProUGUI>().text = élément.description;
        blocInformation.Find("Quantitée/nombre").GetComponent<TextMeshProUGUI>().text = NewDataCenter.Instance.Lire(élément.nom);

        blocInformation.Find("Spécial équipement").gameObject.SetActive(false);
        blocInformation.Find("Spécial alliment").gameObject.SetActive(true);

        string texteAmi = "";
        texteAmi = Item.Instance.ObtenirDescriptionEffetAllie(élément);
        blocInformation.Find("Spécial alliment/ami").GetComponent<TextMeshProUGUI>().text = texteAmi;

        string texteEnnemi = "";
        texteEnnemi = Item.Instance.ObtenirDescriptionEffetEnnemi(élément);
        blocInformation.Find("Spécial alliment/Ennemie").GetComponent<TextMeshProUGUI>().text = texteEnnemi;
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
        // Debug.Log(NewDataCenter.Instance.LireMonaie());//*100 pour voir les centimes.
    }
}
