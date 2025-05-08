using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class montre : MonoBehaviour{
    [Range(0,100)] public int valeur;
    public Image image;
    public Image rangImage;
    public GameObject blocRang;
    public medaliumDictionnaire médalium;

    public string rang;
    private List<GameObject> boîte = new List<GameObject>();
    private GameObject boîteConcerner;
    public PersonnageAléatoireDeLaZonne personageAléatoireDeLaZonne;
    private GameObject joueur;
    private int indiceActuelleDeEguille = -1;

    public Transform aiguille;
    private float vitesseRotation = 200f;

    private List<float> rotations1 = new List<float> {80f, 46f, 14f, -15f, -45f, -75f, -106f, -145f};
    private List<float> rotations = new List<float> {-145f, -106f, -75f, -45f, -15f, 14f, 46f, 80f};

    private Quaternion cibleRotation;
    private bool aiguilleDoitTourner = false;
    public Animator animator;

    void Start() {
        if (personageAléatoireDeLaZonne == null) {
            Debug.LogError("personageAléatoireDeLaZonne n'est pas assigné dans l'inspecteur !");
            return;
        }
        joueur = GameObject.FindWithTag("Player");
        trouverUnYoKai[] composantsTrouvés = FindObjectsOfType<trouverUnYoKai>();
        foreach (trouverUnYoKai composant in composantsTrouvés) {
            if (composant.leLieuEstVide == false){
                boîte.Add(composant.gameObject);
            }
        }
        if (boîte.Count == 0) {
            Debug.Log("Aucun enfant trouvé avec le script trouverUnYoKai.");
        }
    }

    void Update(){
        float total = 1000;
        float totalTemps = 0;
        for (int i = 0; i < boîte.Count ; i++){
            totalTemps = distance(joueur.transform, boîte[i].transform);
            if (totalTemps<total){
                total=totalTemps;
                boîteConcerner = boîte[i];
            }
        }
        valeur = (int)(100-total);
        FonctionnementDeEguille();
        if (aiguilleDoitTourner) {
            aiguille.rotation = Quaternion.RotateTowards(aiguille.rotation, cibleRotation, vitesseRotation * Time.deltaTime);
            if (Quaternion.Angle(aiguille.rotation, cibleRotation) < 0.1f) {
                aiguille.rotation = cibleRotation;
                aiguilleDoitTourner = false;
            }
        }
    }
    float distance(Transform maPosition, Transform ciblePosition){
        return Vector3.Distance(maPosition.position, ciblePosition.position)-3;
    }

    void FonctionnementDeEguille(){
        int n = rotations.Count;
        while (n<10){
            rotations.Insert(0,rotations[0]);
            n = rotations.Count;
        }

        valeur = Mathf.Clamp(valeur, 0, 100);
        int T = 100 - valeur;

        if (indiceActuelleDeEguille != T){
            indiceActuelleDeEguille = T;
            if (indiceActuelleDeEguille >= 0 && indiceActuelleDeEguille < rotations.Count) {
                float angle = rotations[indiceActuelleDeEguille];
                cibleRotation = Quaternion.Euler(0, 0, angle);
                aiguilleDoitTourner = true;
            }else{
                Debug.LogWarning("Rotation cible manquante pour la valeur " + indiceActuelleDeEguille);
            }
        }
        if (valeur >= 98){
            animator.enabled = true;
            animator.SetBool("onEstSurCent",true);
        }else{
            animator.enabled = false;
            image.color = new Color(1f, 1f, 1f, 1f);
        }
        if (valeur >= 92){
            blocRang.SetActive(true);
            string r = boîteConcerner.GetComponent<trouverUnYoKai>().rang;
            if (r != ""){
                rangImage.sprite = médalium.RecevoirUnSpriteDeRang(r);
            }
        }else{
            blocRang.SetActive(false);
        }
    }
}
