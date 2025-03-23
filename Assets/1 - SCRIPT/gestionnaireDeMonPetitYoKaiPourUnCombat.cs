using UnityEngine;
using UnityEngine.UI;
// faire la vie

public class gestionnaireDeMonPetitYoKaiPourUnCombat : MonoBehaviour
{
    public medaliumDictionnaire medaliumDictionnaire;
    public int IDduYoKaiMedalium=1;//c'est un woko par exemple
    [HideInInspector]public int IDduYoKaiSpécifique=-1;//permet d'identifier précisément le woko, c'est le 631 yo-kai obtenu par exemple
    /*[Range(1,100)]*/ [HideInInspector]public int pointDeVie = 100;//c'est sur un pourcentage
    /*[Range(0,100)]*/ [HideInInspector]public int dégatInfligé=50;
    /*[Range(0,100)]*/ [HideInInspector]public int esprit;
    /*[Range(0,100)]*/ [HideInInspector]public int vitesse;

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
    [HideInInspector][Range(0,100)] public int probabilitéDeFlâner=30;
    [HideInInspector][Range(0,100)] public int probabilitéEsquiver=3;
    [HideInInspector][Range(0,100)] public int probabilitéDeCritique=3;

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
        if (IDduYoKaiSpécifique!=-1){
            nom = mimi.nom.ToString();
        }else {
            nom=mimi.nom.ToString();
        }
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
        if (IDduYoKaiSpécifique>=0){
            niveau = int.Parse(NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_niveau"));
            personnalité = NewDataCenter.Instance.Lire(IDduYoKaiSpécifique.ToString()+"_personalité");
        }
        if (dégatInfligé == 0){dégatInfligé=1;}
        if (pointDeVie == 0){pointDeVie=1;}
        if (esprit == 0){esprit=1;}

        dégatInfligé = dégatInfligé * niveau;
        pointDeVie = pointDeVie * niveau;
        esprit = esprit * niveau;
        
        PrendreEnCompteLaTribue();
        PrendreEnCompteLeRang();
        PrendreEnCompteLaPersonalité();
        PrendreEnCompteLeTalent();
        PrendreEnCompteLaTechnique();
        PrendreEnCompteLeType();
    }
    void suiteDeOuverture(){
        //monNom.text=surnom;
    }


    void PrendreEnCompteLaTribue(){
        if (tribue=="Vaillants") dégatInfligé = (int)((float)dégatInfligé*1.5);
        if (tribue=="Mystérieux") esprit = (int)((float)esprit*1.5);
        if (tribue=="Costauds") pointDeVie = (int)((float)pointDeVie*1.5);
        if (tribue=="Mignons") vitesse = (int)((float)vitesse*1.5);
        if (tribue=="Bienveillants") esprit = (int)((float)esprit*1.5);
        if (tribue=="Sombres") esprit = (int)((float)esprit*1.5);
        if (tribue=="Sinistres") vitesse = (int)((float)vitesse*1.5);
        if (tribue=="Insaisissables") vitesse = (int)((float)vitesse*1.5);
        if (tribue=="Perfides") dégatInfligé = (int)((float)dégatInfligé*1.5);
    }
    void PrendreEnCompteLeRang(){
        float taux=1f;
        if (rang == "D") taux+=0.3f*1f;
        if (rang == "C") taux+=0.3f*2f;
        if (rang == "B") taux+=0.3f*3f;
        if (rang == "A") taux+=0.3f*4f;
        if (rang == "S") taux+=0.3f*5f;
        dégatInfligé=(int)((float)dégatInfligé*taux);
        esprit=(int)((float)esprit*taux);
        probabilitéDeFlâner=(int)((float)probabilitéDeFlâner/taux);
    }
    void PrendreEnCompteLaPersonalité(){
        if(personalité == "AuraProtectrice") pointDeVie = (int)((float)pointDeVie*1.5f);
        if(personalité == "CoupCritique") probabilitéDeCritique = probabilitéDeCritique=30;
        if(personalité == "DéfenseAccrue") pointDeVie = (int)((float)pointDeVie * 1.3f);
        if(personalité == "Anticipation") probabilitéEsquiver = probabilitéEsquiver=10;
        if(personalité == "Esquive") probabilitéEsquiver = probabilitéEsquiver=20;
        //probabilitéDeFlâner
        /*
        Absorption,// Récupère des PV proportionnellement aux dégâts infligés.
        Antigel,// Protège contre les effets de gel.
        Antipoison,// : Immunise contre les empoisonnements.
        AutoRégénération,// : Régénère progressivement des PV pendant le combat.
        Blocage,// : Augmente la résistance aux attaques physiques.
        BouclierMagique,// : Augmente la résistance aux attaques magiques.
        Camouflage,// : Réduit les chances d'être ciblé par les ennemis.
        ContreAttaque,// : Riposte automatiquement après avoir subi une attaque.
        CoupCritique,// : Augmente la probabilité de réaliser un coup critique.
        Courage,// : Empêche la peur ou la confusion en combat.

        Détournement,// : Attire les attaques ennemies vers soi pour protéger les alliés.

        Expertise,// : Améliore l'efficacité des techniques spécifiques.
        FrappeDouble,// : Permet d'attaquer deux fois consécutivement.
        GuérisonRapide,// : Réduit le temps nécessaire pour se remettre des altérations d'état.
        Immunité,// : Protège contre certaines altérations d'état.
        Initiative,// : Augmente la vitesse, permettant d'agir plus tôt en combat.
        Inspiration,// : Augmente les chances d'inspirer les ennemis, les rendant inactifs.
        Intimidation,// : Réduit l'attaque des ennemis adjacents.
        MoralDeAcier,// : Empêche la baisse de statistiques en combat.
        Motivation,// : Augmente l'efficacité des encouragements envers les alliés.
        Paralysie,// : Peut paralyser les ennemis lors d'attaques.
        PerceDéfense,// : Ignore partiellement la défense de l'ennemi.
        Persévérance,// : Permet de survivre avec 1 PV après une attaque fatale.
        PiqûreToxique,// : Peut empoisonner l'ennemi lors d'attaques.
        Provocation,// : Augmente les chances d'être ciblé par l'ennemi.
        Récupération,// : Restaure des PV après avoir vaincu un ennemi.
        Renforcement,// : Augmente l'attaque globale du Yo-kai.
        Résistance,// : Réduit les dégâts subis de certaines sources.
        Sérénité,// : Protège contre la confusion et la panique.
        Soutien,// : Augmente l'efficacité des actions de soutien envers les alliés.
        Vampirisme,// : Transfère une partie des PV de l'ennemi au Yo-kai lors d'attaques.
        Vengeance,// : Augmente l'attaque après avoir subi des dégâts.
        Vitalité// : Augmente les points de vie maximum du Yo-kai.
        */
    }
    void PrendreEnCompteLeTalent(){}
    void PrendreEnCompteLaTechnique(){}
    void PrendreEnCompteLeType(){}

}
