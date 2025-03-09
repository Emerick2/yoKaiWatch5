using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class GestionnaireDeCombas : MonoBehaviour
{
    public GameObject panneauDeVictoire;
    public GameObject panneauDeDéfaite;
    public bool finDuCombat=false;
    public bool leJoeurAGagner=false;
    public RuntimeAnimatorController animController;
    public medaliumDictionnaire medaliumDictionnaire;
    public int[] IDDesYoKaiDeEquipe;//identifient unique "mesYoKai"
    public int[] IDDesYoKaiDeEnnemie;//identifient globeau "médalium"
    public Transform[] positionJ;
    public Gestionnaire g;
    public float tempsAttanteEntreLesTours=1f;
    private GameObject yoKaiUtiliser;
    private SliderValue yoKaiUtiliser_Vie;
    public List<GameObject> lesJoueurs;
    void Start(){
        chargementDesPersonnage();
        StartCoroutine(boucleDuJeu());
    }

    void chargementDesPersonnage(){
        lesJoueurs = new List<GameObject>();
        for (int i = 0; i < IDDesYoKaiDeEquipe.Length; i++){
            int ID = MesYoKai.Instance.maListe[IDDesYoKaiDeEquipe[i]].ID;
            g.J.Add(medaliumDictionnaire.dictionnaire[ID].yoKai);
            GameObject TV = Instantiate(g.J[i]);
            TV.transform.localScale = new Vector3(100, 100, 100);
            TV.transform.position = new Vector3(positionJ[i].position.x,positionJ[i].position.y,positionJ[i].position.z);
            Vector3 rotation = TV.transform.eulerAngles;
            rotation.y += 180;
            TV.transform.eulerAngles = rotation;
            TV.AddComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>().medaliumDictionnaire=medaliumDictionnaire;
            TV.GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>().IDduYoKaiMedalium=ID;
            TV.GetComponent<gestionnaireDeMonPetitYoKaiPourUnCombat>().IDduYoKaiSpécifique=IDDesYoKaiDeEquipe[i];
            g.nom[i].text=NewDataCenter.Instance.Lire(IDDesYoKaiDeEquipe[i].ToString()+"_surnom");
            TV.AddComponent<Animator>().runtimeAnimatorController = animController;
            lesJoueurs.Add(TV);
            TV.transform.name = TV.transform.name.Replace("(Clone)", "");
            CrééMonAnimator(TV.GetComponent<Animator>(), TV.transform.name, TV);
        }
    }
    void CrééMonAnimator(Animator animator, string yoKaiName, GameObject modèle3DDuYoKai){
        if (animator == null || modèle3DDuYoKai == null){
            Debug.LogError("Animator passé en paramètre est null.");
            Debug.LogError("Le GameObject modèle3DDuYoKai est null.");
            return;
        }

        Animator modeleAnimator = modèle3DDuYoKai.GetComponent<Animator>();
        if (modeleAnimator == null){
            Debug.LogError("Aucun composant Animator trouvé sur le modèle3DDuYoKai.");
            return;
        }
        RuntimeAnimatorController animController = modeleAnimator.runtimeAnimatorController;
        if (animController == null){
            Debug.LogError("Aucun RuntimeAnimatorController trouvé sur le modèle3DDuYoKai.");
            return;
        }

        AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
        AnimationClip[] clips = animController.animationClips;
        bool animationTrouvée = false;

        string nouveauNomAnimation = yoKaiName + "_p20_25000s_ati";
        AnimationClip monAnimation = null;

        for (int i = 0; i < clips.Length; i++){
            if (clips[i].name.Equals(nouveauNomAnimation)){
                monAnimation = clips[i];
                break;
            }
        }

        if (monAnimation == null){
            Debug.LogError("Animation clip '" + nouveauNomAnimation + "' non trouvée dans le modèle3DDuYoKai.");
            return;
        }

        for (int i = 0; i < clips.Length; i++){
            string nomAnimationTrouvée = clips[i].name;
            if (nomAnimationTrouvée.Contains("_p20_25000s_ati")){
                overrideController[nomAnimationTrouvée] = monAnimation;
                animationTrouvée = true;
                break;
            }
        }

        if (!animationTrouvée){
            Debug.LogWarning("Aucune animation correspondante trouvée pour être remplacée dans l'override.");
        }
        animator.runtimeAnimatorController = overrideController;
    }

    /*
    void CrééMonAnimator(Animator animator, string yoKaiName, GameObject modèle3DDuYoKai) { 
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
        AnimationClip[] clips = modèle3DDuYoKai.GetComponent<Animator>().runtimeAnimatorController.animationClips;
        string nomAnimation = "_p20_25000s_ati";
        for (int i = 0; i < clips.Length; i++){
            nomAnnimationATrouverDansLeFichierFBXDuModèle3DDuYoKai=yoKaiName+(clips[i].name.Replace(yoKaiName,""));
            //trouver l'annimation qui est dans le fichier FBX du game object modèle3DDuYoKai et l'utiliser pour remplacer le clips[i]
            animator.runtimeAnimatorController = overrideController; 
        }   
    }*/

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

            if ((leJoueurJoue && numéro >= g.barreDeVie.Count) || (!leJoueurJoue && numéro >= g.E_Vie.Count)){
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
        float total = lanceur.dégatInfligé/cible.pointDeVie;
        print(total*100);
        return (int)(total*100);
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
