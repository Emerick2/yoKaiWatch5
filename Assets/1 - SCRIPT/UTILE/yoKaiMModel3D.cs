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

    private GameObject préfabIconeMiniMap;
    [HideInInspector] public GameObject yoKaiInTheScene;
    private string name = "";
    // IEnumerator UpdateLant(float vitesse){
    //     while (true){
    //         préfabIconeMiniMap.transform.position = transform.position;
    //         yield return new WaitForSeconds(vitesse);        
    //     }
    // }
    // void Start(){
    //     préfabIconeMiniMap = Resources.Load<GameObject>("prefab/iconeMinimapYokai");
    //     if (préfabIconeMiniMap != null){
    //         préfabIconeMiniMap = Instantiate(préfabIconeMiniMap);
    //         préfabIconeMiniMap.transform.parent = transform;
    //         préfabIconeMiniMap.transform.position = transform.position;
    //         if (!neBougeraPas) StartCoroutine(UpdateLant(0.5f));
    //         else StartCoroutine(UpdateLant(1f));
    //     } else {
    //         Debug.Log("Le préfab est null...");
    //     }
    //     Transform enfantNom = transform.Cast<Transform>().FirstOrDefault(t => t.name.StartsWith("nom:"));
    //     if (enfantNom != null) {
    //         name = enfantNom.name;
    //     } else {
    //         name = gameObject.name;
    //     }
    //     name = name.Replace("²","");
    //     name = name.Replace("!","");
    //     name = name.Replace("(Clone)","");
    //     name = name.Replace("_mod","");
    //     name = name.Substring(4); // Retire "nom:"
    //     gameObject.name = name;
        
    //     if (neBougeraPas) vitesseDeRechargement=1;
    //     CreateAnimator();
    //     GameObject vraisYoKai = gameObject.transform.Find(name).gameObject;
    //     if (vraisYoKai != null){
    //         new ServantBoxCollider3D().CreateBoxCollider(0,vraisYoKai, true);
    //         new ServantBoxCollider3D().CreateACopieOfAColliderWithABonus(0.005f,gameObject, vraisYoKai.GetComponent<BoxCollider>(), false);
    //     }

    //     StartCoroutine(Vitesse());
    // }
    // IEnumerator Vitesse(){
    //     while (animator != null){
    //         float p1x = transform.position.x;
    //         float p1z = transform.position.z;
    //         yield return new WaitForSeconds(vitesseDeRechargement);
    //         float p2x = transform.position.x;
    //         float p2z = transform.position.z;
    //         float rx = p2x-p1x;
    //         if (rx <0) rx = rx*(-1);
    //         float rz = p2z-p1z;
    //         if (rz <0) rz = rz*(-1);
    //         float rt = 0;
    //         if (rz > rx) rt = rz;
    //         else rt = rx;
    //         animator.SetFloat("vitesse",rt);
    //     }
    // }

    // void CreateAnimator(){
    //     if (animController1 != null){
    //         GameObject modèle3DDuYoKai = gameObject;
    //         if (modèle3DDuYoKai.GetComponent<Animator>() == null) animator = modèle3DDuYoKai.AddComponent<Animator>();
    //         else animator = modèle3DDuYoKai.GetComponent<Animator>();
    //         animator.runtimeAnimatorController = animController1;
    //         RuntimeAnimatorController animController = animator.runtimeAnimatorController;
    //         if (animController == null) {
    //             Debug.LogError("Aucun RuntimeAnimatorController n'est assigné à l'Animator !");
    //             return;
    //         }
    //         AnimationClip[] clips = animController.animationClips;
    //         AnimatorOverrideController overrideController = new AnimatorOverrideController(animController);
    //         string nomDuYoKaiDeAnimatorParDéfaut = "y105010";
    //         List<string> aRemplacer = new List<string>{
    //             "_p20_27001d_del",
    //             "_p20_27000d_dei",
    //             "_p20_26500d_dmi",
    //             "_p20_25000s_ati",
    //             "_p20_21001d_stl",
    //             "_p10_02001d_wkl",
    //             "_p10_02501d_rnl",
    //             "_p20_29000s_spi",
    //             "_p20_29001s_spo",
    //             "_p10_01051s_tkl",
    //             "_p20_21000s_sti",
    //             "_p20_24001d_trl",
    //             "_p20_21010d_idi",
    //             "_p20_24201s_lfl",
    //             "_p20_24500d_hpi",
    //             "_p20_25500d_wci",
    //             "_p20_26001d_grl",
    //             "_p20_26301d_adl",
    //             "_p20_27500d_wii",
    //             "_p20_27501d_wil",
    //             "_p20_28001s_pwl"
    //         };

    //         List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

    //         for (int i = 0; i < aRemplacer.Count; i++){
    //             string suffixe = aRemplacer[i];
    //             string nomAnimationOrigine = nomDuYoKaiDeAnimatorParDéfaut + suffixe;

    //             AnimationClip clipOrigine = clips.FirstOrDefault(c => c.name == nomAnimationOrigine);

    //             string nomAnimationRemplacement = name + suffixe;
    //             AnimationClip clipRemplacement = Resources.Load<AnimationClip>("AnimationsExtraites/" + name + "/" + nomAnimationRemplacement);
    //             if (clipOrigine != null && clipRemplacement != null){
    //                 AnimationClip clip = clipRemplacement;
    //                 SerializedObject serializedClip = new SerializedObject(clip);
    //                 serializedClip.FindProperty("m_AnimationClipSettings.m_LoopTime").boolValue = true;
    //                 serializedClip.ApplyModifiedProperties();
    //                 overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clipOrigine, clipRemplacement));
    //             }else{
    //                 Debug.LogWarning($"Animation introuvable : {nomAnimationOrigine} ou {nomAnimationRemplacement}");
    //             }
    //         }
    //         overrideController.ApplyOverrides(overrides);
    //         animator.runtimeAnimatorController = overrideController;
    //     }else{
    //         Debug.Log("erreur d'asignation");
    //     }
    // }
}