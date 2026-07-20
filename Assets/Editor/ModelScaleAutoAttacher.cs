using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class ModelScaleAutoAttacher
{
    static ModelScaleAutoAttacher()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        GameObject go = Selection.activeGameObject;
        if (go == null) return;

        if (go.GetComponent<AdjustModelScale>() != null) return;

            Match match = Regex.Match(go.name, @"\d+");

            if (match.Success)
            {
                if (int.TryParse(match.Value, out int result) && result > 0 && go.name.Contains("y"))
                {
                    go.AddComponent<AdjustModelScale>();
                    Debug.Log($"[Auto-Attach] Script ajouté sur '{go.name}' (ID détecté : {result})");
                }
            }

        /*
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<AdjustModelScale>() != null) continue;

            Match match = Regex.Match(go.name, @"\d+");

            if (match.Success)
            {
                if (int.TryParse(match.Value, out int result) && result > 0)
                {
                    go.AddComponent<AdjustModelScale>();
                    Debug.Log($"[Auto-Attach] Script ajouté sur '{go.name}' (ID détecté : {result})");
                }
            }
        }
        */
    }
}