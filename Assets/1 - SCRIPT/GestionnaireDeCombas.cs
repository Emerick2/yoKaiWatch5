// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.Linq;
// using UnityEditor;
// using System;
// using TMPro;
// using UnityEngine.SceneManagement;

// //TV.transform.localScale = new Vector3(100, 100, 100);

// public class GestionnaireDeCombas : MonoBehaviour{
//     private BaladeDeCombat Lili;
//     public LivraisonPrivée Livraison;

//     private bool suite = false;
//     private string nomDeLaScène;
//     private Vector3 positionPersonnage;

//     public FinDuJeu finDuJeu;
//     public Transform flèche;
//     public List<Image> médaillon;
//     [HideInInspector]public bool finDuCombat=false;
//     [HideInInspector]public bool leJoeurAGagner=false;
//     private medaliumDictionnaire MedaliumDictionnaire;
//     private MesYoKai mesYoKai;
//     public List<int> IDDesYoKaiDeEquipe = new List<int>();//identifient unique "mesYoKai"
//     public Transform[] positionJ;
//     public Transform[] positionE;
//     public Gestionnaire g;
//     private float tempsAttanteEntreLesTours=1.5f;
//     int random;
//     private int numéroDuTour = 0;
    
//     public List<(int vitesse, Personnage source, string origine, int index)> listeVitesse = new();
//     public List<Personnage> ennemieEnEtatDeCombat = new();
//     public List<Personnage> joueurEnEtatDeCombat = new();

//     void Start(){
//         mesYoKai = MesYoKai.Instance.AvoirLeGameObject().GetComponent<MesYoKai>();
//         MedaliumDictionnaire = medaliumDictionnaire._instance.AvoirLeGameObject().GetComponent<medaliumDictionnaire>();
//         Lili = FindObjectOfType<BaladeDeCombat>();
//         foreach (Éléments item in Lili.listeYoKai){
//             UneDeMesLivraison liv = new UneDeMesLivraison();
//             liv.ID = item.ID;
//             liv.niveau = item.niveau;
//             Livraison.numéroDesYoKaiPourLeCombat.Add(liv);
//         }
//         Livraison.nomDeLaScène = Lili.scèneOriginale;
//         Livraison.positionPersonnage = Lili.positionJoueur;
//         Lili.Détruit();
//         if (g == null){
//             Debug.LogError("Le Gestionnaire n'est pas assigné !");
//             return;
//         }
//         g.Initialiser();
//         //print("ici b2");
//         StartCoroutine(start());
//     }

//     IEnumerator start(){
//         while (mesYoKai.yoKaiDeMonEquipeChargé == false || mesYoKai.finDuChargement == false){
//             yield return new WaitForSeconds(0.3f);
//         }
//         foreach (int item in mesYoKai.yoKaiDeMonEquipe){
//             IDDesYoKaiDeEquipe.Add(item);
//         }
//         MedaliumDictionnaire = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
//         chargementDesPersonnage();
//         StartCoroutine(boucleDuJeu());
//         if (g.E.Count == 1) {g.blocDeVieE[1].SetActive(false);g.blocDeVieE[2].SetActive(false);}
//         if (g.E.Count == 2) {g.blocDeVieE[2].SetActive(false);}

//         if (g.J.Count == 1) {g.blocDeVieJ[1].SetActive(false);g.blocDeVieJ[2].SetActive(false);}
//         if (g.J.Count == 2) {g.blocDeVieJ[2].SetActive(false);}
//         for (int i = 0; i < g.E.Count; i++){
//             g.nomTexteE[i].text = g.E[i].nom;
//         }
//         /*for (int i = 0; i < IDDesYoKaiDeEquipe.Count ; i++){
//             médaillon[i].sprite = MedaliumDictionnaire.dictionnaire[IDDesYoKaiDeEquipe[i]].médaillon;
//         }*/
//     }

//     void chargementDesPersonnage(){
//         //joueur :
//         for (int i = 0; i < IDDesYoKaiDeEquipe.Count; i++){
//             if (i >= médaillon.Count){
//                 Debug.LogError($"Pas assez de médaillons pour l'index {i}. médaillon.Count = {médaillon.Count}");
//                 continue;
//             }
//             /*if (MedaliumDictionnaire.dictionnaire.Count < ID){
//                 Debug.LogError($"ID {ID} n'existe pas dans le MedaliumDictionnaire");
//                 continue;
//             }*/

