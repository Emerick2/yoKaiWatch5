using UnityEngine;
using UnityEngine.UI;

public class ArrowUIIndicator : MonoBehaviour
{
    public Camera minimapCamera;        // caméra utilisée pour la map / calculs
    public Transform target;            // la cible dans le monde
    public RectTransform arrowRect;     // RectTransform de l'image flèche (UI)
    [Range(0f, 0.5f)] public float viewportMargin = 0.05f; // marge en viewport (5%)

    void Update()
    {
        if (target == null) return;

        Vector3 vp = minimapCamera.WorldToViewportPoint(target.position);

        bool visible = (vp.z > 0f) && (vp.x >= 0f && vp.x <= 1f) && (vp.y >= 0f && vp.y <= 1f);
        arrowRect.gameObject.SetActive(!visible);

        if (!visible)
        {
            // si la cible est derrière la caméra, "retourne" le point pour le manipuler proprement
            if (vp.z < 0f)
            {
                vp.x = 1f - vp.x;
                vp.y = 1f - vp.y;
                vp.z = Mathf.Abs(vp.z);
            }

            // clamp dans le viewport avec marge
            float cx = Mathf.Clamp(vp.x, viewportMargin, 1f - viewportMargin);
            float cy = Mathf.Clamp(vp.y, viewportMargin, 1f - viewportMargin);

            // convertir en coords écran
            Vector3 screenPos = new Vector3(cx * Screen.width, cy * Screen.height, 0f);

            // placer l'UI (si Canvas en Screen Space - Overlay)
            arrowRect.position = screenPos;

            // rotation : pointer du centre de l'écran vers la flèche
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 dir = (screenPos - (Vector3)screenCenter).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowRect.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // ajuste selon sprite orientation
        }
    }
}
