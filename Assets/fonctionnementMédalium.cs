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
    private List<MedaliumNonPersonel> IDTableaux = new List<MedaliumNonPersonel>();
    void Start(){
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        for (int i = 0; i < médailum.dictionnaire.Count; i++){
            IDTableaux.Add(médailum.dictionnaire[i]);
        }
        nombreTotalDeCase = Mathf.CeilToInt((médailum.dictionnaire.Count) / 12f);
        PlacerLesMédailles();
        MouvementDansLaPage(0,0);

    }
    void Update(){
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            MouvementDansLaPage(ID+3,IDTotal+3);
        }else if (Input.GetKeyDown(KeyCode.UpArrow)){
            MouvementDansLaPage(ID-3,IDTotal-3);
        }else if (Input.GetKeyDown(KeyCode.RightArrow)){
            MouvementDansLaPage(ID+1,IDTotal+1);
        }else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            MouvementDansLaPage(ID-1,IDTotal-1);
        }else if (Input.GetKey(KeyCode.B)){
            numéroDeLaPage = 1;
            PlacerLesMédailles();
            MouvementDansLaPage(0,0);
            gameObject.SetActive(false);
        }
    }

    void MouvementDansLaPage(int nombre, int nombreTotal){
        //if ((numéroDeLaPage<=1 && nombre+ID<0 && nombre+ID != 0) || (nombreTotalDeCase > IDTotal+nombreTotal))return;
        int titi = nombre+ID;
        print(titi);
        if (numéroDeLaPage<=1 && titi<0) return;
        if (titi > 12 && numéroDeLaPage>=nombreTotalDeCase) return;
        //int titi = IDTotal+nombreTotal;
        //if (nombreTotalDeCase >= titi){print("x="+titi);print("ici");return;}
        espaceDesMédailles[ID].GetComponent<Image>().color = new Color(0f, 0f, 0f, 1f);
        ID = nombre;
        /*
        //cas dans lequel on va de la première page à la dernière :
        if (nombreTotal<0){
            int v = 1;
            bool réussie = false;
            nombreTotal = médailum.dictionnaire.Count-1;
            while (réussie == false){
                v = v+12;
                if (v>=nombreTotal){
                    réussie=true;
                    nombreTotal=v-2;
                    IDTotal = nombreTotal;
                }
            }
        }*/
        if (nombreTotal<0) nombreTotal=0;
        IDTotal = nombreTotal;
        if (ID>11){
            ID=0;//avence
            if (médailum.dictionnaire.Count-1 >= 12*numéroDeLaPage){
                numéroDeLaPage++;
                PlacerLesMédailles();
            } else {
                IDTotal=0;
                numéroDeLaPage=1;
                PlacerLesMédailles();
            }
        } else if (ID<0){
            ID=11;//recule
            if (numéroDeLaPage>1){
                numéroDeLaPage--;
                PlacerLesMédailles();
            } else {
                numéroDeLaPage=(médailum.dictionnaire.Count)/12;
                if (médailum.dictionnaire.Count-1 >= 12*numéroDeLaPage){
                    numéroDeLaPage++;
                    PlacerLesMédailles();
                }
            }
        }
        if (IDTotal<0) IDTotal = médailum.dictionnaire.Count-1;
        //else if (IDTotal>médailum.dictionnaire.Count-1) IDTotal = 0;
        texteDuNuméro.text = numéroDeLaPage.ToString();
        RemplirLeBloc();
        espaceDesMédailles[ID].GetComponent<Image>().color = new Color(0.79f, 0.55f, 0.55f, 1f);
    }
    void PlacerLesMédailles(){
        for (int i = 0; i < espaceDesMédailles.Count; i++){
            espaceDesMédailles[i].transform.Find("numéro").GetComponent<Image>().sprite = imageDesNuméro[i];
            if (médailum.dictionnaire.Count > IDTotal+i){
                espaceDesMédailles[i].transform.Find("médaille").gameObject.SetActive(true);
                espaceDesMédailles[i].transform.Find("médaille").GetComponent<Image>().sprite =  médailum.dictionnaire[IDTotal+i].médaillon;
            }else {
                espaceDesMédailles[i].transform.Find("médaille").gameObject.SetActive(false);
            }
        }
        flècheVersLeHaut.SetActive(numéroDeLaPage>1);
        flècheVersLeBas.SetActive(médailum.dictionnaire.Count > IDTotal);
    }

    void RemplirLeBloc(){
        if (médailum.dictionnaire.Count > IDTotal){
            blocInformatif.SetActive(true);
            // ici la blague est que google collecte plein d'information personnel (voir ma vidéo sur le sujet) et donc commme cette variable à plein d'information sur le yo-kai en question c'est google !
            MedaliumNonPersonel google = médailum.dictionnaire[IDTotal];
            blocInformatif.transform.Find("icône").GetComponent<Image>().sprite = google.médaillon;
            blocInformatif.transform.Find("nom").GetComponent<Text>().text = google.nom;
            blocInformatif.transform.Find("description").GetComponent<Text>().text = google.description;

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
