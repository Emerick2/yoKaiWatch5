using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangementEquipe : MonoBehaviour{
    public GameObject choie;
    public List<Image> espaceImagePourMonEquipe;
    public fonctionnementMédalium médaliumFonction;
    private medaliumDictionnaire médailum;
    private MesYoKai mesYoKai;//mesYoKai.maListe
    [HideInInspector]public bool ouvert;
    private bool demandeDeRetirer;
    private Sprite iconeDeVide;
    public Image imageDuYoKaiChoisie;
    public TextMeshProUGUI texteInstruction;
    public sacMouvement sac;
    private mouvementDuJoueur joueur;

    void Start(){
        iconeDeVide = espaceImagePourMonEquipe[0].sprite;
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        mesYoKai = NewDataCenter.Instance.transform.Find("DICTIONNAIRE DE MES YO-KAI").GetComponent<MesYoKai>();
        choie.SetActive(false);
        PlacerMesYoKai();
        joueur = GameObject.FindObjectOfType<mouvementDuJoueur>().GetComponent<mouvementDuJoueur>();
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

            if (i<espaceImagePourMonEquipe.Count)espaceImagePourMonEquipe[i].sprite = médailum.ServantYoKai(mesYoKai.maListe[index].ID).médaillon;
            else Debug.LogWarning("Dépassement de la pile i avec un i="+i);
        }
        if (médaliumFonction.IDDeCeluiSurQuiJeSuis() != -1){
            imageDuYoKaiChoisie.sprite = médailum.ServantYoKai(mesYoKai.maListe[médaliumFonction.IDDeCeluiSurQuiJeSuis()].ID).médaillon;
            imageDuYoKaiChoisie.gameObject.SetActive(true);
        }else{
            imageDuYoKaiChoisie.gameObject.SetActive(false);
        }
    }

    void Update(){
        if (Input.GetButtonDown("X") && !ouvert){
            médaliumFonction.bloquer=true;
            sac.bloquer = true;
            choie.SetActive(true);
            ouvert = true;
            demandeDeRetirer = false;
            texteInstruction.text = "Quelle yo-kai remplacer ?";
            PlacerMesYoKai();
        }
        else if (Input.GetButtonUp("B") && ouvert){
            ouvert = false;
            médaliumFonction.bloquer=false;
            sac.bloquer = false;
            choie.SetActive(false);
        }
        else if (Input.GetButtonDown("Y") && ouvert){
            demandeDeRetirer = true;
            texteInstruction.text = "Quelle yo-kai retirer";
            PlacerMesYoKai();
        }
        else if (Input.GetButtonDown("X") && ouvert){
            demandeDeRetirer = false;
            texteInstruction.text = "Quelle yo-kai remplacer ?";
            PlacerMesYoKai();
        }

    }
    public void ChangerUnYoKai(int numéroAChanger){
        List<int> y = mesYoKai.yoKaiDeMonEquipe;
        int celuiAAjouter=médaliumFonction.IDDeCeluiSurQuiJeSuis();
        Debug.Log(celuiAAjouter);
        if (celuiAAjouter != -1 && !demandeDeRetirer && !y.Contains(celuiAAjouter)){
            if (y.Count>numéroAChanger)y[numéroAChanger]=celuiAAjouter;
            else y.Add(celuiAAjouter);
        }else if (demandeDeRetirer && y.Count>numéroAChanger && y.Count > 1){
            y.Remove(y[numéroAChanger]);
        }else{
            Debug.Log("Contenue ? "+y.Contains(celuiAAjouter));
            Debug.Log("CeluiAAjouter : "+celuiAAjouter);
        }
        mesYoKai.yoKaiDeMonEquipe = y;
        
        //enregistrer :
        string t = "";
        foreach (int item in y){
            t+=item.ToString();
            t+="%";
        }
        //Debug.Log(t);
        NewDataCenter.Instance.SauvegarderSTR("yoKaiDansEquipe",t,"équipe.json");

        PlacerMesYoKai();
    }    
}
