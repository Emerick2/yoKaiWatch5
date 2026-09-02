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
using YKW.Core.YoKai.Class;

public class medaliumDictionnaire : MonoBehaviour{
    public static medaliumDictionnaire _instance;
    public Sprite imageDeUneMédailleInconnue;

    /*[HideInInspector]*/
    [SerializeField] private List<MedaliumYoKai> dictionnaire;

    public bool chargementFini = true;
    public int MAX = 905;
    
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

   public MedaliumYoKai ServantYoKai(int ID) {
        foreach (MedaliumYoKai item in dictionnaire){
            if (item.ID == ID){
                return item;
            }
        }
        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/yokai_data/" + ID.ToString());

        if (targetFile != null) {
            MedaliumYoKai yoKai = JsonUtility.FromJson<MedaliumYoKai>(targetFile.text);
            yoKai.ID = ID;
            yoKai.faceImage = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/picture/" + MakeAnUrl(yoKai.Nom));
            yoKai.médaillon = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/medals/" + MakeAnUrl(yoKai.Nom));
            yoKai.faceRondImage = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/face/" + MakeAnUrl(yoKai.Nom));
            if (yoKai.médaillon == null) {
                yoKai.médaillon = yoKai.faceRondImage;
            }
            if (yoKai.médaillon == null) {
                yoKai.médaillon = yoKai.faceImage;
            }
            yoKai.Bruit = Resources.Load<AudioClip>("yo-kai-API/song/yo-kai-song/" + MakeAnUrl(yoKai.Nom));
            yoKai.tribue = AjouterUneTribue(yoKai.Tribue);
            yoKai.rang = AjouterUnRang(yoKai.Rang);
            dictionnaire.Add(yoKai);
        
            return yoKai;
        } else {
            Debug.LogError("Fichier JSON introuvable pour l'ID : " + ID);
            return null;
        }
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
        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/_3D/yokai_assets_mapping");

        if (targetFile == null) {
            Debug.LogError("Le fichier JSON 'yokai_assets_mapping' est introuvable dans Resources/yo-kai-API/data/_3D/");
            return null;
        }

        if (targetFile != null) {
            var allData = JsonConvert.DeserializeObject<Dictionary<string, YoKaiModele3D>>(targetFile.text);

            if (allData.ContainsKey(ID)) {
                YoKaiModele3D map = allData[ID];
                GameObject modelFile = Resources.Load<GameObject>("yo-kai-API/data/_3D/models/"+map.modelFile.Replace(".fbx",""));
                List<Sprite> textureFiles = new List<Sprite>();
                foreach (string item in map.textureFiles){
                    textureFiles.Add(Resources.Load<Sprite>("yo-kai-API/data/_3D/textures/"+item.Replace(".png","")));
                }
                GameObject TV = Instantiate(modelFile);


                Renderer renderer = TV.GetComponentInChildren<Renderer>();
                if (renderer != null && map.textureFiles.Length > 0) {
                    Material[] mats = renderer.materials;

                    for (int i = 0; i < map.textureFiles.Length; i++) {
                        if (i >= mats.Length) break;

                        string texPath = "yo-kai-API/data/_3D/textures/" + map.textureFiles[i].Replace(".png", "");
                        Texture2D tex = Resources.Load<Texture2D>(texPath);

                        if (tex != null) {
                            mats[i].mainTexture = tex;
                        }
                    }
                    
                    renderer.materials = mats;
                }

                TV.AddComponent<MakeATexture>();
                TV.AddComponent<MakeAShader>();
                return TV;
            } else {
                Debug.Log("Modèle non trouvé");
            }
        }
        return null;
    }

    public Tribue AjouterUneTribue(string nomDeLaTribue){
        if (nomDeLaTribue != null && nomDeLaTribue.Length > 0){
            nomDeLaTribue = nomDeLaTribue.ToLower();
            nomDeLaTribue = nomDeLaTribue.Replace("mystérieux","mystérieu");
            nomDeLaTribue = nomDeLaTribue.Replace("insaisisables","insaisissable");
            
            nomDeLaTribue = char.ToUpper(nomDeLaTribue[0]) + nomDeLaTribue.Substring(1);
            if (Enum.IsDefined(typeof(Tribue), nomDeLaTribue)){
                return (Tribue)Enum.Parse(typeof(Tribue), nomDeLaTribue);
            }
            Debug.Log("La tribue "+nomDeLaTribue+" n'as pas été trouver...");
        }
        return Tribue.Bienveillant;
    }
    public Rang AjouterUnRang(string nomDuRang){
        if (nomDuRang != null && nomDuRang.Length > 0){
            nomDuRang = nomDuRang[nomDuRang.Length-1].ToString();
            nomDuRang = nomDuRang.ToUpper();
            if (Enum.IsDefined(typeof(Rang), nomDuRang)){
                return (Rang)Enum.Parse(typeof(Rang), nomDuRang);
            }
            Debug.Log("Le rang "+nomDuRang+" n'as pas été trouver...");
        }
        return Rang.E;
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
