using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimationManager : MonoBehaviour {
    public GameObject myModelFBX;
    private Dictionary<string, AnimationClip> animationLibrary = new Dictionary<string, AnimationClip>();

    private PlayableGraph graph;
    private AnimationLayerMixerPlayable mixer;
    private string namExample = "";

    void Start() {
        if (gameObject.GetComponent<Animator>() == null){
            gameObject.AddComponent<Animator>();
        }
        new ServantBoxCollider3D().CreateBoxCollider(0,gameObject, true);
        // --- (Toujours en mode Editor ici) ---
        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(myModelFBX));
        foreach (var obj in objects) {
            if (obj is AnimationClip clip) {
                if (!clip.name.Contains("__")) {
                    if(!animationLibrary.ContainsKey(clip.name))
                        animationLibrary.Add(clip.name, clip);
                        namExample = clip.name;
                }
            }
        }

        graph = PlayableGraph.Create("AnimationGraph_" + gameObject.name);
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        var playableOutput = AnimationPlayableOutput.Create(graph, "Layer", GetComponentInChildren<Animator>());
        graph.Play();

        // PlayAnnimation("y101000_p20_27501d_wil", true);
        PlayAnnimation(NameAnnimation(TypeAnnimation.Neutre),true);
    }

    public void PlayAnnimation(string name, bool loop) {
        if (animationLibrary.ContainsKey(name)) {
            AnimationClip originalClip = animationLibrary[name];
            AnimationClip clipToPlay = originalClip;

            if (loop) {
                clipToPlay = Instantiate(originalClip);
                clipToPlay.name = originalClip.name;
                clipToPlay.wrapMode = WrapMode.Loop;
                
                #if UNITY_EDITOR
                var settings = UnityEditor.AnimationUtility.GetAnimationClipSettings(clipToPlay);
                settings.loopTime = true;
                UnityEditor.AnimationUtility.SetAnimationClipSettings(clipToPlay, settings);
                #endif
            }

            var clipPlayable = AnimationClipPlayable.Create(graph, clipToPlay);

            if (loop) {
                clipPlayable.SetDuration(double.MaxValue);
            } else {
                clipPlayable.SetDuration(clipToPlay.length);
            }

            var output = graph.GetOutput(0);
            
            var previous = output.GetSourcePlayable();
            if (previous.IsValid()) graph.DestroyPlayable(previous);

            output.SetSourcePlayable(clipPlayable);
        } else {
            Debug.LogWarning("L'animation " + name + " est introuvable !");
        }
    }

    void OnDestroy() {
        if (graph.IsValid()) {
            graph.Destroy();
        }
    }

    string NameAnnimation(TypeAnnimation type){
        string end = "";
        switch (type){
            case TypeAnnimation.Neutre:
                end = "stl";
                break;
            case TypeAnnimation.Courir:
                end = "ml";
                break;
            case TypeAnnimation.Marche:
                end = "wkl";
                break;
            case TypeAnnimation.Coup:
                end = "dmi";
                break;
            case TypeAnnimation.Amultime1:
                end = "spi";
                break;
            case TypeAnnimation.Amultime2:
                end = "spo";
                break;
            case TypeAnnimation.Attaque:
                end = "ati";
                break;
            case TypeAnnimation.Parle:
                end = "tkl";
                break;
            case TypeAnnimation.Relever:
                end = "sti";
                break;
            case TypeAnnimation.feter://fêter
                end = "idi";
                break;
            case TypeAnnimation.Fatiguer:
                end = "stl";
                break;
            case TypeAnnimation.Assis:
                end = "lfl";
                break;
            case TypeAnnimation.CrisDeVictoire:
                end = "hpi";
                break;
            case TypeAnnimation.LeverLePied:
                end = "adl";
                break;
            case TypeAnnimation.Deffance:
                end = "pwl";
                break;
            case TypeAnnimation.DeffanceAccrue:
                end = "grl";
                break;
            case TypeAnnimation.EnvoieAmultime:
                end = "wci";
                break;
            case TypeAnnimation.DanseDeVictoire:
                end = "wil";
                break;
            case TypeAnnimation.Heureu:
                end = "wii";
                break;
            case TypeAnnimation.Mort1:
                end = "dei";
                break;
            case TypeAnnimation.Mort2:
                end = "del";
                break;
        }
        return namExample.Substring(0, namExample.Length - 3) + end;
    }
}