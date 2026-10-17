using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using YKW.Core.Item.Enum;
using YKW.Core.Item.Class;
using YKW.Core.YoKai.Struct;

public class Item : MonoBehaviour
{
    private const string ApiRoot = "https://raw.githubusercontent.com/Emerick2/yo-kai-API/refs/heads/main/";
    private const string ResourcesRoot = "yo-kai-API/";
    private const string FoodPath = "yo-kai-API/data/food_data/";
    private const string EquipmentPath = "yo-kai-API/data/item-equipement_data/";
    private const string AnimalPath = "yo-kai-API/data/bug_data/";

    public static Item Instance;
    public List<Nouriture> listeDesNouritures = new();
    public List<Équipement> listeDesÉquipements = new();
    public List<Annimal> listeDesAnnimaux = new();
    public bool chargementTerminé { get; private set; }

    private readonly Dictionary<Nouriture, EffetsNourriture> effetsNourritures = new();

    private struct EffetsNourriture
    {
        public string descriptionAllie;
        public string descriptionEnnemi;
        public int valeurAllie;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    private void Start() => ChargerTousLesObjets();

    public void ChargerTousLesObjets()
    {
        chargementTerminé = false;
        listeDesNouritures.Clear();
        listeDesÉquipements.Clear();
        listeDesAnnimaux.Clear();
        effetsNourritures.Clear();

        ChargerNourritures();
        ChargerEquipements();
        ChargerAnimaux();
        chargementTerminé = true;
    }

    private void ChargerNourritures()
    {
        ChargerTousLesFichiers(FoodPath, data =>
        {
            Nouriture nourriture = new Nouriture
            {
                nom = data.name,
                englishName = string.IsNullOrEmpty(data.englishName) ? data.nom_anglais : data.englishName,
                description = data.description,
                image = ChargerSprite(data.imageUrl),
                prixAchat = LirePrix(data.purchasePrice, 1f),
                prixVente = LirePrix(data.sellingPrice, 0f)
            };

            nourriture.effetAlié = LireEffetAllie(data.allyEffect, out int valeurAllie);
            nourriture.effetEnemi = LireEffetEnnemi(data.enemyEffect);
            effetsNourritures[nourriture] = new EffetsNourriture
            {
                descriptionAllie = data.allyEffect ?? string.Empty,
                descriptionEnnemi = data.enemyEffect ?? string.Empty,
                valeurAllie = valeurAllie
            };
            listeDesNouritures.Add(nourriture);
        });
    }

    private void ChargerEquipements()
    {
        ChargerTousLesFichiers(EquipmentPath, data =>
        {
            Équipement équipement = new Équipement
            {
                nom = data.name,
                englishName = string.IsNullOrEmpty(data.englishName) ? data.nom_anglais : data.englishName,
                description = data.description,
                image = ChargerSprite(data.imageUrl),
                prixAchat = LirePrix(data.purchasePrice, 1f),
                prixVente = LirePrix(data.sellingPrice, 0f),
                effet = LireEffetEquipement(data.effect)
            };
            listeDesÉquipements.Add(équipement);
        });
    }

    private void ChargerAnimaux()
    {
        ChargerTousLesFichiers(AnimalPath, data =>
        {
            string nomSansRarete = (data.name ?? string.Empty).Replace("★", string.Empty);
            Annimal animal = new Annimal
            {
                nom = data.name,
                englishName = string.IsNullOrEmpty(data.englishName) ? data.nom_anglais : data.englishName,
                description = data.description,
                image = ChargerSprite(data.imageUrl),
                prixAchat = LirePrix(data.purchasePrice, 1f),
                prixVente = LirePrix(data.sellingPrice, 0f),
                rare = (data.name ?? string.Empty).Contains('★'),
                modèle3D = Resources.Load<GameObject>("annimal/" + nomSansRarete)
            };
            listeDesAnnimaux.Add(animal);
        });
    }

