using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class écrireLesPetitesValeursDuMenu : MonoBehaviour{
    public TextMeshProUGUI textArgent;
    public TextMeshProUGUI textToursBingoKai;
    public List<GameObject> lesEspaceYoKai;
    void Start(){
        if (MesYoKai.Instance != null){
            StartCoroutine(Attend());
        }
    }

    IEnumerator Attend(){
        while (!MesYoKai.Instance.finDuChargement && !MesYoKai.Instance.yoKaiDeMonEquipeChargé){
            yield return new WaitForSeconds(0.1f);
        }
        Initialisation();
    }

    void Initialisation(){
        textToursBingoKai.text = "3 / 3";
        string argent = NewDataCenter.Instance.Lire("€");
        string argent2 = argent;
        if (double.TryParse(argent, out double valeur)){
            argent2 = valeur.ToString("N0", System.Globalization.CultureInfo.InvariantCulture);
            argent2 = argent2.Replace(",", " "); // remplace les virgules par des espaces
        }
        textArgent.text = argent2+"€";
        for (int i = 0; i < lesEspaceYoKai.Count; i++){
            lesEspaceYoKai[i].SetActive(false);
            if (i < MesYoKai.Instance.yoKaiDeMonEquipe.Count){
                // Debug.Log(MesYoKai.Instance.yoKaiDeMonEquipe[i]+"   "+MesYoKai.Instance.maListe.Count);
                PlacerUnYoKai(MesYoKai.Instance.maListe[MesYoKai.Instance.yoKaiDeMonEquipe[i]],lesEspaceYoKai[i]);
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
        blocDePostionUI.transform.Find("tribue").gameObject.GetComponent<Image>().sprite=medaliumDictionnaire._instance.SpriteDeLaTribue(informationGénéral.tribue);
    }
}
