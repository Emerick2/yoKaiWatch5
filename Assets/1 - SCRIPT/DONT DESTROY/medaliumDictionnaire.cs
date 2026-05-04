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

// le scripte qui lie le fichier "Médalium.csv" est ComplétéLeMédaliumAvecCSV
public class medaliumDictionnaire : MonoBehaviour{
    public static medaliumDictionnaire _instance;
    public Sprite imageDeUneMédailleInconnue;

    /*[HideInInspector]*/
    [SerializeField] private List<MedaliumNonPersonel> dictionnaire;

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

   public MedaliumNonPersonel ServantYoKai(int ID) {
        foreach (MedaliumNonPersonel item in dictionnaire){
            if (item.ID == ID){
                return item;
            }
        }
        TextAsset targetFile = Resources.Load<TextAsset>("yo-kai-API/data/yokai_data/" + ID.ToString());

        if (targetFile != null) {
            MedaliumNonPersonel yoKai = JsonUtility.FromJson<MedaliumNonPersonel>(targetFile.text);
            yoKai.ID = ID;
            yoKai.faceImage = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/picture/" + MakeAnUrl(yoKai.Nom));
            yoKai.médaillon = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/face/" + MakeAnUrl(yoKai.Nom));
            yoKai.faceRondImage = Resources.Load<Sprite>("yo-kai-API/img/yo-kai_img/medals/" + MakeAnUrl(yoKai.Nom));
            yoKai.Bruit = Resources.Load<AudioClip>("yo-kai-API/song/yo-kai-song/" + MakeAnUrl(yoKai.Nom));
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

        if (targetFile != null) {
            var allData = JsonConvert.DeserializeObject<Dictionary<string, Yokai_assets_mapping>>(targetFile.text);

            if (allData.ContainsKey(ID)) {
                Yokai_assets_mapping map = allData[ID];
                GameObject modelFile = Resources.Load<GameObject>("yo-kai-API/data/_3D/models/"+map.modelFile.Replace(".fbx",""));
                List<Sprite> textureFiles = new List<Sprite>();
                foreach (string item in map.textureFiles){
                    textureFiles.Add(Resources.Load<Sprite>("yo-kai-API/data/_3D/textures/"+item.Replace(".png","")));
                }
                GameObject TV = Instantiate(modelFile);


                Renderer renderer = TV.GetComponentInChildren<Renderer>();
                if (renderer != null && map.textureFiles.Length > 0) {
                    
                    // On crée un tableau de matériaux basé sur le nombre de textures
                    Material[] mats = renderer.materials;

                    for (int i = 0; i < map.textureFiles.Length; i++) {
                        if (i >= mats.Length) break; // Sécurité si plus de textures que de slots

                        // On charge en Texture2D (pas en Sprite !)
                        string texPath = "yo-kai-API/data/_3D/textures/" + map.textureFiles[i].Replace(".png", "");
                        Texture2D tex = Resources.Load<Texture2D>(texPath);

                        if (tex != null) {
                            mats[i].mainTexture = tex;
                        }
                    }
                    
                    // Très important : Réassigner le tableau de matériaux
                    renderer.materials = mats;
                }

                return TV;
            } else {
                Debug.Log("Modèle non trouvé");
            }
        }
        return null;
    }

    public void AjouterUneTribue(int IDdeLaCible, string nomDeLaTribue){
        if (dictionnaire.Count >= IDdeLaCible) return;
        nomDeLaTribue = nomDeLaTribue.ToLower();
        nomDeLaTribue = char.ToUpper(nomDeLaTribue[0]) + nomDeLaTribue.Substring(1);
        if (Enum.IsDefined(typeof(Tribue), nomDeLaTribue)){
            dictionnaire[IDdeLaCible].tribue = (Tribue)Enum.Parse(typeof(Tribue), nomDeLaTribue);
        }
    }
    public void AjouterUnRang(int IDdeLaCible, string nomDuRang){
        if (dictionnaire.Count >= IDdeLaCible) return;
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
public class Yokai_assets_mapping{
    public string modelFile;
    public string[] textureFiles;
}

[System.Serializable]
public class MedaliumNonPersonel{
    public int ID;
    public Sprite médaillon;
    public Sprite faceImage;
    public Sprite faceRondImage;
    public bool rare;
    public string Nom;
    public string Description;
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
    public int Force;
    public int Vitesse;
    public int Esprit;
    public int Défense;// IL faut le rensaigner dans le CSV
    public int PV;
    public string nomDeMonSquellette;

    [Header("Affinités Élémentaires")]
    public Element elementFaiblesse;    // L'élément qui fait mal (x2 ou x1.5)
    public Element elementResistance;   // L'élément qui fait moins mal (x0.5)
    public Element monElement;
    public AudioClip Bruit;
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
