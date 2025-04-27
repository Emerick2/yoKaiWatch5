using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;


public class GestionnaireDeCombas : MonoBehaviour{
    public Transform flèche;
    public List<Image> médaillon;
    public GameObject panneauDeVictoire;
    public GameObject panneauDeDéfaite;
    [HideInInspector]public bool finDuCombat=false;
    [HideInInspector]public bool leJoeurAGagner=false;
    public RuntimeAnimatorController animController;
    private medaliumDictionnaire MedaliumDictionnaire;
    public int[] IDDesYoKaiDeEquipe;//identifient unique "mesYoKai"
    public Transform[] positionJ;
    public Transform[] positionE;
    public Gestionnaire g;
    private float tempsAttanteEntreLesTours=1.5f;
    int random;
    private int numéroDuTour = 0;
    
    public List<(int vitesse, Personnage source, string origine, int index)> listeVitesse = new();

    void Start(){
        MedaliumDictionnaire = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        chargementDesPersonnage();
        StartCoroutine(boucleDuJeu());
        for (int i = 0; i < IDDesYoKaiDeEquipe.Length ; i++){
            médaillon[i].sprite = MedaliumDictionnaire.dictionnaire[IDDesYoKaiDeEquipe[i]].médaillon;
        }
    }

    void chargementDesPersonnage(){
        //joeur :
        for (int i = 0; i < IDDesYoKaiDeEquipe.Length; i++){
            int ID = MesYoKai.Instance.maListe[IDDesYoKaiDeEquipe[i]].ID;
            if (MedaliumDictionnaire.dictionnaire.Count <= ID){Debug.Log("ERREUR, le médalium n'est pas asser grand");}
            GameObject TV = Instantiate(MedaliumDictionnaire.dictionnaire[ID].yoKai);
            
            var script = TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>();
            script.IDduYoKaiMedalium = ID;
            script.IDduYoKaiSpécifique = IDDesYoKaiDeEquipe[i];
            Personnage nouveau = new Personnage {
                objet = TV,
                script = script,
                vie = 100,//revenir ici quand je sovgarderais les vie
                positionActuel = i
            };
            g.J.Add(nouveau);

            TV.transform.localScale = new Vector3(100, 100, 100);
            TV.transform.position = new Vector3(positionJ[i].position.x,positionJ[i].position.y,positionJ[i].position.z);
            Vector3 rotation = TV.transform.eulerAngles;
            rotation.y += 180;
            TV.transform.eulerAngles = rotation;
            g.nom[i].text=NewDataCenter.Instance.Lire(IDDesYoKaiDeEquipe[i].ToString()+"_surnom");
            TV.AddComponent<Animator>().runtimeAnimatorController = animController;
            TV.transform.name = TV.transform.name.Replace("(Clone)", "");
            CrééMonAnimator(TV.GetComponent<Animator>(), TV.transform.name, TV);
        }
        //ennemie :
        livraison Livraison = GameObject.Find("livraison").GetComponent<livraison>();
        for (int i = 0; i < Livraison.numéroDesYoKaiPourLeCombat.Count; i++){
            Instantiate(MedaliumDictionnaire.dictionnaire[Livraison.numéroDesYoKaiPourLeCombat[i].ID].yoKai);
            //Livraison.numéroDesYoKaiPourLeCombat[i].ID

            int ID = Livraison.numéroDesYoKaiPourLeCombat[i].ID;
            if (MedaliumDictionnaire.dictionnaire.Count <= ID){Debug.Log("ERREUR, le médalium n'est pas asser grand");}
            //g.E.Add(MedaliumDictionnaire.dictionnaire[ID].yoKai);
            GameObject TV = Instantiate(MedaliumDictionnaire.dictionnaire[ID].yoKai);
            
            var script = TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>();
            script.niveau=Livraison.numéroDesYoKaiPourLeCombat[i].niveau;
            script.IDduYoKaiMedalium = ID;
            Personnage nouveau = new Personnage {
                objet = TV,
                script = script,
                vie = 100
            };
            g.E.Add(nouveau);


            TV.transform.localScale = new Vector3(200, 200, 200);
            TV.transform.position = new Vector3(positionE[i].position.x,positionE[i].position.y,positionE[i].position.z);
            TV.AddComponent<Animator>().runtimeAnimatorController = animController;
            TV.transform.name = TV.transform.name.Replace("(Clone)", "");
            CrééMonAnimator(TV.GetComponent<Animator>(), TV.transform.name, TV);
        }
        Destroy(GameObject.Find("livraison"));
    }
    
