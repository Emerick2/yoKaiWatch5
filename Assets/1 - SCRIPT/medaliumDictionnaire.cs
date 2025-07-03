using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using UnityEngine.UI;
using System.Reflection;
using Unity.VisualScripting;

public class medaliumDictionnaire : MonoBehaviour{
    public static medaliumDictionnaire _instance;
    public Sprite imageDeUneMédailleInconnue;

    /*[HideInInspector]*/
    public List<MedaliumNonPersonel> dictionnaire;
    public List<Sprite> imageDesRang;
    public ImageDesTribue imageDesTribue;
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
        foreach (MedaliumNonPersonel item in dictionnaire){
            if (item.médaillon == null){
                item.médaillon = imageDeUneMédailleInconnue;
            }
            //inversion squellette et description :
            string t = item.nomDeMonSquellette;
            item.nomDeMonSquellette = item.description;
            item.description = t;
        }
    }
    /*
    void Start(){
        if (int.Parse(NewDataCenter.Instance.Lire("IDdeMesYoKai"))<3){
            MesYoKai.Instance.AjouterUnYoKai(5,52,"une personalité !","un surnom !");
            MesYoKai.Instance.AjouterUnYoKai(5,52,"une personalité !","un surnom !");
            MesYoKai.Instance.AjouterUnYoKai(5,52,"une personalité !","un surnom !");
        }
    }*//*
    public Sprite RecevoirUnSpriteDeTribue(string nomDeLaTribue){
        return imageDesTribue.nomDeLaTribue;
        /*
        if (nomDuRang == "Wandroïd"){
            return imageDesRang[10];
        }else if (nomDuRang == "Enma"){
            return imageDesRang[9];
        }else if (nomDuRang == "Perfide"){
            return imageDesRang[8];
        }else if (nomDuRang == "Insaisissable"){
            return imageDesRang[7];
        }else if (nomDuRang == "Sinistre"){
            return imageDesRang[6];
        }else if (nomDuRang == "Sombre"){
            return imageDesRang[5];
        }else if (nomDuRang == "Bienveillant"){
            return imageDesRang[4];
        }else if (nomDuRang == "Mignon"){
            return imageDesRang[3];
        }else if (nomDuRang == "Costaud"){
            return imageDesRang[2];
        }else if (nomDuRang == "Mystérieu"){
            return imageDesRang[1];
        }else if (nomDuRang == "Vaillant"){
            return imageDesRang[0];
        }else{
            Debug.LogWarning("La tribue "+nomDuRang+" est manquante dans le médalium");
            return imageDesRang[7];
        }*//*
    }*/
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

    public void ajouterUneTribue(int IDdeLaCible, string nomDeLaTribue){
        nomDeLaTribue = nomDeLaTribue.ToLower();
        nomDeLaTribue = char.ToUpper(nomDeLaTribue[0]) + nomDeLaTribue.Substring(1);
        if (Enum.IsDefined(typeof(Tribue), nomDeLaTribue)){
            dictionnaire[IDdeLaCible].tribue = (Tribue)Enum.Parse(typeof(Tribue), nomDeLaTribue);
        }
    }
    public void ajouterUnRang(int IDdeLaCible, string nomDuRang){
        nomDuRang = nomDuRang.ToUpper();
        if (Enum.IsDefined(typeof(Rang), nomDuRang)){
            dictionnaire[IDdeLaCible].rang = (Rang)Enum.Parse(typeof(Rang), nomDuRang);
        }
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
    public string nom;
    public string description;
    public GameObject yoKai;
    public Tribue tribue;
    public Rang rang;
    public Personnalité personalité;
    public Talent talent;
    public Technique technique;
    public PlatPréféré platPréféré;
    public Type type;
    public Ammultime amultime;
    public Envoutement envoutement;
    public int force;
    public int vitesse;
    public int esprit;
    public int vie;
    public string nomDeMonSquellette;
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

public enum Personnalité{
    //1
    flâneur,
    frivole,
    ferme,
    sérieux,
    //2
    grognon,
    dur,
    logique,
    futé,
    prudent,
    calme,
    gentil,
    tendre,
    bizarre,
    cruel,
    altruiste,
    dévoué,
    violant,
}

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