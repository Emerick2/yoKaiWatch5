using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Initializes the combat UI components from their serialized scene references.</summary>
public sealed class CombatUiManager : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Canvas sceneCanvas;
    [SerializeField] private CombatMessageUI messageController;
    [SerializeField] private CombatResultUI resultController;
    [SerializeField] private CombatBagUI bagController;
    [SerializeField] private CombatSlotAnimator slotController;
    [SerializeField] private CombatActorMessageUI actorMessageController;
    [SerializeField] private RectTransform messagePanel;
    [SerializeField] private RectTransform resultPanel;
    [SerializeField] private RectTransform bagPanel;

    private void Awake()
    {
        if (combatManager != null)
        {
            combatManager.UseModularResultScreen();
            Transform battleUI = sceneCanvas != null ? sceneCanvas.transform.Find("Interface") : null;
            Transform buttons = battleUI != null ? battleUI.Find("blocDesBoutonGénéraux") : null;
            if (buttons != null)
            {
                combatManager.blocDesBoutonGénéraux = buttons.gameObject;
                combatManager.lesOption = new List<GameObject>();
                foreach (Transform button in buttons) combatManager.lesOption.Add(button.gameObject);
            }
            Transform speed = battleUI != null ? battleUI.Find("vitesse") : null;
            if (speed != null) combatManager.imageVitesseAccéléré = speed.GetComponent<Image>();
        }
        if (messageController != null) messageController.Initialize(messagePanel);
        if (resultController != null)
        {
            resultController.BindRoster(combatManager);
            resultController.Initialize(resultPanel);
        }
        if (bagController != null) bagController.Initialize(bagPanel);
        if (combatManager != null)
        {
            List<Image> wheelSlots = ResolveWheelSlots();
            combatManager.lesEspacesMédaillonDansLaRoue = wheelSlots;
            if (slotController != null)
            {
                List<Image> battleSlots = combatManager.lesEspacesMédaillonDansLaRoue
                    .Where(image => image != null &&
                        (resultPanel == null || !image.transform.IsChildOf(resultPanel))).ToList();
                slotController.Initialize(battleSlots, sceneCanvas);
            }
        }
        if (actorMessageController != null && combatManager != null)
            actorMessageController.Initialize(combatManager);
    }

    private List<Image> ResolveWheelSlots()
    {
        if (sceneCanvas == null) return new List<Image>();
        return sceneCanvas.GetComponentsInChildren<Image>(true)
            .Where(image => image != null && image.name == "YoKaiSlot" &&
                (resultPanel == null || !image.transform.IsChildOf(resultPanel))).ToList();
    }
}
