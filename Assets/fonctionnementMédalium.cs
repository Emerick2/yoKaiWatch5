using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fonctionnementMédalium : MonoBehaviour{
    public Text texteDuNuméro;
    public GameObject blocInformatif;
    public List<GameObject> espaceDesMédailles;
    public List<Sprite> imageDesNuméro;
    private medaliumDictionnaire médailum;
    private int numéroDeLaPage=1;
    public GameObject flècheVersLeBas;
    public GameObject flècheVersLeHaut;
    [Range(0,11)] public int ID;
    public int IDTotal;
    public int nombreTotalDeCase;
    public List<int> lesIdAInséré = new List<int>();
    public bool cesLeMédalium = true;
    private MesYoKai mesYoKai;//mesYoKai.maListe

    void Start(){
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        mesYoKai = NewDataCenter.Instance.transform.Find("DICTIONNAIRE DE MES YO-KAI").GetComponent<MesYoKai>();

        if (cesLeMédalium){
            int i=1;
            foreach (MedaliumNonPersonel item in médailum.dictionnaire){
                lesIdAInséré.Add(i);
                i++;
            }
        }else{
            foreach (ListeDeMesYoKai item in mesYoKai.maListe){
                lesIdAInséré.Add(item.ID);
            }
        }
        nombreTotalDeCase = Mathf.CeilToInt((lesIdAInséré.Count) / 12f);
        PlacerLesMédailles();
        MouvementDansLaPage(0);

    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            MouvementDansLaPage(3);
        }else if (Input.GetKeyDown(KeyCode.UpArrow)){
            MouvementDansLaPage(-3);
        }else if (Input.GetKeyDown(KeyCode.RightArrow)){
            MouvementDansLaPage(1);
        }else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            MouvementDansLaPage(-1);
        }else if (Input.GetKey(KeyCode.B)){
            numéroDeLaPage = 1;
            PlacerLesMédailles();
            MouvementDansLaPage(0);
            gameObject.SetActive(false);
        }
    }
    void ChangerDePage(int nouveau){
        int pageVoulu = numéroDeLaPage + nouveau;

        // Sécurité : si la page demandée n’existe pas, on arrête tout
        if (pageVoulu < 1 || pageVoulu > nombreTotalDeCase) return;

        if (nouveau == 0){
            numéroDeLaPage = 1;
            IDTotal = 0;
            ID = 0;
            PlacerLesMédailles();
        }
        else if (nouveau == 1){
            // Aller à la page suivante
            if (lesIdAInséré.Count > 12 * numéroDeLaPage){
                numéroDeLaPage++;
                ID = 0;
                IDTotal = (numéroDeLaPage - 1) * 12;
                PlacerLesMédailles();
            }
        }
        else if (nouveau == -1){
            // Revenir à la page précédente
            if (numéroDeLaPage > 1){
                numéroDeLaPage--;
                ID = 11;
                IDTotal = (numéroDeLaPage - 1) * 12;
                PlacerLesMédailles();
            }
        }
    }

    void MouvementDansLaPage(int nombre){
        if (numéroDeLaPage == 1 && ID+nombre<0) return;
        if (numéroDeLaPage == nombreTotalDeCase && ID+nombre>11) return;
        int x=0;
        espaceDesMédailles[ID].GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        if (nombre == 0){
            ID=0;
            IDTotal=0;
            ChangerDePage(0);
        }else {
            x = ID+nombre;
            ID += nombre;
            IDTotal += nombre;
            if (IDTotal<0) IDTotal=0;
        }
        
        if (x < 0){
            ChangerDePage(-1);
            ID = 11;
            IDTotal = (numéroDeLaPage - 1) * 12 + ID;
        }else if (x > 11){
            ChangerDePage(1);
            ID = 0;
            IDTotal = (numéroDeLaPage - 1) * 12 + ID;
        }

        
        texteDuNuméro.text = numéroDeLaPage.ToString();
        RemplirLeBloc();
        espaceDesMédailles[ID].GetComponent<Image>().color = new Color(0.79f, 0.55f, 0.55f, 1f);
    }
    void PlacerLesMédailles(){
        for (int i = 0; i < espaceDesMédailles.Count; i++){
            espaceDesMédailles[i].transform.Find("numéro").GetComponent<Image>().sprite = imageDesNuméro[i];
            if (lesIdAInséré.Count > IDTotal+i){
                espaceDesMédailles[i].transform.Find("médaille").gameObject.SetActive(true);
                espaceDesMédailles[i].transform.Find("médaille").GetComponent<Image>().sprite =  médailum.dictionnaire[lesIdAInséré[IDTotal+i]].médaillon;
            }else {
                espaceDesMédailles[i].transform.Find("médaille").gameObject.SetActive(false);
            }
        }
        flècheVersLeHaut.SetActive(numéroDeLaPage>1);
        flècheVersLeBas.SetActive(numéroDeLaPage < nombreTotalDeCase);
    }

    void RemplirLeBloc(){
        if (lesIdAInséré.Count > IDTotal){
            blocInformatif.SetActive(true);
            // ici la blague est que google collecte plein d'information personnel (voir ma vidéo sur le sujet) et donc commme cette variable à plein d'information sur le yo-kai en question c'est google !
            MedaliumNonPersonel google = médailum.dictionnaire[lesIdAInséré[IDTotal]];
            blocInformatif.transform.Find("icône").GetComponent<Image>().sprite = google.médaillon;
            blocInformatif.transform.Find("nom").GetComponent<Text>().text = google.nom;
            if (cesLeMédalium) blocInformatif.transform.Find("description").GetComponent<Text>().text = google.description;

            string maTribue = google.tribue.ToString();
            if (maTribue == "Vaillant") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Vaillant;
            else if (maTribue == "Mystérieu") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Mystérieu;
            else if (maTribue == "Costaud") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Costaud;
            else if (maTribue == "Mignon") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Mignon;
            else if (maTribue == "Insaisissable") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Insaisissable;
            else if (maTribue == "Bienveillant") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Bienveillant;
            else if (maTribue == "Sombre") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Sombre;
            else if (maTribue == "Sinistre") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Sinistre;
            else if (maTribue == "Perfide") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Perfide;
            else if (maTribue == "Enma") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Enma;
            else if (maTribue == "Wandroïd") blocInformatif.transform.Find("tribu").GetComponent<Image>().sprite = médailum.imageDesTribue.Wandroïd;


            int monRang = 0;
            string monRangSTR  = google.rang.ToString();
            if (monRangSTR == "D") monRang=1;
            else if (monRangSTR == "C") monRang=2;
            else if (monRangSTR == "B") monRang=3;
            else if (monRangSTR == "A") monRang=4;
            else if (monRangSTR == "S") monRang=5;
            blocInformatif.transform.Find("rang").GetComponent<Image>().sprite = médailum.imageDesRang[monRang];
        }else{
            blocInformatif.SetActive(false);
        }
    }
}
