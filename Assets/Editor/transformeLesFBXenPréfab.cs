using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class transformeLesFBXenPréfab : EditorWindow{
    private DefaultAsset leDossierAScanné;
    [MenuItem("Tools/transforme Les FBX en Préfab")]
    public static void ShowWindow(){
        GetWindow<transformeLesFBXenPréfab>("transforme Les FBX en Préfab");
    }
    private void OnGUI(){
        GUILayout.Label("transforme Les FBX en Préfab", EditorStyles.boldLabel);
        leDossierAScanné = (DefaultAsset)EditorGUILayout.ObjectField("Dossier contenant les FBX", leDossierAScanné, typeof(DefaultAsset), false);
        if (GUILayout.Button("Retirer les boîtes")){
            if (leDossierAScanné == null){
                Debug.LogError("Merci de sélectionner un dossier contenant des FBX.");
                return;
            }

            string dossier = AssetDatabase.GetAssetPath(leDossierAScanné);
            RetirerLesBoîtes(dossier);
        }
    }
    void RetirerLesBoîtes(string dossier){
        string[] daeFiles = Directory.GetFiles(dossier, "*.fbx", SearchOption.AllDirectories);
        Debug.Log($"{daeFiles.Length} fichiers .fbx trouvés.");

        foreach (string daePath in daeFiles){
            string assetPath = daePath.Replace(Application.dataPath, "Assets");
            GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (fbxModel == null){
                Debug.LogWarning($"Impossible de charger le modèle à l'emplacement : {assetPath}");
                continue;
            }

            string dossierDuFBX = Path.GetDirectoryName(assetPath);
            string nomBase = Path.GetFileNameWithoutExtension(assetPath);
            string dossierPrefab = dossierDuFBX + "/PrefabsNettoyés";

            if (!AssetDatabase.IsValidFolder(dossierPrefab)){
                AssetDatabase.CreateFolder(dossierDuFBX, "PrefabsNettoyés");
            }

            string prefabPath = $"{dossierPrefab}/{nomBase}_mod.prefab";

            // Sauter si le prefab existe déjà
            if (File.Exists(prefabPath)){
                Debug.Log($"Préfab déjà existant, ignoré : {prefabPath}");
                continue;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(fbxModel);
            bool aÉtéModifié = false;

            Transform[] allChildren = instance.GetComponentsInChildren<Transform>(true);
            /*
            foreach (Transform child in allChildren){
                if (child.name.ToLower().Contains("output.col")){
                    child.gameObject.SetActive(false);
                    aÉtéModifié = true;
                    Debug.Log($"Désactivé : {child.name} dans {nomBase}");
                }
            }*/

            // Sauvegarde du prefab même s’il n’a pas été modifié pour éviter le traitement futur
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"Prefab sauvegardé : {prefabPath}");

            DestroyImmediate(instance);
        }

        AssetDatabase.Refresh();
    }
}
