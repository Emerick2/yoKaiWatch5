using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComplétéLeMédaliumAvecCSV : MonoBehaviour{
    private medaliumDictionnaire médailum;
    private char séparateur = ';';
    private string nomDuFichierCSV = "Médalium";
    List<string[]> lignesNom;
    void Awake(){
        lignesNom = LireCSV(nomDuFichierCSV);
        /*foreach (string[] ligne in lignesNom){
            Debug.Log(string.Join(" | ", ligne));
        }*/
    }
    void Start(){
        médailum = medaliumDictionnaire._instance.AvoirLeGameObject().GetComponent<medaliumDictionnaire>();
        AméliorerLeMédalium();
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
    void AméliorerLeMédalium(){
        bool sauterLaPemièreLigne = false;
        foreach (string[] ligne in lignesNom){
            ligne[0].Replace(" ","");

            if (sauterLaPemièreLigne == true && ligne[0] != "" && médailum!=null){
                int IDD = int.Parse(ligne[0]);
                //IDD--;
                string ID = IDD.ToString();
                // while (médailum.MAX <= int.Parse(ID)){
                //     var v = new MedaliumNonPersonel();
                //     médailum.dictionnaire.Add(v);
                // }
                MedaliumNonPersonel l = médailum.ServantYoKai(int.Parse(ID));
                l.Nom = ligne[1];
                if (ligne[2] != "")médailum.AjouterUneTribue(int.Parse(ID), ligne[2]);
                if (ligne[3] != "")médailum.AjouterUnRang(int.Parse(ID), ligne[3]);
                if (ligne[4] != "") l.PV = int.Parse(ligne[4]);
                if (ligne[5] != "")l.Esprit = int.Parse(ligne[5]);
                if (ligne[6] != "")l.Vitesse = int.Parse(ligne[6]);
                if (ligne[7] != "")l.Force = int.Parse(ligne[7]);
                l.Description = ligne[8];
                
                string chemin = "Medaillons/" + ID;
                Sprite image = Resources.Load<Sprite>(chemin);
                if (image != null){
                    l.médaillon = image;
                } else {
                    Debug.LogWarning("Image non trouvée pour le chemin : " + chemin);
                }



                GameObject prefab = Resources.Load<GameObject>("tousLesYoKai/"+ID);
                if (prefab != null) {
                    l.yoKai = prefab;
                } else {
                    Debug.LogWarning("Prefab non trouvé :"+ID);
                }

                if (ligne[9] != "") l.nomDeMonSquellette = ligne[9];
            }
            sauterLaPemièreLigne=true;
        }
    }
}