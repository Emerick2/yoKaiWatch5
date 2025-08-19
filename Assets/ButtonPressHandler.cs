using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressHandler : MonoBehaviour, IPointerDownHandler
{
    public jeuAmultime jeuScripte;

    [Header("Paramètres de l’agrandissement")]
    public float scale = 2f;              // pour changer la taille
    public Vector2 extraSize = new Vector2(50, 50); // pour élargir la zone cliquable

    void Start()
    {
        // Applique l’échelle
        transform.localScale = new Vector3(scale, scale, scale);

        // Agrandir la zone cliquable (RectTransform)
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta += extraSize; // élargit la zone de clic
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        jeuScripte.BoulleToucher(gameObject);
    }
}
