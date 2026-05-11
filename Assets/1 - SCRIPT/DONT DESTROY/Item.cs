using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System.Globalization;

public class Item : MonoBehaviour{
    public static Item _instance;
    public List<EffetNouriture> listeDesEffetsNouritures;
    public List<Nouriture> listeDesNouritures;
    public List<Équipement> listeDesÉquipements;
    public List<Annimal> listeDesAnnimaux;
    public bool chargementTerminé = false;

    private void Awake(){
        if (_instance == null){
            _instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }
    void Start(){
        RemplireLesListe();
    }
    void RemplireLesListe(){
        chargementTerminé = false;
        RemplirePourAlliment();
        RemplirePourÉquipement();
        RemplirePourAnnimal();
        // SéparerTexteEtNombre("épée de légende !");//attendu[épée de légende !]
        // SéparerTexteEtNombre("def+1");//attendu [def,+1]
        // SéparerTexteEtNombre("def+1ou-5");//attendu[def,+1,ou,-5]
        // SéparerTexteEtNombre("def1ou5");//attendu[def,1,ou,5]
        chargementTerminé = true;

    }

    List<object> SéparerTexteEtNombre(string texte, bool debug = false){
        List<char> nombre = new List<char>{'+','-','0','1','2','3','4','5','6','7','8','9'};
        List<object> chaine = new List<object>();
        string temps = "";
        bool cherchePartieNombre = false;
        foreach (char item in texte){
            bool passeMonCheminCesPlusUneClef = !cherchePartieNombre && nombre.Contains(item);
            bool passeMonCheminCesPlusUneValeur = cherchePartieNombre && !nombre.Contains(item);
            if (!passeMonCheminCesPlusUneClef && !passeMonCheminCesPlusUneValeur){
                temps += item;
            } else if (temps.Length > 0) {
                if (!cherchePartieNombre){
                    chaine.Add(temps);
                } else {
                    if (debug) Debug.Log("|nombre");
                    string temps2 = "";
                    if ((temps[0] == '-' || temps[0] == '+') && temps.Length >= 1){
                        for (int i = 1; i < temps.Length; i++){
                            temps2+=i;
                        }
                    } else {
                        for (int i = 0; i < temps.Length; i++){
                            temps2+=i;
                        }
                    }
                    int temps2Nombre = 0;
                    if (int.TryParse(temps2, out temps2Nombre)){
                        temps2Nombre = int.Parse(temps2);
                        if (temps[0] == '-'){
                            chaine.Add(temps2Nombre*(-1));
                        } else {
                            chaine.Add(temps2Nombre);
                        }
                    } else {
                        chaine.Add(temps);
                    }
                }
                temps = item.ToString();
                cherchePartieNombre = !cherchePartieNombre;
            }
        }
        if (temps != ""){
            chaine.Add(temps);
        }
        if (debug){
            Debug.Log("---");
            foreach (var item in chaine){
                Debug.Log(item);
            }
            Debug.Log("---");
        }
        return chaine;
    }

    void RemplirePourAlliment(){
        int i = 1;
        bool encoreDesFichiers = true;

        while (encoreDesFichiers) {
            TextAsset fichierJson = Resources.Load<TextAsset>($"yo-kai-API/data/food_data/{i}");

            if (fichierJson != null) {
                Food_tampons data = JsonUtility.FromJson<Food_tampons>(fichierJson.text);

                Nouriture nouv = new Nouriture();
                nouv.nom = data.name;
                nouv.description = data.description;
                data.imageUrl = data.imageUrl.Replace("https://raw.githubusercontent.com/Emerick2/yo-kai-API/refs/heads/main/","yo-kai-API/");
                data.imageUrl = data.imageUrl.Replace(".png","");
                nouv.image = Resources.Load<Sprite>(data.imageUrl);
                
                // Conversion des prix
                nouv.prixAchat = TransformeTexteEnNombre(data.purchasePrice, 1);
                nouv.prixVente = TransformeTexteEnNombre(data.sellingPrice, 0);

                listeDesNouritures.Add(nouv);
                i++; 
            } else {
                encoreDesFichiers = false;
            }
        }
    }

    void RemplirePourÉquipement(){
        int e = 1;
        bool encoreDesFichiers = true;

        while (encoreDesFichiers) {
            TextAsset fichierJson = Resources.Load<TextAsset>($"yo-kai-API/data/item-equipement_data/{e}");

            if (fichierJson != null) {
                Food_tampons data = JsonUtility.FromJson<Food_tampons>(fichierJson.text);

                Équipement nouv = new Équipement();
                nouv.nom = data.name;
                nouv.description = data.description;
                data.imageUrl = data.imageUrl.Replace("https://raw.githubusercontent.com/Emerick2/yo-kai-API/refs/heads/main/","yo-kai-API/");
                data.imageUrl = data.imageUrl.Replace(".png","");
                nouv.sprite = Resources.Load<Sprite>(data.imageUrl);
                
                // Conversion des prix
                nouv.prixAchat = TransformeTexteEnNombre(data.purchasePrice, 1);
                nouv.prixVente = TransformeTexteEnNombre(data.sellingPrice, 0);

                string texteEffet = data.effect.Replace(" ","");
                texteEffet = texteEffet.Replace("\"","");
                List<object> lesEffets = SéparerTexteEtNombre(texteEffet);
                for (int i = 0; i <= 1; i++){
                    string v1 = "";
                    int nombre = 0;
                    if (i==0 && lesEffets.Count>=2){
                        v1 = lesEffets[0].ToString();
                        if (int.TryParse(lesEffets[1].ToString(), out nombre)){
                            nombre = int.Parse(lesEffets[1].ToString());
                        }
                    } else if (i == 1 && lesEffets.Count>=4) {
                        v1 = lesEffets[2].ToString();
                        if (int.TryParse(lesEffets[3].ToString(), out nombre)){
                            nombre = int.Parse(lesEffets[3].ToString());
                        }
                    }
                    if (v1 == "DÉF" || v1 == "VIT" || v1 == "ESP" || v1 == "FOR"){
                        switch (v1){
                            case "DÉF":
                                nouv.effetSur_Défance = nombre;
                                break;
                            case "VIT":
                                nouv.effetSur_Vitesse = nombre;
                                break;
                            case "ESP":
                                nouv.effetSur_Esprit = nombre;
                                break;
                            case "FOR":
                                nouv.effetSur_Force = nombre;
                                break;
                        }
                    } else if (i==0) {
                        if (nouv.effetSur_ELSE != ""){
                            nouv.effetSur_ELSE = data.effect.Replace("\"","");
                        }
                    }
                }

                listeDesÉquipements.Add(nouv);
                e++; 
            } else {
                encoreDesFichiers = false;
            }
        }        
    }

    void RemplirePourAnnimal(){
        int i = 1;
        bool encoreDesFichiers = true;

        while (encoreDesFichiers) {
            TextAsset fichierJson = Resources.Load<TextAsset>($"yo-kai-API/data/bug_data/{i}");

            if (fichierJson != null) {
                Food_tampons data = JsonUtility.FromJson<Food_tampons>(fichierJson.text);

                Annimal nouv = new Annimal();
                nouv.nom = data.name;
                nouv.description = data.description;
                data.imageUrl = data.imageUrl.Replace("https://raw.githubusercontent.com/Emerick2/yo-kai-API/refs/heads/main/","yo-kai-API/");
                data.imageUrl = data.imageUrl.Replace(".png","");
                nouv.image = Resources.Load<Sprite>(data.imageUrl);
                
                // Conversion des prix
                nouv.prixAchat = TransformeTexteEnNombre(data.purchasePrice, 1);
                nouv.prixVente = TransformeTexteEnNombre(data.sellingPrice, 0);

                nouv.rare = nouv.nom.Contains('★');
                string t = nouv.nom;
                t = t.Replace("★","");
                nouv.modèle3D = Resources.Load<GameObject>("annimal/"+t);

                listeDesAnnimaux.Add(nouv);
                i++; 
            } else {
                encoreDesFichiers = false;
            }
        }
    }

    public float TransformeTexteEnNombre(string texte, float valeurParDéfaut){
        if (float.TryParse(texte, NumberStyles.Any, CultureInfo.InvariantCulture, out float nombre)) {
            return nombre;
        }
        return valeurParDéfaut;
    }
}

/*
Nouriture
Objet
Annimal
Équipement
ObjetClef
Âme
*/









