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

    public bool TryInitialiser(){
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
    void Update(){
        if (activé){
            if (Input.GetButtonDown("left")){
                ChangerCible(-1);
            } else if (Input.GetButtonDown("right")){
                ChangerCible(1);
            } else if (Input.GetButtonDown("A")){
                Input.ResetInputAxes();
                EventBus.Publish(new CombatAllyTargetSelectedEvent(cible));
            }
        }
    }

    bool ChangerCible(int direction){
        if (CombatManager == null || CombatManager.troisDisplateUIAmi == null) return false;
        List<int> slots = new List<int>();
        int slotCount = Mathf.Min(3, CombatManager.troisDisplateUIAmi.Count);
        for (int i = 0; i < slotCount; i++)
        {
            GameObject card = CombatManager.troisDisplateUIAmi[i];
            if (card != null && CombatManager.GetAllyAtWheelSlot(i) != null && card.transform.Find("point") != null)
                slots.Add(i);
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
        Transform point = CombatManager.troisDisplateUIAmi[cible].transform.Find("point");
        point.gameObject.SetActive(true);
        if (dernierCible != null) dernierCible.SetActive(false);
        dernierCible = point.gameObject;
        return true;
    }
}
