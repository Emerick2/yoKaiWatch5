/*
Se script est créé par Emerick et ne doit pas être modifié !
Pour l'utiliser 🚀
NewDataCenter.Instance.SauvegarderINT("monScore", 100);
NewDataCenter.Instance.Lire("monScore");
*/

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

public class NewDataCenter : MonoBehaviour{
    private static NewDataCenter _instance;
    private DataContainer dataContainer;
    public string filePath;
    public bool lesDonnéesDoiventÊtreChiffré=false; //il faudras l'activé pour les publication du jeu mais pour annalyser les sauvegard il est plus simple de ne pas chiffré
    private string clefDeChiffrement="g89d56566fg4d32fsdf1gsdfsf32";
    private string crypted = "";
    public int journalNumber = 1;
    public string journalNameKeys = "journal";//journal1, journal2, journal3

    public static NewDataCenter Instance{
        get{
            if (_instance == null){
                GameObject obj = new GameObject("NewDataCenter");
                _instance = obj.AddComponent<NewDataCenter>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    void Start(){
        StartCoroutine(compterLeTempsDeJeu());
    }
    IEnumerator compterLeTempsDeJeu(){
        while (true){
            yield return new WaitForSeconds(60f);
            SauvegarderINT("tempsDeJeu", 1);
        }
    }

    private Dictionary<string, string> dataDict = new Dictionary<string, string>();

    private void Awake(){
        filePath = Application.persistentDataPath+"/";
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
        //filePath = Application.persistentDataPath + "/mesData.json";
        NewJournalNumber(1);
    }

    public void NewJournalNumber(int newNumber){
        journalNumber = newNumber;
        ChargerDonnées();
        MeilleurScoreADéfinir();
    }
    public void ResetJournal(int number){
        if (dataContainer != null) dataContainer.Clear();
        if (dataDict != null) dataDict.Clear();
        string fileName = journalNameKeys + number.ToString() + ".json";
        string path = Path.Combine(filePath, fileName);
        if (File.Exists(path)) {
            string newFileName = journalNameKeys + number.ToString() + "_supr-"+DateTime.Now.ToString("dd-MM-yyyy_HHmm")+".json";
            string newPath = Path.Combine(filePath, newFileName);

            try {
                if (File.Exists(newPath)) {
                    File.Delete(newPath);
                }

                File.Move(path, newPath);
                Debug.Log($"Journal {number} archivé avec succès sous : {newFileName}");
            }
            catch (System.Exception e) {
                Debug.LogError($"Erreur lors du reset du journal : {e.Message}");
            }
        }
    }
    public string NameOfPlayerInJournalNumber(int number, string clef = "nomDuJoueur") {
        string fileName = journalNameKeys + number.ToString() + ".json";
        string path = Path.Combine(filePath, fileName);

        if (File.Exists(path)) {
            string jsonContent = File.ReadAllText(path);
            var data = JObject.Parse(jsonContent);

            JArray keys = (JArray)data["keys"];
            JArray values = (JArray)data["values"];

            int index = -1;
            for (int i = 0; i < keys.Count; i++) {
                if (keys[i].ToString() == clef) {
                    index = i;
                    break;
                }
            }

            if (index != -1 && index < values.Count) {
                return values[index].ToString();
            }
        }
        return "";
    }

    public void SauvegarderSTR(string clef, string valeur, string dossier="mesData.json"){
        // dossier = CorrigerNom(dossier);
        dossier = journalNameKeys+journalNumber.ToString()+".json";
        valeur=chiffrerLesDonnées(valeur);
        dataDict[clef] = valeur;
        string json = JsonUtility.ToJson(new DataContainer(dataDict));
        File.WriteAllText(Path.Combine(filePath+dossier), json);
    }
    public void SauvegarderINT(string clef, int valeur, string dossier="mesData.json"){
        // dossier = CorrigerNom(dossier);
        dossier = journalNameKeys+journalNumber.ToString()+".json";
        string valeurLue = Lire(clef);
        int valeurExistante = int.TryParse(valeurLue, out int resultat) ? resultat : 0;

        valeur += valeurExistante;
        string donnéDeLaValeur = chiffrerLesDonnées(valeur.ToString());
        dataDict[clef] = donnéDeLaValeur;
        string json = JsonUtility.ToJson(new DataContainer(dataDict));
        File.WriteAllText(Path.Combine(filePath+dossier), json);
    }

    public string Lire(string clef, string dossier="mesData.json"){
        // dossier = CorrigerNom(dossier);
        dossier = journalNameKeys+journalNumber.ToString()+".json";
        if (dataDict.TryGetValue(clef, out string valeur)){
            valeur = déchiffrerLesDonnées(valeur);
            //if (int.TryParse(valeur, out int intValue)){
                //return intValue;
            //}
            return valeur;
        }
        return "0";
    }

    /// <summary>
    /// On utilise la technique des centièmes (*100)
    /// </summary>
    public void SauvegarderMonaie(decimal modification){
        Debug.Log("► changement de la monnaie de "+modification);
        decimal soldeActuel = LireMonaie();
        decimal nouveauSolde = soldeActuel + modification;
        if (nouveauSolde < 0m) {
            nouveauSolde = 0m; 
        }

        int valeurEnCentimes = (int)Math.Round(nouveauSolde * 100);
        SauvegarderSTR("€",valeurEnCentimes.ToString());
    }
    public decimal LireMonaie(){
        string texteSomme = Lire("€");
    
        if (decimal.TryParse(texteSomme, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal centimes)) {
            decimal somme = centimes / 100m;
            return somme < 0m ? 0m : somme;
        }
        return 0m;
    }

    string CorrigerNom(string nom){
        if (string.IsNullOrWhiteSpace(nom)) return "fichier.json";
        if (!nom.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) nom += ".json";
        return nom;
    }

    private void ChargerDonnées(){
        dataDict = new Dictionary<string, string>(); // Reset de base
        string name = journalNameKeys+journalNumber.ToString()+".json";
        
        string path = Path.Combine(filePath + name);
        if (File.Exists(path)){
            string json = File.ReadAllText(path);
            dataContainer = JsonUtility.FromJson<DataContainer>(json);
            Dictionary<string, string> tempDict = dataContainer.ToDictionary();
            foreach (var pair in tempDict){
                if (!dataDict.ContainsKey(pair.Key)) {
                    dataDict.Add(pair.Key, pair.Value);
                } else {
                    // Optionnel : tu peux choisir de remplacer ou d’ignorer
                    dataDict[pair.Key] = pair.Value; // ou bien ne rien faire
                }
            }
        }
    }

    public void MeilleurScoreADéfinir(){
        int meilleurScore = int.Parse(Lire("dataMeilleurScore"));
        int nombreLarvuce = int.Parse(Lire("dataNombreDeLarvuceDansLaPartie"));
        if (nombreLarvuce > meilleurScore) {
            SauvegarderINT("dataMeilleurScore", nombreLarvuce);
        }
    }
    string chiffrerLesDonnées(string data){
        if (lesDonnéesDoiventÊtreChiffré){
            crypted="";
            for (int i=0;i<data.Length;i++){
                crypted += (char)(data[i] ^ clefDeChiffrement[i % clefDeChiffrement.Length]);
            }
            return crypted;
        }else{
            return data;
        }
    }
    private string déchiffrerLesDonnées(string data){
        if (lesDonnéesDoiventÊtreChiffré){
            string decrypted = "";
            for (int i = 0; i < data.Length; i++){
                decrypted += (char)(data[i] ^ clefDeChiffrement[i % clefDeChiffrement.Length]);
            }
            return decrypted;
        }else{
            return data;
        }
    }
}

[System.Serializable]
public class DataContainer{
    public List<string> keys;
    public List<string> values;

    public DataContainer(Dictionary<string, string> dict){
        keys = new List<string>(dict.Keys);
        values = new List<string>(dict.Values);
    }

    public Dictionary<string, string> ToDictionary() {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        for (int i = 0; i < keys.Count && i < values.Count; i++) { // Vérification
            dict[keys[i]] = values[i];
        }
        return dict;
    }

    public void Clear() {
        keys.Clear();
        values.Clear();
    }
}
