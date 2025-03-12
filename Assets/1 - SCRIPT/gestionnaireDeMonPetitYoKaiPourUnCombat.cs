using UnityEngine;
using UnityEngine.UI;
// faire la vie

public class gestionnaireDeMonPetitYoKaiPourUnCombat : MonoBehaviour
{
    public medaliumDictionnaire medaliumDictionnaire;
    public int IDduYoKaiMedalium=1;//c'est un woko par exemple
    public int IDduYoKaiSpécifique=2;//permet d'identifier précisément le woko, c'est le 631 yo-kai obtenu par exemple
    /*[Range(1,100)]*/ public int pointDeVie = 100;//c'est sur un pourcentage
    /*[Range(0,100)]*/ public int dégatInfligé=50;
    /*[Range(0,100)]*/ public int esprit;
    /*[Range(0,100)]*/ public int vitesse;

    //public Text monNom;

    //yo-kai en général :
    [HideInInspector] public string nom;
    [HideInInspector] public GameObject yoKai;
    [HideInInspector] public string tribue;
    [HideInInspector] public string rang;
    [HideInInspector] public string description;
    [HideInInspector] public string personalité;
    [HideInInspector] public string talent;
    [HideInInspector] public string technique;
    [HideInInspector] public string platPréféré;
    [HideInInspector] public string type;
    [HideInInspector] public string amultime;
    [HideInInspector] public string envoutement;
    [HideInInspector] public string nomDeAmultime;
    [HideInInspector] public int multiplicationDeLaPuissance;
    [HideInInspector] public string catégorieÂmultime;

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
        MedaliumNonPersonel mimi = medaliumDictionnaire.dictionnaire[IDduYoKaiMedalium];
        nom = mimi.nom.ToString();
        yoKai = mimi.yoKai;
        tribue = mimi.tribue.ToString();
        rang = mimi.rang.ToString();
        description = mimi.description.ToString();
        personalité = mimi.personalité.ToString();
        talent = mimi.talent.ToString();
        technique = mimi.technique.ToString();
        platPréféré = mimi.platPréféré.ToString();
        type = mimi.type.ToString();
        envoutement = mimi.envoutement.ToString();
        dégatInfligé = mimi.force;
        vitesse = mimi.vitesse;
        esprit = mimi.esprit;
        pointDeVie = mimi.vie;

        nomDeAmultime = mimi.amultime.nomDeAmultime.ToString();
        multiplicationDeLaPuissance = mimi.amultime.multiplicationDeLaPuissance;
        catégorieÂmultime = mimi.amultime.catégorieÂmultime.ToString();

        //yo-kai en particulier :
        niveau = int.Parse(NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_niveau"));
        personnalité = NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_personalité");
        
        if (dégatInfligé == 0){dégatInfligé=1;}
        if (pointDeVie == 0){pointDeVie=1;}
        if (esprit == 0){esprit=1;}

        float taux=1f;
        if (rang == "D") taux+=0.3f*1f;
        if (rang == "C") taux+=0.3f*2f;
        if (rang == "B") taux+=0.3f*3f;
        if (rang == "A") taux+=0.3f*4f;
        if (rang == "S") taux+=0.3f*5f;
        dégatInfligé=(int)((float)dégatInfligé*taux);
        esprit=(int)((float)esprit*taux);

        dégatInfligé = dégatInfligé * niveau;
        pointDeVie = pointDeVie * niveau;
        esprit = esprit * niveau;
    }
    void suiteDeOuverture(){
        //monNom.text=surnom;
    }
}
