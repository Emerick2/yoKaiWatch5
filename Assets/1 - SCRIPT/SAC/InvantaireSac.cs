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
        if (n == 0) return;
        cible = ((cible%n)+n)%n;
        for (int i = 0; i < laListeDesCatégorie.Count; i++){
            laListeDesCatégorie[i].GetComponent<Image>().enabled = cible==i;
        }
        if (cible < lesItems.Count){
            servantGrille.Initialisation(lesItems[cible].Count,conteneur, masque, PlaceObject, this, AppllerAuChangemantDePosition);
            RemplireLeBlocInformation(0);
        }
    }

    void Update(){
        servantGrille.Update();    
        if (Input.GetButtonDown("L")){
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

    public GameObject AppllerAuChangemantDePosition(){
        RemplireLeBlocInformation(servantGrille.MyPosition());
        return null;
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
}

