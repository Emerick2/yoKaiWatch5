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

    [SerializeField] private Sprite pointerIdleSprite;
    [SerializeField] private Sprite pointerHoverSprite;

    private void Awake()
    {
        if (combatManager != null)
        {
            combatManager.UseModularResultScreen();
            CombatTargetPointer pointer = combatManager.GetComponent<CombatTargetPointer>();
            if (pointer == null) pointer = combatManager.gameObject.AddComponent<CombatTargetPointer>();
            pointer.Initialize(combatManager, sceneCanvas, pointerIdleSprite, pointerHoverSprite);
            SacCombats bag = combatManager.GetComponent<SacCombats>();
            if (bag != null && bagPanel != null)
            {
                bag.blocAppSac = bagPanel.gameObject;
                GridLayoutGroup grid = bagPanel.GetComponentInChildren<GridLayoutGroup>(true);
                if (grid != null) bag.conteneur = grid.gameObject;
                bag.texte = bagPanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true)
                    .FirstOrDefault(text => !text.transform.IsChildOf(bag.conteneur != null ? bag.conteneur.transform : bagPanel));
            }
            Transform battleUI = sceneCanvas != null ? sceneCanvas.transform.Find("Interface") : null;
            Transform buttons = battleUI != null ? battleUI.Find("blocDesBoutonGénéraux") : null;
            if (buttons != null)
            {
                combatManager.blocDesBoutonGénéraux = buttons.gameObject;
                combatManager.lesOption = new List<GameObject>();
                string[] names = { "bouton - attaque", "bouton - cible", "bouton - purification", "bouton - objet" };
                for (int i = 0; i < names.Length; i++)
                {
                    Transform button = buttons.Find(names[i]);
                    if (button == null) continue;
                    combatManager.lesOption.Add(button.gameObject);
                    Button clickable = button.GetComponent<Button>();
                    if (clickable == null) clickable = button.gameObject.AddComponent<Button>();
                    if (clickable != null)
                    {
                        Navigation navigation = clickable.navigation;
                        navigation.mode = Navigation.Mode.None;
                        clickable.navigation = navigation;
                        int option = i;
                        clickable.onClick.AddListener(() =>
                        {
                            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
                            combatManager.GetComponent<CombatActionController>()?.SelectOption(option);
                        });
                    }
                }
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
        if (bagPanel != null && combatManager != null)
        {
            BindBagButton("ButtonsPlaces/Use", () => combatManager.ConfirmerAction());
            BindBagButton("ButtonsPlaces/Return", () => combatManager.AnnulerAction());
        }
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

    private void BindBagButton(string path, UnityEngine.Events.UnityAction action)
    {
        Transform target = bagPanel.Find(path);
        if (target == null) return;
        Button button = target.GetComponent<Button>();
        if (button == null) button = target.gameObject.AddComponent<Button>();
        button.onClick.AddListener(action);
    }

    private List<Image> ResolveWheelSlots()
    {
        if (sceneCanvas == null) return new List<Image>();
        return sceneCanvas.GetComponentsInChildren<Image>(true)
            .Where(image => image != null && image.name == "YoKaiSlot" &&
                (resultPanel == null || !image.transform.IsChildOf(resultPanel))).ToList();
    }
}
