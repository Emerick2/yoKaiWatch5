using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKW.Core.Events;

/// <summary>Animates complete combat cards, or image slots in image-based wheel views.</summary>
public sealed class CombatSlotAnimator : EventSubscriberBehaviour
{
    [SerializeField, Min(0.05f)] private float transitionDuration = 0.32f;
    [SerializeField] private AnimationCurve transitionEasing = null;
    private readonly List<Image> slots = new List<Image>();
    private readonly List<Image> overlays = new List<Image>();
    private readonly List<RectTransform> overlayTargets = new List<RectTransform>();
    private readonly List<int> overlayStableIds = new List<int>();
    private readonly List<Image> hiddenImages = new List<Image>();
    private readonly List<Color> hiddenImageColors = new List<Color>();
    private Coroutine transition;
    private RectTransform canvasRect;
    private Canvas canvas;
    private Camera transitionCamera;

    private readonly List<GameObject> cardSlots = new List<GameObject>();
    private readonly Dictionary<int, CardMotion> cardMotions = new Dictionary<int, CardMotion>();
    private readonly Dictionary<CanvasGroup, float> hiddenCardGroups = new Dictionary<CanvasGroup, float>();
    private readonly HashSet<int> maskedCardSlots = new HashSet<int>();
    private Coroutine cardTransition;
    public bool IsTransitioning => transition != null || cardTransition != null;
    public float TransitionDuration => Mathf.Max(0.05f, transitionDuration);

    private sealed class CardMotion
    {
        public RectTransform Overlay, Target;
        public Vector2 StartPosition, StartSize;
        public Vector3 StartScale;
        public bool CrossesRows;
        public float EdgeSign;
    }

    public void BindCards(IList<GameObject> cards)
    {
        cardSlots.Clear();
        if (cards != null) cardSlots.AddRange(cards);
        // Gameplay has just refreshed the cards. Record their new occupancy
        // before hiding the destination content underneath the moving copies.
        MaskCardContent();
    }

    private void AnimateCards(CombatSlotsChangedEvent change)
    {
        if (canvasRect == null) return;
        if (cardTransition != null) StopCoroutine(cardTransition);
        cardTransition = null;
        RestoreCardContent();
        maskedCardSlots.Clear();
        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        foreach (SlotChange move in change.Changes)
        {
            if (move.FromSlot == move.ToSlot || move.FromSlot < 0 || move.ToSlot < 0 ||
                move.FromSlot >= cardSlots.Count || move.ToSlot >= cardSlots.Count) continue;
            GameObject source = cardSlots[move.FromSlot];
            GameObject target = cardSlots[move.ToSlot];
            if (source == null || target == null) continue;
            if (!cardMotions.TryGetValue(move.StableId, out CardMotion motion))
            {
                CanvasGroup visibility = source.GetComponent<CanvasGroup>();
                if (!source.activeInHierarchy || (visibility != null && visibility.alpha <= 0f)) continue;
                RectTransform sourceRect = source.transform as RectTransform;
                GameObject copy = Instantiate(source, canvasRect, false);
                copy.name = "Combat card transition";
                // Keep the snapshot's visuals without running copied gameplay/UI controllers.
                foreach (MonoBehaviour behaviour in copy.GetComponentsInChildren<MonoBehaviour>(true))
                    if (!(behaviour is Graphic)) behaviour.enabled = false;
                RectTransform overlay = copy.transform as RectTransform;
                overlay.anchorMin = overlay.anchorMax = new Vector2(0.5f, 0.5f);
                overlay.pivot = new Vector2(0.5f, 0.5f);
                overlay.sizeDelta = sourceRect.rect.size;
                overlay.localScale = CanvasScale(sourceRect);
                overlay.rotation = sourceRect.rotation;
                overlay.anchoredPosition = WorldToCanvasLocal(sourceRect.TransformPoint(sourceRect.rect.center), camera);
                CanvasGroup group = copy.GetComponent<CanvasGroup>();
                if (group == null) group = copy.AddComponent<CanvasGroup>();
                group.alpha = 1f;
                group.interactable = group.blocksRaycasts = false;
                motion = new CardMotion { Overlay = overlay };
                cardMotions.Add(move.StableId, motion);
            }
            motion.CrossesRows = (move.FromSlot < 3) != (move.ToSlot < 3);
            // Front 2 joins reserve 3 on the right; reserve 5 joins front 0 on the left.
            motion.EdgeSign = move.FromSlot == 2 || move.FromSlot == 3 ||
                move.ToSlot == 2 || move.ToSlot == 3 ? 1f : -1f;
            motion.Target = target.transform as RectTransform;
            motion.StartPosition = motion.Overlay.anchoredPosition;
            motion.StartSize = motion.Overlay.sizeDelta;
            motion.StartScale = motion.Overlay.localScale;
            maskedCardSlots.Add(move.FromSlot);
            maskedCardSlots.Add(move.ToSlot);
        }
        MaskCardContent();
        if (cardMotions.Count > 0) cardTransition = StartCoroutine(MoveCards(camera));
    }

