using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InvantaireSac : MonoBehaviour{
    public List<GameObject> laListeDesCatégorie;
    private int cible = 0;
    public Vector2 cible2;
    [Min(1)] public int nombreImageParLigne = 5;
    [Min(1)] public int nombreImageVuEnVerticale = 5;
    [Min(0)] private int ligneLaPlusHauteVisible = 0;
    public Sprite spriteRSélection;
    public Sprite spriteLSélection;
    public Sprite spriteRNonSélection;
    public Sprite spriteLNonSélection;
    public Image L;
    public Image R;
    public float tempsTouche = 0.3f;
    private Coroutine coroutineCliqueLR;
    private List<ObjetDansLeSacInformation> lesItems = new List<ObjetDansLeSacInformation>();
    public GameObject conteneur;
    private GameObject exemple;
    private List<GameObject> objetPlacerEnPrévu = new List<GameObject>();
    public bool étatDeDebug = true;
    public GameObject blocInformation;
    public écrireLesPetitesValeursDuMenu menu;
    public Sprite spriteNouritures;
    public Sprite spriteObjets;
    public Sprite spriteAnnimaux;
    public Sprite spriteÉquipements;
    public Sprite spriteÂmes;
    public Sprite spriteObjetsClés;
    void Start(){
        Initialiser();
    }
    public void Initialiser(){
        cible = 0;
        ligneLaPlusHauteVisible = 0;
        ChangerPositionCible();
    }

    void Update(){
        if (Input.GetButtonDown("L")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(true));
            R.sprite=spriteRNonSélection;
            cible--;
            ChangerPositionCible();
        } else if (Input.GetButtonDown("R")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(false));
            L.sprite=spriteLNonSélection;
            cible++;
            ChangerPositionCible();
        } else if (Input.GetButtonDown("left")){
            cible2.x--;
            ChangerPositionCible2();
        } else if (Input.GetButtonDown("right")){
            cible2.x++;
            ChangerPositionCible2();
        } else if (Input.GetButtonDown("down")){
            cible2.y++;
            ChangerPositionCible2();
        } else if (Input.GetButtonDown("up")){
            cible2.y--;
            ChangerPositionCible2();
        } else if (Input.GetButtonDown("B")) {
            if (menu != null )menu.RevenirAuMenu();
            else Debug.LogWarning("Le gameObject menu n'est pas assigner");
        }
    }

    void ChangerPositionCible(){
        cible %= laListeDesCatégorie.Count;
        if (cible < 0){
            cible = laListeDesCatégorie.Count+cible;
        }
        for (int i = 0; i < laListeDesCatégorie.Count; i++){
            if (i == cible){
                laListeDesCatégorie[i].GetComponent<Image>().enabled = true;
            } else {
                laListeDesCatégorie[i].GetComponent<Image>().enabled = false;
            }
        }
        lesItems.Clear();
        switch (cible){
            case 0:
                PlacerLaNouriture();
                break;
            case 2:
                PlacerLesAnnimaux();
                break;
            case 3:
                PlacerLesÉquipement();
                break;
        }
        PlacerLesItems();
    }

    IEnumerator CliqueLR(bool cesLeL){
        if (cesLeL) L.sprite=spriteLSélection;
        else R.sprite=spriteRSélection;
        yield return new WaitForSeconds(tempsTouche);
        if (cesLeL) L.sprite=spriteLNonSélection;
        else R.sprite=spriteRNonSélection;
    }

    void PlacerLaNouriture(){
        if (étatDeDebug){
            foreach (Nouriture item in Item._instance.listeDesNouritures){
                lesItems.Add(new ObjetDansLeSacInformation{
                    nom = item.nom,
                    sprite = item.image,
                    infoNouriture = item,
                    quantitée = Random.Range(0,1000),
                });
            }
        } else {
            foreach (Nouriture item in Item._instance.listeDesNouritures){
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    lesItems.Add(new ObjetDansLeSacInformation{
                        nom = item.nom,
                        sprite = item.image,
                        infoNouriture = item,
                        quantitée = quantitées,
                    });
                }
            }
        }
    }

    void PlacerLesAnnimaux(){
        if (étatDeDebug){
            foreach (Annimal item in Item._instance.listeDesAnnimaux){
                lesItems.Add(new ObjetDansLeSacInformation{
                    nom = item.nom,
                    sprite = item.image,
                    infoAnimal = item,
                    quantitée = Random.Range(0,1000),
                });
            }
        } else {
            foreach (Annimal item in Item._instance.listeDesAnnimaux){
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    lesItems.Add(new ObjetDansLeSacInformation{
                        nom = item.nom,
                        sprite = item.image,
                        infoAnimal = item,
                        quantitée = quantitées,
                    });
                }
            }
        }
    }

    void PlacerLesÉquipement(){
        if (étatDeDebug){
            foreach (Équipement item in Item._instance.listeDesÉquipements){
                lesItems.Add(new ObjetDansLeSacInformation{
                    nom = item.nom,
                    sprite = item.sprite,
                    infoÉquipement = item,
                    quantitée = Random.Range(0,1000),
                });
            }
        } else {
            foreach (Équipement item in Item._instance.listeDesÉquipements){
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    lesItems.Add(new ObjetDansLeSacInformation{
                        nom = item.nom,
                        sprite = item.sprite,
                        infoÉquipement = item,
                        quantitée = quantitées,
                    });
                }
            }
        }
    }

    void PlacerLesItems(){
        objetPlacerEnPrévu.Clear();
        foreach (Transform child in conteneur.transform){
            if (exemple == null){
                exemple = child.gameObject;
                exemple.SetActive(false);
            }
            if (child.gameObject != exemple){
                Destroy(child.gameObject);
            }
        }
        foreach (ObjetDansLeSacInformation item in lesItems){
            GameObject TV = Instantiate(exemple);
            TV.transform.Find("img").GetComponent<Image>().sprite = item.sprite;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi0 = false;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi1 = false;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(item.quantitée);
            TV.transform.parent = conteneur.transform;
            TV.transform.localScale = new Vector3(1,1,1);
            TV.SetActive(true);
            objetPlacerEnPrévu.Add(TV);
        }

        // changer le height :
        StartCoroutine(LargeurBloc());

        cible2 = new Vector2(0,0);
        ChangerPositionCible2();
    }

    void ChangerPositionCible2(){
        if (nombreImageParLigne<=0){
            nombreImageParLigne = 1;
            Debug.LogWarning("Attention à la division par 0 avec nombreImageParLigne !");
        }
        cible2.x %= nombreImageParLigne;
        cible2.y %= objetPlacerEnPrévu.Count/nombreImageParLigne;

        if (cible2.x < 0){
            cible2.x = nombreImageParLigne+cible2.x;
        }
        if (cible2.y < 0){
            cible2.y = (int)(objetPlacerEnPrévu.Count+cible2.y)/nombreImageParLigne;
        }
        for (int i = 0; i < objetPlacerEnPrévu.Count; i++){
            bool laCible = i == cible2.y * nombreImageParLigne + cible2.x;
            objetPlacerEnPrévu[i].GetComponent<Image>().enabled = laCible;
        }
        ChangerLaHauteurDuBloque();
        RemplireLeBlocInformation();
    }

    void RemplireLeBlocInformation(){
        int identifiant = (int)cible2.y * nombreImageParLigne + (int)cible2.x;
        if (objetPlacerEnPrévu.Count <= 0 || objetPlacerEnPrévu.Count <= identifiant){
            blocInformation.SetActive(false);
        } else {
            blocInformation.SetActive(true);
        
            blocInformation.transform.Find("icone/Image").gameObject.GetComponent<Image>().sprite = lesItems[identifiant].sprite;
            blocInformation.transform.Find("Nom").gameObject.GetComponent<TextMeshProUGUI>().text = lesItems[identifiant].nom;
            blocInformation.transform.Find("Quantitée/nombre").gameObject.GetComponent<TextMeshProUGUI>().text = lesItems[identifiant].quantitée.ToString();
            if (lesItems[identifiant].infoNouriture != null){
                blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(true);
                blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);

                string texteAmis = Item._instance.listeDesEffetsNouritures[lesItems[identifiant].infoNouriture.IdEffetSiSurAmi].description;
                string texteEnnemies = Item._instance.listeDesEffetsNouritures[lesItems[identifiant].infoNouriture.IdEffetSurSiSurEnnemie].description;
                blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = lesItems[identifiant].infoNouriture.description;
                if (texteAmis != ""){
                    blocInformation.transform.Find("Spécial alliment/Image ami").gameObject.SetActive(true);
                    blocInformation.transform.Find("Spécial alliment/ami").gameObject.GetComponent<TextMeshProUGUI>().text = texteAmis;
                    blocInformation.transform.Find("Spécial alliment/ami").gameObject.SetActive(true);
                } else {
                    blocInformation.transform.Find("Spécial alliment/Image ami").gameObject.SetActive(false);
                    blocInformation.transform.Find("Spécial alliment/ami").gameObject.SetActive(false);
                }
                if (texteAmis != ""){
                    blocInformation.transform.Find("Spécial alliment/Image ennemie").gameObject.SetActive(true);
                    blocInformation.transform.Find("Spécial alliment/Ennemie").gameObject.GetComponent<TextMeshProUGUI>().text = texteEnnemies;
                    blocInformation.transform.Find("Spécial alliment/Ennemie").gameObject.SetActive(true);
                } else {
                    blocInformation.transform.Find("Spécial alliment/Image ennemie").gameObject.SetActive(false);
                    blocInformation.transform.Find("Spécial alliment/Ennemie").gameObject.SetActive(false);
                }
            } else if (lesItems[identifiant].infoÉquipement != null){
                blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
                blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(true);

                blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = lesItems[identifiant].infoÉquipement.description;
                string texteEffet = "";
                Équipement équip = lesItems[identifiant].infoÉquipement;
                texteEffet += équip.effetSur_ELSE;
                if (texteEffet == "") {
                    if (équip.effetSur_Défance != 0) texteEffet+="Défence "+équip.effetSur_Défance.ToString() + " ";
                    if (équip.effetSur_Esprit != 0) texteEffet+="Esprit "+équip.effetSur_Esprit.ToString() + " ";
                    if (équip.effetSur_Force != 0) texteEffet+="Force "+équip.effetSur_Force.ToString() + " ";
                    if (équip.effetSur_Vitesse != 0) texteEffet+="Vitesse "+équip.effetSur_Vitesse.ToString() + " ";
                }
                if (texteEffet != ""){
                    blocInformation.transform.Find("Spécial équipement/information effet").gameObject.GetComponent<TextMeshProUGUI>().text = texteEffet;
                    blocInformation.transform.Find("Spécial équipement/information effet").gameObject.SetActive(true);
                    blocInformation.transform.Find("Spécial équipement/Image information").gameObject.SetActive(true);
                } else {
                    blocInformation.transform.Find("Spécial équipement/information effet").gameObject.SetActive(false);
                    blocInformation.transform.Find("Spécial équipement/Image information").gameObject.SetActive(false);
                }
            } else if (lesItems[identifiant].infoAnimal != null) {
                blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = lesItems[identifiant].infoAnimal.description;
                blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
                blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);
            } else {
                blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "";
                blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
                blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);
            }

            switch (cible){
                case 0 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteNouritures;
                    break;
                case 1 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteObjets;
                    break;
                case 2 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteAnnimaux;
                    break;
                case 3 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteÉquipements;
                    break;
                case 4 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteÂmes;
                    break;
                case 5 :
                    blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = spriteObjetsClés;
                    break;
            }
        }
    }

    IEnumerator LargeurBloc(){
        yield return new WaitForSeconds(0.2f);
        if (objetPlacerEnPrévu.Count > 0){
            RectTransform rtConteneur = conteneur.GetComponent<RectTransform>();
            GridLayoutGroup gridLayout = conteneur.GetComponent<GridLayoutGroup>();
    
            RectTransform dernierElement = objetPlacerEnPrévu[objetPlacerEnPrévu.Count - 1].GetComponent<RectTransform>();

            float lastElementY = dernierElement.anchoredPosition.y;
    
            float height = Mathf.Abs(lastElementY);
            height += gridLayout.cellSize.y;
            height += gridLayout.padding.bottom;

            rtConteneur.sizeDelta = new Vector2(rtConteneur.sizeDelta.x, height);

            // Debug.Log(height);
        }
    }

    void ChangerLaHauteurDuBloque() {
        GridLayoutGroup grid = conteneur.GetComponent<GridLayoutGroup>();
        float hauteurLigne = grid.cellSize.y + grid.spacing.y; 
    
        float limiteBasse = ligneLaPlusHauteVisible + nombreImageVuEnVerticale;
        bool estAuDessus = cible2.y <= ligneLaPlusHauteVisible;
        bool estEnDessous = cible2.y >= limiteBasse;

        if (estAuDessus || estEnDessous) {
            int nouveauSommet = ligneLaPlusHauteVisible;
            if (estAuDessus) {
                nouveauSommet = (int)cible2.y;
            }    
            else if (estEnDessous) {
                nouveauSommet = (int)cible2.y - nombreImageVuEnVerticale;
            }
            ligneLaPlusHauteVisible = nouveauSommet;
            float nouvelleHauteurY = grid.padding.top + (nouveauSommet * hauteurLigne);
            if (estAuDessus) nouvelleHauteurY-=grid.cellSize.y;
            else nouvelleHauteurY+=grid.cellSize.y;
            RectTransform rt = conteneur.GetComponent<RectTransform>();    
            Vector2 newAnchoredPos = rt.anchoredPosition;
            newAnchoredPos.y = nouvelleHauteurY;
            rt.anchoredPosition = newAnchoredPos;
        }
    }
}

