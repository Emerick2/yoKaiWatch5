using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using UnityEngine.UI;
using System.Reflection;
using Mono.Cecil;

// le scripte qui lie le fichier "Médalium.csv" est ComplétéLeMédaliumAvecCSV

public class medaliumDictionnaire : MonoBehaviour{
    public static medaliumDictionnaire _instance;
    public Sprite imageDeUneMédailleInconnue;

    /*[HideInInspector]*/
    public List<MedaliumNonPersonel> dictionnaire;
    public List<Sprite> imageDesRang;
    public List<Sprite> imageDesTribues;
    public List<Sprite> imageDesElements;

    public ImageDesTribue imageDesTribue;
    public bool chargementFini = false;
    
    public GameObject AvoirLeGameObject(){
        return gameObject;
    }
 
    private void Awake(){
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
    void Start(){
        int i = 0;
        foreach (MedaliumNonPersonel item in dictionnaire){
            if (item.médaillon == null){
                item.médaillon = imageDeUneMédailleInconnue;
            }
            
            //inversion squellette et description :
            string t = item.nomDeMonSquellette;
            item.nomDeMonSquellette = item.description;
            item.description = t;
            
            item.monElement = ObtenirElementDeLaTechnique(item.technique);

            item.faceImage = Resources.Load<Sprite>("PuniArt/"+i);
            i++;

            string nom = "";
            if (item.yoKai != null){
                foreach (Transform obj in item.yoKai.transform){
                    if (obj.name[0]=='n'&&obj.name[1]=='o'&&obj.name[2]=='m'&&obj.name[3]==':'){
                        nom = obj.name;
                        nom = nom.Replace("nom:","");
                        nom = nom.Replace("_p00","");
                        nom = nom.Replace("_mod","");
                        item.faceRondImage = Resources.Load<Sprite>("Face icon/"+nom+".xi.00");
                        break;
                    }
                }
            } else {
                Debug.Log("Le yo-kai était null");
            }
        }
        chargementFini = true;
    }
    public Sprite RecevoirUnSpriteDeRang(string nomDuRang){
        if (nomDuRang == "S"){
            return imageDesRang[5];
        }else if (nomDuRang == "A"){
            return imageDesRang[4];
        }else if (nomDuRang == "B"){
            return imageDesRang[3];
        }else if (nomDuRang == "C"){
            return imageDesRang[2];
        }else if (nomDuRang == "D"){
            return imageDesRang[1];
        }else if (nomDuRang == "E"){
            return imageDesRang[0];
        }else{
            Debug.LogWarning("La tribue "+nomDuRang+" est manquante dans le médalium");
            return imageDesRang[1];
        }
    }

    public void AjouterUneTribue(int IDdeLaCible, string nomDeLaTribue){
        nomDeLaTribue = nomDeLaTribue.ToLower();
        nomDeLaTribue = char.ToUpper(nomDeLaTribue[0]) + nomDeLaTribue.Substring(1);
        if (Enum.IsDefined(typeof(Tribue), nomDeLaTribue)){
            dictionnaire[IDdeLaCible].tribue = (Tribue)Enum.Parse(typeof(Tribue), nomDeLaTribue);
        }
    }
    public void AjouterUnRang(int IDdeLaCible, string nomDuRang){
        nomDuRang = nomDuRang[nomDuRang.Length-1].ToString();
        nomDuRang = nomDuRang.ToUpper();
        if (Enum.IsDefined(typeof(Rang), nomDuRang)){
            dictionnaire[IDdeLaCible].rang = (Rang)Enum.Parse(typeof(Rang), nomDuRang);
        }
    }

    public Sprite SpriteDeLaTribue(Tribue tribue){
        List<Tribue> aurdre = new List<Tribue>{
            Tribue.Vaillant,
            Tribue.Mystérieu,
            Tribue.Costaud,
            Tribue.Mignon,
            Tribue.Bienveillant,
            Tribue.Sombre,
            Tribue.Sinistre,
            Tribue.Insaisissable,
            Tribue.Perfide,
            Tribue.Enma,
            Tribue.Wandroïd
        };
        for (int i = 0; i < aurdre.Count; i++){
            if (tribue == aurdre[i]){
               return imageDesTribues[i];
            }
        }
        
        return null;
    }

    public Element ObtenirElementDeLaTechnique(Technique technique){
        switch (technique){
            // FEU
            case Technique.Feu:
            case Technique.Incendie:
            case Technique.Brasier:
                return Element.Feu;

            // EAU
            case Technique.Torrent:
            case Technique.Rapides:
            case Technique.Cascade:
                return Element.Eau;

            // FOUDRE
            case Technique.Décharge:
            case Technique.Foudre:
            case Technique.HauteTension:
                return Element.Foudre;

            // TERRE
            case Technique.Galet:
            case Technique.Éboulement:
            case Technique.Météore:
                return Element.Terre;

            // VENT
            case Technique.Tourbillon:
            case Technique.Tornade:
            case Technique.Ouragan:
                return Element.Vent;

            // GLACE
            case Technique.Grêle:
            case Technique.Gel:
            case Technique.Blizzard:
                return Element.Glace;

            // DRAIN
            case Technique.Absorption:
            case Technique.Vortex:
            case Technique.Récolte:
                return Element.Absorption;

            // SOIN (Ne fait pas de dégâts, mais bon à classer)
            case Technique.Guérison:
            case Technique.Restauration:
            case Technique.Paradis:
                return Element.Soin;

            default:
                return Element.Neutre;
        }
    }

    public Sprite ObtenirImageElement(Element technique){
        switch (technique){
            case Element.Feu:
                return imageDesElements[0];
            case Element.Eau:
                return imageDesElements[1];
            case Element.Foudre:
                return imageDesElements[3];
            case Element.Terre:
                return imageDesElements[4];
            case Element.Glace:
                return imageDesElements[5];
            case Element.Vent:
                return imageDesElements[6];
            case Element.Absorption:
                return imageDesElements[7];
            case Element.Soin:
                return imageDesElements[8];
        }
        Debug.LogWarning("Aucun élément n'a été trouvé");
        return imageDesElements[0];
    }
}

[System.Serializable]
public class ImageDesTribue{
    public Sprite Vaillant;
    public Sprite Mystérieu;
    public Sprite Costaud;
    public Sprite Mignon;
    public Sprite Bienveillant;
    public Sprite Sombre;
    public Sprite Sinistre;
    public Sprite Insaisissable;
    public Sprite Perfide;
    public Sprite Enma;
    public Sprite Wandroïd;
}

[System.Serializable]
public class MedaliumNonPersonel{
    public Sprite médaillon;
    public Sprite faceImage;
    public Sprite faceRondImage;
    public bool rare;
    public string nom;
    public string description;
    public GameObject yoKai;
    public Tribue tribue;
    public Rang rang;
    public Personalité personalité;
    public Talent talent;
    public Technique technique;
    public PlatPréféré platPréféré;
    public Type type;
    public Ammultime amultime;
    public Envoutement envoutement;
    public int force;
    public int vitesse;
    public int esprit;
    public int défance;// IL faut le rensaigner dans le CSV
    public int vie;
    public string nomDeMonSquellette;

