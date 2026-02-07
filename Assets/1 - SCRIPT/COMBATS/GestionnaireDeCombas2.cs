using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;


public class GestionnaireDeCombas2 : MonoBehaviour{
    [Header("Données de débug")]
    public List<YoKaiPersonnage> yoKai = new List<YoKaiPersonnage>();
    private List<YoKaiPersonnage> yoKaiOrdreExécution = new List<YoKaiPersonnage>(); // se sont des référence au élément de yoKai
    private List<YoKaiPersonnage> yoKaiEnnemieEnVie = new List<YoKaiPersonnage>(); // se sont des référence au élément de yoKai
    [HideInInspector] public List<YoKaiPersonnage> yoKaiJoueurEnVie = new List<YoKaiPersonnage>(); // se sont des référence au élément de yoKai
    [HideInInspector] public List<YoKaiPersonnage> yoKaiJoueur = new List<YoKaiPersonnage>(); // se sont des référence au élément de yoKai

    public int vitesseChargeAmultime = 10;
    [Header("Pour les informations canevas")]
    public List<GameObject> blocInfoJoueur;
    public List<GameObject> blocInfoEnnemie;
    private List<TextMeshProUGUI> espaceNomJoueur = new List<TextMeshProUGUI>();
    private List<TextMeshProUGUI> espaceNomEnnemie = new List<TextMeshProUGUI>();
    private List<Image> imageBarreDeVieJoueur = new List<Image>();
    private List<Image> imageBarreDeVieEnnemie = new List<Image>();
    private List<Image> imageBarreDeAmultimeJoueur = new List<Image>();
    private List<Image> imageBarreDeAmultimeEnnemie = new List<Image>();
    public List<Transform> pointApparitionJoueur;
    public List<Transform> pointApparitionEnnemie;
    public List<Image> imageDesMédaillonDansLaRoue;
    public Transform blocRoueATourner;
    [Header("autres")]
    public bool finDuCombat;
    private bool leJoeurAGagner;
    private bool suite;
    private string nomDeLaScène;
    private Vector3 positionPersonnage;
    public FinDuJeu finDuJeu;
    private int rotationRoue;
    private bool top;


    void Start(){
        foreach (GameObject bloc in blocInfoJoueur){
            espaceNomJoueur.Add(bloc.transform.Find("nom").GetComponent<TextMeshProUGUI>());
            imageBarreDeVieJoueur.Add(bloc.transform.Find("barre de vie/IMAGE vie").GetComponent<Image>());
            imageBarreDeAmultimeJoueur.Add(bloc.transform.Find("amultime/IMAGE amultime").GetComponent<Image>());
        }
        foreach (GameObject bloc in blocInfoEnnemie){
            espaceNomEnnemie.Add(bloc.transform.Find("nom").GetComponent<TextMeshProUGUI>());
            imageBarreDeVieEnnemie.Add(bloc.transform.Find("barre de vie/IMAGE vie").GetComponent<Image>());
            imageBarreDeAmultimeEnnemie.Add(bloc.transform.Find("amultime/IMAGE amultime").GetComponent<Image>());
        }
        ComplétéLesPersonnage();
        StartCoroutine(BoucleDuJeu());
        RafraichireLeCanvas();
        CalculerVitesse();
    }
    void Update(){
        if (finDuCombat){
            if (Input.GetButtonDown("A")){
                if (!suite){
                    finDuJeu.RemplireLaFinDuJeu(leJoeurAGagner);
                    suite=true;
                }else{
                    finDuJeu.RetoursSurLeJeu(nomDeLaScène, positionPersonnage);
                }
            }
        }
        if (Input.GetButtonDown("L")) RotationDeLaRoue(-1);
        if (Input.GetButtonDown("R")) RotationDeLaRoue(1);
    }
    
    int Mod(int x, int m){
        return (x % m + m) % m;
    }

