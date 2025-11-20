// using UnityEngine;
// public class yoKaiMModel3D : MonoBehaviour
// {
//     [HideInInspector] public RuntimeAnimatorController animController1;
//     public bool neBougeraPas;
//     public float vitesseDeRechargement=0.2f;
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class yoKaiMModel3D : MonoBehaviour{
    [HideInInspector] public RuntimeAnimatorController animController1;
    private Animator animator;
    public bool neBougeraPas;
    public float vitesseDeRechargement=0.2f;
    medaliumDictionnaire médalium;
    void Start(){
        Transform enfantNom = transform.Cast<Transform>().FirstOrDefault(t => t.name.StartsWith("nom:"));
        médalium = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();

        string ID = gameObject.name;
        ID.Replace("²","");
        ID.Replace("!","");
        ID.Replace("(Clone)","");
        bool nombre = true;

        for (int i = 0; i < ID.Length; i++){
            if (ID[0]!='0' && ID[0]!='1' && ID[0]!='2' && ID[0]!='3' && ID[0]!='4' && ID[0]!='5' && ID[0]!='6' && ID[0]!='7' && ID[0]!='8' && ID[0]!='9'){
                nombre = false;
            }
        }
        /*
        ID.Replace("(Clone)","");
        if (nombre && médalium.dictionnaire.Count < int.Parse(ID)){        
            string x = médalium.dictionnaire[int.Parse(ID)].nomDeMonSquellette;
            string nomDeBase = enfantNom.name.Substring(4);
            if (x != ""){
                enfantNom.name="nom:"+x;
                transform.Find(nomDeBase+"_output").transform.parent = transform;
                transform.Find(nomDeBase+"_output").gameObject.name = x;
            }            
        }
        */
        if (enfantNom != null){
            string nouveauNom = enfantNom.name.Substring(4); // Retire "nom:"
            gameObject.name = nouveauNom;
        }
        gameObject.name = gameObject.name.Replace("_mod", "");



        gameObject.name = gameObject.name.Replace("(Clone)", "");
        
        if (neBougeraPas){vitesseDeRechargement=1;}
        
        if (animController1 != null){
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animController1;
            string yoKaiName = transform.name;
            GameObject modèle3DDuYoKai = gameObject;
            Animator modeleAnimator = modèle3DDuYoKai.GetComponent<Animator>();
            RuntimeAnimatorController animController = modeleAnimator.runtimeAnimatorController;
            if (animController == null) {
                Debug.LogError("Aucun RuntimeAnimatorController n'est assigné à l'Animator !");
                return;
            }
            AnimationClip[] clips = animController.animationClips;
            AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
            string nomDuYoKaiDeAnimatorParDéfaut = "y105010";
            List<string> aRemplacer = new List<string>{
                "_p20_27001d_del",
                "_p20_27000d_dei",
                "_p20_26500d_dmi",
                "_p20_25000s_ati",
                "_p20_21001d_stl",
                "_p10_02001d_wkl",
                "_p10_02501d_rnl",
                "_p20_29000s_spi",
                "_p20_29001s_spo",
                "_p10_01051s_tkl",
                "_p20_21000s_sti",
                "_p20_24001d_trl",
                "_p20_21010d_idi",
                "_p20_24201s_lfl",
                "_p20_24500d_hpi",
                "_p20_25500d_wci",
                "_p20_26001d_grl",
                "_p20_26301d_adl",
                "_p20_27500d_wii",
                "_p20_27501d_wil",
                "_p20_28001s_pwl"
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
        }else{
            Debug.Log("erreur d'asignation");
        }
        StartCoroutine(Vitesse());
    }
    IEnumerator Vitesse(){
        while (animator != null){
            float p1x = transform.position.x;
            float p1z = transform.position.z;
            yield return new WaitForSeconds(vitesseDeRechargement);
            float p2x = transform.position.x;
            float p2z = transform.position.z;
            float rx = p2x-p1x;
            if (rx <0) rx = rx*(-1);
            float rz = p2z-p1z;
            if (rz <0) rz = rz*(-1);
            float rt = 0;
            if (rz > rx) rt = rz;
            else rt = rx;
            animator.SetFloat("vitesse",rt);
        }
    }
}