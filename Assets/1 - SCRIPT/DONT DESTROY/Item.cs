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

    private char séparateur = ';';
    List<string[]> lignesNom = new List<string[]>();


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
        // lignesNom.Clear();
        // lignesNom = LireCSV("CSV/CSV_Nouriture");
        // listeDesNouritures.Clear();
        // lignesNom.RemoveAt(0);
        // foreach (string[] ligne in lignesNom){
        //     Nouriture nouv = new Nouriture{};
        //     nouv.nom = ligne[1];
        //     nouv.image = Resources.Load<Sprite>("Image_Objet/alliment/"+ligne[3]);
        //     nouv.description = ligne[6];
        //     nouv.prixAchat = TransformeTexteEnNombre(ligne[4],1);
        //     nouv.prixVente = TransformeTexteEnNombre(ligne[5],0);
        //     listeDesNouritures.Add(nouv);
        // }
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
        
        // lignesNom.Clear();
        // lignesNom = LireCSV("CSV/CSV_Équipements");
        // listeDesÉquipements.Clear();
        // if (lignesNom.Count > 0) lignesNom.RemoveAt(0);
        // // Debug.Log("nombre :"+lignesNom.Count.ToString());
        // //8-4-5
        // for (int i = 0; i < lignesNom.Count; i++){
        //     if (lignesNom[i].Length==4 && i+1<lignesNom.Count){
        //         string[] nouvelleLigne = new string[lignesNom[i].Length+lignesNom[i+1].Length];
        //         for (int k = 0; k < lignesNom[i].Length; k++){
        //             if (k != 3){
        //                 nouvelleLigne[k] = lignesNom[i][k];
        //             } else {
        //                 nouvelleLigne[3] = lignesNom[i][3]+lignesNom[i+1][0];
        //                 // Debug.Log(nouvelleLigne[3]);
        //             }
        //         }
        //         for (int k = 1; k < lignesNom[i+1].Length; k++){
        //             nouvelleLigne[k+lignesNom[i].Length-1] = lignesNom[i+1][k];
        //         }
        //         lignesNom[i] = nouvelleLigne;
        //         lignesNom.RemoveAt(i+1);
        //     }
        // }
        // foreach (string[] ligne in lignesNom){
        //     if (ligne[1].Length>0){
        //         Équipement nouv = new Équipement{};
        //         nouv.nom = ligne[1];
        //         nouv.sprite = Resources.Load<Sprite>("Image_Objet/équipement/"+ligne[2]);
        //         if (ligne.Length > 6) nouv.description = ligne[6];
        //         nouv.prixAchat = TransformeTexteEnNombre(ligne[4],1);
        //         nouv.prixVente = TransformeTexteEnNombre(ligne[5],0);

        //         string texteEffet = ligne[3].Replace(" ","");
        //         texteEffet = texteEffet.Replace("\"","");
        //         List<object> lesEffets = SéparerTexteEtNombre(texteEffet);
        //         for (int i = 0; i <= 1; i++){
        //             string v1 = "";
        //             int nombre = 0;
        //             if (i==0 && lesEffets.Count>=2){
        //                 v1 = lesEffets[0].ToString();
        //                 if (int.TryParse(lesEffets[1].ToString(), out nombre)){
        //                     nombre = int.Parse(lesEffets[1].ToString());
        //                 }
        //             } else if (i == 1 && lesEffets.Count>=4) {
        //                 v1 = lesEffets[2].ToString();
        //                 if (int.TryParse(lesEffets[3].ToString(), out nombre)){
        //                     nombre = int.Parse(lesEffets[3].ToString());
        //                 }
        //             }
        //             if (v1 == "DÉF" || v1 == "VIT" || v1 == "ESP" || v1 == "FOR"){
        //                 switch (v1){
        //                     case "DÉF":
        //                         nouv.effetSur_Défance = nombre;
        //                         break;
        //                     case "VIT":
        //                         nouv.effetSur_Vitesse = nombre;
        //                         break;
        //                     case "ESP":
        //                         nouv.effetSur_Esprit = nombre;
        //                         break;
        //                     case "FOR":
        //                         nouv.effetSur_Force = nombre;
        //                         break;
        //                 }
        //             } else if (i==0) {
        //                 if (nouv.effetSur_ELSE != ""){
        //                     nouv.effetSur_ELSE = ligne[3].Replace("\"","");
        //                 }
        //             }
        //         }
        //         listeDesÉquipements.Add(nouv);
        //     }
        // }
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
        // lignesNom.Clear();
        // lignesNom = LireCSV("CSV/CSV_Animaux");
        // listeDesAnnimaux.Clear();
        // lignesNom.RemoveAt(0);
        // foreach (string[] ligne in lignesNom){
        //     if (ligne[3] != ""){
        //         ligne[1] = ligne[1].Replace('?','★');
        //         ligne[3] = ligne[3].Replace('?','★');
        //         Annimal nouv = new Annimal{};
        //         nouv.nom = ligne[1];
        //         nouv.image = Resources.Load<Sprite>("Image_Objet/annimal/"+ligne[3]);
        //         nouv.description = ligne[6];
        //         nouv.prixAchat = TransformeTexteEnNombre(ligne[4],1);
        //         nouv.prixVente = TransformeTexteEnNombre(ligne[5],0);
        //         nouv.rare = ligne[1].Contains('★');
        //         string t = ligne[1];
        //         t = t.Replace("★","");
        //         nouv.modèle3D = Resources.Load<GameObject>("annimal/"+t);
        //         listeDesAnnimaux.Add(nouv);
        //     }
        // }
    }

    // public List<string[]> LireCSV(string nomFichierSansExtension){
    //     string chemin = Application.dataPath + "/Resources/" + nomFichierSansExtension + ".csv";
    //     List<string[]> resultat = new List<string[]>();
    //     try{
    //         string contenuPropre = File.ReadAllText(chemin, Encoding.GetEncoding("windows-1252")); 
    //         string[] lignes = contenuPropre.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
    //         foreach (string ligne in lignes){
    //             string[] valeurs = ligne.Split(séparateur); 
    //             resultat.Add(valeurs);
    //         }
    //     }
    //     catch (FileNotFoundException){
    //         Debug.LogError("Fichier CSV non trouvé : " + chemin);
    //     }
    //     return resultat;
    // }

    public float TransformeTexteEnNombre(string texte, float valeurParDéfaut){
        if (float.TryParse(texte, NumberStyles.Any, CultureInfo.InvariantCulture, out float nombre)) {
            return nombre;
        }
        return valeurParDéfaut;
    }
}

/*
NullReferenceException: Object reference not set to an instance of an object
RemplisLaRoue.ChangerPositionCible2 (UnityEngine.GameObject pageGameObject, System.Int32 maximum) (at Assets/RemplisLaRoue.cs:240)
RemplisLaRoue.Update () (at Assets/RemplisLaRoue.cs:133)
*/
/*
Nouriture
Objet
Annimal
Équipement
ObjetClef
Âme
*/









