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
