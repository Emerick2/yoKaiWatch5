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
    void Awake(){
        Transform enfantNom = transform.Cast<Transform>().FirstOrDefault(t => t.name.StartsWith("nom:"));
        if (enfantNom != null){
            string nouveauNom = enfantNom.name.Substring(4); // Retire "nom:"
            gameObject.name = nouveauNom;
        }
        gameObject.name = gameObject.name.Replace("_mod", "");
    }
    void Start(){
        //gameObject.name = gameObject.name.Replace("(Clone)", "");
        
        if (neBougeraPas){vitesseDeRechargement=1;}
        if (animController1 != null){
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animController1;
            string yoKaiName = transform.name;
            GameObject modèle3DDuYoKai = gameObject;
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
                "_p10_02001d_wkl",
                "_p10_02501d_rnl"
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