    private static void ChargerTousLesFichiers(string dossierResources, Action<Food_tampons> traiter)
    {
        TextAsset[] fichiers = Resources.LoadAll<TextAsset>(dossierResources.TrimEnd('/'));
        Array.Sort(fichiers, ComparerFichiersParId);

        foreach (TextAsset fichier in fichiers)
        {
            Food_tampons data = JsonUtility.FromJson<Food_tampons>(fichier.text);
            if (data == null)
            {
                Debug.LogWarning($"Donnée invalide : {dossierResources}{fichier.name}");
                continue;
            }

            traiter(data);
        }
    }

    private static int ComparerFichiersParId(TextAsset premier, TextAsset second)
    {
        bool premierEstNumerique = int.TryParse(premier.name, out int premierId);
        bool secondEstNumerique = int.TryParse(second.name, out int secondId);
        if (premierEstNumerique && secondEstNumerique) return premierId.CompareTo(secondId);
        if (premierEstNumerique) return -1;
        if (secondEstNumerique) return 1;
        return string.CompareOrdinal(premier.name, second.name);
    }

    private static Sprite ChargerSprite(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return null;
        string chemin = imageUrl.Replace(ApiRoot, ResourcesRoot).Replace(".png", string.Empty);
        return Resources.Load<Sprite>(chemin);
    }

    private static Stats LireEffetEquipement(string texte)
    {
        Stats stats = new Stats();
        if (string.IsNullOrWhiteSpace(texte)) return stats;

        foreach (Match match in Regex.Matches(texte, @"(FOR|VIT|ESP|DÉF)\s*([+-]\s*\d+)"))
        {
            int valeur = int.Parse(match.Groups[2].Value.Replace(" ", string.Empty), CultureInfo.InvariantCulture);
            switch (match.Groups[1].Value)
            {
                case "FOR": stats.Force += valeur; break;
                case "VIT": stats.Vitesse += valeur; break;
                case "ESP": stats.Esprit += valeur; break;
                case "DÉF": stats.Défence += valeur; break;
            }
        }

        string effetSpecial = Regex.Replace(texte, @"(FOR|VIT|ESP|DÉF)\s*[+-]\s*\d+", string.Empty).Trim();
        stats.modificateurAutre = string.IsNullOrWhiteSpace(effetSpecial) ? null : effetSpecial;
        return stats;
    }

    private static EffetNourritureAllie LireEffetAllie(string texte, out int valeur)
    {
        valeur = 0;
        string effet = Normaliser(texte);
        Match soinFixe = Regex.Match(effet, @"restaure\s+(\d+)\s+pv");
        if (soinFixe.Success)
        {
            valeur = int.Parse(soinFixe.Groups[1].Value, CultureInfo.InvariantCulture);
            return EffetNourritureAllie.RestaurePVFixe;
        }

        if (effet.Contains("rend tres peu de pv")) return EffetNourritureAllie.RendTresPeuPV;
        if (effet.Contains("rend un peu de pv")) return EffetNourritureAllie.RendUnPeuPV;
        if (effet.Contains("rend pas mal de pv")) return EffetNourritureAllie.RendPasMalPV;
        if (effet.Contains("rend beaucoup de pv")) return EffetNourritureAllie.RendBeaucoupPV;
        if (effet.Contains("rend enormement de pv")) return EffetNourritureAllie.RendEnormementPV;
        if (effet.Contains("rend tous les pv")) return EffetNourritureAllie.RendTousLesPV;
        if (effet.Contains("rend un peu d'ame")) return EffetNourritureAllie.RendUnPeuAme;
        if (effet.Contains("rend pas mal d'ame")) return EffetNourritureAllie.RendPasMalAme;
        if (effet.Contains("rend enormement d'ame")) return EffetNourritureAllie.RendEnormementAme;
        if (effet.Contains("rend toute l'ame")) return EffetNourritureAllie.RendTouteAme;

        Match amePourcentage = Regex.Match(effet, @"recupere\s+(\d+)\s*%\s+de l'ame");
        if (!amePourcentage.Success) return EffetNourritureAllie.Inconnu;

        valeur = int.Parse(amePourcentage.Groups[1].Value, CultureInfo.InvariantCulture);
        return EffetNourritureAllie.RecuperePourcentageAme;
    }