    private Vector3 CanvasScale(RectTransform rect)
    {
        Vector3 scale = canvasRect.lossyScale;
        Vector3 source = rect.lossyScale;
        return new Vector3(scale.x != 0f ? source.x / scale.x : 1f,
            scale.y != 0f ? source.y / scale.y : 1f, 1f);
    }

    private IEnumerator MoveCards(Camera camera)
    {
        float elapsed = 0f;
        while (elapsed < TransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / TransitionDuration);
            float eased = transitionEasing != null && transitionEasing.length > 1
                ? transitionEasing.Evaluate(t) : 1f - Mathf.Pow(1f - t, 3f);
            foreach (CardMotion motion in cardMotions.Values)
            {
                if (motion.Overlay == null || motion.Target == null) continue;
                Vector2 target = WorldToCanvasLocal(motion.Target.TransformPoint(motion.Target.rect.center), camera);
                if (motion.CrossesRows)
                {
                    Vector2 control = new Vector2(canvasRect.rect.center.x +
                        motion.EdgeSign * canvasRect.rect.width * 0.48f,
                        (motion.StartPosition.y + target.y) * 0.5f);
                    motion.Overlay.anchoredPosition = (1f - eased) * (1f - eased) * motion.StartPosition +
                        2f * (1f - eased) * eased * control + eased * eased * target;
                }
                else motion.Overlay.anchoredPosition = Vector2.Lerp(motion.StartPosition, target, eased);
                motion.Overlay.sizeDelta = Vector2.Lerp(motion.StartSize, motion.Target.rect.size, eased);
                motion.Overlay.localScale = Vector3.Lerp(motion.StartScale, CanvasScale(motion.Target), eased);
            }
            yield return null;
        }
        CompleteCardTransition(false);
    }

    private void MaskCardContent()
    {
        foreach (int index in maskedCardSlots)
        {
            if (index < 0 || index >= cardSlots.Count || cardSlots[index] == null) continue;
            CanvasGroup group = cardSlots[index].GetComponent<CanvasGroup>();
            if (group == null) group = cardSlots[index].AddComponent<CanvasGroup>();
            // BindCards is called after each refresh, so alpha is the latest desired state.
            hiddenCardGroups[group] = group.alpha;
            group.alpha = 0f;
        }
    }

    private void RestoreCardContent()
    {
        foreach (var entry in hiddenCardGroups)
            if (entry.Key != null) entry.Key.alpha = entry.Value;
        hiddenCardGroups.Clear();
    }

    private void CompleteCardTransition(bool stopCoroutine = true)
    {
        if (stopCoroutine && cardTransition != null) StopCoroutine(cardTransition);
        cardTransition = null;
        RestoreCardContent();
        maskedCardSlots.Clear();
        foreach (CardMotion motion in cardMotions.Values)
            if (motion.Overlay != null) Destroy(motion.Overlay.gameObject);
        cardMotions.Clear();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        CompleteCurrentTransition();
        CompleteCardTransition();
    }

    public void Initialize(IList<Image> wheelSlots, Canvas sceneCanvas = null)
    {
        CompleteCurrentTransition();
        CompleteCardTransition();
        cardSlots.Clear();
        slots.Clear();
        if (wheelSlots != null) slots.AddRange(wheelSlots);
        canvas = sceneCanvas != null ? sceneCanvas : GetComponentInChildren<Canvas>(true);
        if (canvas != null) canvasRect = canvas.transform as RectTransform;
    }

    [SubscribeEvent]
    private void OnSlotsChanged(CombatSlotsChangedEvent change)
    {
        if (change == null || change.Changes == null) return;
        if (cardSlots.Count > 0) { AnimateCards(change); return; }
        if (slots.Count == 0) return;
        if (canvasRect == null) return;

        // Keep the current visual overlays when another wheel rotation arrives.
        // The gameplay's slot images already contain the previous final layout,
        // so destroying an in-flight overlay here would visibly snap the medal.
        if (transition != null) StopCoroutine(transition);
        transition = null;

        Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        transitionCamera = camera;
        List<SlotChange> validChanges = new List<SlotChange>(change.Changes.Count);
        for (int i = 0; i < change.Changes.Count; i++)
        {
            SlotChange slotChange = change.Changes[i];
            if (!ValidSlot(slotChange.FromSlot) || !ValidSlot(slotChange.ToSlot) ||
                slotChange.FromSlot == slotChange.ToSlot) continue;

            Image source = slots[slotChange.FromSlot];
            Image destination = slots[slotChange.ToSlot];
            if (source == null || destination == null || source.sprite == null ||
                !source.isActiveAndEnabled || !destination.gameObject.activeInHierarchy) continue;
            validChanges.Add(slotChange);
        }

        if (validChanges.Count == 0)
        {
            if (overlays.Count == 0) return;
            for (int i = 0; i < overlays.Count; i++)
                if (overlays[i] != null) overlayStartPositions[i] = overlays[i].rectTransform.anchoredPosition;
            transition = StartCoroutine(AnimateOverlays());
            return;
        }

        RestoreHiddenImages();

        // Hide every origin before starting any path. This avoids showing the
        // new layout's stationary sprites underneath moving overlays during a
        // permutation or an exchange.
        for (int i = 0; i < validChanges.Count; i++)
        {
            Image source = slots[validChanges[i].FromSlot];
            if (!hiddenImages.Contains(source))
            {
                hiddenImages.Add(source);
                Color originalColor = source.color;
                hiddenImageColors.Add(originalColor);
                originalColor.a = 0f;
                source.color = originalColor;
            }
        }

        var overlayByStableId = new Dictionary<int, int>();
        for (int i = 0; i < overlayStableIds.Count; i++)
            if (!overlayByStableId.ContainsKey(overlayStableIds[i])) overlayByStableId.Add(overlayStableIds[i], i);

        for (int i = 0; i < overlays.Count; i++)
            if (overlays[i] != null) overlayStartPositions[i] = overlays[i].rectTransform.anchoredPosition;

        for (int i = 0; i < validChanges.Count; i++)
        {
            SlotChange slotChange = validChanges[i];
            Image source = slots[slotChange.FromSlot];
            Image destination = slots[slotChange.ToSlot];
            int existingOverlayIndex;
            if (overlayByStableId.TryGetValue(slotChange.StableId, out existingOverlayIndex))
            {
                overlayTargets[existingOverlayIndex] = destination.rectTransform;
                continue;
            }

            int hiddenIndex = hiddenImages.IndexOf(source);
            Color sourceColor = hiddenIndex >= 0 ? hiddenImageColors[hiddenIndex] : source.color;
            Image overlay = CreateOverlay(source, camera, sourceColor);
            overlays.Add(overlay);
            overlayTargets.Add(destination.rectTransform);
            overlay.rectTransform.anchoredPosition = WorldToCanvasLocal(source.rectTransform.position, camera);
            overlayStartPositions.Add(overlay.rectTransform.anchoredPosition);
            overlayStableIds.Add(slotChange.StableId);
        }

        if (overlays.Count > 0) transition = StartCoroutine(AnimateOverlays());
    }

    private readonly List<Vector2> overlayStartPositions = new List<Vector2>();

    private IEnumerator AnimateOverlays()
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float eased = transitionEasing != null && transitionEasing.length > 1
                ? transitionEasing.Evaluate(t)
                : 1f - Mathf.Pow(1f - t, 3f);
            for (int i = 0; i < overlays.Count; i++)
            {
                if (overlays[i] == null || overlayTargets[i] == null) continue;
                Vector2 currentTarget = WorldToCanvasLocal(overlayTargets[i].position, transitionCamera);
                overlays[i].rectTransform.anchoredPosition =
                    Vector2.Lerp(overlayStartPositions[i], currentTarget, eased);
            }
            yield return null;
        }
        CompleteCurrentTransition(false);
    }

    private Image CreateOverlay(Image source, Camera camera, Color sourceColor)
    {
        GameObject overlayObject = new GameObject("Combat medal transition", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        overlayObject.transform.SetParent(canvasRect, false);
        Image overlay = overlayObject.GetComponent<Image>();
        overlay.sprite = source.sprite;
        overlay.color = sourceColor;
        overlay.preserveAspect = source.preserveAspect;
        overlay.raycastTarget = false;
        RectTransform rect = overlay.rectTransform;
        rect.sizeDelta = source.rectTransform.rect.size;
        rect.rotation = source.rectTransform.rotation;
        rect.localScale = Vector3.one;
        rect.anchoredPosition = WorldToCanvasLocal(source.rectTransform.position, camera);
        return overlay;
    }

    private Vector2 WorldToCanvasLocal(Vector3 worldPosition, Camera camera)
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect,
            RectTransformUtility.WorldToScreenPoint(camera, worldPosition), camera, out local);
        return local;
    }

    private bool ValidSlot(int index) => index >= 0 && index < slots.Count;

    private void CompleteCurrentTransition(bool stopCoroutine = true)
    {
        if (stopCoroutine && transition != null) StopCoroutine(transition);
        transition = null;
        RestoreHiddenImages();
        for (int i = 0; i < overlays.Count; i++)
            if (overlays[i] != null) Destroy(overlays[i].gameObject);
        overlays.Clear();
        overlayTargets.Clear();
        overlayStableIds.Clear();
        overlayStartPositions.Clear();
        transitionCamera = null;
    }

    private void RestoreHiddenImages()
    {
        for (int i = 0; i < hiddenImages.Count; i++)
            if (hiddenImages[i] != null) hiddenImages[i].color = hiddenImageColors[i];
        hiddenImages.Clear();
        hiddenImageColors.Clear();
    }
}
