using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class YokaiAnimatorBuilder : EditorWindow
{
    // Ajoute un bouton dans la barre de menu en haut d'Unity
    [MenuItem("Tools/Générer les Animators Yo-kai")]
    public static void GenerateAnimators()
    {
        string inputFolder = "Assets/Resources/yo-kai-API/data/_3D/models"; // Dossier où tu mets tes FBX
        string outputFolder = "Assets/Resources/annimatorYoKai"; // Dossier de sortie

        // Créer le dossier de sortie s'il n'existe pas
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            string[] folders = outputFolder.Split('/');
            AssetDatabase.CreateFolder(folders[0], folders[1]);
        }

        // Trouver tous les modèles 3D dans le dossier source
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { inputFolder });

        if (guids.Length == 0)
        {
            Debug.LogWarning($"Aucun modèle trouvé dans {inputFolder}. Vérifie le nom du dossier.");
            return;
        }

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!assetPath.ToLower().EndsWith(".fbx")) continue;

            string yokaiName = Path.GetFileNameWithoutExtension(assetPath);
            string controllerPath = $"{outputFolder}/{yokaiName}.controller";

            // 1. Créer le Controller d'animation
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Debug.Log($"Création de l'animator pour : {yokaiName}");

            // 2. Extraire les clips d'animation du FBX
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            Dictionary<string, AnimationClip> clipsBySuffix = new Dictionary<string, AnimationClip>();

            foreach (Object subAsset in subAssets)
            {
                if (subAsset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    // Récupère les 3 dernières lettres (ex: "dmi", "spo")
                    if (clip.name.Length >= 3)
                    {
                        string suffix = clip.name.Substring(clip.name.Length - 3).ToLower();
                        clipsBySuffix[suffix] = clip;
                    }
                }
            }

            // 3. Définir les paramètres
            controller.AddParameter("amultime", AnimatorControllerParameterType.Bool);
            controller.AddParameter("finAmultime", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("coup", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("mort", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("protect", AnimatorControllerParameterType.Bool);
            controller.AddParameter("fatigue", AnimatorControllerParameterType.Bool);
            controller.AddParameter("attaque", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine rootSM = controller.layers[0].stateMachine;

            // Fonction locale pour créer un State et y attacher l'animation trouvée
            AnimatorState CreateState(string stateName, string suffix)
            {
                AnimatorState state = rootSM.AddState(stateName);
                if (clipsBySuffix.ContainsKey(suffix))
                {
                    state.motion = clipsBySuffix[suffix];
                }
                else
                {
                    Debug.LogWarning($"Clip '{suffix}' manquant pour {yokaiName} ! Le State sera vide.");
                }
                return state;
            }

            // 4. Création des States
            AnimatorState sti = CreateState("sti", "sti");
            rootSM.defaultState = sti; // STI est l'animation par défaut

            AnimatorState spi = CreateState("spi", "spi");
            AnimatorState spo = CreateState("spo", "spo");
            AnimatorState dmi = CreateState("dmi", "dmi");
            AnimatorState ml = CreateState("ml", "ml");
            AnimatorState wkl = CreateState("wkl", "wkl");
            AnimatorState dei = CreateState("dei", "dei");
            AnimatorState del = CreateState("del", "del");
            AnimatorState pwl = CreateState("pwl", "pwl");
            AnimatorState trl = CreateState("trl", "trl");
            AnimatorState ati = CreateState("ati", "ati");

            // 5. Création des Transitions

            // -- Amultime --
            var tSpi = sti.AddTransition(spi);
            tSpi.AddCondition(AnimatorConditionMode.If, 0, "amultime");

            var tSpo = spi.AddTransition(spo);
            tSpo.AddCondition(AnimatorConditionMode.If, 0, "finAmultime");

            var tSpoToSti = spo.AddTransition(sti);
            tSpoToSti.hasExitTime = true; // Retourne à sti à la fin de l'animation spo

            // -- Coup (Depuis n'importe quel état) --
            var tDmi = rootSM.AddAnyStateTransition(dmi);
            tDmi.AddCondition(AnimatorConditionMode.If, 0, "coup");
            tDmi.canTransitionToSelf = false;

            var tDmiToSti = dmi.AddTransition(sti);
            tDmiToSti.hasExitTime = true;

            // -- Déplacement (Speed) --
            var tMl = sti.AddTransition(ml);
            tMl.AddCondition(AnimatorConditionMode.Greater, 2f, "speed");

            var tWkl = ml.AddTransition(wkl);
            tWkl.AddCondition(AnimatorConditionMode.Greater, 8f, "speed");

            var tWklToMl = wkl.AddTransition(ml);
            tWklToMl.AddCondition(AnimatorConditionMode.Less, 8f, "speed");

            var tMlToSti = ml.AddTransition(sti);
            tMlToSti.AddCondition(AnimatorConditionMode.Less, 2f, "speed");

            // -- Mort (Depuis n'importe quel état) --
            var tDei = rootSM.AddAnyStateTransition(dei);
            tDei.AddCondition(AnimatorConditionMode.If, 0, "mort");
            tDei.canTransitionToSelf = false;

            var tDeiToDel = dei.AddTransition(del);
            tDeiToDel.hasExitTime = true; // Ne revient pas, reste sur del

            // -- Protect --
            var tPwl = sti.AddTransition(pwl);
            tPwl.AddCondition(AnimatorConditionMode.If, 0, "protect");

            var tPwlToSti = pwl.AddTransition(sti);
            tPwlToSti.AddCondition(AnimatorConditionMode.IfNot, 0, "protect");

            // -- Fatigue --
            var tTrl = sti.AddTransition(trl);
            tTrl.AddCondition(AnimatorConditionMode.If, 0, "fatigue");

            var tTrlToSti = trl.AddTransition(sti);
            tTrlToSti.AddCondition(AnimatorConditionMode.IfNot, 0, "fatigue");

            // -- Attaque --
            var tAti = rootSM.AddAnyStateTransition(ati);
            tAti.AddCondition(AnimatorConditionMode.If, 0, "attaque");
            tAti.canTransitionToSelf = false;

            var tAtiToSti = ati.AddTransition(sti);
            tAtiToSti.hasExitTime = true;
        }

        Debug.Log("✅ Tous les Animators ont été générés avec succès !");
        AssetDatabase.SaveAssets();
    }
}