using UnityEngine;
using UnityEditor;
using System.IO;

public class TransformerLesPrefabs : EditorWindow{
    private DefaultAsset leDossierAScanné;

    [MenuItem("Tools/Transformer Les Prefabs")]
    public static void ShowWindow(){
        GetWindow<TransformerLesPrefabs>("Transformer Les Prefabs");
    }

    private void OnGUI(){
        GUILayout.Label("Transformer Les Prefabs", EditorStyles.boldLabel);
        leDossierAScanné = (DefaultAsset)EditorGUILayout.ObjectField("Dossier contenant les prefabs", leDossierAScanné, typeof(DefaultAsset), false);

        if (GUILayout.Button("Ajouter les enfants 'nom:'")){
            if (leDossierAScanné == null){
                Debug.LogError("Merci de sélectionner un dossier contenant des prefabs.");
                return;
            }

            string dossier = AssetDatabase.GetAssetPath(leDossierAScanné);
            AjouterLesNoms(dossier);
        }
    }

    private void AjouterLesNoms(string dossier){
        string[] prefabFiles = Directory.GetFiles(dossier, "*.prefab", SearchOption.AllDirectories);
        Debug.Log($"{prefabFiles.Length} fichiers .prefab trouvés.");

        foreach (string path in prefabFiles){
            string assetPath = path.Replace(Application.dataPath, "Assets");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab == null){
                Debug.LogWarning($"Impossible de charger le prefab à : {assetPath}");
                continue;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            string nomBase = Path.GetFileNameWithoutExtension(assetPath);

            // Vérifie si un enfant "nom:xxx" existe déjà
            bool existeDéjà = false;
            foreach (Transform child in instance.transform){
                if (child.name == $"nom:{nomBase}"){
                    existeDéjà = true;
                    break;
                }
            }

            if (!existeDéjà){
                GameObject nomGO = new GameObject($"nom:{nomBase}");
                nomGO.transform.SetParent(instance.transform);
                nomGO.transform.localPosition = Vector3.zero;
                PrefabUtility.SaveAsPrefabAsset(instance, assetPath);
                Debug.Log($"Ajouté 'nom:{nomBase}' au prefab : {assetPath}");
            }
            else{
                Debug.Log($"Le prefab '{assetPath}' contient déjà un objet 'nom:{nomBase}'. Ignoré.");
            }

            DestroyImmediate(instance);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
