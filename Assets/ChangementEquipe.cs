using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangementEquipe : MonoBehaviour{
    public GameObject choie;
    public List<Image> espaceImagePourMonEquipe;
    public fonctionnementMédalium médaliumFonction;
    private medaliumDictionnaire médailum;
    private MesYoKai mesYoKai;//mesYoKai.maListe
    private bool ouvert;
    private bool demandeDeRetirer;
    private Sprite iconeDeVide;

    void Start(){
        iconeDeVide = espaceImagePourMonEquipe[0].sprite;
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        mesYoKai = NewDataCenter.Instance.transform.Find("DICTIONNAIRE DE MES YO-KAI").GetComponent<MesYoKai>();
        choie.SetActive(false);
        PlacerMesYoKai();
    }
    void PlacerMesYoKai() {
        foreach (Image item in espaceImagePourMonEquipe){
            item.sprite = iconeDeVide;
        }
        for (int i = 0; i < mesYoKai.yoKaiDeMonEquipe.Count; i++) {        
            int index = mesYoKai.yoKaiDeMonEquipe[i];
            if (index < 0 || index >= mesYoKai.maListe.Count) {
                Debug.LogError($"Index invalide: {index} dans yoKaiDeMonEquipe");
                continue; // évite le crash
            }

            espaceImagePourMonEquipe[i].sprite = médailum.dictionnaire[mesYoKai.maListe[index].ID].médaillon;
        }
    }

    void Update(){
        if (Input.GetButtonDown("X")){
            choie.SetActive(true);
            ouvert = true;
            demandeDeRetirer = false;
            PlacerMesYoKai();
        }
        if (Input.GetButtonDown("Y")||Input.GetButtonDown("B")){
            choie.SetActive(false);
            ouvert = false;
            demandeDeRetirer = false;
        }
        if (Input.GetKeyDown(KeyCode.V)){
            choie.SetActive(true);
            ouvert = true;
            demandeDeRetirer = true;
            PlacerMesYoKai();
        }
    }
    public void ChangerUnYoKai(int numéroAChanger){
        List<int> y = mesYoKai.yoKaiDeMonEquipe;
        int CeluiAAjouter=médaliumFonction.IDDeCeluiSurQuiJeSuis();
        if (CeluiAAjouter != -1 && !demandeDeRetirer && !y.Contains(CeluiAAjouter)){
            if (y.Count>numéroAChanger)y[numéroAChanger]=CeluiAAjouter;
            else y.Add(CeluiAAjouter);
        }else if (demandeDeRetirer && y.Count>numéroAChanger && y.Count > 1){
            y.Remove(y[numéroAChanger]);
        }else{
            Debug.Log("Contenue ? "+y.Contains(CeluiAAjouter));
            Debug.Log("CeluiAAjouter : "+CeluiAAjouter);
        }
        mesYoKai.yoKaiDeMonEquipe = y;
        
        //enregistrer :
        string t = "";
        foreach (int item in y){
            t+=item.ToString();
        }
        //Debug.Log(t);
        NewDataCenter.Instance.SauvegarderSTR("yoKaiDansEquipe",t,"équipe.json");

        PlacerMesYoKai();
        choie.SetActive(false);
    }    
}
