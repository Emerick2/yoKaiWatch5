using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using YKW.Core.Events;

/// <summary>A free screen-space pointer for combat actors and other selectable objects.</summary>
public sealed class CombatTargetPointer : MonoBehaviour
{
    private CombatManager manager;
    private Canvas canvas;
    private Image pointer;
    private Image hoverPointer;
    [SerializeField, Min(0.01f)] private float stateTransitionDuration = 0.12f;
    private float hoverBlend;
    private Sprite idleSprite, hoverSprite;
    private Vector2 screenPosition;
    private int openedFrame;
    private bool previousCursorVisible;
    public bool IsOpen { get; private set; }
    public GameObject CurrentTarget { get; private set; }

    public void Initialize(CombatManager owner, Canvas sceneCanvas, Sprite idle, Sprite hover)
    {
        manager = owner;
        canvas = sceneCanvas;
        idleSprite = idle;
        hoverSprite = hover;
    }

    public Transform EnsureSelectionMarker(GameObject card)
    {
        if (card == null) return null;
        Transform existing = card.transform.Find("point");
        if (existing != null) return existing;
        GameObject marker = new GameObject("point", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        marker.transform.SetParent(card.transform, false);
        Image image = marker.GetComponent<Image>();
        image.sprite = hoverSprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.one * 0.5f;
        rect.sizeDelta = new Vector2(36f, 36f);
        rect.anchoredPosition = new Vector2(0f, 48f);
        marker.SetActive(false);
        return marker.transform;
    }

    public bool Open()
    {
        if (canvas == null || idleSprite == null || hoverSprite == null) return false;
        if (pointer == null)
        {
            GameObject visual = new GameObject("Combat target pointer", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            visual.transform.SetParent(canvas.transform, false);
            pointer = visual.GetComponent<Image>();
            pointer.raycastTarget = false;
            pointer.preserveAspect = true;
            pointer.rectTransform.sizeDelta = new Vector2(48f, 48f);
            GameObject hoverVisual = new GameObject("Selectable state", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            hoverVisual.transform.SetParent(visual.transform, false);
            hoverPointer = hoverVisual.GetComponent<Image>();
            hoverPointer.raycastTarget = false;
            hoverPointer.preserveAspect = true;
            hoverPointer.rectTransform.anchorMin = Vector2.zero;
            hoverPointer.rectTransform.anchorMax = Vector2.one;
            hoverPointer.rectTransform.offsetMin = hoverPointer.rectTransform.offsetMax = Vector2.zero;
        }
        if (IsOpen) return true;
        pointer.sprite = idleSprite;
        hoverPointer.sprite = hoverSprite;
        hoverBlend = 0f;
        previousCursorVisible = Cursor.visible;
        Cursor.visible = false;
        screenPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(Screen.width, Screen.height) * 0.5f;
        openedFrame = Time.frameCount;
        IsOpen = true;
        pointer.gameObject.SetActive(true);
        UpdateVisual();
        return true;
    }

    public void Close()
    {
        if (IsOpen) Cursor.visible = previousCursorVisible;
        IsOpen = false;
        CurrentTarget = null;
        if (pointer != null) pointer.gameObject.SetActive(false);
    }

    private void OnDisable() => Close();
    private void OnDestroy() { if (pointer != null) Destroy(pointer.gameObject); }

    private void Update()
    {
        if (!IsOpen) return;
        if (manager == null || !manager.CombatEnCours) { Close(); return; }
        if (Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.01f)
            screenPosition = Mouse.current.position.ReadValue();
        Vector2 movement = Gamepad.current != null ? Gamepad.current.leftStick.ReadValue() : Vector2.zero;
        if (Gamepad.current != null && movement.sqrMagnitude < 0.01f) movement = Gamepad.current.rightStick.ReadValue();
        if (Keyboard.current != null)
        {
            movement.x += (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) - (Keyboard.current.leftArrowKey.isPressed ? 1 : 0);
            movement.y += (Keyboard.current.upArrowKey.isPressed ? 1 : 0) - (Keyboard.current.downArrowKey.isPressed ? 1 : 0);
        }
        if (movement.sqrMagnitude > 0.01f) screenPosition += Vector2.ClampMagnitude(movement, 1f) * 700f * Time.unscaledDeltaTime;
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, Screen.width);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0f, Screen.height);
        UpdateVisual();
        if (Time.frameCount > openedFrame && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            manager.ConfirmerAction();
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) manager.AnnulerAction();
    }

    private void UpdateVisual()
    {
        CurrentTarget = FindTarget(screenPosition);
        UpdateCursorAppearance(CurrentTarget != null, Time.unscaledDeltaTime);
        RectTransform canvasRect = canvas.transform as RectTransform;
        Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, uiCamera, out Vector2 local);
        pointer.rectTransform.anchorMin = pointer.rectTransform.anchorMax = canvasRect.pivot;
        pointer.rectTransform.anchoredPosition = local;
        pointer.transform.SetAsLastSibling();
    }

