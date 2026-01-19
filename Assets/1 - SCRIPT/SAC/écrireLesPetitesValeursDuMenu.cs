using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class écrireLesPetitesValeursDuMenu : MonoBehaviour{
    public TextMeshProUGUI textArgent;
    public TextMeshProUGUI textToursBingoKai;
    public List<GameObject> lesEspaceYoKai;
    public GameObject boutDroit;
    public List<GameObject> lesPagesApplications;
    public GameObject choieApplication;
    private bool peutChoisireUneApplication;
    void Start(){
        if (MesYoKai.Instance != null){
            StartCoroutine(Attend());
        }
        RevenirAuMenu();
    }

    void Update(){
        if (Input.GetButtonDown("A")){
            if (peutChoisireUneApplication && lesPagesApplications.Count > choieApplication.GetComponent<PlaceLesApplicationsSac>().cible){
                for (int i = 0; i < lesPagesApplications.Count; i++){
                    lesPagesApplications[i].SetActive(false);
                }
                boutDroit.SetActive(false);
                choieApplication.SetActive(false);
                GameObject application = lesPagesApplications[choieApplication.GetComponent<PlaceLesApplicationsSac>().cible];
                application.SetActive(true);
                peutChoisireUneApplication = false;
                if (application.GetComponent<RemplisLaRoue>() != null){
                    application.GetComponent<RemplisLaRoue>().Initialisation(true);
                }
                if (application.GetComponent<InvantaireSac>() != null){
                    application.GetComponent<InvantaireSac>().Initialiser();
                }
            }
        }
    }

    IEnumerator Attend(){
        while (!MesYoKai.Instance.finDuChargement && !MesYoKai.Instance.yoKaiDeMonEquipeChargé){
            yield return new WaitForSeconds(0.1f);
        }
        Initialisation();
    }

    public void Initialisation(){
        textToursBingoKai.text = "3 / 3";
        string argent = NewDataCenter.Instance.LireMonaie().ToString();
        string argent2 = argent;
        if (double.TryParse(argent, out double valeur)){
            argent2 = valeur.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
            argent2 = argent2.Replace(",", " ");
        }
        textArgent.text = argent2+"€";
        for (int i = 0; i < lesEspaceYoKai.Count; i++){
            lesEspaceYoKai[i].SetActive(false);
            for (int j = 0; j < MesYoKai.Instance.yoKaiDeMonEquipe.Count; j++){
                ListeDeMesYoKai yoKai = MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]];
                yoKai.positionRoue %= lesEspaceYoKai.Count;
                if (yoKai.positionRoue<0){
                    yoKai.positionRoue=0;
                }
                if (yoKai.positionRoue==i){
                    PlacerUnYoKai(yoKai,lesEspaceYoKai[i]);
                }
            }
        }
    }

    void PlacerUnYoKai(ListeDeMesYoKai yoKai, GameObject blocDePostionUI){
        blocDePostionUI.SetActive(true);
        MedaliumNonPersonel informationGénéral = medaliumDictionnaire._instance.dictionnaire[yoKai.ID];

        blocDePostionUI.transform.Find("bloc image/médaillon").gameObject.GetComponent<Image>().sprite=informationGénéral.médaillon;
        blocDePostionUI.transform.Find("Niveau").gameObject.GetComponent<TextMeshProUGUI>().text=yoKai.niveau.ToString();
        blocDePostionUI.transform.Find("bloc nom/nom").gameObject.GetComponent<TextMeshProUGUI>().text=yoKai.surnom;
        blocDePostionUI.transform.Find("bloc info/nombre max vie").gameObject.GetComponent<TextMeshProUGUI>().text=(informationGénéral.vie*yoKai.niveau).ToString();
        blocDePostionUI.transform.Find("tribue").gameObject.GetComponent<Image>().sprite=new ServantImage().ReadImage_Tribue(informationGénéral.tribue);
    }

    public void RevenirAuMenu(){
        for (int i = 0; i < lesPagesApplications.Count; i++){
            lesPagesApplications[i].SetActive(false);
        }
        boutDroit.SetActive(true);
        choieApplication.SetActive(true);
        peutChoisireUneApplication = true;
        Initialisation();
    }
}