//             ListeDeMesYoKai ici = MesYoKai.Instance.maListe[IDDesYoKaiDeEquipe[i]];
//             int ID = ici.ID;
//             médaillon[i].sprite = MedaliumDictionnaire.dictionnaire[ID].médaillon;
//             if (MedaliumDictionnaire.dictionnaire.Count <= ID){Debug.Log("ERREUR, le médalium n'est pas asser grand");}
//             GameObject TV = Instantiate(MedaliumDictionnaire.dictionnaire[ID].yoKai);
            
//             var script = TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>();
//             script.IDduYoKaiMedalium = ID;
//             script.IDduYoKaiSpécifique = IDDesYoKaiDeEquipe[i];
//             Personnage nouveau = new Personnage {
//                 objet = TV,
//                 script = script,
//                 vie = 100,//revenir ici quand je sovgarderais les vie
//                 positionActuel = i,
//                 nom = ici.surnom
//             };
//             g.J.Add(nouveau);
//             TV.transform.position = new Vector3(positionJ[i].position.x,positionJ[i].position.y,positionJ[i].position.z);
//             //g.nom[i].text=NewDataCenter.Instance.Lire(IDDesYoKaiDeEquipe[i].ToString()+"_surnom");
//             //g.nomTexteJ[i].text=g.J[i].nom;
//             TV.transform.name = TV.transform.name.Replace("(Clone)", "");
//         }
//         //ennemie :
//         if (Livraison == null){
//             Debug.LogWarning("aucun livreur n'est passer !");
//             return;
//         }
//         nomDeLaScène = Livraison.nomDeLaScène;
//         positionPersonnage = Livraison.positionPersonnage;//new Vector3(Livraison.nomDeLaScène.x,Livraison.nomDeLaScène.y,Livraison.nomDeLaScène.z);

//         for (int i = 0; i < Livraison.numéroDesYoKaiPourLeCombat.Count; i++){
//             int ID = Livraison.numéroDesYoKaiPourLeCombat[i].ID;
//             if (MedaliumDictionnaire.dictionnaire.Count <= ID){Debug.Log("ERREUR, le médalium n'est pas asser grand");}
//             GameObject TV = Instantiate(MedaliumDictionnaire.dictionnaire[ID].yoKai);
//             var script = TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>();
//             script.niveau=Livraison.numéroDesYoKaiPourLeCombat[i].niveau;
//             script.IDduYoKaiMedalium = ID;
//             Personnage nouveau = new Personnage {
//                 objet = TV,
//                 script = script,
//                 vie = 100,
//                 nom = MedaliumDictionnaire.dictionnaire[ID].nom
//             };
//             g.E.Add(nouveau);
//             TV.transform.position = new Vector3(positionE[i].position.x,positionE[i].position.y,positionE[i].position.z);
//             Vector3 rotation = TV.transform.eulerAngles;
//             rotation.y += 180;
//             TV.transform.eulerAngles = rotation;
//             TV.transform.name = TV.transform.name.Replace("(Clone)", "");
//         }
//     }
//     IEnumerator MouvementYoKai(GameObject élément, Transform positionCible, bool animationFluide){
//         Vector2 vectorCible = positionCible.position;
//         float vitesse = 5f; // Valeur ajustée pour un mouvement fluide

//         if (!animationFluide){
//             élément.transform.position = vectorCible;
//         }else{
//             Vector2 depart = élément.transform.position;
//             float distance = Vector2.Distance(depart, vectorCible);
//             float duree = distance / vitesse;
//             float t = 0f;

//             while (t < 1f){
//                 t += Time.deltaTime / duree;
//                 élément.transform.position = Vector2.Lerp(depart, vectorCible, Mathf.SmoothStep(0, 1, t));
//                 yield return null;
//             }

//             // Pour s'assurer que la position finale est exacte (éviter des arrêts approximatifs)
//             élément.transform.position = vectorCible;
//         }
//     }


//     int Mod(int x, int m){
//         return (x % m + m) % m;
//     }

//     public void RotationDeLaRoue(int mouvement){
//         mouvement = Math.Sign(mouvement);

