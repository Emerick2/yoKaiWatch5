using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Reflection;

public class medaliumDictionnaire : MonoBehaviour{
    public List<MedaliumNonPersonel> dictionnaire;
    /*void Start(){
        //MesYoKai.Instance.AjouterUnYoKai(5,52,"une personalité !","un surnom !");
    }*/
}


[System.Serializable]
public class MedaliumNonPersonel{
    public string nom;
    public GameObject yoKai;
    public Tribue tribue;
    public Rang rang;
    //public Personnalité personnalité; la personnalité sera aléatoire
    public Ammultime amultime;
    public AttaqueNormal attaqueNormal;
}

public enum Tribue{
    [Description("vaillant")]       Vaillant,
    [Description("mystérieu")]      Mystérieu,
    [Description("costaud")]        Costaud,
    [Description("mignon")]         Mignon,
    [Description("bienveillant")]   Bienveillant,
    [Description("sombre")]         Sombre,
    [Description("sinistre")]       Sinistre,
    [Description("insaisissable")]  Insaisissable,
    [Description("perfide")]        Perfide,
    [Description("enma")]           Enma,
    [Description("wandroïd")]       Wandroïd
}

public enum Personnalité{
    //1
    [Description("flâneur")]        flâneur,
    [Description("frivole")]        frivole,
    [Description("ferme")]          ferme,
    [Description("sérieux")]        sérieux,
    //2
    [Description("grognon")]        grognon,
    [Description("dur")]            dur,
    [Description("logique")]        logique,
    [Description("futé")]           futé,
    [Description("prudent")]        prudent,
    [Description("calme")]          calme,
    [Description("gentil")]         gentil,
    [Description("tendre")]         tendre,
    [Description("bizarre")]        bizarre,
    [Description("cruel")]          cruel,
    [Description("altruiste")]      altruiste,
    [Description("dévoué")]         dévoué,
    [Description("violant")]        violant,
}

public enum Rang{
    [Description("E")]  E,
    [Description("D")]  D,
    [Description("C")]  C,
    [Description("B")]  B,
    [Description("A")]  A,
    [Description("S")]  S,
    [Description("SS")] SS,
    [Description("SSS")]SSS,
    [Description("Z")]  Z,
    [Description("ZZ")] ZZ,
    [Description("ZZZ")]ZZZ,
    [Description("UZ")] UZ
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
}

[System.Serializable]
public class AttaqueNormal{
    public bool attaqueDeSoin;
    public bool attaqueDeDégât;
    public bool attaqueEnvoutement;//pas encor fini
    [Header("Paramètres à rensegner si c'est une attaque de soin :")]
    public bool soigneUniquementLeLanceur;
    public bool soigneAléatoirementUnDesYoKaiDeLaRanger;
    [Range(0,100)] public int pourcentageDeSoitRendu; 
    [Header("Paramètres à rensegner si c'est une attaque d'attaque :")]
    [Range(0,100)] public int dégatInfligé;
    //[Header("Paramètres à rensegner si c'est une attaque d'envoutement (ce n'est pas fini désolé) :")]
}
