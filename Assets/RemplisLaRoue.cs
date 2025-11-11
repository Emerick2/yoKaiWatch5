using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemplisLaRoue : MonoBehaviour{
    public int numéroPageActuelle;
    public List<GameObject> lesMédaillon;
    private int cible = 0;
    public GameObject leParentDuModèle3DGénéré;
    private GameObject TV;
    public GameObject panneauDeInfo;
    public GameObject infoYokaiBandeau;

    public GameObject espaceBlocListeYokai;

    private int numéroDePageInfo = 1;
    void Start(){
        if (MesYoKai.Instance != null){
            StartCoroutine(Attend());
        }
    }

    IEnumerator Attend(){
        while (!MesYoKai.Instance.finDuChargement && !MesYoKai.Instance.yoKaiDeMonEquipeChargé){
            yield return new WaitForSeconds(0.1f);
        }
        Initialisation(true);
    }
    public void Initialisation(bool total){
        for (int i = 0; i < lesMédaillon.Count; i++){
            lesMédaillon[i].SetActive(true);
            if (MesYoKai.Instance.yoKaiDeMonEquipe.Count > i){
                lesMédaillon[i].GetComponent<Image>().enabled = true;
                lesMédaillon[i].GetComponent<Image>().sprite = medaliumDictionnaire._instance.dictionnaire[MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]].ID].médaillon;
            } else {
                lesMédaillon[i].GetComponent<Image>().enabled = false;
            }
        }
        if (true){
            cible = 0;
            NouvelleCible();
        }
    }

    void Update(){
        if (numéroPageActuelle == 1){
            if (Input.GetButtonDown("flèche droite")){
                cible++;
                NouvelleCible();
            } else if (Input.GetButtonDown("flèche gauche")){
                cible--;
                NouvelleCible();
            }
            if (Input.GetButtonDown("X")){
                int nombreDePage = 3;
                numéroDePageInfo++;
                if (numéroDePageInfo > nombreDePage){
                    numéroDePageInfo = 1;
                } else if (numéroDePageInfo < 1){
                    numéroDePageInfo = nombreDePage;
                }
                for (int i = 1; i <= nombreDePage; i++){
                    panneauDeInfo.transform.Find("page "+i.ToString()).gameObject.SetActive(false);
                }
                panneauDeInfo.transform.Find("page "+numéroDePageInfo.ToString()).gameObject.SetActive(true);
                panneauDeInfo.transform.Find("page/Numéro").gameObject.GetComponent<TextMeshProUGUI>().text = numéroDePageInfo.ToString()+" / "+nombreDePage.ToString();
            }
        }
    }

    void NouvelleCible(){
        if (cible < 0){
            cible = lesMédaillon.Count-1;
        } else if (cible >= lesMédaillon.Count){
            cible = 0;
        }
        for (int i = 0; i < lesMédaillon.Count; i++){
            lesMédaillon[i].transform.Find("pointeur").gameObject.SetActive(i == cible);
        }
        if (TV != null){
            Destroy(TV);
        }
        if (MesYoKai.Instance.yoKaiDeMonEquipe.Count > cible){
            ListeDeMesYoKai informationSpécifique = MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[cible]];
            MedaliumNonPersonel informationGénéral = medaliumDictionnaire._instance.dictionnaire[informationSpécifique.ID];
            TV = Instantiate(informationGénéral.yoKai);
            TV.transform.parent = leParentDuModèle3DGénéré.transform;
            TV.transform.localPosition = Vector3.zero;
            int layerUI = LayerMask.NameToLayer("CaméraUImodèle3D");
            AppliquerLayer(TV, layerUI);

            panneauDeInfo.SetActive(true);
            // numéroDePageInfo = 1;
            // panneauDeInfo.transform.Find("page 1").gameObject.SetActive(true);
            // panneauDeInfo.transform.Find("page 2").gameObject.SetActive(false);
            // panneauDeInfo.transform.Find("page 3").gameObject.SetActive(false);
            // panneauDeInfo.transform.Find("page/Numéro").gameObject.GetComponent<TextMeshProUGUI>().text = "1 / 3";
            
            panneauDeInfo.transform.Find("page 1/Force/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.force * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/Défance/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.vie * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/esprit/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.esprit * informationSpécifique.niveau).ToString();
            panneauDeInfo.transform.Find("page 1/vitesse/text").gameObject.GetComponent<TextMeshProUGUI>().text = (informationGénéral.vitesse * informationSpécifique.niveau).ToString();

            panneauDeInfo.transform.Find("page 1/Personalité/text").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.personnalité.ToString();
            panneauDeInfo.transform.Find("page 1/Talent/text").gameObject.GetComponent<TextMeshProUGUI>().text = informationGénéral.talent.ToString();
            panneauDeInfo.transform.Find("page 1/Talent/text (1)").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            
            infoYokaiBandeau.SetActive(true);
            infoYokaiBandeau.transform.Find("tribue").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.SpriteDeLaTribue(informationGénéral.tribue);
            infoYokaiBandeau.transform.Find("rang").gameObject.GetComponent<Image>().sprite = medaliumDictionnaire._instance.RecevoirUnSpriteDeRang(informationGénéral.rang.ToString());
            infoYokaiBandeau.transform.Find("nom").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.surnom;
            infoYokaiBandeau.transform.Find("niveau").gameObject.GetComponent<TextMeshProUGUI>().text = informationSpécifique.niveau.ToString();
        } else {
            infoYokaiBandeau.SetActive(false);
            panneauDeInfo.SetActive(false);
        }
    }
    void AppliquerLayer(GameObject obj, int layer){
        obj.layer = layer;
        foreach (Transform enfant in obj.transform){
            AppliquerLayer(enfant.gameObject, layer);
        }
    }
}