//         for (int nb = 0; nb < g.J.Count; nb++){
//             g.J[nb].positionActuel=Mod(g.J[nb].positionActuel+mouvement,6);
//             StartCoroutine(MouvementYoKai(g.J[nb].objet, positionJ[g.J[nb].positionActuel], true));
//         }
//         g.numéroPrésentDevant = Mod(g.numéroPrésentDevant + mouvement, 6);
//         CalculerLaVitesse();
//         Rafraichire();
//     }
//     List<int> GetPlageCirculaire(int start, int longueur, int tailleMaxIncluse) {
//         List<int> result = new List<int>();
//         for (int i = 0; i < longueur; i++) {
//             result.Add((start + i) % (tailleMaxIncluse + 1));
//         }
//         return result;
//     }


//     void CalculerLaVitesse(){
//         listeVitesse.Clear();
//         joueurEnEtatDeCombat.Clear();
//         ennemieEnEtatDeCombat.Clear();
//         int borneMin = g.numéroPrésentDevant;//Mod(g.numéroPrésentDevant - 3, 6);
//         List<int> plage = GetPlageCirculaire(borneMin, 3, 6); // récupère 3 positions à partir de borneMin dans un espace de 0 à 6
//         for (int i = 0; i < g.J.Count; i++){
//             int pos = g.J[i].positionActuel;
//             if (g.J[i] != null && g.J[i].vie > 0 && plage.Contains(pos)){
//                 listeVitesse.Add((g.J[i].script.vitesse, g.J[i], "g.J", i));
//                 joueurEnEtatDeCombat.Add(g.J[i]);
//             }
//         }
//         for (int i = 0; i < g.E.Count; i++){
//             int pos = g.E[i].positionActuel;
//             if (g.E[i] != null && g.E[i].vie > 0){
//                 listeVitesse.Add((g.E[i].script.vitesse, g.E[i], "g.E", i));
//                 ennemieEnEtatDeCombat.Add(g.E[i]);
//             }
//         }
//         listeVitesse.Sort((a, b) => b.vitesse.CompareTo(a.vitesse));
//     }



//     IEnumerator boucleDuJeu(){//g.J[0].vie  g.E[0].vie
//         RotationDeLaRoue(0);
//         Personnage lanceur = null;
//         Personnage cible = null;
//         while (!finDuCombat){
//             numéroDuTour++;
//             System.Random rng = new System.Random();
//             int STOP = 1000;
//             for (int i = 0; i < listeVitesse.Count; i++){
//                 cible = null;
//                 lanceur = listeVitesse[i].source;
//                 //print(listeVitesse[i].origine);
//                 if (listeVitesse[i].origine[2] == 'J'){
//                     //print("tour du joueur");
//                     if (lanceur.vie > 0 && lanceur.numéroDuDernierTourJouer < numéroDuTour){
//                         lanceur.numéroDuDernierTourJouer = numéroDuTour;
//                         cible = ennemieEnEtatDeCombat[UnityEngine.Random.Range(0,ennemieEnEtatDeCombat.Count)];
//                         flèche.position = new Vector2(cible.objet.transform.position.x,cible.objet.transform.position.y+4);
//                     }
//                 }else{//c'est un 'E'
//                     //print("tour de l'ennemie");
//                     if (lanceur.vie > 0 && lanceur.numéroDuDernierTourJouer < numéroDuTour){
//                         lanceur.numéroDuDernierTourJouer = numéroDuTour;
//                         cible = joueurEnEtatDeCombat[UnityEngine.Random.Range(0,joueurEnEtatDeCombat.Count)];
//                         flèche.position = new Vector2(cible.objet.transform.position.x,cible.objet.transform.position.y+1);
//                     }
//                 }
//                 if (cible != null){
//                     annimation(lanceur.objet, "attaque");
//                     yield return new WaitForSecondsRealtime(tempsAttanteEntreLesTours);
//                     cible.vie -= calculerDégâts(lanceur.script, cible.script);
//                     //print(calculerDégâts(lanceur.script, cible.script));
//                     if (cible.vie <= 0) annimation(cible.objet, "mort");
//                     else annimation(cible.objet, "coup");
//                     RafraichireLeCanvas();
//                     vérifieSiUnJoueurEstEnVie();
//                 }
//                 //print(cible+" : "+lanceur);
//             }
//             yield return new WaitForSeconds(0.1f);
//         }
//     }
//     int calculerDégâts(gestionnaireDeMonPetitYoKaiPourUnCombat lanceur, gestionnaireDeMonPetitYoKaiPourUnCombat cible){
//         if (lanceur == null || cible == null){
//             Debug.LogError("Lanceur ou cible est null dans calculerDégâts !");
//             return 0;
//         }
//         float total = ((float)lanceur.dégatInfligé/(float)cible.pointDeVie)*100;
//         //Debug.Log($"{lanceur.nom} inflige {total} à {cible.nom} !");
//         return (int)(total);
//     }

