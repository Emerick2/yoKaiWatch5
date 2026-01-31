using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class InvantaireSac : MonoBehaviour{
    public List<GameObject> laListeDesCatégorie;
    private int cible = 0;
    public Image L;
    public Image R;
    public float tempsTouche = 0.3f;
    private Coroutine coroutineCliqueLR;
    public List<List<ObjetDansLeSacInformation>> lesItems = new List<List<ObjetDansLeSacInformation>>();//Il s'agit d'un ensemble de liste de plusieurs types (Animal, Objet, équipement...)
    public GameObject conteneur;
    public bool étatDeDebug = true;
    public GameObject blocInformation;
    public écrireLesPetitesValeursDuMenu menu;
    public ServantGrille servantGrille = new ServantGrille();
    public ServantImage servantImage = new ServantImage();
    public RectTransform masque;

    public void Initialiser(bool total = true){
        if (total){
            lesItems.Clear();
            PlacerLaNouriture();
            lesItems.Add(new List<ObjetDansLeSacInformation>{});
            PlacerLesAnnimaux();
            PlacerLesÉquipement();
            lesItems.Add(new List<ObjetDansLeSacInformation>{});
            lesItems.Add(new List<ObjetDansLeSacInformation>{});
        }
        int n = laListeDesCatégorie.Count;
        cible = ((cible%n)+n)%n;
        for (int i = 0; i < laListeDesCatégorie.Count; i++){
            laListeDesCatégorie[i].GetComponent<Image>().enabled = cible==i;
        }
        servantGrille.Initialisation(lesItems[cible].Count,conteneur, masque, PlaceObject);
        RemplireLeBlocInformation(0);
    }

    void Update(){
        if (servantGrille.Update()){
            RemplireLeBlocInformation(servantGrille.MyPosition());
        } else if (Input.GetButtonDown("L")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(true));
            R.sprite=servantImage.ReadImage_LR("notR");
            cible--;
            Initialiser(false);
        } else if (Input.GetButtonDown("R")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(false));
            L.sprite=servantImage.ReadImage_LR("notL");
            cible++;
            Initialiser(false);
        } else if (Input.GetButtonDown("B")) {
            if (menu != null )menu.RevenirAuMenu();
            else Debug.LogWarning("Le gameObject menu n'est pas assigner");
        }
    }

    IEnumerator CliqueLR(bool cesLeL){
        if (cesLeL) L.sprite=servantImage.ReadImage_LR("L");
        else R.sprite=servantImage.ReadImage_LR("R");
        yield return new WaitForSeconds(tempsTouche);
        if (cesLeL) L.sprite=servantImage.ReadImage_LR("notL");
        else R.sprite=servantImage.ReadImage_LR("notR");
    }

    GameObject PlaceObject(GameObject exemple, GameObject block, int ID) {
        if (lesItems.Count <= cible) return null;
        if (lesItems[cible].Count <= ID) return null;
        ObjetDansLeSacInformation item = lesItems[cible][ID];
        GameObject TV = Instantiate(exemple);
        TV.transform.Find("img").GetComponent<Image>().sprite = item.sprite;
        TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi0 = false;
        TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi1 = false;
        TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(item.quantitée);
        TV.transform.parent = block.transform;
        TV.transform.localScale = new Vector3(1,1,1);
        TV.SetActive(true);
        return TV;
    }

    void PlacerLaNouriture(){
        List<ObjetDansLeSacInformation> list = new List<ObjetDansLeSacInformation>();
        foreach (Nouriture item in Item._instance.listeDesNouritures){
            ObjetDansLeSacInformation element = new ObjetDansLeSacInformation{ nom = item.nom, sprite = item.image, infoNouriture = item};
            if (étatDeDebug){
                element.quantitée = Random.Range(0,1000);
                list.Add(element);
            } else {
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    element.quantitée = quantitées;
                    list.Add(element);
                }
            }
        }
        lesItems.Add(list);
    }

    void PlacerLesAnnimaux(){
        List<ObjetDansLeSacInformation> list = new List<ObjetDansLeSacInformation>();
        foreach (Annimal item in Item._instance.listeDesAnnimaux){
            ObjetDansLeSacInformation element = new ObjetDansLeSacInformation{ nom = item.nom, sprite = item.image, infoAnimal = item};
            if (étatDeDebug){
                element.quantitée = Random.Range(0,1000);
                list.Add(element);
            } else {
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    element.quantitée = quantitées;
                    list.Add(element);
                }
            }
        }
        lesItems.Add(list);
    }

    void PlacerLesÉquipement(){
        List<ObjetDansLeSacInformation> list = new List<ObjetDansLeSacInformation>();
        foreach (Équipement item in Item._instance.listeDesÉquipements){
            ObjetDansLeSacInformation element = new ObjetDansLeSacInformation{ nom = item.nom, sprite = item.sprite, infoÉquipement = item};
            if (étatDeDebug){
                element.quantitée = Random.Range(0,1000);
                list.Add(element);
            } else {
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    element.quantitée = quantitées;
                    list.Add(element);
                }
            }
        }
        lesItems.Add(list);
    }


    void RemplireLeBlocInformation(int identifiant){
        if (lesItems.Count <= cible || lesItems[cible].Count <= identifiant){
            blocInformation.SetActive(false);
            return;
        }

        ObjetDansLeSacInformation list = lesItems[cible][identifiant];

        blocInformation.SetActive(true);
    
        blocInformation.transform.Find("icone/Image").gameObject.GetComponent<Image>().sprite = list.sprite;
        blocInformation.transform.Find("Nom").gameObject.GetComponent<TextMeshProUGUI>().text = list.nom;
        blocInformation.transform.Find("Quantitée/nombre").gameObject.GetComponent<TextMeshProUGUI>().text = list.quantitée.ToString();
        if (list.infoNouriture != null){
            blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(true);
            blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);
            string texteAmis = Item._instance.listeDesEffetsNouritures[list.infoNouriture.IdEffetSiSurAmi].description;
            string texteEnnemies = Item._instance.listeDesEffetsNouritures[list.infoNouriture.IdEffetSurSiSurEnnemie].description;
            blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = list.infoNouriture.description;
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
        } else if (list.infoÉquipement != null){
            blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
            blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(true);
            blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = list.infoÉquipement.description;
            string texteEffet = "";
            Équipement équip = list.infoÉquipement;
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
        } else if (list.infoAnimal != null) {
            blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = list.infoAnimal.description;
            blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
            blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);
        } else {
            blocInformation.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            blocInformation.transform.Find("Spécial alliment").gameObject.SetActive(false);
            blocInformation.transform.Find("Spécial équipement").gameObject.SetActive(false);
        }
        switch (cible){
            case 0 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.Nouriture);
                break;
            case 1 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.Objet);
                break;
            case 2 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.Animal);
                break;
            case 3 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.Equipement);
                break;
            case 4 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.Ame);
                break;
            case 5 :
                blocInformation.transform.Find("type").gameObject.GetComponent<Image>().sprite = servantImage.ReadImage_TypeItem(CatégorieItem.ObjetClef);
                break;
        }
    }
    /*

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
    */
}

