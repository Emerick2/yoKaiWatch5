using UnityEngine;
using UnityEngine.UI;
// faire la vie

public class gestionnaireDeMonPetitYoKaiPourUnCombat : MonoBehaviour
{
    public medaliumDictionnaire medaliumDictionnaire;
    public int IDduYoKaiMedalium=1;//c'est un woko par exemple
    public int IDduYoKaiSpécifique=2;//permet d'identifier précisément le woko, c'est le 631 yo-kai obtenu par exemple
    [Range(1,100)] public int pointDeVie = 100;//c'est sur un pourcentage
    //public Text monNom;

    //yo-kai en général :
    [HideInInspector] public string nom;
    [HideInInspector] public GameObject yoKai;
    [HideInInspector] public Tribue tribue;
    [HideInInspector] public Rang rang;
    //amultime :
    [HideInInspector] public string nomDeAmultime;
    [HideInInspector] public bool amultimeDeSoin;
    [HideInInspector] public bool amultimeDeAttaque;
    [HideInInspector] public bool amultimeEnvoutement;//pas encor fini
    //[Header("Paramètres à rensegner si c'est une ammultime de soin :")]
    [HideInInspector] public bool soigneUniquementLeLanceurAmu;
    [HideInInspector] public bool soigneUniquementLeYoKaiÀSaDroite;
    [HideInInspector] public bool soigneUniquementLesDeuxYoKaiAutoursDeLui;
    [HideInInspector] public bool soingneTousLesYoKaiDeLaRanger;
    [HideInInspector] [Range(0,100)] public int pourcentageDeSoitRenduAmu; 
    //[Header("Paramètres à rensegner si c'est une ammultime d'attaque :")]
    [HideInInspector] [Range(0,5)] public int multiplicationDeLaForceParRapportAUneAttaqueNormal;
    [HideInInspector] [Range(0,3)] public int nombreDeAnnemieToucher;

    //attaque: --------------------------------------------------------------
    [HideInInspector] public bool attaqueDeSoin;
    [HideInInspector] public bool attaqueDeDégât;
    [HideInInspector] public bool attaqueEnvoutement;//pas encor fini
    //[Header("Paramètres à rensegner si c'est une attaque de soin :")]
    [HideInInspector] public bool soigneUniquementLeLanceur;
    [HideInInspector] public bool soigneAléatoirementUnDesYoKaiDeLaRanger;
    [HideInInspector] [Range(0,100)] public int pourcentageDeSoitRendu; 
    //[Header("Paramètres à rensegner si c'est une attaque d'attaque :")]
    [HideInInspector] [Range(0,100)] public int dégatInfligé=50;

    //yo-kai en particulier :
    [Range(1,120)]public int niveau;
    [HideInInspector] public string personnalité;
    [HideInInspector] public string surnom;

    //personel :
    private bool déjaActiver;
    void Update(){
        if (!déjaActiver){
            if (MesYoKai.Instance.finDuChargement == true){
                OuvrirLeYoKai();
                déjaActiver=true;
                suiteDeOuverture();
            }
        }
    }
    void OuvrirLeYoKai(){
        //yo-kai en général :
        if (IDduYoKaiMedalium < 0 || IDduYoKaiMedalium >= medaliumDictionnaire.dictionnaire.Count){
            Debug.LogError("Index hors limites ! IDduYoKaiMedalium = " + IDduYoKaiMedalium);
        }
        nom = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].nom;
        yoKai = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].yoKai;
        tribue = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].tribue;
        rang = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].rang;
        
        //amultime :
        nomDeAmultime = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.nomDeAmultime;
        amultimeDeSoin = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.amultimeDeSoin;
        amultimeDeAttaque = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.amultimeDeAttaque;
        amultimeEnvoutement = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.amultimeEnvoutement;
        //[Header("Paramètres à rensegner si c'est une ammultime de soin :")]
        soigneUniquementLeLanceurAmu = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.soigneUniquementLeLanceur;
        soigneUniquementLeYoKaiÀSaDroite = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.soigneUniquementLeYoKaiÀSaDroite;
        soigneUniquementLesDeuxYoKaiAutoursDeLui = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.soigneUniquementLesDeuxYoKaiAutoursDeLui;
        soingneTousLesYoKaiDeLaRanger = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.soingneTousLesYoKaiDeLaRanger;
        pourcentageDeSoitRenduAmu = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.pourcentageDeSoitRendu; 
        //[Header("Paramètres à rensegner si c'est une ammultime d'attaque :")]
        multiplicationDeLaForceParRapportAUneAttaqueNormal = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.multiplicationDeLaForceParRapportAUneAttaqueNormal;
        nombreDeAnnemieToucher = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].amultime.nombreDeAnnemieToucher;
        //attaque:
        attaqueDeSoin = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.attaqueDeSoin;
        attaqueDeDégât = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.attaqueDeDégât;
        attaqueEnvoutement = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.attaqueEnvoutement;//pas encor fini
        //[Header("Paramètres à rensegner si c'est une attaque de soin :")]
        soigneUniquementLeLanceur = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.soigneUniquementLeLanceur;
        soigneAléatoirementUnDesYoKaiDeLaRanger = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.soigneAléatoirementUnDesYoKaiDeLaRanger;
        pourcentageDeSoitRendu = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.pourcentageDeSoitRendu; 
        //[Header("Paramètres à rensegner si c'est une attaque d'attaque :")]
        dégatInfligé = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium].attaqueNormal.dégatInfligé;

        //yo-kai en particulier :
        niveau = int.Parse(NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_niveau"));
        personnalité = NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_personalité");
        surnom = NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_surnom");
    }
    void suiteDeOuverture(){
        //monNom.text=surnom;
    }
}
