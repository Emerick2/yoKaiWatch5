using System.Collections.Generic;
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
        Debug.Log("nombre :"+lignesNom.Count.ToString());
        foreach (string[] ligne in lignesNom){
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
            for (int i = 1; i <= 2; i++){
                int bonus = i-1;
                string v1 = RécupéréLaChaineDeCaractère(texteEffet,((i-1)*3)+bonus);
                if (v1 == "DÉF" || v1 == "VIT" || v1 == "ESP" || v1 == "FOR"){
                    switch (v1){
                        case "DÉF":
                            nouv.effetSur_Défance = RécupéréLeNombreDansLaChaineDeCaractère(texteEffet,(3*i)+bonus);
                            break;
                        case "VIT":
                            nouv.effetSur_Vitesse = RécupéréLeNombreDansLaChaineDeCaractère(texteEffet,(3*i)+bonus);
                            break;
                        case "ESP":
                            nouv.effetSur_Esprit = RécupéréLeNombreDansLaChaineDeCaractère(texteEffet,(3*i)+bonus);
                            break;
                        case "FOR":
                            nouv.effetSur_Force = RécupéréLeNombreDansLaChaineDeCaractère(texteEffet,(3*i)+bonus);
                            break;
                    }
                } else {
                    if (nouv.effetSur_ELSE != ""){
                        nouv.effetSur_ELSE = ligne[3].Replace("\"","");
                    }
                }
            }
            listeDesÉquipements.Add(nouv);
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

    string RécupéréLaChaineDeCaractère(string mot, int début){
        string résulta = "";
        for (int i = début; i < mot.Length; i++){
            char item = mot[i];
            if (item != '+' && item != '-'){
                résulta+=item;
            } else {
                return résulta;
            }
        }
        return "";
    }
    int RécupéréLeNombreDansLaChaineDeCaractère(string mot, int debut){
       if (string.IsNullOrEmpty(mot) || debut >= mot.Length){
            return 0;
        }

        int resultat = 0;
        bool estNegatif = false;
        int i = debut;
        char indicateur = mot[debut];
        if (indicateur == '-'){
            estNegatif = true;
            i++;
        } else if (indicateur == '+'){
            estNegatif = false;
            i++;
        }
        // else if (char.IsDigit(indicateur)){}
        for (; i < mot.Length; i++){
            char item = mot[i];

            if (char.IsDigit(item)){
                resultat = (resultat * 10) + (item - '0');
            }else{
                break;
            }
        }

        if (estNegatif){
            return -resultat;
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