    private void UpdateCursorAppearance(bool selectable, float deltaTime)
    {
        hoverBlend = Mathf.MoveTowards(hoverBlend, selectable ? 1f : 0f,
            Mathf.Max(0f, deltaTime) / Mathf.Max(0.01f, stateTransitionDuration));
        float eased = Mathf.SmoothStep(0f, 1f, hoverBlend);
        pointer.color = new Color(1f, 1f, 1f, 1f - eased);
        hoverPointer.color = new Color(1f, 1f, 1f, eased);
        pointer.rectTransform.localScale = Vector3.one * Mathf.Lerp(1f, 1.12f, eased);
    }

    private GameObject FindTarget(Vector2 position)
    {
        // Cards are valid pointer targets even when a model has no collider.
        foreach (var enemy in manager.listeYoKaiEnnemieSurLeTerrain)
            if (enemy != null && enemy.hp_me > 0 && enemy.positionRoue >= 0 && enemy.positionRoue < manager.troisDisplateUIEnnemie.Count &&
                Contains(manager.troisDisplateUIEnnemie[enemy.positionRoue], position)) return enemy.gameObjectSurLaScène;
        foreach (var ally in manager.listeYoKaiAmiSurLeTerrain)
        {
            if (ally == null || ally.hp_me <= 0 || ally.positionRoue < 0 || ally.positionRoue >= 6) continue;
            List<GameObject> cards = ally.positionRoue < 3 ? manager.troisDisplateUIAmi : manager.troisDisplateUIAmiDerrière;
            int slot = ally.positionRoue < 3 ? ally.positionRoue : 5 - ally.positionRoue;
            if (slot < cards.Count && Contains(cards[slot], position)) return ally.gameObjectSurLaScène;
        }
        Camera camera = Camera.main;
        if (camera == null) return null;
        Ray ray = camera.ScreenPointToRay(position);
        if (!Physics.Raycast(ray, out RaycastHit hit, camera.farClipPlane)) return null;
        foreach (var actor in manager.listeYoKaiAmiSurLeTerrain)
            if (actor != null && actor.hp_me > 0 && actor.gameObjectSurLaScène != null && hit.transform.IsChildOf(actor.gameObjectSurLaScène.transform)) return actor.gameObjectSurLaScène;
        foreach (var actor in manager.listeYoKaiEnnemieSurLeTerrain)
            if (actor != null && actor.hp_me > 0 && actor.gameObjectSurLaScène != null && hit.transform.IsChildOf(actor.gameObjectSurLaScène.transform)) return actor.gameObjectSurLaScène;
        CombatPointerTarget selectable = hit.collider.GetComponentInParent<CombatPointerTarget>();
        return selectable != null && selectable.isActiveAndEnabled ? selectable.gameObject : null;
    }

    private bool Contains(GameObject card, Vector2 position)
    {
        if (card == null || !card.activeInHierarchy) return false;
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        // Authored card roots can have a zero-sized rect; test their visible graphics as well.
        foreach (Graphic graphic in card.GetComponentsInChildren<Graphic>())
            if (graphic.enabled && RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, position, camera)) return true;
        return false;
    }
}
