using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AnimationExtractor : EditorWindow{
    private DefaultAsset folderToScan;

    [MenuItem("Tools/Extract Animations from All FBX in Folder")]
    public static void ShowWindow(){
        GetWindow<AnimationExtractor>("FBX Batch Animation Extractor");
    }

    private void OnGUI(){
        GUILayout.Label("Extraction automatique des animations depuis un dossier de FBX", EditorStyles.boldLabel);
        folderToScan = (DefaultAsset)EditorGUILayout.ObjectField("Dossier FBX", folderToScan, typeof(DefaultAsset), false);

        if (GUILayout.Button("Extraire toutes les animations")){
            if (folderToScan == null){
                Debug.LogError("Merci de sélectionner un dossier contenant des FBX.");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(folderToScan);
            ExtractAllAnimations(folderPath);
        }
    }

    private void ExtractAllAnimations(string folderPath){
        string[] fbxFiles = Directory.GetFiles(folderPath, "*.fbx", SearchOption.AllDirectories);

        if (fbxFiles.Length == 0){
            Debug.LogWarning("Aucun fichier .fbx trouvé dans ce dossier.");
            return;
        }

        string baseOutput = "Assets/AnimationsExtraites";
        if (!Directory.Exists(baseOutput))
            Directory.CreateDirectory(baseOutput);

        int totalClips = 0;

        foreach (string fbxPath in fbxFiles){
            string assetPath = fbxPath.Replace(Application.dataPath, "Assets").Replace('\\', '/');
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            string fbxName = Path.GetFileNameWithoutExtension(assetPath);
            string outputPath = Path.Combine(baseOutput, fbxName);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            foreach (Object asset in assets){
                if (asset is AnimationClip clip){
                    if (clip.name.StartsWith("__")) continue;

                    string newClipPath = Path.Combine(outputPath, clip.name + ".anim");
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(clip, newClip);
                    AssetDatabase.CreateAsset(newClip, newClipPath);
                    totalClips++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"🎉 Extraction terminée : {totalClips} animation(s) extraites depuis {fbxFiles.Length} fichier(s) FBX !");
    }
}
