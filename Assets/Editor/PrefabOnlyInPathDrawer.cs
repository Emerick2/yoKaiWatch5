#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomPropertyDrawer(typeof(PrefabOnlyInPathAttribute))]
public class PrefabOnlyInPathDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        PrefabOnlyInPathAttribute attr = (PrefabOnlyInPathAttribute)attribute;

        // Champ visuel classique
        Object original = property.objectReferenceValue;
        Object selected = EditorGUI.ObjectField(position, label, original, typeof(GameObject), false);

        if (selected != null) {
            string selectedPath = AssetDatabase.GetAssetPath(selected);
            string fullAttrPath = Path.GetFullPath(attr.Path).Replace("\\", "/");
            string fullSelectedPath = Path.GetFullPath(selectedPath).Replace("\\", "/");

            // Vérifie que le prefab est bien dans le dossier demandé (ou ses sous-dossiers)
            if (!fullSelectedPath.StartsWith(fullAttrPath)) {
                Debug.Log(fullAttrPath); 
                Debug.LogWarning($"❌ Le prefab '{selected.name}' n'est pas dans le dossier autorisé : '{attr.Path}'");
                selected = original; // Rétablit l'ancien prefab
            } else {
                // Vérifie que c'est bien un prefab
                var prefabType = PrefabUtility.GetPrefabAssetType(selected);
                if (prefabType == PrefabAssetType.NotAPrefab) {
                    Debug.LogWarning($"❌ '{selected.name}' n'est pas un prefab valide !");
                    selected = original;
                }
            }
        }

        property.objectReferenceValue = selected;
        EditorGUI.EndProperty();
    }
}
#endif
