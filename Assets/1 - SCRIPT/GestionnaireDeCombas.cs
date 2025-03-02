using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class GestionnaireDeCombas : MonoBehaviour
{
    public RuntimeAnimatorController animController;
    public medaliumDictionnaire medaliumDictionnaire;
    public int[] IDDesYoKaiDeEquipe;//identifient unique "mesYoKai"
    public int[] IDDesYoKaiDeEnnemie;//identifient globeau "médalium"
    public Transform[] positionJ;
    public Gestionnaire g;
    public float tempsAttanteEntreLesTours=0.5f;
    private int numéroDuTour = 1;
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
    void CrééMonAnimator(Animator animator, string yoKaiName, GameObject modèle3DDuYoKai) { 
        print("début");
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
        AnimationClip[] clips = modèle3DDuYoKai.GetComponent<Animator>().runtimeAnimatorController.animationClips;
        string nomAnimation = "_p20_25000s_ati";
        for (int i = 0; i < clips.Length; i++){
            print(clips[i]);
            if (clips[i].name.Replace(yoKaiName,"") == nomAnimation) {
                print ("clipTrouvé qui doit être modifier");
                AnimationClip newClip = null;/*
                //recherche du remplacent
                AnimationClip[] dansLeYoKai
                for (int x = 0; x < clips.Length; x++){
                    if (clips[x].name == yoKaiName + "_p20_25000s_ati") {
                        print ("clipTrouvé qui va le remplacer");
                        newClip = clips[x];
                        break;
                    }
                }

                if (newClip != null) {
                    overrideController[clips[i].name] = newClip;
                } else {
                    Debug.LogWarning("Animation clip non trouvée : " + yoKaiName + "_p20_25000s_ati");
                }*/
            }
        }
        animator.runtimeAnimatorController = overrideController;
        /*
        if (animator == null) {
            Debug.LogError("L'Animator est null pour " + yoKaiName);
            return;
        }

        // Vérifier que le modèle 3D a bien une animation intégrée
        AnimationClip[] clips = modèle3DDuYoKai.GetComponent<Animator>().runtimeAnimatorController.animationClips;
        if (clips == null || clips.Length == 0) {
            Debug.LogError("Aucune animation trouvée dans le modèle 3D : " + modèle3DDuYoKai.name);
            return;
        }

        // Créer un AnimatorOverrideController basé sur le controller de base
        AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);

        // Dictionnaire des animations
        Dictionary<string, string> animations = new Dictionary<string, string> {
            { "coup", yoKaiName + "_p20_25000s_ati" },
            { "danse", yoKaiName + "_p10_10000s_dan" },
            { "gagner", yoKaiName + "_p15_20000s_win" },
            { "perdu", yoKaiName + "_p10_15000s_lose" }
        };

        // Associer chaque animation si elle est trouvée dans le modèle
        foreach (var entry in animations) {
            string etatAnimator = entry.Key;
            string nomAnimation = entry.Value;

            AnimationClip clipTrouvé = null;
            foreach (AnimationClip clip in clips) {
                if (clip.name == nomAnimation) {
                    clipTrouvé = clip;
                    break;
                }
            }

            if (clipTrouvé != null) {
                overrideController[etatAnimator] = clipTrouvé;
            } else {
                Debug.LogWarning("Animation non trouvée dans le modèle : " + nomAnimation);
            }
        }
        animator.runtimeAnimatorController = overrideController;
        */
    }

    IEnumerator boucleDuJeu(){
        int numéro=0;
        while (true){
            if (numéro >= g.barreDeVie.Count){numéro=0;}
            if (numéroDuTour==1 && g.J[numéro] != null && g.barreDeVie[numéro] != null){
                //récupérer l'ID du personnage, sa force etc... tous doit être charger au début de la partie dans un script que tien le gameObject du personnage (ce script lui est assigner par le gestionnaire de combat)
                g.barreDeVie[numéro].value-=10;
                StartCoroutine(annimation(numéro,"coup"));//coup, mort, attaque
            }if (g.barreDeVie[numéro].value<=0){
                StartCoroutine(annimation(numéro,"mort"));//coup, mort, attaque
            }
            numéro++;
            yield return new WaitForSeconds(tempsAttanteEntreLesTours);
        }
    }
    IEnumerator annimation(int ID, string nom){//coup, mort, attaque
        lesJoueurs[ID].GetComponent<Animator>().SetBool(nom,true);
        yield return new WaitForSeconds(0.1f);
        lesJoueurs[ID].GetComponent<Animator>().SetBool(nom,false);
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
