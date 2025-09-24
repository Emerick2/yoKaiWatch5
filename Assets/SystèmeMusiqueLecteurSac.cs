using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class SystèmeMusiqueLecteurSac : MonoBehaviour{
    public AudioClip[] clips;
    public TextMeshProUGUI nomMusique;
    public GameObject lieuOuPossé;
    public Sprite imageQuandMusiqueSélectionnée;
    public Sprite imageQuandMusiqueNonSélectionnée;
    private List<GameObject> listeElementMusique = new List<GameObject>();

    void Start(){
        clips = Resources.LoadAll<AudioClip>("Musique/musique de fond");
        for (int i = 0; i < clips.Length; i++){
            GameObject nouvelleImage = new GameObject("Musique !");
            nouvelleImage.transform.parent = lieuOuPossé.transform;
            nouvelleImage.AddComponent<Image>().sprite = imageQuandMusiqueNonSélectionnée;
            Button button = nouvelleImage.AddComponent<Button>();
            var n = i;
            button.onClick.AddListener(() =>GetComponent<SystèmeMusiqueLecteurSac>().CliqueMusie(n));
            listeElementMusique.Add(nouvelleImage);
            nouvelleImage.transform.localScale = new Vector3(1,1,1);
        }
    }

    void CliqueMusie(int numéro){
        if (numéro<0 || numéro>=listeElementMusique.Count){
            Debug.Log("Erreur, dépassement de la limite avec le numéro");
            Debug.Log(numéro);
            return;
        }
        if (nomMusique != null) nomMusique.text = clips[numéro].name;
        GestionnaireMusique._Komasan.ChangerLaMusique(clips[numéro],true);
        for (int i = 0; i < listeElementMusique.Count; i++){
            listeElementMusique[i].GetComponent<Image>().sprite = imageQuandMusiqueNonSélectionnée;
        }
        listeElementMusique[numéro].GetComponent<Image>().sprite = imageQuandMusiqueSélectionnée;
    }
}
