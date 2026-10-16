using System.Collections;
using UnityEngine;
using YKW.Core.Events;

/// <summary>Interruptible slide transition for the existing battle bag panel.</summary>
public sealed class CombatBagUI : EventSubscriberBehaviour
{
    [SerializeField, Min(0.05f)] private float transitionDuration = 0.35f;
    [SerializeField] private AnimationCurve transitionEasing = null;
    private RectTransform panel;
    private Vector2 targetPosition;
    private Coroutine transition;
    private bool initialized;
    private bool isOpen;

    protected override void OnDisable()
    {
        base.OnDisable();
        if (transition != null) StopCoroutine(transition);
        transition = null;
    }

    public void Initialize(Transform panelTransform)
    {
        panel = panelTransform as RectTransform;
        if (panel == null || initialized) return;
        targetPosition = panel.anchoredPosition;
        initialized = true;
    }

    public void Open()
    {
        if (panel == null) panel = transform as RectTransform;
        if (panel == null) return;
        panel.gameObject.SetActive(true);
        EventBus.Publish(new CombatBagVisibilityEvent(true));
    }

    public void Close()
    {
        EventBus.Publish(new CombatBagVisibilityEvent(false));
    }

    [SubscribeEvent]
    private void OnBagVisibilityChanged(CombatBagVisibilityEvent visibility)
    {
        if (panel == null) panel = transform as RectTransform;
        if (panel == null) return;

        if (visibility.IsOpen)
        {
            if (!initialized) Initialize(panel);
            bool shouldEnterFromBelow = !panel.gameObject.activeSelf || !isOpen;
            panel.gameObject.SetActive(true);
            if (shouldEnterFromBelow) panel.anchoredPosition = OffscreenPosition();
            isOpen = true;
            StartTransition(targetPosition, false);
        }
        else
        {
            StartTransition(OffscreenPosition(), true);
        }
    }

    private void StartTransition(Vector2 destination, bool deactivateAtEnd)
    {
        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(Transition(destination, deactivateAtEnd));
    }

    private IEnumerator Transition(Vector2 destination, bool deactivateAtEnd)
    {
        yield return CombatUIAnimator.Move(panel, destination, transitionDuration, false, transitionEasing);
        transition = null;
        if (deactivateAtEnd && panel != null)
        {
            isOpen = false;
            panel.gameObject.SetActive(false);
        }
    }

    private Vector2 OffscreenPosition()
    {
        RectTransform parent = panel.parent as RectTransform;
        float distance = (parent != null ? parent.rect.height : Screen.height) + panel.rect.height;
        return targetPosition + Vector2.down * distance;
    }
}
