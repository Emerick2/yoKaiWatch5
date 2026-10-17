using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;

public class ChoisieYoKaiAmi : MonoBehaviour{
    private CombatManager CombatManager = null;
    private int cible = 0;
    private GameObject dernierCible = null;
    private bool activé = false;
    void Start(){
        CombatManager = GetComponent<CombatManager>();
        if (CombatManager == null) return;
        if (CombatManager.troisDisplateUIAmi != null)
            foreach (GameObject card in CombatManager.troisDisplateUIAmi)
                card?.transform.Find("point")?.gameObject.SetActive(false);
        if (CombatManager.troisDisplateUIEnnemie != null)
            foreach (GameObject card in CombatManager.troisDisplateUIEnnemie)
                card?.transform.Find("point")?.gameObject.SetActive(false);
    }
    public void Initialiser() => TryInitialiser();

    public bool TryInitialiser(bool reserveOnly = false){
        this.reserveOnly = reserveOnly;
        if (CombatManager == null) CombatManager = GetComponent<CombatManager>();
        if (CombatManager == null || CombatManager.troisDisplateUIAmi == null) return false;
        Arrêter();
        cible = 0;
        activé = true;
        if (!ChangerCible(0))
        {
            activé = false;
            return false;
        }
        return true;
    }
    public void Arrêter(){
        activé = false;
        if (dernierCible != null) dernierCible.SetActive(false);
        dernierCible = null;
    }
    public bool IsSelecting => activé;
    private bool reserveOnly;
    public void Move(int direction) { if (activé) ChangerCible(direction); }
    public void Confirm()
    {
        if (activé) EventBus.Publish(new CombatAllyTargetSelectedEvent(cible));
    }

    public void SelectCard(GameObject card)
    {
        if (!activé) return;
        for (int slot = reserveOnly ? 3 : 0; slot < (reserveOnly ? 6 : 3); slot++)
        {
            var ally = CombatManager.GetAllyAtWheelSlot(slot);
            if (GetCard(slot) != card || ally == null || ally.hp_me <= 0) continue;
            cible = slot;
            Confirm();
            return;
        }
    }

    private GameObject GetCard(int slot)
    {
        var cards = slot < 3 ? CombatManager.troisDisplateUIAmi : CombatManager.troisDisplateUIAmiDerrière;
        int index = slot < 3 ? slot : 5 - slot;
        return cards != null && index < cards.Count ? cards[index] : null;
    }

    bool ChangerCible(int direction){
        if (CombatManager == null || CombatManager.troisDisplateUIAmi == null) return false;
        List<int> slots = new List<int>();
        for (int i = reserveOnly ? 3 : 0; i < (reserveOnly ? 6 : 3); i++)
        {
            var ally = CombatManager.GetAllyAtWheelSlot(i);
            GameObject card = GetCard(i);
            if (ally == null || ally.hp_me <= 0 || card == null) continue;
            Transform marker = card.transform.Find("point");
            if (marker == null) marker = CombatManager.GetComponent<CombatTargetPointer>()?.EnsureSelectionMarker(card);
            if (marker != null) slots.Add(i);
        }
        if (slots.Count == 0)
        {
            if (dernierCible != null) dernierCible.SetActive(false);
            dernierCible = null;
            return false;
        }
        int index = slots.IndexOf(cible);
        if (index < 0) index = direction < 0 ? slots.Count - 1 : 0;
        else index = ((index + direction) % slots.Count + slots.Count) % slots.Count;
        cible = slots[index];
        Transform point = GetCard(cible).transform.Find("point");
        if (dernierCible != null) dernierCible.SetActive(false);
        point.gameObject.SetActive(true);
        dernierCible = point.gameObject;
        return true;
    }
}
