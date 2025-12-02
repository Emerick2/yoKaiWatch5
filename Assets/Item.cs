using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour{
    public static Item _instance;
    public List<EffetNouriture> listeDesEffetsNouritures;
    public List<Nouriture> listeDesNouritures;
    public List<Équipement> listeDesÉquipements;

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
        RemplirePourAlliment();
        RemplirePourÉquipement();
        // SéparerTexteEtNombre("épée de légende !");//attendu[épée de légende !]
        // SéparerTexteEtNombre("def+1");//attendu [def,+1]
        // SéparerTexteEtNombre("def+1ou-5");//attendu[def,+1,ou,-5]
        // SéparerTexteEtNombre("def1ou5");//attendu[def,1,ou,5]

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
        lignesNom.Clear();
        lignesNom = LireCSV("CSV/CSV_Nouriture");
        listeDesNouritures.Clear();
        lignesNom.RemoveAt(0);
        foreach (string[] ligne in lignesNom){
            Nouriture nouv = new Nouriture{};
            nouv.nom = ligne[1];
            nouv.image = Resources.Load<Sprite>("Image_Objet/alliment/"+ligne[3]);
            nouv.description = ligne[6];
            if (!int.TryParse(ligne[4], out nouv.prixAchat)) nouv.prixAchat = 1;
            else nouv.prixAchat = int.Parse(ligne[4]);
            
            if (!int.TryParse(ligne[5], out nouv.prixVente)) nouv.prixVente = 0;
            else nouv.prixVente = int.Parse(ligne[5]);
            listeDesNouritures.Add(nouv);
        }
    }

    void RemplirePourÉquipement(){
        lignesNom.Clear();
        lignesNom = LireCSV("CSV/CSV_Équipements");
        listeDesÉquipements.Clear();
        if (lignesNom.Count > 0) lignesNom.RemoveAt(0);
        // Debug.Log("nombre :"+lignesNom.Count.ToString());
        //8-4-5
        for (int i = 0; i < lignesNom.Count; i++){
            if (lignesNom[i].Length==4 && i+1<lignesNom.Count){
                string[] nouvelleLigne = new string[lignesNom[i].Length+lignesNom[i+1].Length];
                for (int k = 0; k < lignesNom[i].Length; k++){
                    if (k != 3){
                        nouvelleLigne[k] = lignesNom[i][k];
                    } else {
                        nouvelleLigne[3] = lignesNom[i][3]+lignesNom[i+1][0];
                        // Debug.Log(nouvelleLigne[3]);
                    }
                }
                for (int k = 1; k < lignesNom[i+1].Length; k++){
                    nouvelleLigne[k+lignesNom[i].Length-1] = lignesNom[i+1][k];
                }
                lignesNom[i] = nouvelleLigne;
                lignesNom.RemoveAt(i+1);
            }
        }
        Debug.Log("-------");
        foreach (string[] ligne in lignesNom){
            if (ligne[1].Length>0){
                Debug.Log(ligne.Length);
                Équipement nouv = new Équipement{};
                nouv.nom = ligne[1];
                nouv.sprite = Resources.Load<Sprite>("Image_Objet/équipement/"+ligne[2]);
                if (ligne.Length > 6) nouv.description = ligne[6];
                if (ligne.Length <= 4 || !int.TryParse(ligne[4], out nouv.prixAchat)) nouv.prixAchat = 1;
                else nouv.prixAchat = int.Parse(ligne[4]);
            
                if (ligne.Length <= 5 || !int.TryParse(ligne[5], out nouv.prixVente)) nouv.prixVente = 0;
                else nouv.prixVente = int.Parse(ligne[5]);

                string texteEffet = ligne[3].Replace(" ","");
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
                            nouv.effetSur_ELSE = ligne[3].Replace("\"","");
                        }
                    }
                }
                listeDesÉquipements.Add(nouv);
            }
            else{
                Debug.Log(ligne.Length);
            }
        }
    }

    public List<string[]> LireCSV(string nomFichierSansExtension){
        TextAsset fichier = Resources.Load<TextAsset>(nomFichierSansExtension);
        if (fichier == null){
            Debug.LogError("Fichier CSV non trouvé : " + nomFichierSansExtension);
            return new List<string[]>();
        }
        string contenuPropre = fichier.text.Replace('\r', ' '); // Remplace par un espace pour éviter de coller le texte
        string[] lignes = contenuPropre.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string[]> resultat = new List<string[]>();
        foreach (string ligne in lignes){
            string[] valeurs = ligne.Split(séparateur); 
            resultat.Add(valeurs);
        }
        return resultat;
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

[System.Serializable]
public class Nouriture{
    public string nom;
    public Sprite image;
    [TextArea(3, 5)] public string description;
    public int prixAchat;
    public int prixVente;
    [Min(0)] public int IdEffetSurSiSurEnnemie;
    [Min(0)] public int IdEffetSiSurAmi;
}



[System.Serializable]
public class EffetNouriture{
    [TextArea(3, 5)] public string description;
}

[System.Serializable]
public class Équipement{
    public string nom;
    public Sprite sprite;
    [TextArea(3, 5)] public string description;
    public string catégorieDeYoKaiPorteur;
    public int prixAchat;
    public int prixVente;
    public int effetSur_Force;
    public int effetSur_Vitesse;
    public int effetSur_Défance;
    public int effetSur_Esprit;
    public string effetSur_ELSE;
}
