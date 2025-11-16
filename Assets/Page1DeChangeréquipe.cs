using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Page1DeChangeréquipe : MonoBehaviour{
    public RemplisLaRoue remplisLaRoue;
    public Sprite spriteDeSélection;
    public Sprite spriteDeNonSélection;
    public Image choieEquipe;
    public Image choieCollectés;
    private int cible = 0;
    public TextMeshProUGUI espacesNombres;
    void Start(){
        Initialisation();
    }

    void Initialisation(){
        cible=0;
        ChangerChoie();
        espacesNombres.text = MesYoKai.Instance.maListe.Count.ToString()+" / "+medaliumDictionnaire._instance.dictionnaire.Count.ToString();
    }

    void Update(){
        if (Input.GetButtonDown("flèche haut") || Input.GetButtonDown("flèche bas")){
            cible = (cible+1)%2;
            ChangerChoie();
        } 
        if (Input.GetButtonDown("A")){
            remplisLaRoue.numéroPageActuelle = cible+2;
            remplisLaRoue.ChangerDePage();
        }
    }

    void ChangerChoie(){
        if (cible == 0){
            choieEquipe.sprite = spriteDeSélection;
            choieCollectés.sprite = spriteDeNonSélection;
        } else{
            choieEquipe.sprite = spriteDeNonSélection;
            choieCollectés.sprite = spriteDeSélection;
        }
    }
}