    void CrééMonAnimator(Animator animator, string yoKaiName, GameObject modèle3DDuYoKai){
        Animator modeleAnimator = modèle3DDuYoKai.GetComponent<Animator>();
        RuntimeAnimatorController animController = modeleAnimator.runtimeAnimatorController;
        AnimationClip[] clips = animController.animationClips;
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
        string nomDuYoKaiDeAnimatorParDéfaut = "y105010";
        List<string> aRemplacer = new List<string>{
            "_p20_27001d_del",
            "_p20_27000d_dei",
            "_p20_26500d_dmi",
            "_p20_25000s_ati",
            "_p20_21001d_stl",
            "_p20_21001d_stl_1"
        };

        // Dictionnaire pour stocker les correspondances entre anciennes et nouvelles animations
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        for (int i = 0; i < aRemplacer.Count; i++){
            string suffixe = aRemplacer[i];
            string nomAnimationOrigine = nomDuYoKaiDeAnimatorParDéfaut + suffixe;

            // Trouve l'animation existante dans le contrôleur d'origine
            AnimationClip clipOrigine = clips.FirstOrDefault(c => c.name == nomAnimationOrigine);

            // Charge la nouvelle animation dans le projet
            string nomAnimationRemplacement = yoKaiName + suffixe;
            AnimationClip clipRemplacement = Resources.Load<AnimationClip>("AnimationsExtraites/" + yoKaiName + "/" + nomAnimationRemplacement);
            //print (clipRemplacement.ToString());
            if (clipOrigine != null && clipRemplacement != null){
                AnimationClip clip = clipRemplacement;
                SerializedObject serializedClip = new SerializedObject(clip);
                serializedClip.FindProperty("m_AnimationClipSettings.m_LoopTime").boolValue = true;
                serializedClip.ApplyModifiedProperties();
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clipOrigine, clipRemplacement));
            }else{
                Debug.LogWarning($"Animation introuvable : {nomAnimationOrigine} ou {nomAnimationRemplacement}");
            }
        }
        overrideController.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = overrideController;
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


    int Mod(int x, int m){
        return (x % m + m) % m;
    }

    public void RotationDeLaRoue(int mouvement){
        mouvement = Math.Sign(mouvement);

        for (int nb = 0; nb < g.J.Count; nb++){
            g.J[nb].positionActuel=Mod(g.J[nb].positionActuel+mouvement,6);
            StartCoroutine(MouvementYoKai(g.J[nb].objet, positionJ[g.J[nb].positionActuel], true));
            /*if (g.J[nb].positionActuel < 5){
                StartCoroutine(MouvementYoKai(g.J[nb].objet, positionJ[g.J[nb].positionActuel], true));
            }else{
                StartCoroutine(MouvementYoKai(g.J[nb].objet, positionJ[g.J[nb].positionActuel], false));
            }*/
        }
        g.numéroPrésentDevant = Mod(g.numéroPrésentDevant + mouvement, 6);
        CalculerLaVitesse();
    }

    void CalculerLaVitesse(){
        listeVitesse.Clear();
        for (int i = 0; i < g.J.Count; i++){
            if (g.J[i] != null){
                listeVitesse.Add((g.J[i].script.vitesse, g.J[i], "g.J", i));
            }
        }

        for (int i = 0; i < g.E.Count; i++){
            if (g.E[i] != null){
                listeVitesse.Add((g.E[i].script.vitesse, g.E[i], "g.E", i));
            }
        }
        listeVitesse.Sort((a, b) => b.vitesse.CompareTo(a.vitesse));
    }



    IEnumerator boucleDuJeu(){//g.J[0].vie  g.E[0].vie
        RotationDeLaRoue(0);
        Personnage lanceur = null;
        Personnage cible = null;
        while (!finDuCombat){
            numéroDuTour++;
            System.Random rng = new System.Random();
            int STOP = 1000;
            //ne plus être dans un for parce que cela bloque le retournement de la roue
            //int nombreDeTour = g.J.Count+g.E.Count;
            for (int i = 0; i < listeVitesse.Count; i++){//six fois parce que 3 pour les ennemie et trois pour les joueur
                cible = null;
                lanceur = listeVitesse[i].source;
                if (listeVitesse[i].source.ToString()[2] == 'J'){
                    if (lanceur.vie > 0 && lanceur.numéroDuDernierTourJouer < numéroDuTour){
                        lanceur.numéroDuDernierTourJouer = numéroDuTour;
                        int cibleNuméro = rng.Next(0, g.E.Count-1);
                        while (g.E[cibleNuméro].vie > 0 && STOP > 0){
                            cibleNuméro = rng.Next(0, g.E.Count-1);
                            STOP--;
                        }
                        cible = g.E[cibleNuméro];
                        flèche.position = new Vector2(cible.objet.transform.position.x,cible.objet.transform.position.y+4);
                    }
                }else{//c'est un 'E'
                    if (lanceur.vie > 0 && lanceur.numéroDuDernierTourJouer < numéroDuTour){
                        lanceur.numéroDuDernierTourJouer = numéroDuTour;
                        List<int> indicesVisibles = new List<int>();
                        for (int j = 0; j < 3; j++){
                            int index = (g.numéroPrésentDevant + j) % 6;
                            if (g.J.Count > index){
                                if (g.J[index].vie > 0)
                                    indicesVisibles.Add(index);
                            }
                        }
                        if (indicesVisibles.Count > 0){
                            int cibleNuméro = indicesVisibles[rng.Next(0, indicesVisibles.Count)];
                            cible = g.J[cibleNuméro];
                            flèche.position = new Vector2(cible.objet.transform.position.x,cible.objet.transform.position.y+1);
                        }
                    }
                }
                if (cible != null){
                    annimation(lanceur.objet, "attaque");
                    yield return new WaitForSecondsRealtime(tempsAttanteEntreLesTours);
                    cible.vie -= calculerDégâts(lanceur.script, cible.script);
                    if (cible.vie <= 0) annimation(cible.objet, "mort");
                    else annimation(cible.objet, "coup");
                    RafraichireLeCanvas();
                    vérifieSiUnJoueurEstEnVie();
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    int calculerDégâts(gestionnaireDeMonPetitYoKaiPourUnCombat lanceur, gestionnaireDeMonPetitYoKaiPourUnCombat cible){
        if (lanceur == null || cible == null){
            Debug.LogError("Lanceur ou cible est null dans calculerDégâts !");
            return 0;
        }
        float total = ((float)lanceur.dégatInfligé/(float)cible.pointDeVie)*100;
        //Debug.Log($"{lanceur.nom} inflige {total} à {cible.nom} !");
        return (int)(total);
    }

    void RafraichireLeCanvas(){
        for (int i = 0; i < g.J_Vie.Count; i++){
            g.J_Vie[i].value = g.J[i].vie;
            g.J[i].amûltime += 5;
            g.barreDeAmultime[i].value=g.J[i].amûltime;
        }
        for (int i = 0; i < g.E_Vie.Count; i++){
            g.E_Vie[i].value = g.E[i].vie;
        }
    }

    void annimation(GameObject objet, string nom){//coup, mort, attaque
        if (objet.GetComponent<Animator>() != null && objet != null){
            objet.GetComponent<Animator>().SetTrigger(nom);
        }else{
            Debug.LogWarning("Attention l'annimator du joueur avec l'ID"+objet+"du gestionnaire de combat n'est pas activé et donc il ne peut pas jouer son annimation de "+nom);
        }
    }
    void vérifieSiUnJoueurEstEnVie(){
        int conteur = 0;
        for (int i = 0; i < g.E_Vie.Count; i++){
            if (g.E_Vie[i].value <=0){conteur++;}
        }
        if (conteur == g.E_Vie.Count){
            print("fin de la partie, le joueur à gagner");
            finDuCombat = true;
            leJoeurAGagner = true;
            panneauDeVictoire.SetActive(true);
        }
        conteur = 0;
        for (int i = 0; i < g.J.Count; i++){
            if (g.J[i].vie <=0){conteur++;}
        }
        if (conteur == g.J.Count){
            print("fin de la partie, l'ennemie à gagner");
            finDuCombat = true;
            leJoeurAGagner = false;
            panneauDeDéfaite.SetActive(true);
        }
    }
}
[System.Serializable]
public class Gestionnaire{
    [HideInInspector] public List<Personnage> J = new List<Personnage>();
    [HideInInspector] public List<Personnage> E = new List<Personnage>();
    [Range(0,5)]public int numéroPrésentDevant = 0;//se ne sont pas les id mais les numéro de 0 à 5
    //si c'est 3 se sera |3,4,5|
    public List<Text> nom;
    public List<SliderValue> J_Vie;
    public List<SliderValue> E_Vie;
    public List<SliderValue> barreDeAmultime;
}

[System.Serializable]
public class Personnage{
    public GameObject objet;
    public int vie;
    public int amûltime;
    public string nom;
    [Range(0,5)]public int positionActuel;
    public gestionnaireDeMonPetitYoKaiPourUnCombat script;
    public int numéroDuDernierTourJouer = 0;
}