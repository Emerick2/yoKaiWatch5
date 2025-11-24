using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour{
    public static Item _instance;
    public List<EffetNouriture> listeDesEffetsNouritures;
    public List<Nouriture> listeDesNouritures;
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
        // 1 - Mettre les objets nouritures
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

    List<string[]> LireCSV(string nomFichierSansExtension){
        TextAsset fichier = Resources.Load<TextAsset>(nomFichierSansExtension);
        if (fichier == null){
            Debug.LogError("Fichier CSV non trouvé : " + nomFichierSansExtension);
            return new List<string[]>();
        }

        string[] lignes = fichier.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
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