//     void RafraichireLeCanvas(){
//         /*----- joueur -----*/
//         int minCount = Mathf.Min(g.J_Vie.Count, g.J.Count);
//         for (int i = 0; i < minCount; i++){
//             if (Mod(g.numéroPrésentDevant+i,6) < g.J.Count){
//                 g.J[Mod(g.numéroPrésentDevant+i,6)].amûltime += 5;
//             }
//         }
        
//         minCount = Mathf.Min(g.E_Vie.Count, g.E.Count);
//         for (int i = 0; i < minCount; i++){
//             g.E_Vie[i].value = g.E[i].vie;
//         }

//         /*----- ennemie -----*/


//         Rafraichire();
//     }
//     void Rafraichire(){
//         for (int i = 0; i < g.J_Vie.Count; i++){
//             int idx = Mod(g.numéroPrésentDevant+i, g.J.Count);
//             if (i < g.J.Count && i < g.barreDeAmultimeJ.Count && i < g.nomTexteJ.Count) {
//                 g.J_Vie[i].value = g.J[idx].vie;
//                 g.J[idx].amûltime += 5;
//                 g.barreDeAmultimeJ[i].value = g.J[idx].amûltime;
//                 g.nomTexteJ[i].text = g.J[idx].nom;
//             } else {
//                 Debug.LogWarning($"Index {i} hors limites dans Rafraichire : g.J.Count={g.J.Count}, g.barreDeAmultimeJ.Count={g.barreDeAmultimeJ.Count}, g.nomTexteJ.Count={g.nomTexteJ.Count}");
//             }
//         }
//     }

//     void annimation(GameObject objet, string nom){//coup, mort, attaque
//         if (objet.GetComponent<Animator>() != null && objet != null){
//             objet.GetComponent<Animator>().SetTrigger(nom);
//         }else{
//             Debug.LogWarning("Attention l'annimator du joueur avec l'ID"+objet+"du gestionnaire de combat n'est pas activé et donc il ne peut pas jouer son annimation de "+nom);
//         }
//     }
//     void vérifieSiUnJoueurEstEnVie(){
//         int conteur = 0;
//         for (int i = 0; i < g.E.Count; i++){
//             if (g.E[i].vie <=0){
//                 conteur++;
//                 if (ennemieEnEtatDeCombat.Contains(g.E[i])){
//                     ennemieEnEtatDeCombat.Remove(g.E[i]);
//                 }
//             }
//         }
//         if (conteur == g.E.Count){
//             print("fin de la partie, le joueur à gagner");
//             finDuCombat = true;
//             leJoeurAGagner = true;
//             finDuJeu.panneauDeVictoire1.SetActive(true);
//         }
//         conteur = 0;
//         for (int i = 0; i < g.J.Count; i++){
//             if (g.J[i].vie <=0){
//                 conteur++;
//                 if (joueurEnEtatDeCombat.Contains(g.J[i])){
//                     joueurEnEtatDeCombat.Remove(g.J[i]);
//                 }
//             }
//         }
//         if (conteur == g.J.Count){
//             print("fin de la partie, l'ennemie à gagner");
//             finDuCombat = true;
//             leJoeurAGagner = false;
//             finDuJeu.panneauDeDéfaite1.SetActive(true);
//         }
//     }
//     void Update(){
//         if (finDuCombat){
//             if (Input.GetButtonDown("A")){
//                 if (!suite){
//                     finDuJeu.RemplireLaFinDuJeu(leJoeurAGagner);
//                     suite=true;
//                 }else{
//                     finDuJeu.RetoursSurLeJeu(nomDeLaScène, positionPersonnage);
//                 }
//             }
//         }
//     }
// }

