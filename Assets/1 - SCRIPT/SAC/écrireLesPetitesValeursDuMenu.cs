using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using YKW.Core.YoKai.Class;

public class écrireLesPetitesValeursDuMenu : MonoBehaviour{
    public TextMeshProUGUI textArgent;
    public TextMeshProUGUI textToursBingoKai;
    public List<GameObject> lesEspaceYoKai;
    public GameObject boutDroit;
    public GameObject choieApplication;
    private bool peutChoisireUneApplication;
    private bool seMenuEstFermer = true;
    private bool windowClose;
    public List<ApplicationMenu> applicationMenus = new List<ApplicationMenu>();
    public List<GameObject> objetOpen;
    void Start(){
        foreach (GameObject item in objetOpen){
            item.SetActive(true);
        }
        if (MesYoKai.Instance != null){
            StartCoroutine(Attend());
        }
        RevenirAuMenu();
        windowClose = true;
        transform.Find("C").gameObject.SetActive(false);
    }

    void Update(){
        if (Input.GetButtonDown("A")){
            ApplicationMenu app = null;
            int cible = choieApplication.GetComponent<PlaceLesApplicationsSac>().cible;
                Sprite spriteExample = choieApplication.GetComponent<PlaceLesApplicationsSac>().listeImageApplication[cible];
                for (int i = 0; i < applicationMenus.Count; i++){
                    if (applicationMenus[i].sprite == spriteExample){
                        app = applicationMenus[i];
                    }
                
            }
            
            if (!windowClose && peutChoisireUneApplication && app != null){
                for (int i = 0; i < applicationMenus.Count; i++){
                    applicationMenus[i].page.SetActive(false);
                }
                boutDroit.SetActive(false);
                choieApplication.SetActive(false);
                GameObject application = app.page;
                application.SetActive(true);
                peutChoisireUneApplication = false;
                if (app.scriptAInitialiser == null){
                    app.scriptAInitialiser = app.page;
                }
                if (app.scriptAInitialiser != null){
                    if (app.scriptAInitialiser.GetComponent<RemplisLaRoue>() != null){
                        app.scriptAInitialiser.GetComponent<RemplisLaRoue>().Initialisation(true);
                    }
                    if (app.scriptAInitialiser.GetComponent<InvantaireSac>() != null){
                        app.scriptAInitialiser.GetComponent<InvantaireSac>().Initialiser();
                    }
                    if (app.scriptAInitialiser.GetComponent<complétéLeJournal>() != null){
                        app.scriptAInitialiser.GetComponent<complétéLeJournal>().Initialiser(this);
                    }
                    if (app.scriptAInitialiser.GetComponent<QuestMenu>() != null){
                        app.scriptAInitialiser.GetComponent<QuestMenu>().Initialiser(this);
                    }
                    if (app.scriptAInitialiser.GetComponent<FonctionnementMédalium>() != null){
                        app.scriptAInitialiser.GetComponent<FonctionnementMédalium>().Initialiser(this);
                    }
                }
            }
        } else if (Input.GetButtonDown("X") && windowClose && mouvementDuJoueur.droitDeMarcher) {
            windowClose = false;
            transform.Find("C").gameObject.SetActive(true);
            mouvementDuJoueur.droitDeMarcher = false;
            seMenuEstFermer=false;
            RevenirAuMenu();
        } else if (Input.GetButtonDown("B")){
            if (peutChoisireUneApplication && !seMenuEstFermer) {
                windowClose = true;
                transform.Find("C").gameObject.SetActive(false);
                mouvementDuJoueur.droitDeMarcher = true;
                seMenuEstFermer = true;
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
                if (MesYoKai.Instance.maListe.Count > MesYoKai.Instance.yoKaiDeMonEquipe[j]) {
                    YoKaiActif yoKai = MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[j]];
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
    }

    void PlacerUnYoKai(YoKaiActif yoKai, GameObject blocDePostionUI){
        blocDePostionUI.SetActive(true);
        YoKai informationGénéral = medaliumDictionnaire._instance.ServantYoKai(yoKai.ID);

        blocDePostionUI.transform.Find("bloc image/médaillon").gameObject.GetComponent<Image>().sprite=informationGénéral.imageMédaillon;
        blocDePostionUI.transform.Find("Niveau").gameObject.GetComponent<TextMeshProUGUI>().text=yoKai.niveau.ToString();
        blocDePostionUI.transform.Find("bloc nom/nom").gameObject.GetComponent<TextMeshProUGUI>().text=yoKai.surnom;
        blocDePostionUI.transform.Find("bloc info/nombre max vie").gameObject.GetComponent<TextMeshProUGUI>().text=(informationGénéral.PV*yoKai.niveau).ToString();
        blocDePostionUI.transform.Find("tribue").gameObject.GetComponent<Image>().sprite=new ServantImage().ReadImage_Tribue(informationGénéral.tribue);
    }

    public void RevenirAuMenu(){
        for (int i = 0; i < applicationMenus.Count; i++){
            applicationMenus[i].page.SetActive(false);
        }
        boutDroit.SetActive(true);
        choieApplication.SetActive(true);
        peutChoisireUneApplication = true;
        Initialisation();
    }
}

[System.Serializable]
public class ApplicationMenu{
    public Sprite sprite;
    public GameObject page;
    public GameObject scriptAInitialiser;
}
