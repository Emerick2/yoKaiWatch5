using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YKW.Core.Events;

/// <summary>Queues combat messages and animates the existing Message placeholder.</summary>
public sealed class CombatMessageUI : EventSubscriberBehaviour
{
    private struct QueuedMessage
    {
        public string Text;
        public float Duration;
        public QueuedMessage(string text, float duration) { Text = text; Duration = duration; }
    }

    [SerializeField, Min(0.05f)] private float transitionDuration = 0.42f;
    [SerializeField, Min(0f)] private float defaultHoldDuration = 2.5f;
    [SerializeField] private AnimationCurve panelEasing = null;
    [SerializeField] private AnimationCurve textFadeEasing = null;
    private readonly Queue<QueuedMessage> messages = new Queue<QueuedMessage>();
    private RectTransform panel;
    private RectTransform parentRect;
    private CanvasGroup panelGroup;
    private CanvasGroup textGroup;
    private Graphic textGraphic;
    private Vector2 targetPosition;
    private Coroutine queueRoutine;

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        queueRoutine = null;
        messages.Clear();
        SetOutcomeVisuals(false, false);
        if (panel != null)
        {
            panel.anchoredPosition = targetPosition;
            panel.gameObject.SetActive(false);
        }
        if (panelGroup != null) panelGroup.alpha = 1f;
        if (textGroup != null) textGroup.alpha = 1f;
    }

    public void Initialize(Transform panelTransform)
    {
        if (panelTransform == null) return;
        panel = panelTransform as RectTransform;
        if (panel == null) return;
        parentRect = panel.parent as RectTransform;
        panelGroup = GetOrAddCanvasGroup(panel.gameObject);
        textGraphic = panel.GetComponentInChildren<TMP_Text>(true);
        if (textGraphic == null) textGraphic = panel.GetComponentInChildren<Text>(true);
        if (textGraphic != null) textGroup = GetOrAddCanvasGroup(textGraphic.gameObject);
        targetPosition = panel.anchoredPosition;
    }

    [SubscribeEvent]
    private void OnCombatMessage(CombatMessageEvent message)
    {
        if (panel == null || message == null) return;
        messages.Enqueue(new QueuedMessage(message.Text,
            message.Duration > 0f ? message.Duration : defaultHoldDuration));
        if (queueRoutine == null) queueRoutine = StartCoroutine(PlayQueue());
    }

    [SubscribeEvent]
    private void OnCombatStarted(CombatStartedEvent started)
    {
        OnCombatMessage(new CombatMessageEvent("Le combat commence !", 1.4f));
    }

    [SubscribeEvent]
    private void OnOutcomeAnnouncement(CombatOutcomeAnnouncementEvent outcome)
    {
        if (panel == null) return;
        StopAllCoroutines();
        queueRoutine = null;
        messages.Clear();
        SetOutcomeVisuals(true, outcome.Victory);
        SetText(outcome.Victory ? "Victoire !" : "Défaite…");
        panel.gameObject.SetActive(true);
        panel.SetAsLastSibling();
        panel.anchoredPosition = StartAbovePosition();
        panelGroup.alpha = 1f;
        if (textGroup != null) textGroup.alpha = 1f;
        StartCoroutine(CombatUIAnimator.Move(panel, targetPosition, transitionDuration, true, panelEasing));
    }

    [SubscribeEvent]
    private void OnOutcomeDismissed(CombatOutcomeAnnouncementDismissedEvent dismissed)
    {
        StopAllCoroutines();
        messages.Clear();
        queueRoutine = null;
        SetOutcomeVisuals(false, false);
        if (panel != null)
        {
            panel.gameObject.SetActive(false);
            panel.anchoredPosition = targetPosition;
        }
    }

    private void SetOutcomeVisuals(bool visible, bool victory)
    {
        if (panel == null) return;
        panel.Find("win")?.gameObject.SetActive(visible && victory);
        panel.Find("lose")?.gameObject.SetActive(visible && !victory);
    }

    private IEnumerator PlayQueue()
    {
        while (messages.Count > 0 && panel != null)
        {
            QueuedMessage message = messages.Dequeue();
            SetText(message.Text);
            panel.gameObject.SetActive(true);
            panel.anchoredPosition = StartAbovePosition();
            panelGroup.alpha = 1f;
            if (textGroup != null) textGroup.alpha = 0f;

            StartCoroutine(CombatUIAnimator.Move(panel, targetPosition, transitionDuration, true, panelEasing));
            if (textGroup != null)
                StartCoroutine(CombatUIAnimator.Fade(textGroup, 1f, transitionDuration, textFadeEasing));
            yield return WaitForTransition(transitionDuration);
            yield return new WaitForSecondsRealtime(message.Duration);

            if (textGroup != null)
                StartCoroutine(CombatUIAnimator.Fade(textGroup, 0f, transitionDuration * 0.7f, textFadeEasing));
            StartCoroutine(CombatUIAnimator.Move(panel, StartAbovePosition(), transitionDuration * 0.7f,
                false, panelEasing));
            yield return WaitForTransition(transitionDuration * 0.7f);
            panel.gameObject.SetActive(false);
        }
        queueRoutine = null;
    }

    private IEnumerator WaitForTransition(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration) { elapsed += Time.unscaledDeltaTime; yield return null; }
    }

    private Vector2 StartAbovePosition()
    {
        float viewportHeight = parentRect != null ? parentRect.rect.height : Screen.height;
        return targetPosition + Vector2.up * (viewportHeight + panel.rect.height);
    }

    private void SetText(string value)
    {
        TMP_Text tmp = textGraphic as TMP_Text;
        if (tmp != null) tmp.text = value;
        else
        {
            Text legacyText = textGraphic as Text;
            if (legacyText != null) legacyText.text = value;
        }
    }

    private static CanvasGroup GetOrAddCanvasGroup(GameObject target)
    {
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        return group != null ? group : target.AddComponent<CanvasGroup>();
    }
}
