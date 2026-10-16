using System.Collections;
using UnityEngine;
using YKW.Core.Item.Class;
using YKW.Core.YoKai.Class;
using StructureYoKaiCombat = YKW.Core.YoKai.Class.YoKaiCombat;

/// <summary>Handles player commands and the choice/action UI state during combat.</summary>
public sealed class CombatActionController : MonoBehaviour
{
    private CombatManager manager;
    private int selectedOptionIndex;
    private int fastForwardSpriteIndex;
    private bool isFastForwardEnabled;
    private ActionState actionState;

    private enum ActionState
    {
        None,
        Target,
        Bag,
        Soultimate,
        Purification
    }
    private int selectedFoodIndex;

    public void Initialize(CombatManager owner)
    {
        manager = owner;
    }

    public void InitializeUI()
    {
        actionState = ActionState.None;
        StopSpeed();
        RefreshOptionSelection();
    }

    public void StopSpeed()
    {
        isFastForwardEnabled = false;
        Time.timeScale = 1f;
        if (manager.imageVitesseAccéléré != null)
            manager.imageVitesseAccéléré.gameObject.SetActive(false);
    }

    public void ToggleFastForward()
    {
        if (!manager.CombatEnCours || manager.imageVitesseAccéléré == null ||
            manager.spritePourLaVitesse == null || manager.spritePourLaVitesse.Count == 0)
            return;

        if (isFastForwardEnabled)
        {
            isFastForwardEnabled = false;
            manager.imageVitesseAccéléré.gameObject.SetActive(false);
            Time.timeScale = 1;
            return;
        }

        manager.imageVitesseAccéléré.gameObject.SetActive(true);
        Time.timeScale = 2;
        isFastForwardEnabled = true;
        StartCoroutine(FastForwardIconRoutine());
    }

    public void ChangeSelectedOption(int direction)
    {
        if (!manager.CombatEnCours || actionState != ActionState.None)
            return;

        selectedOptionIndex += direction;
        RefreshOptionSelection();
    }
    public void ConfirmAction()
    {
        if (!manager.CombatEnCours)
        {
            manager.ConfirmCombatResult();
            return;
        }

        if (actionState != ActionState.None)
            return;

        switch (selectedOptionIndex)
        {
            case 0:
                actionState = ActionState.Soultimate;
                OpenAllySelector();
                break;
            case 1:
                if (manager.cible == null || manager.cible.GetComponent<Cible>() == null)
                {
                    Debug.LogError("Target action is unavailable because its Cible reference is missing.");
                    return;
                }
                actionState = ActionState.Target;
                if (manager.aAfficherSeulementAvecLaCible != null)
                    foreach (GameObject objectToShow in manager.aAfficherSeulementAvecLaCible)
                        if (objectToShow != null) objectToShow.SetActive(true);
                manager.cible.GetComponent<Cible>().Initialisation();
                manager.cible.SetActive(true);
                if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(false);
                break;
            case 2:
                actionState = ActionState.Purification;
                OpenAllySelector();
                break;
            case 3:
                manager.gameInStop = true;
                actionState = ActionState.Bag;
                SacCombats bag = manager.GetComponent<SacCombats>();
                if (bag == null)
                {
                    Debug.LogError("Bag action is unavailable because the SacCombats component is missing.");
                    AbortBagSelection();
                    return;
                }
                bag.Initialiser();
                if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(false);
                break;
        }
    }
    public void CancelAction()
    {
        if (!manager.CombatEnCours)
            return;

        if (actionState == ActionState.Target)
        {
            if (manager.aAfficherSeulementAvecLaCible != null)
                foreach (GameObject objectToHide in manager.aAfficherSeulementAvecLaCible)
                    if (objectToHide != null) objectToHide.SetActive(false);
            if (manager.cible != null) manager.cible.SetActive(false);
            if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
        }

        if (actionState == ActionState.Bag)
        {
            manager.gameInStop = false;
            manager.GetComponent<ChoisieYoKaiAmi>()?.Arrêter();
            manager.GetComponent<SacCombats>()?.Arrêter();
            if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
        }

        if (actionState == ActionState.Soultimate)
        {
            manager.gameInStop = false;
            manager.GetComponent<ChoisieYoKaiAmi>()?.Arrêter();
            if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
        }

        if (actionState == ActionState.Purification)
        {
            manager.gameInStop = false;
            manager.GetComponent<ChoisieYoKaiAmi>()?.Arrêter();
            if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
        }

        actionState = ActionState.None;
    }
    public void SelectFood(int foodIndex){
        if (Item.Instance == null || foodIndex < 0 || foodIndex >= Item.Instance.listeDesNouritures.Count)
        {
            Debug.LogWarning("Food selection ignored: invalid index or unavailable inventory.");
            AbortBagSelection();
            return;
        }
        selectedFoodIndex = foodIndex;
        manager.GetComponent<SacCombats>().Arrêter();
        OpenAllySelector();
    }

