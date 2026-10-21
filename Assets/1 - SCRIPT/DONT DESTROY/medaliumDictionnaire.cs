using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using YKW.Core.YoKai.Enums;
using YKW.Core.YoKai.Class;

public class medaliumDictionnaire : MonoBehaviour{
    public static medaliumDictionnaire _instance;
    public Sprite imageDeUneMédailleInconnue;

    /*[HideInInspector]*/
    [SerializeField] private List<YoKai> dictionnaire;
    private readonly Dictionary<int, YoKai> _yokaiById = new Dictionary<int, YoKai>();

    public bool chargementFini = true;
    public int MAX = 905;
    
    public GameObject AvoirLeGameObject(){
        return gameObject;
    }
 
    private void Awake(){
        if (_instance == null){
            _instance = this;
            if (transform.parent != null) transform.SetParent(null, true);
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

   public YoKai ServantYoKai(int ID) {
        if (dictionnaire == null) dictionnaire = new List<YoKai>();
        if (_yokaiById.TryGetValue(ID, out YoKai cached)) return cached;
        foreach (YoKai item in dictionnaire)
        {
            if (item != null && item.ID == ID) { _yokaiById[ID] = item; return item; }
        }

        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/yokai_data/" + ID.ToString());

        if (targetFile != null) {
            YoKaiJsonData data = JsonUtility.FromJson<YoKaiJsonData>(targetFile.text);
            YoKai yoKai = ConstruireYoKai(data);
            yoKai.ID = ID;
            yoKai.imageFace = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/picture/" + MakeAnUrl(yoKai.nom));
            yoKai.imageMédaillon = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/medals/" + MakeAnUrl(yoKai.nom));
            yoKai.faceRondImage = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/face/" + MakeAnUrl(yoKai.nom));
            if (yoKai.imageMédaillon == null) {
                yoKai.imageMédaillon = yoKai.faceRondImage;
            }
            if (yoKai.imageMédaillon == null) {
                yoKai.imageMédaillon = yoKai.imageFace;
            }
            yoKai.voiceClip = Resources.Load<AudioClip>("yo-kai-API/song/yo-kai-song/" + MakeAnUrl(yoKai.nom));
            dictionnaire.Add(yoKai);
            _yokaiById[ID] = yoKai;
        
            return yoKai;
        } else {
            Debug.LogError("Fichier JSON introuvable pour l'ID : " + ID);
            return null;
        }
    }

    private static YoKai ConstruireYoKai(YoKaiJsonData data)
    {
        YoKai yoKai = new YoKai();
        if (data == null) return yoKai;

        yoKai.nom = data.Nom;
        yoKai.englishName = data.nom_anglais;
        yoKai.Description = data.Description;
        yoKai.Force = LireEntier(data.Force);
        yoKai.Vitesse = LireEntier(data.Vitesse);
        yoKai.Esprit = LireEntier(data.Esprit);
        yoKai.Défense = LireEntier(data.Défense);
        yoKai.PV = LireEntier(data.PV);
        yoKai.rare = string.Equals(data.rarite, "Rare", StringComparison.OrdinalIgnoreCase);
        yoKai.tribue = LireEnum(data.Tribue, Tribue.Bienveillant);
        yoKai.rang = LireEnum(data.Rang, Rang.E);
        yoKai.technique = LireEnum(data.Technique, Technique.Feu);
        yoKai.talent = LireEnum(data.talent, Talent.Absorption);
        yoKai.type = LireEnum(data.Type, YKW.Core.YoKai.Enums.Type.Attaque);
        yoKai.platPréféré = LireEnum(data.nourriture, PlatPréféré.Riz);
        yoKai.monElement = LireEnum(data.élément, Element.Neutre);
        yoKai.phrasesNourriture = new Dictionary<string, string>();
        return yoKai;
    }

    private static int LireEntier(string valeur)
    {
        return int.TryParse(valeur, NumberStyles.Integer, CultureInfo.InvariantCulture, out int résultat)
            ? résultat
            : 0;
    }

    private static T LireEnum<T>(string valeur, T valeurParDéfaut) where T : struct
    {
        return Enum.TryParse(valeur, true, out T résultat) ? résultat : valeurParDéfaut;
    }

    [Serializable]
    private sealed class YoKaiJsonData
    {
        public string Nom;
        public string nom_anglais;
        public string Tribue;
        public string Rang;
        public string PV;
        public string Force;
        public string Vitesse;
        public string Esprit;
        public string Défense;
        public string Description;
        public string nourriture;
        public string élément;
        public string Technique;
        public string Type;
        public string talent;
        public string rarite;
    }

    public static string MakeAnUrl(object valeur){
        if (valeur == null) return string.Empty;
        string texte = valeur.ToString().Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        foreach (char c in texte){
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark){
                sb.Append(c);
            }
        }
        texte = sb.ToString();
        texte = texte.ToLowerInvariant().Trim();
        texte = Regex.Replace(texte, @"\s+", "-");
        texte = Regex.Replace(texte, @"[^\w\-]+", "");
        texte = Regex.Replace(texte, @"--+", "-");
        return texte;
    }

    public GameObject InstantiateYoKai(string ID){
        TextAsset mappingFile = Resources.Load<TextAsset>("yo-kai-API/data/_3D/yokai_assets_mapping");
        if (mappingFile == null) {
            Debug.LogError("Yo-kai model mapping was not found.", this);
            return null;
        }
        var mappings = JsonConvert.DeserializeObject<Dictionary<string, YoKaiModele3D>>(mappingFile.text);
        if (mappings == null || !mappings.TryGetValue(ID, out YoKaiModele3D mapping) || mapping == null) {
            Debug.LogError("No model mapping exists for Yo-kai " + ID, this);
            return null;
        }
        string modelPath = "yo-kai-API/data/_3D/models/" + System.IO.Path.GetFileNameWithoutExtension(mapping.modelFile);
        GameObject prefab = Resources.Load<GameObject>(modelPath);
        if (prefab == null) {
            Debug.LogError("Yo-kai model was not found: " + modelPath, this);
            return null;
        }
        GameObject actor = Instantiate(prefab);
        MakeAShader shader = actor.GetComponent<MakeAShader>();
        if (shader == null) shader = actor.AddComponent<MakeAShader>();
        shader.choixForcer = true;
        shader.forcerToonShader = true;
        shader.forcerUnlitShader = false;
        shader.ApplyYokaiShader();
        MakeATexture textures = actor.GetComponent<MakeATexture>();
        if (textures == null) textures = actor.AddComponent<MakeATexture>();
        textures.Initialize(mapping);

        actor.AddComponent<AddAnimator>();
        return actor;
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


    public YoKai TrouverUnYoKaiParSonNomAnglais(string nom_anglais){
        for (int i = 1; i <= MAX; i++){
            YoKai yoKaiTrouver = ServantYoKai(i);
            if (yoKaiTrouver != null && yoKaiTrouver.englishName == nom_anglais){
                return yoKaiTrouver;
            }
        }

        return null;
    }
}

public static class EnumExtensions{
    public static string GetDescription(this Enum value){
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute == null ? value.ToString() : attribute.Description;
    }
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
