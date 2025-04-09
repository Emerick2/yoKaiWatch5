using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;


public class GestionnaireDeCombas : MonoBehaviour
{
    public GameObject panneauDeVictoire;
    public GameObject panneauDeDéfaite;
    public bool finDuCombat=false;
    public bool leJoeurAGagner=false;
    public RuntimeAnimatorController animController;
    private medaliumDictionnaire MedaliumDictionnaire;
    public int[] IDDesYoKaiDeEquipe;//identifient unique "mesYoKai"
    public int[] IDDesYoKaiDeEnnemie;//identifient globeau "médalium"
    public Transform[] positionJ;
    public Gestionnaire g;
    public float tempsAttanteEntreLesTours=1f;
    private GameObject yoKaiUtiliser;
    private SliderValue yoKaiUtiliser_Vie;
    public List<GameObject> lesJoueurs;


    public int joueurSurLeTerrin;
    public int ennemieSurLeTerrin;
    void Start(){
        MedaliumDictionnaire = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        chargementDesPersonnage();
        StartCoroutine(boucleDuJeu());
    }

    void chargementDesPersonnage(){
        lesJoueurs = new List<GameObject>();
        for (int i = 0; i < IDDesYoKaiDeEquipe.Length; i++){
            int ID = MesYoKai.Instance.maListe[IDDesYoKaiDeEquipe[i]].ID;
            print(ID);
            if (MedaliumDictionnaire.dictionnaire.Count <= ID){Debug.Log("ERREUR, le médalium n'est pas asser grand");}
            g.J.Add(MedaliumDictionnaire.dictionnaire[ID].yoKai);
            GameObject TV = Instantiate(g.J[i]);
            TV.transform.localScale = new Vector3(100, 100, 100);
            TV.transform.position = new Vector3(positionJ[i].position.x,positionJ[i].position.y,positionJ[i].position.z);
            Vector3 rotation = TV.transform.eulerAngles;
            rotation.y += 180;
            TV.transform.eulerAngles = rotation;
            TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>();
            TV.GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>().IDduYoKaiMedalium=ID;
            TV.GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>().IDduYoKaiSpécifique=IDDesYoKaiDeEquipe[i];
            g.nom[i].text=NewDataCenter.Instance.Lire(IDDesYoKaiDeEquipe[i].ToString()+"_surnom");
            TV.AddComponent<Animator>().runtimeAnimatorController = animController;
            lesJoueurs.Add(TV);
            TV.transform.name = TV.transform.name.Replace("(Clone)", "");
            CrééMonAnimator(TV.GetComponent<Animator>(), TV.transform.name, TV);
        }
        joueurSurLeTerrin = g.J.Count;
        ennemieSurLeTerrin = g.E.Count;
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

    IEnumerator boucleDuJeu(){
        int numéro=0;
        int random=0;
        bool leJoueurJoue = true;
        int initialisation = calculerDégâts(lesJoueurs[0].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>(),g.E[0].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>());

        while (!finDuCombat){
            if (g.barreDeVie.Count == 0 || g.E_Vie.Count == 0 || g.E.Count == 0 || g.J.Count == 0){
                Debug.LogError("Erreur : Une des listes est vide !");
                yield break;
            }

            if ((leJoueurJoue && numéro >= joueurSurLeTerrin) || (!leJoueurJoue && numéro >= ennemieSurLeTerrin)){
                numéro=0;
                leJoueurJoue = !leJoueurJoue;
            }
            vérifieSiUnJoueurEstEnVie();

            if (!leJoueurJoue){ // C'est un ennemi qui attaque
                if (g.J.Count > 0 && g.E_Vie[numéro].value>0) {
                    random = Random.Range(0, g.J.Count);int stoper=100;
                    while(g.barreDeVie[random].value<=0 && stoper >0){random = Random.Range(0, g.J.Count);stoper--;}
                    if (random < g.barreDeVie.Count) {
                        annimation(g.E[numéro], "attaque");
                        yield return new WaitForSeconds(0.5f);
                        g.barreDeVie[random].value -= calculerDégâts(g.E[numéro].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>(),lesJoueurs[random].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>());
                        if (g.barreDeVie[random].value <= 0){
                            annimation(lesJoueurs[random], "mort");
                            joueurSurLeTerrin--;
                        }else{
                            annimation(lesJoueurs[random], "coup");
                        }
                    }
                }
            } else { // C'est le joueur qui attaque
                if (g.E.Count > 0 && g.barreDeVie[random].value>0) {
                    g.barreDeAmultime[numéro].value+=(100/3);
                    random = Random.Range(0, g.E.Count);int stoper=100;
                    while(g.E_Vie[random].value<=0 && stoper >0){random = Random.Range(0, g.E.Count);stoper--;}
                    if (random < g.E_Vie.Count) {
                        annimation(lesJoueurs[numéro], "attaque");
                        yield return new WaitForSeconds(0.5f);
                        g.E_Vie[random].value -= calculerDégâts(lesJoueurs[numéro].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>(),g.E[random].GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>());
                        if (g.E_Vie[random].value <= 0){
                           annimation(g.E[random], "mort");
                           ennemieSurLeTerrin--;
                        }else{
                            annimation(g.E[random], "coup");
                        }
                    }
                }
            }

            numéro++;
            yield return new WaitForSeconds(tempsAttanteEntreLesTours);
        }
    }
    int calculerDégâts(gestionnaireDeMonPetitYoKaiPourUnCombat lanceur, gestionnaireDeMonPetitYoKaiPourUnCombat cible){
        float total = ((float)lanceur.dégatInfligé/(float)cible.pointDeVie)*100;
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
        for (int i = 0; i < g.barreDeVie.Count; i++){
            if (g.barreDeVie[i].value <=0){conteur++;}
        }
        if (conteur == g.barreDeVie.Count){
            print("fin de la partie, l'ennemie à gagner");
            finDuCombat = true;
            leJoeurAGagner = false;
            panneauDeDéfaite.SetActive(true);
        }
    }
}
[System.Serializable]
public class Gestionnaire{
    public List<GameObject> J;
    public List<GameObject> E;
    public List<SliderValue> E_Vie;
    public List<Text> nom;
    public List<SliderValue> barreDeVie;
    public List<SliderValue> barreDeAmultime;
}
