using System.Collections.Generic;
using UnityEngine;

namespace YKW.Core.Events
{
    /// <summary>Selection made with the free combat pointer, including non-enemy objects.</summary>
    public sealed class CombatObjectTargetSelectedEvent
    {
        public GameObject Target { get; }
        public CombatObjectTargetSelectedEvent(GameObject target) { Target = target; }
    }

    public sealed class CombatRotateRequestedEvent
    {
        public int Direction { get; private set; }
        public CombatRotateRequestedEvent(int direction) { Direction = direction; }
    }

    public sealed class CombatOptionChangeRequestedEvent
    {
        public int Direction { get; private set; }
        public CombatOptionChangeRequestedEvent(int direction) { Direction = direction; }
    }

    public sealed class CombatConfirmRequestedEvent { }
    public sealed class CombatCancelRequestedEvent { }
    public sealed class CombatSpeedToggleRequestedEvent { }

    public sealed class CombatAllyTargetSelectedEvent
    {
        public int AllyIndex { get; private set; }
        public CombatAllyTargetSelectedEvent(int allyIndex) { AllyIndex = allyIndex; }
    }

    public sealed class CombatFoodSelectedEvent
    {
        public int FoodIndex { get; private set; }
        public CombatFoodSelectedEvent(int foodIndex) { FoodIndex = foodIndex; }
    }

    /// <summary>Requests a Soultimate minigame for the selected persistent ally.</summary>
    public sealed class CombatSoultimateRequestedEvent
    {
        public int AllyId { get; private set; }
        public CombatSoultimateRequestedEvent(int allyId) { AllyId = allyId; }
    }

    /// <summary>Requests a purification minigame for the selected persistent ally.</summary>
    public sealed class CombatPurificationRequestedEvent
    {
        public int AllyId { get; private set; }
        public CombatPurificationRequestedEvent(int allyId) { AllyId = allyId; }
    }

    public enum CombatMinigameKind
    {
        Soultimate,
        Purification
    }

    /// <summary>Reports the outcome of a combat minigame to the battle state.</summary>
    public sealed class CombatMinigameCompletedEvent
    {
        public int AllyId { get; private set; }
        public CombatMinigameKind Kind { get; private set; }
        public bool Succeeded { get; private set; }

        public CombatMinigameCompletedEvent(int allyId, CombatMinigameKind kind, bool succeeded)
        {
            AllyId = allyId;
            Kind = kind;
            Succeeded = succeeded;
        }
    }

    public sealed class CombatStartedEvent : MethodEvent { }

    public sealed class CombatOutcomeAnnouncementEvent
    {
        public bool Victory { get; }
        public float Duration { get; }
        public CombatOutcomeAnnouncementEvent(bool victory, float duration)
        { Victory = victory; Duration = duration; }
    }

    public sealed class CombatOutcomeAnnouncementDismissedEvent { }

    public sealed class CombatEndedEvent : MethodEvent
    {
        public bool Victory { get; private set; }
        public int Experience { get; private set; }
        public int? Money { get; private set; }
        public IReadOnlyList<CombatLootItem> Loot { get; private set; }

        public CombatEndedEvent(bool victory, int experience, int? money = null,
            IReadOnlyList<CombatLootItem> loot = null)
        {
            Victory = victory;
            Experience = experience;
            Money = money;
            Loot = loot;
        }
    }

    /// <summary>Published in the restored exploration scene after returning from combat.</summary>
    public sealed class CombatReturnedEvent
    {
        public bool Victory { get; }
        public string SceneName { get; }
        public CombatReturnedEvent(bool victory, string sceneName)
        { Victory = victory; SceneName = sceneName; }
    }

    public sealed class CombatResultDismissedEvent { }

    public sealed class CombatLootItem
    {
        public string ItemId { get; private set; }
        public string DisplayName { get; private set; }
        public int Quantity { get; private set; }
        public Sprite Icon { get; private set; }

        public CombatLootItem(string itemId, string displayName, int quantity, Sprite icon = null)
        {
            ItemId = itemId;
            DisplayName = displayName;
            Quantity = quantity;
            Icon = icon;
        }
    }

    public sealed class CombatMessageEvent
    {
        public string Text { get; private set; }
        public float Duration { get; private set; }
        public CombatMessageEvent(string text, float duration = 2.5f)
        {
            Text = text ?? string.Empty;
            Duration = duration;
        }
    }

    public sealed class CombatActorMessageEvent
    {
        public bool IsFriendly { get; private set; }
        public int SlotIndex { get; private set; }
        public string Text { get; private set; }
        public Sprite Icon { get; private set; }
        public bool IsVisible { get; private set; }

        public CombatActorMessageEvent(bool isFriendly, int slotIndex, bool isVisible,
            string text = null, Sprite icon = null)
        {
            IsFriendly = isFriendly;
            SlotIndex = slotIndex;
            IsVisible = isVisible;
            Text = text ?? string.Empty;
            Icon = icon;
        }
    }

    public sealed class CombatBagVisibilityEvent
    {
        public bool IsOpen { get; private set; }
        public CombatBagVisibilityEvent(bool isOpen) { IsOpen = isOpen; }
    }

    public sealed class CombatSlotChangedEvent
    {
        public int YoKaiId { get; private set; }
        public int FromSlot { get; private set; }
        public int ToSlot { get; private set; }
        public CombatSlotChangedEvent(int yoKaiId, int fromSlot, int toSlot)
        {
            YoKaiId = yoKaiId;
            FromSlot = fromSlot;
            ToSlot = toSlot;
        }
    }

    public sealed class CombatSlotsChangedEvent
    {
        public IReadOnlyList<SlotChange> Changes { get; private set; }
        public CombatSlotsChangedEvent(IReadOnlyList<SlotChange> changes) { Changes = changes; }
    }

    public struct SlotChange
    {
        public readonly int StableId;
        public readonly int FromSlot;
        public readonly int ToSlot;
        public SlotChange(int stableId, int fromSlot, int toSlot)
        {
            StableId = stableId;
            FromSlot = fromSlot;
            ToSlot = toSlot;
        }
    }
}
