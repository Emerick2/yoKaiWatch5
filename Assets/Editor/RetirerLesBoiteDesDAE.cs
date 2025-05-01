using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class RetirerLesBoiteDesDAE : EditorWindow{
    private DefaultAsset leDossierAScanné;
    [MenuItem("Tools/retirer les boite des fichier DAE")]
    public static void ShowWindow(){
        GetWindow<RetirerLesBoiteDesDAE>("retirer les boite des fichier DAE");
    }
    private void OnGUI(){
        GUILayout.Label("retirer les boites des fichier DAE", EditorStyles.boldLabel);
        leDossierAScanné = (DefaultAsset)EditorGUILayout.ObjectField("Dossier contenant les DAE", leDossierAScanné, typeof(DefaultAsset), false);
        if (GUILayout.Button("Retirer les boîtes")){
            if (leDossierAScanné == null){
                Debug.LogError("Merci de sélectionner un dossier contenant des DAE.");
                return;
            }

            string dossier = AssetDatabase.GetAssetPath(leDossierAScanné);
            RetirerLesBoîtes(dossier);
        }
    }
    void RetirerLesBoîtes(string dossier){
        string[] daeFiles = Directory.GetFiles(dossier, "*.dae", SearchOption.AllDirectories);
        Debug.Log($"{daeFiles.Length} fichiers .dae trouvés.");

        foreach (string daePath in daeFiles){
            string assetPath = daePath.Replace(Application.dataPath, "Assets");
            GameObject daeModel = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (daeModel == null){
                Debug.LogWarning($"Impossible de charger le modèle à l'emplacement : {assetPath}");
                continue;
            }

            string dossierDuDAE = Path.GetDirectoryName(assetPath);
            string nomBase = Path.GetFileNameWithoutExtension(assetPath);
            string dossierPrefab = dossierDuDAE + "/PrefabsNettoyés";

            if (!AssetDatabase.IsValidFolder(dossierPrefab)){
                AssetDatabase.CreateFolder(dossierDuDAE, "PrefabsNettoyés");
            }

            string prefabPath = $"{dossierPrefab}/{nomBase}_mod.prefab";

            // Sauter si le prefab existe déjà
            if (File.Exists(prefabPath)){
                Debug.Log($"Préfab déjà existant, ignoré : {prefabPath}");
                continue;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(daeModel);
            bool aÉtéModifié = false;

            Transform[] allChildren = instance.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren){
                if (child.name.ToLower().Contains("output.col")){
                    child.gameObject.SetActive(false);
                    aÉtéModifié = true;
                    Debug.Log($"Désactivé : {child.name} dans {nomBase}");
                }
            }

            // Sauvegarde du prefab même s’il n’a pas été modifié pour éviter le traitement futur
            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            Debug.Log($"Prefab sauvegardé : {prefabPath}");

            DestroyImmediate(instance);
        }

        AssetDatabase.Refresh();
    }
}