    [Header("Affinités Élémentaires")]
    public Element elementFaiblesse;    // L'élément qui fait mal (x2 ou x1.5)
    public Element elementResistance;   // L'élément qui fait moins mal (x0.5)
    public Element monElement;
}

public enum Tribue{
    Vaillant,
    Mystérieu,
    Costaud,
    Mignon,
    Bienveillant,
    Sombre,
    Sinistre,
    Insaisissable,
    Perfide,
    Enma,
    Wandroïd
}

// public enum Personnalité{
//     //1
//     flâneur,
//     frivole,
//     ferme,
//     sérieux,
//     //2
//     grognon,
//     dur,
//     logique,
//     futé,
//     prudent,
//     calme,
//     gentil,
//     tendre,
//     bizarre,
//     cruel,
//     altruiste,
//     dévoué,
//     violant,
// }

public enum Rang{
    E,
    D,
    C,
    B,
    A,
    S
}

public static class EnumExtensions{
    public static string GetDescription(this Enum value){
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
[System.Serializable]
public class Ammultime{
    public string nomDeAmultime;
    [Range(0,5)] public int multiplicationDeLaPuissance;
    public CatégorieÂmultime catégorieÂmultime;

    /*
    public bool amultimeDeSoin;
    public bool amultimeDeAttaque;
    public bool amultimeEnvoutement;//pas encor fini
    [Header("Paramètres à rensegner si c'est une ammultime de soin :")]
    public bool soigneUniquementLeLanceur;
    public bool soigneUniquementLeYoKaiÀSaDroite;
    public bool soigneUniquementLesDeuxYoKaiAutoursDeLui;
    public bool soingneTousLesYoKaiDeLaRanger;
    [Range(0,100)] public int pourcentageDeSoitRendu; 
    [Header("Paramètres à rensegner si c'est une ammultime d'attaque :")]
    [Range(0,5)] public int multiplicationDeLaForceParRapportAUneAttaqueNormal;
    [Range(0,3)] public int nombreDeAnnemieToucher;
    //[Header("Paramètres à rensegner si c'est une ammultime d'envoutement (ce n'est pas fini désolé) :")]
    */
}
public enum CatégorieÂmultime{
    envoutementAllier,
    envoutementEnnemie,
    soin,
    attaque
}

public enum Talent{
    Absorption,// Récupère des PV proportionnellement aux dégâts infligés.
    Anticipation,//    Esquive les attaques ennemies plus facilement.
    Antigel,// Protège contre les effets de gel.
    Antipoison,// : Immunise contre les empoisonnements.
    AuraProtectrice,// : Réduit les dégâts subis par les alliés adjacents.
    AutoRégénération,// : Régénère progressivement des PV pendant le combat.
    Blocage,// : Augmente la résistance aux attaques physiques.
    BouclierMagique,// : Augmente la résistance aux attaques magiques.
    Camouflage,// : Réduit les chances d'être ciblé par les ennemis.
    ContreAttaque,// : Riposte automatiquement après avoir subi une attaque.
    CoupCritique,// : Augmente la probabilité de réaliser un coup critique.
    Courage,// : Empêche la peur ou la confusion en combat.
    DéfenseAccrue,// : Augmente la défense globale du Yo-kai.
    Détournement,// : Attire les attaques ennemies vers soi pour protéger les alliés.
    Esquive,// Augmente la capacité à éviter les attaques ennemies.
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
}

public enum Technique{
    Feu,
    Incendie,
    Brasier,
    Torrent,
    Rapides,
    Cascade,
    Décharge,
    Foudre,
    HauteTension,
    Galet,
    Éboulement,
    Météore,
    Tourbillon,
    Tornade,
    Ouragan,
    Grêle,
    Gel,
    Blizzard,
    Absorption,
    Vortex,
    Récolte,
    Guérison,
    Restauration,
    Paradis
}
public enum PlatPréféré{
    Riz,
    Pain,
    Bonbons,
    Lait,
    Jus,
    Hamburgers,
    Ramen,
    SpécialitésChinoises,
    Légumes,
    Viande,
    FruitsDeMer,
    Chocolat,
    SuperMégaBarres,
    Curry,
    Douceurs,
    Odens,
    Soda,
    Encas,
    Sushis,
    Glaces,
    Beignets,
    Pizza,
    HotDog,
    Pâtes,
    Tempuras,
    Sukiyaki,
    spécial
}
public enum Type{ 
    Attaque,
    Sabotage,
    Technique,
    Défense,
    Soutien,
    Guérison 
}
public enum Envoutement{
    ennemie,
    allié
}
/*
talant*
attaque 
!! charge amultime
technique* : feu
!! envoutement : imprudent / sans but
type* : attaque / sabotage / technique / défense / soutien / guérison / 
plat préféré* : 
description :
nom :
tribut
rang





*/


public enum Element{
    Neutre, // Pour les attaques physiques de base
    Feu,
    Eau,
    Foudre,
    Terre,
    Vent,
    Glace,
    Absorption,
    Soin   // Pour exclure les soins des calculs de dégâts
}

