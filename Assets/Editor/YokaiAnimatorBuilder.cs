using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class YokaiAnimatorBuilder : EditorWindow
{
    [MenuItem("Tools/Générer les Animators Yo-kai")]
    public static void GenerateAnimators()
    {
        string inputFolder = "Assets/Resources/yo-kai-API/data/_3D/models";
        string outputFolder = "Assets/Resources/annimatorYoKai";

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            string[] folders = outputFolder.Split('/');
            AssetDatabase.CreateFolder(folders[0], folders[1]);
        }

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

            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            Debug.Log($"Création de l'animator pour : {yokaiName}");

            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            Dictionary<string, AnimationClip> clipsBySuffix = new Dictionary<string, AnimationClip>();

            foreach (Object subAsset in subAssets)
            {
                if (subAsset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    if (clip.name.Length >= 3)
                    {
                        string suffix = clip.name.Substring(clip.name.Length - 3).ToLower();
                        clipsBySuffix[suffix] = clip;
                    }
                }
            }

            controller.AddParameter("amultime", AnimatorControllerParameterType.Bool);
            controller.AddParameter("finAmultime", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("coup", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("mort", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("protect", AnimatorControllerParameterType.Bool);
            controller.AddParameter("fatigue", AnimatorControllerParameterType.Bool);
            controller.AddParameter("attaque", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine rootSM = controller.layers[0].stateMachine;

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

            // FORCE LE CLIP À REBOUCLER SUR LUI-MÊME INSTANTANÉMENT
            void ForceAnimationLoop(AnimatorState targetState)
            {
                // Crée une transition de l'état... vers lui-même !
                var loopTransition = targetState.AddTransition(targetState);
                loopTransition.hasExitTime = true;
                loopTransition.exitTime = 1f;       // Se déclenche pile à la fin du clip (100%)
                loopTransition.duration = 0f;       // Reset instantané à 0% sans transition visuelle
            }

            // Création des états principaux
            AnimatorState sti = CreateState("stl", "stl");
            rootSM.defaultState = sti;

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

            // Application de la boucle instantanée sur tes animations continues
            ForceAnimationLoop(sti);  // Idle
            ForceAnimationLoop(spi);  // Attente Amultime
            ForceAnimationLoop(ml);   // Course
            ForceAnimationLoop(wkl);  // Marche
            ForceAnimationLoop(pwl);  // Protection
            ForceAnimationLoop(trl);  // Fatigue

            // --- Configuration des transitions et des conditions ---

            var tSpi = sti.AddTransition(spi);
            tSpi.AddCondition(AnimatorConditionMode.If, 0, "amultime");

            var tSpo = spi.AddTransition(spo);
            tSpo.AddCondition(AnimatorConditionMode.If, 0, "finAmultime");

            var tSpoToSti = spo.AddTransition(sti);
            tSpoToSti.hasExitTime = true;

            var tDmi = rootSM.AddAnyStateTransition(dmi);
            tDmi.AddCondition(AnimatorConditionMode.If, 0, "coup");
            tDmi.canTransitionToSelf = false;

            var tDmiToSti = dmi.AddTransition(sti);
            tDmiToSti.hasExitTime = true;

            var tMl = sti.AddTransition(ml);
            tMl.AddCondition(AnimatorConditionMode.Greater, 2f, "speed");

            var tWkl = ml.AddTransition(wkl);
            tWkl.AddCondition(AnimatorConditionMode.Greater, 8f, "speed");

            var tWklToMl = wkl.AddTransition(ml);
            tWklToMl.AddCondition(AnimatorConditionMode.Less, 8f, "speed");

            var tMlToSti = ml.AddTransition(sti);
            tMlToSti.AddCondition(AnimatorConditionMode.Less, 2f, "speed");

            var tDei = rootSM.AddAnyStateTransition(dei);
            tDei.AddCondition(AnimatorConditionMode.If, 0, "mort");
            tDei.canTransitionToSelf = false;

            var tDeiToDel = dei.AddTransition(del);
            tDeiToDel.hasExitTime = true;

            var tPwl = sti.AddTransition(pwl);
            tPwl.AddCondition(AnimatorConditionMode.If, 0, "protect");

            var tPwlToSti = pwl.AddTransition(sti);
            tPwlToSti.AddCondition(AnimatorConditionMode.IfNot, 0, "protect");

            var tTrl = sti.AddTransition(trl);
            tTrl.AddCondition(AnimatorConditionMode.If, 0, "fatigue");

            var tTrlToSti = trl.AddTransition(sti);
            tTrlToSti.AddCondition(AnimatorConditionMode.IfNot, 0, "fatigue");

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