    void ComplétéLesPersonnage(){
        MesYoKai mesYoKai = MesYoKai.Instance.AvoirLeGameObject().GetComponent<MesYoKai>();
        medaliumDictionnaire MedaliumDictionnaire = medaliumDictionnaire._instance.AvoirLeGameObject().GetComponent<medaliumDictionnaire>();
        BaladeDeCombat Lili = FindObjectOfType<BaladeDeCombat>();
        foreach (Éléments item in Lili.listeYoKai){
            YoKaiPersonnage créationEnnemie = new YoKaiPersonnage();
            créationEnnemie.cesUnEnnemie = true;
            MedaliumNonPersonel y = MedaliumDictionnaire.ServantYoKai(item.ID);
            créationEnnemie.vieMax = y.PV;
            créationEnnemie.vieRestante = y.PV;
            créationEnnemie.force = y.Force;
            créationEnnemie.vitesse = y.Vitesse;
            créationEnnemie.essprit = y.Esprit;
            créationEnnemie.niveau = item.niveau;
            créationEnnemie.IDMédalium = item.ID;
            créationEnnemie.jaugeAmultime = 0;
            créationEnnemie.objetDansLaScène = medaliumDictionnaire._instance.InstantiateYoKai(y.ID.ToString());
            créationEnnemie.objetDansLaScène.transform.position = pointApparitionEnnemie[yoKaiEnnemieEnVie.Count].position;
            créationEnnemie.nom = y.Nom;
            créationEnnemie.médaille = y.médaillon;
            yoKai.Add(créationEnnemie);
            yoKaiEnnemieEnVie.Add(créationEnnemie);
        }
        for (int i = 0; i < mesYoKai.yoKaiDeMonEquipe.Count; i++){
            ListeDeMesYoKai infoYoaiAmi = mesYoKai.maListe[mesYoKai.yoKaiDeMonEquipe[i]];
            YoKaiPersonnage créationJoueur = new YoKaiPersonnage();
            créationJoueur.cesUnEnnemie = false;
            MedaliumNonPersonel y = MedaliumDictionnaire.ServantYoKai(infoYoaiAmi.ID);
            créationJoueur.vieMax = y.PV;
            créationJoueur.vieRestante = y.PV;
            créationJoueur.force = y.Force;
            créationJoueur.vitesse = y.Vitesse;
            créationJoueur.essprit = y.Esprit;
            créationJoueur.niveau = infoYoaiAmi.niveau;
            créationJoueur.IDMédalium = infoYoaiAmi.ID;
            créationJoueur.jaugeAmultime = 0;
            créationJoueur.objetDansLaScène = medaliumDictionnaire._instance.InstantiateYoKai(y.ID.ToString());
            créationJoueur.objetDansLaScène.transform.position = pointApparitionJoueur[yoKaiJoueurEnVie.Count].position;
            créationJoueur.nom = infoYoaiAmi.surnom;
            créationJoueur.médaille = y.médaillon;
            imageDesMédaillonDansLaRoue[i].sprite = créationJoueur.médaille;
            créationJoueur.positionActuel = i;
            yoKai.Add(créationJoueur);
            yoKaiJoueurEnVie.Add(créationJoueur);
            yoKaiJoueur.Add(créationJoueur);
        }
        nomDeLaScène = Lili.scèneOriginale;
        positionPersonnage = Lili.positionJoueur;
        Lili.Détruit();
    }
    public void RotationDeLaRoue(int rotation){//ajouter une nouvelle liste qui contien les référence à tous les yoKaiJoueur
        rotation = (int)Mathf.Sign(rotation);
        rotationRoue = AubtenirValeur(0, 6, rotationRoue, rotation);
        
        for (int nb = 0; nb < yoKaiJoueur.Count; nb++){
            yoKaiJoueur[nb].positionActuel=Mod(yoKaiJoueur[nb].positionActuel+rotation,6);
            StartCoroutine(MouvementYoKai(yoKaiJoueur[nb].objetDansLaScène, pointApparitionJoueur[yoKaiJoueur[nb].positionActuel], true));
        }
        List<int> positionAutoriser = new List<int>();
        positionAutoriser.Add(rotationRoue);
        positionAutoriser.Add(AubtenirValeur(0, 6, rotationRoue, 1));
        positionAutoriser.Add(AubtenirValeur(0, 6, rotationRoue, 2));

        foreach (YoKaiPersonnage item in yoKaiJoueur){
            if (positionAutoriser.Contains(item.positionActuel)) item.estSurLaScène = true;
            else item.estSurLaScène = false;
        }
        StartCoroutine(RotationFluide(blocRoueATourner, -yoKaiJoueur[0].positionActuel * (360/6), 0.25f));
        CalculerVitesse();
    }
    IEnumerator RotationFluide(Transform roue, float angleCible, float duree) {
        float temps = 0f;
        Quaternion rotationDepart = roue.rotation;
        Quaternion rotationCible = Quaternion.Euler(0, 0, angleCible);

        while (temps < duree) {
            temps += Time.deltaTime;
            roue.rotation = Quaternion.Slerp(rotationDepart, rotationCible, Mathf.SmoothStep(0,1,temps/duree));
            yield return null;
        }

        roue.rotation = rotationCible; // garantir l’angle final exact
    }

