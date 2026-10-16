using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YKW.Core.Events;

/// <summary>Updates the existing battle bubbles in response to gameplay messages.</summary>
public sealed class CombatActorMessageUI : EventSubscriberBehaviour
{
    private sealed class BubbleView
    {
        public GameObject Root;
        public TMP_Text Text;
        public Image Icon;
        public Animator Animator;
    }

    private CombatManager manager;
    private readonly List<BubbleView> friendlyBubbles = new List<BubbleView>();
    private readonly List<BubbleView> enemyBubbles = new List<BubbleView>();

    public void Initialize(CombatManager combatManager)
    {
        if (combatManager == null) return;
        manager = combatManager;
        CacheBubbles(combatManager.troisDisplateUIAmi, friendlyBubbles);
        CacheBubbles(combatManager.troisDisplateUIEnnemie, enemyBubbles);
    }

    [SubscribeEvent]
    private void OnActorMessage(CombatActorMessageEvent message)
    {
        if (message == null) return;
        if (manager != null)
        {
            CacheBubbles(manager.troisDisplateUIAmi, friendlyBubbles);
            CacheBubbles(manager.troisDisplateUIEnnemie, enemyBubbles);
        }
        List<BubbleView> bubbles = message.IsFriendly ? friendlyBubbles : enemyBubbles;
        if (message.SlotIndex < 0 || message.SlotIndex >= bubbles.Count) return;

        BubbleView bubble = bubbles[message.SlotIndex];
        if (bubble == null || bubble.Root == null) return;
        bubble.Root.SetActive(message.IsVisible);
        if (!message.IsVisible) return;

        if (bubble.Text != null) bubble.Text.text = message.Text;
        if (bubble.Icon != null) bubble.Icon.sprite = message.Icon;
        if (HasAnimatorTrigger(bubble.Animator, "ouvre"))
            bubble.Animator.SetTrigger("ouvre");
    }

    private static void CacheBubbles(IList<GameObject> displays, List<BubbleView> destination)
    {
        destination.Clear();
        if (displays == null) return;
        for (int i = 0; i < displays.Count; i++)
        {
            GameObject display = displays[i];
            Transform bubble = display != null ? display.transform.Find("bulle") : null;
            if (bubble == null)
            {
                destination.Add(null);
                continue;
            }

            Transform text = bubble.Find("t");
            Transform icon = bubble.Find("Image");
            destination.Add(new BubbleView
            {
                Root = bubble.gameObject,
                Text = text != null ? text.GetComponent<TMP_Text>() : null,
                Icon = icon != null ? icon.GetComponent<Image>() : null,
                Animator = bubble.GetComponent<Animator>()
            });
        }
    }

    private static bool HasAnimatorTrigger(Animator animator, string triggerName)
    {
        if (animator == null || !animator.isActiveAndEnabled || !animator.isInitialized || animator.runtimeAnimatorController == null) return false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.type == AnimatorControllerParameterType.Trigger && parameter.name == triggerName)
                return true;
        return false;
    }
}