// [System.Serializable]
// public class LivraisonPrivée{
//     public List<UneDeMesLivraison> numéroDesYoKaiPourLeCombat = new List<UneDeMesLivraison>();//le premier est l'ID et le deuxième son niveau.
//     public string nomDeLaScène;
//     public Vector3 positionPersonnage;
// }


// [System.Serializable]
// public class FinDuJeu{
//     public int score;
//     public int argent;
//     //public List<Objet> objet = new List<Objet>(); // à mettre quand les objets seront référencer
//     public TextMeshProUGUI texteScore;
//     public TextMeshProUGUI texteArgent;
    
//     public GameObject blocDeFinDuJeux;
//     public GameObject panneauDeVictoire1;
//     public GameObject panneauDeDéfaite1;

//     public GameObject panneauDeVictoire2;
//     public GameObject panneauDeDéfaite2;

//     public void RemplireLaFinDuJeu(bool leJoeurAGagner){
//         blocDeFinDuJeux.SetActive(true);
//         texteScore.text = score.ToString();
//         texteArgent.text = argent.ToString();

//         panneauDeVictoire2.SetActive(leJoeurAGagner);
//         panneauDeDéfaite2.SetActive(!leJoeurAGagner);
//     }
//     public void RetoursSurLeJeu(string nomDeLaScène, Vector3 positionPersonnage){
//         if (nomDeLaScène != ""){
//             GameObject TV = new GameObject("livraison TP");
//             LivraisonTp tp = TV.AddComponent<LivraisonTp>();
//             tp.positionPersonnage = positionPersonnage;
//             tp.nomDeLaScèneCible = nomDeLaScène;
//             SceneManager.LoadScene(nomDeLaScène);
//         }else{
//             Debug.LogWarning("IL N'Y A PAS DE SCENNE DE téléportation pour retourner sur le jeu");
//         }
//     }
// }




// [System.Serializable]
// public class Gestionnaire{
//     /*[HideInInspector]*/ public List<Personnage> J = new List<Personnage>();
//     /*[HideInInspector]*/ public List<Personnage> E = new List<Personnage>();
//     [HideInInspector] [Range(0,5)]public int numéroPrésentDevant = 0;//se ne sont pas les id mais les numéro de 0 à 5
//     //si c'est 3 se sera |3,4,5|
//     public List<GameObject> blocDeVieJ;
//     public List<GameObject> blocDeVieE;
//     [HideInInspector] public List<Text> nomTexteJ = new List<Text>();
//     [HideInInspector] public List<Text> nomTexteE = new List<Text>();
//     /*[HideInInspector]*/ public List<SliderValue> J_Vie = new List<SliderValue>();
//     /*[HideInInspector]*/ public List<SliderValue> E_Vie = new List<SliderValue>();
//     /*[HideInInspector]*/ public List<SliderValue> barreDeAmultimeJ = new List<SliderValue>();
//     /*[HideInInspector]*/ public List<SliderValue> barreDeAmultimeE = new List<SliderValue>();

//     public void Initialiser(){
//         //barre de vie _ SLIDER + masque
//         //amultime _ SLIDER + masque
//         //nom
//         foreach (GameObject item in blocDeVieJ){
//             nomTexteJ.Add(item.transform.Find("nom").GetComponent<Text>());
//             //J_Vie.Add(item.transform.Find("barreDeVie_SLIDER+masque").GetComponent<SliderValue>());
//             //barreDeAmultimeJ.Add(item.transform.Find("amultime/amultime_SLIDER+masque").GetComponent<SliderValue>());
//         }
//         foreach (GameObject item in blocDeVieE){
//             nomTexteE.Add(item.transform.Find("nom").GetComponent<Text>());
//             //E_Vie.Add(item.transform.Find("barre de vie/barreDeVie_SLIDER+masque").GetComponent<SliderValue>());
//             //barreDeAmultimeE.Add(item.transform.Find("amultime/amultime_SLIDER+masque").GetComponent<SliderValue>());
//         }
//     }
// }

// [System.Serializable]
// public class Personnage{
//     public GameObject objet;
//     public int vie;
//     public int amûltime;
//     public string nom;
//     [Range(0,5)]public int positionActuel;
//     public gestionnaireDeMonPetitYoKaiPourUnCombat script;
//     public int numéroDuDernierTourJouer = 0;
// }