    int AubtenirValeur(int min, int max, int n, int ajout){
        n = (n+ajout)%max;
        if (n<min){
            n = (max+ajout)%max;//6+(-1) donne 5 et 5 est un nombre [0, 6[
        }
        return n;
    }
    IEnumerator MouvementYoKai(GameObject élément, Transform positionCible, bool animationFluide){
        Vector2 vectorCible = positionCible.position;
        float vitesse = 5f; // Valeur ajustée pour un mouvement fluide

        if (!animationFluide){
            élément.transform.position = vectorCible;
        }else{
            Vector2 depart = élément.transform.position;
            float distance = Vector2.Distance(depart, vectorCible);
            float duree = distance / vitesse;
            float t = 0f;

            while (t < 1f){
                t += Time.deltaTime / duree;
                élément.transform.position = Vector2.Lerp(depart, vectorCible, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            // Pour s'assurer que la position finale est exacte (éviter des arrêts approximatifs)
            élément.transform.position = vectorCible;
        }
    }

    void CalculerVitesse(){
        yoKaiOrdreExécution = yoKai.Where(y => y.estSurLaScène && y.vieRestante > 0).ToList();
        yoKaiOrdreExécution.Sort((a, b) => b.vitesse.CompareTo(a.vitesse));
    }
    IEnumerator FaitUnCoup(YoKaiPersonnage item){
        if (item.vieRestante > 0 && item.estSurLaScène){
            YoKaiPersonnage cible = null;
            if (item.cesUnEnnemie && yoKaiJoueurEnVie.Count != 0) cible = yoKaiJoueurEnVie[Random.Range(0,yoKaiJoueurEnVie.Count)];
            else if (yoKaiEnnemieEnVie.Count != 0) cible = yoKaiEnnemieEnVie[Random.Range(0,yoKaiEnnemieEnVie.Count)];
            if (cible != null && item != null){
                item.objetDansLaScène.GetComponent<Animator>().SetTrigger("attaque");
                yield return new WaitForSecondsRealtime(0.2f);
                cible.vieRestante-=item.force;
                item.jaugeAmultime+=vitesseChargeAmultime;
                if (cible.vieRestante <= 0){
                    cible.objetDansLaScène.GetComponent<Animator>().SetTrigger("mort");
                    if (yoKaiEnnemieEnVie.Contains(cible)) yoKaiEnnemieEnVie.Remove(cible);
                    if (yoKaiJoueurEnVie.Contains(cible)) yoKaiJoueurEnVie.Remove(cible);
                }else{
                    cible.objetDansLaScène.GetComponent<Animator>().SetTrigger("coup");
                }
                yield return new WaitForSecondsRealtime(1f);
                RafraichireLeCanvas();
                top = false;
            }
        }
    }
    IEnumerator BoucleDuJeu(){
        while (!finDuCombat){
            yield return new WaitForEndOfFrame();
            foreach (YoKaiPersonnage item in yoKaiOrdreExécution){
                while (top){
                    yield return new WaitForSeconds(0.1f);
                }
                top=true;
                StartCoroutine(FaitUnCoup(item));      
            }
        }
    }
    void RafraichireLeCanvas(){
        foreach (GameObject item in blocInfoJoueur){
            item.SetActive(false);
        }
        foreach (GameObject item in blocInfoEnnemie){
            item.SetActive(false);
        }
        int passéeJoueur = 0;
        int passéeEnnemie = 0;
        foreach (YoKaiPersonnage item in yoKai){
            if (item.cesUnEnnemie && item.estSurLaScène && blocInfoEnnemie.Count > passéeEnnemie){
                blocInfoEnnemie[passéeEnnemie].SetActive(true);
                espaceNomEnnemie[passéeEnnemie].text = item.nom;
                imageBarreDeVieEnnemie[passéeEnnemie].fillAmount = (float)item.vieRestante/item.vieMax;
                imageBarreDeAmultimeEnnemie[passéeEnnemie].fillAmount = item.jaugeAmultime/100f;
                passéeEnnemie++;
            }else if (!item.cesUnEnnemie && item.estSurLaScène && blocInfoJoueur.Count > passéeJoueur){
                blocInfoJoueur[passéeJoueur].SetActive(true);
                espaceNomJoueur[passéeJoueur].text = item.nom;
                imageBarreDeVieJoueur[passéeJoueur].fillAmount = (float)item.vieRestante/item.vieMax;
                imageBarreDeAmultimeJoueur[passéeJoueur].fillAmount = item.jaugeAmultime/100f;
                passéeJoueur++;
            }
        }
        if (yoKaiJoueurEnVie.Count==0 || yoKaiEnnemieEnVie.Count==0){
            finDuCombat = true;
            leJoeurAGagner = yoKaiEnnemieEnVie.Count == 0;
            finDuJeu.panneauDeVictoire1.SetActive(true);
        }
    }

    public void UtiliseAmultime(int position){
        foreach (YoKaiPersonnage item in yoKaiJoueurEnVie){
            if (item.positionActuel==position){
                item.force *= 2;
                StartCoroutine(FaitUnCoup(item)); 
                item.force /= 2;
                Debug.Log("Amultime lancer");
            }
        }
    }
}

[System.Serializable]
public class YoKaiPersonnage{
    public bool cesUnEnnemie;
    public bool estSurLaScène = true;
    [Min(0)] public int vieMax;
    [Min(0)] public int vieRestante;
    [Min(0)] public int force;
    [Min(0)] public int vitesse;
    [Min(0)] public int essprit;
    [Min(0)] public int niveau;
    [Min(0)] public int IDMédalium;
    [Range(0,100)] public int jaugeAmultime;
    public GameObject objetDansLaScène;
    public string nom;
    public Sprite médaille;
    public int positionActuel=0;
}


[System.Serializable]
public class FinDuJeu{
    public int score;
    public int argent;
    public TextMeshProUGUI texteScore;
    public TextMeshProUGUI texteArgent;
    public GameObject blocDeFinDuJeux;
    public GameObject panneauDeVictoire1;
    public GameObject panneauDeDéfaite1;
    public GameObject panneauDeVictoire2;
    public GameObject panneauDeDéfaite2;

    public void RemplireLaFinDuJeu(bool leJoeurAGagner){
        blocDeFinDuJeux.SetActive(true);
        texteScore.text = score.ToString();
        texteArgent.text = argent.ToString();

        panneauDeVictoire2.SetActive(leJoeurAGagner);
        panneauDeDéfaite2.SetActive(!leJoeurAGagner);
    }
    public void RetoursSurLeJeu(string nomDeLaScène, Vector3 positionPersonnage){
        if (nomDeLaScène != ""){
            GameObject TV = new GameObject("livraison TP");
            LivraisonTp tp = TV.AddComponent<LivraisonTp>();
            tp.positionPersonnage = positionPersonnage;
            tp.nomDeLaScèneCible = nomDeLaScène;
            SceneManager.LoadScene(nomDeLaScène);
        }else{
            Debug.LogWarning("IL N'Y A PAS DE SCENNE DE téléportation pour retourner sur le jeu");
        }
    }
}
