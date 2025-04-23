using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using System;


public class GestionnaireDeCombas : MonoBehaviour{
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

    public int joueurSurLeTerrin;
    int random;
    
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
                vie = 100//revenir ici quand je sovgarderais les vie
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

    IEnumerator MouvementYoKai(GameObject élément, Transform positionCible,bool annimationFluide){
        Vector2 vectorCible = new Vector2(positionCible.position.x, positionCible.position.y);
        float vitesse = 20;
        if (!annimationFluide){
            élément.transform.position = vectorCible;
        }else{
            Vector2 currentPos = élément.transform.position;
            float t = 0f;
            while (t < 1){
                t += Time.deltaTime / vitesse;
                élément.transform.position = Vector2.Lerp(currentPos, vectorCible, t);
                yield return null;
            }
        }
    }

    int Mod(int x, int m){
        return (x % m + m) % m;
    }

    public void RotationDeLaRoue(int mouvement){
        mouvement = Math.Sign(mouvement);
        // Prépare la nouvelle disposition des Yo-kai selon le sens
        int[] offsets;
        int[] positions;

        if (mouvement < 0){
            offsets = new int[] { -1, 0, 1, 2, 3, 4 };
            positions = new int[] { 0, 1, 2, 3, 4, 5 };
        }else if (mouvement > 0){
            offsets = new int[] { -1, 0, 1, 2, 3, 4 };
            positions = new int[] { 5, 4, 3, 2, 1, 0 };
        }else{
            offsets = new int[] { -1, 0, 1, 2, 3, 4 };
            positions = new int[] { -1, 0, 1, 2, 3, 4 };
        }

        for (int i = 0; i < offsets.Length; i++){
            int index = Mod(g.numéroPrésentDevant + offsets[i], 6);
            if (index < g.J.Count){
                print("ici ici !");
                StartCoroutine(MouvementYoKai(g.J[index].objet, positionJ[positions[i]], false));
            }
        }

        // Met à jour l'index du Yo-kai actuellement devant
        g.numéroPrésentDevant = Mod(g.numéroPrésentDevant + mouvement, 6);

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
            System.Random rng = new System.Random();
            int STOP = 1000;
            //ne plus être dans un for parce que cela bloque le retournement de la roue
            //int nombreDeTour = g.J.Count+g.E.Count;
            for (int i = 0; i < listeVitesse.Count; i++){//six fois parce que 3 pour les ennemie et trois pour les joeur
                lanceur = listeVitesse[i].source;
                if (listeVitesse[i].source.ToString()[2] == 'J'){
                    int cibleNuméro = rng.Next(0, g.E.Count-1);
                    while (g.E[cibleNuméro].vie > 0 && STOP > 0){
                        cibleNuméro = rng.Next(0, g.E.Count-1);
                        STOP--;
                    }
                    cible = g.E[cibleNuméro];
                }else{//c'est un 'E'
                    List<int> indicesVisibles = new List<int>();

                    for (int j = 0; j < 3; j++){
                        int index = (g.numéroPrésentDevant + j) % g.J.Count;
                        if (g.J[index].vie > 0)
                            indicesVisibles.Add(index);
                    }

                    if (indicesVisibles.Count > 0){
                        int cibleNuméro = indicesVisibles[rng.Next(0, indicesVisibles.Count)];
                        cible = g.J[cibleNuméro];
                    }
                }

                annimation(lanceur.objet, "attaque");
                yield return new WaitForSecondsRealtime(tempsAttanteEntreLesTours);
                        
                g.E_Vie[0/*cibleNuméro*/].value -= calculerDégâts(lanceur.script, cible.script);//problème
                
                if (cible.vie <= 0) annimation(cible.objet, "mort");
                else annimation(cible.objet, "coup");
            }
            /*
            for (int i = 0; i < g.J.Count; i++){//le joueur attaque l'ennemie
                lanceur = g.J;
                cible = g.E;
                if (g.J[i].vie > 0){
                    annimation(g.J[i].objet, "attaque");
                    yield return new WaitForSecondsRealtime(tempsAttanteEntreLesTours);
                        
                    int cibleNuméro = Random.Range(0, g.E.Count-1);
                    while (g.E_Vie[cibleNuméro].value > 0 && STOP > 0){
                        cibleNuméro = Random.Range(0, g.E.Count-1);
                        STOP--;
                    }
                    g.E_Vie[cibleNuméro].value -= calculerDégâts(g.J[i].script, g.E[cibleNuméro].script);
                    
                    if (g.E[cibleNuméro].vie <= 0) annimation(g.E[cibleNuméro].objet, "mort");
                    else annimation(g.E[cibleNuméro].objet, "coup");
                }
            }
            for (int i = 0; i < g.E.Count; i++){//l'ennemie attaque le joueur
                if (g.E[i].vie > 0){
                    annimation(g.E[i].objet, "attaque");
                    yield return new WaitForSecondsRealtime(tempsAttanteEntreLesTours);
                    int cibleNuméro = Random.Range(0, g.J.Count-1);
                    while (g.J[cibleNuméro].vie > 0 && STOP > 0){
                        cibleNuméro = Random.Range(0, g.J.Count-1);
                        STOP--;
                    }
                    g.J[cibleNuméro].vie -= calculerDégâts(g.E[i].script, g.J[cibleNuméro].script);

                    if (g.J[cibleNuméro].vie <= 0) annimation(g.J[cibleNuméro].objet, "mort");
                    else annimation(g.J[cibleNuméro].objet, "coup");
                }
            }*/
            vérifieSiUnJoueurEstEnVie();
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
    public int numéroPrésentDevant = 0;//se ne sont pas les id mais les numéro de 0 à 5
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
    public gestionnaireDeMonPetitYoKaiPourUnCombat script;
}