    private static EffetNourritureEnnemi LireEffetEnnemi(string texte)
    {
        string effet = Normaliser(texte);
        if (effet.Contains("deviendra ton ami")) return EffetNourritureEnnemi.DevientAmi;
        if (effet.Contains("adorer")) return EffetNourritureEnnemi.Adore;
        if (effet.Contains("apprecieront beaucoup") || effet.Contains("t'aimeront beaucoup")) return EffetNourritureEnnemi.ApprecieBeaucoup;
        if (effet.Contains("vont t'aimer")) return EffetNourritureEnnemi.Aime;
        if (effet.Contains("apprecieront un peu")) return EffetNourritureEnnemi.ApprecieUnPeu;
        return effet.Contains("appreci") ? EffetNourritureEnnemi.Apprecie : EffetNourritureEnnemi.Inconnu;
    }

    private static string Normaliser(string texte)
    {
        string decomposé = (texte ?? string.Empty).Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder resultat = new StringBuilder(decomposé.Length);
        foreach (char caractere in decomposé)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) != UnicodeCategory.NonSpacingMark)
                resultat.Append(caractere);
        }
        return resultat.ToString().TrimEnd('.');
    }

    public float TransformeTexteEnNombre(string texte, float valeurParDéfaut) => LirePrix(texte, valeurParDéfaut);

    private static float LirePrix(string texte, float valeurParDéfaut) =>
        float.TryParse(texte, NumberStyles.Any, CultureInfo.InvariantCulture, out float valeur) ? valeur : valeurParDéfaut;

    public int ObtenirValeurEffetAllie(Nouriture nourriture) =>
        effetsNourritures.TryGetValue(nourriture, out EffetsNourriture effet) ? effet.valeurAllie : 0;

    public string ObtenirDescriptionEffetAllie(Nouriture nourriture) =>
        effetsNourritures.TryGetValue(nourriture, out EffetsNourriture effet) ? effet.descriptionAllie : string.Empty;

    public string ObtenirDescriptionEffetEnnemi(Nouriture nourriture) =>
        effetsNourritures.TryGetValue(nourriture, out EffetsNourriture effet) ? effet.descriptionEnnemi : string.Empty;

    public Nouriture GetNouritureByName(string nom) => listeDesNouritures.Find(nourriture => nourriture.nom == nom);

    public Sprite GetSpriteByName(string nom)
    {
        Nouriture nourriture = GetNouritureByName(nom);
        if (nourriture != null) return nourriture.image;

        Équipement équipement = listeDesÉquipements.Find(item => item.nom == nom);
        if (équipement != null) return équipement.image;

        Annimal animal = listeDesAnnimaux.Find(item => item.nom == nom);
        return animal?.image;
    }

    public Nouriture FindFoodWithEnglishName(string englishName){
        foreach (Nouriture item in listeDesNouritures){
            if (!string.IsNullOrEmpty(englishName) && string.Equals(item.englishName, englishName, StringComparison.OrdinalIgnoreCase)){
                return item;
            }
        }
        return null;
    }

    public Équipement FindEquipmentWithEnglishName(string englishName){
        foreach (Équipement item in listeDesÉquipements){
            if (!string.IsNullOrEmpty(englishName) && string.Equals(item.englishName, englishName, StringComparison.OrdinalIgnoreCase)){
                return item;
            }
        }
        return null;
    }

    public Annimal FindAnimalWithEnglishName(string englishName){
        foreach (Annimal item in listeDesAnnimaux){
            if (!string.IsNullOrEmpty(englishName) && string.Equals(item.englishName, englishName, StringComparison.OrdinalIgnoreCase)){
                return item;
            }
        }
        return null;
    }
}
