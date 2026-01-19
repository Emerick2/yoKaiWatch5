using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoisirSaPièceDeBingoKai : MonoBehaviour{
    public List<Sprite> lesPièces = new List<Sprite>();
    public GameObject exempleDePréfabPièce;
    public GameObject lePanel;
    private List<GameObject> lesPiècesDisponibles = new List<GameObject>();
    private int position;
    public int nombreDePièceMaximumSurUneRanger;
    void Start(){
        exempleDePréfabPièce.SetActive(false);
        for (int i = 0; i < lesPièces.Count; i++){
            var obj = lesObjetsQuiExistes.Instance?.TrouverUnObjetAvecSonImage(lesPièces[i]);
            if (obj == null) {
                Debug.LogWarning("Aucun objet trouvé pour le sprite : " + lesPièces[i].name);
                continue;
            }
            string nom = obj.nom;
            if (string.IsNullOrEmpty(nom)) {
                Debug.LogWarning("Nom de l'objet null ou vide pour le sprite : " + lesPièces[i].name);
                continue;
            }
            string valeurStr = NewDataCenter.Instance?.Lire(nom);
            if (string.IsNullOrEmpty(valeurStr)) {
                Debug.LogWarning("Valeur non trouvée pour l'objet : " + nom);
                continue;
            }
            int valeur;
            if (!int.TryParse(valeurStr, out valeur)) {
                Debug.LogWarning("Impossible de parser la valeur '" + valeurStr + "' pour l'objet : " + nom);
                continue;
            }
            if (valeur > 0){
                AjouterUnePièce(i);
            }
        }
        ChangerPosition(0);
    }
    public int PièceSélectionné(){// je revindrais ici plus tard
        //Sprite image = lesPiècesDisponibles[position].transform.Find("image").GetComponent<Image>().sprite;
        //lesObjetsQuiExistes.Instance.TrouverUnObjetAvecSonImage(image).nom
        return 0;
    }
    void AjouterUnePièce(int numéroPièce){
        GameObject TV = Instantiate(exempleDePréfabPièce);
        TV.SetActive(true);
        TV.transform.parent = lePanel.transform;
        lesPiècesDisponibles.Add(TV);
        TV.transform.Find("image").GetComponent<Image>().sprite = lesPièces[numéroPièce];
        TV.transform.Find("nombre").GetComponent<TextMeshProUGUI>().text = NewDataCenter.Instance.Lire(lesObjetsQuiExistes.Instance.TrouverUnObjetAvecSonImage(lesPièces[numéroPièce]).nom);
    }
    void RetirerUnePièce(int numéroPièce){
        int num = TrouverUnGameObject(lesPièces[numéroPièce]);
        GameObject TV = lesPiècesDisponibles[num];
        if (TV != null){
            NewDataCenter.Instance.SauvegarderINT(lesObjetsQuiExistes.Instance.TrouverUnObjetAvecSonImage(lesPièces[numéroPièce]).nom, -1);
            if (int.Parse(NewDataCenter.Instance.Lire(lesObjetsQuiExistes.Instance.TrouverUnObjetAvecSonImage(lesPièces[numéroPièce]).nom)) <= 0){
                lesPiècesDisponibles.RemoveAt(num);
                Destroy(TV);
            }else{
                TV.transform.Find("nombre").GetComponent<TextMeshProUGUI>().text = (int.Parse(TV.transform.Find("nombre").GetComponent<TextMeshProUGUI>().text)-1).ToString();
            }
        }
    }
    int TrouverUnGameObject(Sprite monImage){
        for (int i = 0; i < lesPiècesDisponibles.Count; i++){
            GameObject item = lesPiècesDisponibles[i];
            if (item.transform.Find("image").GetComponent<Image>().sprite == monImage){
                return i;
            }
        }
        return 0;
    }

    void Update(){
        if (Input.GetButtonDown("right")) ChangerPosition(1);
        if (Input.GetButtonDown("left")) ChangerPosition(-1);
        if (lesPiècesDisponibles.Count > nombreDePièceMaximumSurUneRanger){
            if (Input.GetButtonDown("down")) ChangerPosition(nombreDePièceMaximumSurUneRanger);
            if (Input.GetButtonDown("up")) ChangerPosition(nombreDePièceMaximumSurUneRanger*(-1));
        }
    }
    void ChangerPosition(int valeur){
        if (lesPiècesDisponibles.Count > 0){
            lesPiècesDisponibles[position].GetComponent<Image>().enabled = false;
            position=(position+valeur)%lesPiècesDisponibles.Count;
            if (position<0) position = lesPiècesDisponibles.Count+valeur;//le + est se transforme en - sinon se serai -- et donc + alors que +- fait -
            lesPiècesDisponibles[position].GetComponent<Image>().enabled = true;
        }
    }
}