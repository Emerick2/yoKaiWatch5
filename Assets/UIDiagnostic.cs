using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDiagnostic : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Debug.Log("[UI] Clic détecté !");

        EventSystem system = EventSystem.current;

        if (system == null)
        {
            Debug.LogError("[UI] Aucun EventSystem actif !");
            return;
        }

        Debug.Log(
            "[UI] Input Module : " +
            (system.currentInputModule != null
                ? system.currentInputModule.GetType().Name
                : "AUCUN")
        );

        PointerEventData pointer = new PointerEventData(system)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();

        system.RaycastAll(pointer, results);

        Debug.Log($"[UI] Objets détectés : {results.Count}");

        for (int i = 0; i < Mathf.Min(results.Count, 10); i++)
        {
            var result = results[i];

            Slider slider =
                result.gameObject.GetComponentInParent<Slider>();

            Debug.Log(
                $"[UI] #{i} : {GetPath(result.gameObject.transform)}" +
                $" | Slider : {(slider != null ? slider.name : "NON")}" +
                $" | Raycaster : {result.module.GetType().Name}"
            );
        }
    }

    private static string GetPath(Transform transform)
    {
        string path = transform.name;

        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }

        return path;
    }
}