    public void CompleteAllySelection(int target){
        if (actionState == ActionState.None) return;
        StructureYoKaiCombat ally = manager.GetAllyAtWheelSlot(target);
        if (ally == null)
        {
            Debug.LogWarning($"Ally selection ignored: no ally occupies slot {target}.");
            CancelAllySelection();
            return;
        }

        ActionState selectedAction = actionState;
        manager.gameInStop = false;
        if (selectedAction == ActionState.Bag){
            if (Item.Instance == null || selectedFoodIndex < 0 ||
                selectedFoodIndex >= Item.Instance.listeDesNouritures.Count || NewDataCenter.Instance == null)
            {
                Debug.LogWarning("Food use cancelled because the inventory changed during selection.");
                AbortBagSelection();
                return;
            }
            Nouriture food = Item.Instance.listeDesNouritures[selectedFoodIndex];
            int quantity;
            if (food == null ||
                !int.TryParse(NewDataCenter.Instance.Lire(food.nom), out quantity) || quantity <= 0)
            {
                Debug.LogWarning("Food use cancelled because this item is no longer in the inventory.");
                AbortBagSelection();
                return;
            }

            (var restoredHp, var restoredSoultimate) =
                YoKaiCombat.EffetNourritureAllié(food, ally.hp_max_me);

            ally.amultime = Mathf.Clamp(ally.amultime + restoredSoultimate, 0, 100);
            ally.ModifierPV(restoredHp);
            NewDataCenter.Instance.SauvegarderSTR(food.nom, (quantity - 1).ToString());

            selectedFoodIndex = -1;
        }
        else if (selectedAction == ActionState.Soultimate)
            YKW.Core.Events.EventBus.Publish(new YKW.Core.Events.CombatSoultimateRequestedEvent(ally.IDListeYoKai));
        else if (selectedAction == ActionState.Purification)
            YKW.Core.Events.EventBus.Publish(new YKW.Core.Events.CombatPurificationRequestedEvent(ally.IDListeYoKai));

        actionState = ActionState.None;
        manager.GetComponent<ChoisieYoKaiAmi>().Arrêter();
        if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
        manager.RefreshCombatUI();
    }
    public void AbortBagSelection()
    {
        if (actionState != ActionState.Bag) return;
        selectedFoodIndex = -1;
        actionState = ActionState.None;
        manager.gameInStop = false;
        SacCombats bag = manager.GetComponent<SacCombats>();
        if (bag != null) bag.Arrêter();
        ChoisieYoKaiAmi selector = manager.GetComponent<ChoisieYoKaiAmi>();
        if (selector != null) selector.Arrêter();
        if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
    }

    private void OpenAllySelector()
    {
        manager.gameInStop = true;
        ChoisieYoKaiAmi selector = manager.GetComponent<ChoisieYoKaiAmi>();
        if (selector == null || !selector.TryInitialiser())
        {
            actionState = ActionState.None;
            manager.gameInStop = false;
            manager.GetComponent<SacCombats>()?.Arrêter();
            if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
            return;
        }
        if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(false);
    }

    private void CancelAllySelection()
    {
        if (actionState == ActionState.Bag) manager.gameInStop = false;
        actionState = ActionState.None;
        ChoisieYoKaiAmi selector = manager.GetComponent<ChoisieYoKaiAmi>();
        if (selector != null) selector.Arrêter();
        SacCombats bag = manager.GetComponent<SacCombats>();
        if (bag != null) bag.Arrêter();
        if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(true);
    }
    private void RefreshOptionSelection(){
        if (manager == null || manager.lesOption == null || manager.lesOption.Count == 0) return;
        selectedOptionIndex = ((selectedOptionIndex%manager.lesOption.Count)+manager.lesOption.Count)%manager.lesOption.Count;
        for (int i = 0; i < manager.lesOption.Count; i++){
            GameObject option = manager.lesOption[i];
            if (option == null) continue;
            Transform cursor = option.transform.Find("cible");
            Transform main = option.transform.Find("main");
            if (cursor != null) cursor.gameObject.SetActive(selectedOptionIndex==i);
            if (main != null) main.gameObject.SetActive(selectedOptionIndex==i);
        }
    }
    private IEnumerator FastForwardIconRoutine(){
        while (isFastForwardEnabled && manager.CombatEnCours && manager.imageVitesseAccéléré != null &&
               manager.spritePourLaVitesse != null && manager.spritePourLaVitesse.Count > 0){
            manager.imageVitesseAccéléré.gameObject.SetActive(true);
            manager.imageVitesseAccéléré.sprite = manager.spritePourLaVitesse[fastForwardSpriteIndex];
            fastForwardSpriteIndex = (fastForwardSpriteIndex+1)%manager.spritePourLaVitesse.Count;
            yield return new WaitForSeconds(0.25f);
        }
        if (manager.imageVitesseAccéléré != null) manager.imageVitesseAccéléré.gameObject.SetActive(false);
    }
}
