using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatYokai = YKW.Core.YoKai.Class.YoKaiCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YKW.Core.Events;

/// <summary>Displays gameplay-owned combat rewards in the existing result placeholders.</summary>
public sealed class CombatResultUI : EventSubscriberBehaviour
{
    [SerializeField, Min(0.05f)] private float transitionDuration = 0.55f;
    [SerializeField] private AnimationCurve transitionEasing = null;
    private Transform resultRoot;
    private CombatManager manager;
    private readonly Dictionary<CombatYokai, int> initialSlots = new Dictionary<CombatYokai, int>();
    private RectTransform shownScreen;
    private RectTransform winScreen;
    private RectTransform lostScreen;
    private Vector2 targetPosition;
    private Vector2 winScreenPosition;
    private Vector2 lostScreenPosition;
    private CanvasGroup group;
    private Coroutine transition;
    private readonly Dictionary<Transform, List<Transform>> lootRows =
        new Dictionary<Transform, List<Transform>>();

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        transition = null;
        if (shownScreen != null)
        {
            shownScreen.anchoredPosition = shownScreen == winScreen ? winScreenPosition : lostScreenPosition;
            shownScreen.gameObject.SetActive(false);
        }
        shownScreen = null;
        if (group != null) group.alpha = 1f;
    }

    public void BindRoster(CombatManager owner) => manager = owner;

    public void CaptureInitialRoster()
    {
        initialSlots.Clear();
        if (manager == null) return;
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain)
            if (ally != null) initialSlots[ally] = ally.positionRoue;
    }

    public void Initialize(Transform root)
    {
        resultRoot = root;
        if (resultRoot == null) return;
        winScreen = resultRoot.Find("WinScreen") as RectTransform;
        lostScreen = resultRoot.Find("LostScreen") as RectTransform;
        if (winScreen != null)
        {
            BindResultConfirmation(winScreen);
            winScreenPosition = winScreen.anchoredPosition;
            winScreen.gameObject.SetActive(false);
        }
        if (lostScreen != null)
        {
            BindResultConfirmation(lostScreen);
            lostScreenPosition = lostScreen.anchoredPosition;
            lostScreen.gameObject.SetActive(false);
        }
        NormalizeRewardSlots(winScreen);
        NormalizeRewardSlots(lostScreen);
        PrepareLootScrollContainer(winScreen);
        PrepareLootScrollContainer(lostScreen);
        resultRoot.gameObject.SetActive(false);
    }

    private void BindResultConfirmation(RectTransform screen)
    {
        Button button = screen.GetComponent<Button>();
        if (button == null) button = screen.gameObject.AddComponent<Button>();
        Navigation navigation = button.navigation;
        navigation.mode = Navigation.Mode.None;
        button.navigation = navigation;
        button.onClick.AddListener(() =>
        {
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
            if (shownScreen == screen && manager != null) manager.ConfirmerAction();
        });
    }

    [SubscribeEvent]
    private void OnCombatEnded(CombatEndedEvent result)
    {
        if (resultRoot == null || result == null) return;
        shownScreen = result.Victory ? winScreen : lostScreen;
        if (shownScreen == null) return;
        if (winScreen != null) winScreen.gameObject.SetActive(result.Victory);
        if (lostScreen != null) lostScreen.gameObject.SetActive(!result.Victory);
        PopulateRewardSlots(shownScreen);
        if (manager != null)
        {
            Transform canvasRoot = resultRoot.parent;
            Transform battleUI = canvasRoot != null ? canvasRoot.Find("Interface") : null;
            Transform legacyUI = canvasRoot != null ? canvasRoot.Find("interface") : null;
            if (battleUI != null) battleUI.gameObject.SetActive(false);
            if (legacyUI != null) legacyUI.gameObject.SetActive(false);
        }
        UpdateRewardPlaceholders(shownScreen, result);
        shownScreen.gameObject.SetActive(true);
        resultRoot.gameObject.SetActive(true);
        targetPosition = result.Victory ? winScreenPosition : lostScreenPosition;
        RectTransform parent = shownScreen.parent as RectTransform;
        float height = parent != null ? parent.rect.height : Screen.height;
        shownScreen.anchoredPosition = targetPosition + Vector2.up * (height + shownScreen.rect.height);
        group = shownScreen.GetComponent<CanvasGroup>();
        if (group == null) group = shownScreen.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(ShowPanel());
    }

    private static List<Transform> GetRewardSlots(Transform screen)
    {
        var slots = new List<Transform>();
        Transform container = screen != null ? screen.Find("Content/YokaiRoue/YokaiSlotsContainer") : null;
        if (container == null) return slots;
        for (int row = 1; row <= 2; row++)
            for (int slot = 1; slot <= 3; slot++)
            {
                Transform content = container.Find($"Container{row}/Slot{slot}/YoKaiSlot");
                if (content != null) slots.Add(content);
            }
        return slots;
    }

    private static void NormalizeRewardSlots(Transform screen)
    {
        // Slot1/2/3 own the authored wheel positions and persistent backgrounds.
        // Their YoKaiSlot children are complete views, shown only for occupied places.
        foreach (Transform slot in GetRewardSlots(screen)) slot.gameObject.SetActive(false);
    }

    private void PopulateRewardSlots(Transform screen)
    {
        List<Transform> slots = GetRewardSlots(screen);
        List<CombatYokai> allies = manager != null ? manager.listeYoKaiAmiSurLeTerrain
            .Where(ally => ally != null).OrderBy(ally => ally.positionRoue).ToList() : new List<CombatYokai>();
        for (int i = 0; i < slots.Count; i++)
        {
            Transform slot = slots[i];
            CombatYokai ally = allies.FirstOrDefault(yokai =>
                (initialSlots.TryGetValue(yokai, out int originalSlot) ? originalSlot : yokai.positionRoue) == i);
            slot.gameObject.SetActive(ally != null);
            if (ally == null) continue;
            Transform contentRoot = slot;
            Image portrait = contentRoot.Find("YoKaiImages")?.GetComponent<Image>();
            if (portrait != null)
            {
                portrait.sprite = ally.imageFace;
                portrait.preserveAspect = true;
                portrait.gameObject.SetActive(ally.imageFace != null);
            }
            TMP_Text label = contentRoot.Find("Bubble/YoKaiName")?.GetComponent<TMP_Text>();
            if (label != null) label.text = ally.nom;
            Transform type = contentRoot.Find("SpriteType");
            if (type != null) type.gameObject.SetActive(false);
            Transform xp = contentRoot.Find("XP");
            if (xp != null)
            {
                float required = ally.NombreDePointXPAGagner(ally.niveau + 1);
                foreach (Image image in xp.GetComponentsInChildren<Image>(true))
                    if (image.type == Image.Type.Filled)
                        image.fillAmount = required > 0 ? Mathf.Clamp01(ally.xp / required) : 0;
            }
        }
    }

    private void UpdateRewardPlaceholders(Transform screen, CombatEndedEvent result)
    {
        TMP_Text experience = screen.Find("Content/Loot/Container1/Content/Bars/Text")
            ?.GetComponent<TMP_Text>();
        if (experience != null)
        {
            experience.text = result.Experience.ToString("N0");
            experience.alignment = TextAlignmentOptions.Right;
        }

        Transform moneyPanel = screen.Find("Money");
        if (moneyPanel != null) moneyPanel.gameObject.SetActive(result.Money.HasValue);
        if (result.Money.HasValue)
        {
            TMP_Text money = screen.Find("Money/Text (TMP)")?.GetComponent<TMP_Text>();
            if (money != null)
            {
                money.text = result.Money.Value.ToString("N0");
                money.alignment = TextAlignmentOptions.Right;
            }
        }

        Transform lootPanel = screen.Find("Content/Loot/ItemContainer");
        bool hasLoot = result.Loot != null && result.Loot.Count > 0;
        if (lootPanel != null) lootPanel.gameObject.SetActive(hasLoot);
        Transform content = GetLootRowsContainer(screen);
        if (content == null) return;

        List<Transform> rows;
        if (!lootRows.TryGetValue(content, out rows))
        {
            rows = new List<Transform>(content.childCount);
            for (int i = 0; i < content.childCount; i++) rows.Add(content.GetChild(i));
            lootRows.Add(content, rows);
        }

        if (rows.Count == 0) return;
        if (!hasLoot)
        {
            for (int i = 0; i < rows.Count; i++)
                if (rows[i] != null) rows[i].gameObject.SetActive(false);
            return;
        }

        Transform template = rows[0];
        while (rows.Count < result.Loot.Count)
        {
            Transform row = Instantiate(template.gameObject, content).transform;
            row.name = template.name;
            rows.Add(row);
        }

        for (int i = 0; i < rows.Count; i++)
        {
            Transform row = rows[i];
            if (row == null) continue;
            bool visible = i < result.Loot.Count;
            row.gameObject.SetActive(visible);
            if (!visible) continue;

            CombatLootItem item = result.Loot[i];
            row.gameObject.SetActive(true);
            Image icon = row.Find("ItemIcon")?.GetComponent<Image>();
            if (icon != null) icon.sprite = item.Icon;
            TMP_Text label = row.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = item.DisplayName + " × " + item.Quantity;
        }

        ScrollRect scroll = content.parent != null ? content.parent.GetComponent<ScrollRect>() : null;
        if (scroll != null) scroll.verticalNormalizedPosition = 1f;
    }

    private static void PrepareLootScrollContainer(Transform screen)
    {
        if (screen == null) return;
        Transform viewport = screen.Find("Content/Loot/ItemContainer/Content");
        if (viewport == null) return;

        ScrollRect scroll = viewport.GetComponent<ScrollRect>();
        if (scroll == null) return;
        RectTransform viewportRect = viewport as RectTransform;
        if (viewportRect == null) return;

        RectTransform rows = scroll.content;
        if (rows == null || rows == viewportRect)
        {
            Transform existingRows = viewport.Find("Items");
            if (existingRows == null)
            {
                GameObject rowsObject = new GameObject("Items", typeof(RectTransform));
                existingRows = rowsObject.transform;
                existingRows.SetParent(viewport, false);
            }
            rows = existingRows as RectTransform;
            if (rows == null) return;

            for (int i = viewport.childCount - 1; i >= 0; i--)
            {
                Transform child = viewport.GetChild(i);
                if (child == rows) continue;
                child.SetParent(rows, false);
            }
        }

        scroll.viewport = viewportRect;
        scroll.content = rows;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        RectMask2D mask = viewport.GetComponent<RectMask2D>();
        if (mask == null) viewport.gameObject.AddComponent<RectMask2D>();

        rows.anchorMin = new Vector2(0f, 1f);
        rows.anchorMax = new Vector2(1f, 1f);
        rows.pivot = new Vector2(0.5f, 1f);
        rows.anchoredPosition = Vector2.zero;
        rows.sizeDelta = Vector2.zero;

        VerticalLayoutGroup layout = rows.GetComponent<VerticalLayoutGroup>();
        if (layout == null) layout = rows.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter sizeFitter = rows.GetComponent<ContentSizeFitter>();
        if (sizeFitter == null) sizeFitter = rows.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private static Transform GetLootRowsContainer(Transform screen)
    {
        Transform viewport = screen.Find("Content/Loot/ItemContainer/Content");
        if (viewport == null) return null;
        ScrollRect scroll = viewport.GetComponent<ScrollRect>();
        return scroll != null && scroll.content != null ? scroll.content : viewport;
    }

    private IEnumerator ShowPanel()
    {
        yield return CombatUIAnimator.Move(shownScreen, targetPosition, transitionDuration, true, transitionEasing);
        transition = null;
    }

    [SubscribeEvent]
    private void OnResultDismissed(CombatResultDismissedEvent dismissed)
    {
        if (shownScreen == null || !shownScreen.gameObject.activeSelf) return;
        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(HidePanel());
    }

    private IEnumerator HidePanel()
    {
        RectTransform parent = shownScreen.parent as RectTransform;
        float height = parent != null ? parent.rect.height : Screen.height;
        yield return CombatUIAnimator.Move(shownScreen,
            targetPosition + Vector2.down * (height + shownScreen.rect.height), transitionDuration, false,
            transitionEasing);
        shownScreen.gameObject.SetActive(false);
        transition = null;
    }
}
