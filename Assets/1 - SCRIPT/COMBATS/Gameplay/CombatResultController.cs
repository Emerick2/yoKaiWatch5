using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;

/// <summary>Owns the transition from battle completion to result and progression screens.</summary>
public sealed class CombatResultController : MonoBehaviour
{
    private CombatManager manager;
    private CombatProgressionController progression;
    private bool usesModularResultScreen;
    private bool resultScreenIsClosed = true;
    private bool resultEventPublished;

    public void Initialize(CombatManager owner, CombatProgressionController progressionController)
    {
        manager = owner;
        progression = progressionController;
    }

    public void UseModularResultScreen() => usesModularResultScreen = true;

    public void ProcessEndedCombat()
    {
        if (!resultScreenIsClosed || manager.stepPanelVictory != 0) return;

        if (usesModularResultScreen)
        {
            if (manager.panelVictory != null)
            {
                Transform legacyFirstPage = manager.panelVictory.transform.Find("partie-1");
                if (legacyFirstPage != null) legacyFirstPage.gameObject.SetActive(false);
                manager.panelVictory.SetActive(false);
            }
            manager.stepPanelVictory = 1;
        }
        else
        {
            if (manager.panelVictory == null)
            {
                Debug.LogError("Result screen is missing: assign panelVictory or enable the modular result screen.");
                manager.stepPanelVictory = 1;
                resultScreenIsClosed = false;
            }
            else
            {
                manager.panelVictory.SetActive(true);
                Transform panel = manager.panelVictory.transform.Find("partie-1");
                if (panel != null) panel.gameObject.SetActive(false);
                manager.stepPanelVictory = 1;
                if (panel != null)
                {
                    panel.gameObject.SetActive(true);
                    GameObject win = panel.Find("win")?.gameObject;
                    GameObject lose = panel.Find("lose")?.gameObject;
                    bool victory = manager.EstVictoire();
                    if (win != null) win.SetActive(victory);
                    if (lose != null) lose.SetActive(!victory);
                }
            }
        }

        System.Collections.Generic.List<YKW.Core.YoKai.Class.YoKaiCombat> defeatedEnemies =
            manager.listeYoKaiEnnemieSurLeTerrain
                .Where(enemy => enemy != null && enemy.hp_me <= 0)
                .ToList();
        progression.GrantExperience(defeatedEnemies);
        if (resultEventPublished) return;

        resultEventPublished = true;
        EventBus.Publish(new CombatEndedEvent(
            manager.EstVictoire(), manager.CalculerXpTotaleCombat(defeatedEnemies)));
    }

    public void ConfirmResult()
    {
        if (usesModularResultScreen)
        {
            EventBus.Publish(new CombatResultDismissedEvent());
            PlayerPositionLocally.Instance?.ReturnToTheSaveScene();
            return;
        }
        if (manager.stepPanelVictory < 2)
        {
            EventBus.Publish(new CombatResultDismissedEvent());
            if (manager.panelVictory == null)
            {
                PlayerPositionLocally.Instance?.ReturnToTheSaveScene();
                return;
            }
            Transform panel = manager.panelVictory.transform.Find("partie-" + manager.stepPanelVictory);
            if (panel != null) panel.gameObject.SetActive(false);
            manager.stepPanelVictory++;
            panel = manager.panelVictory.transform.Find("partie-" + manager.stepPanelVictory);
            if (panel != null) panel.gameObject.SetActive(true);
            OpenProgressionPanel();
            return;
        }

        PlayerPositionLocally.Instance?.ReturnToTheSaveScene();
    }

    private void OpenProgressionPanel()
    {
        resultScreenIsClosed = false;
        if (!manager.panelVictory) return;

        manager.panelVictory.SetActive(true);
        for (int i = 0; i < 6; i++)
        {
            Transform blockTransform = manager.panelVictory.transform.Find("partie-2/" + i);
            if (blockTransform == null)
            {
                Debug.LogWarning($"Progression screen is incomplete: result block partie-2/{i} is missing.");
                continue;
            }
            GameObject block = blockTransform.gameObject;
            if (i >= manager.listeYoKaiAmiSurLeTerrain.Count)
            {
                block.SetActive(false);
                continue;
            }

            block.SetActive(true);
            Transform medal = block.transform.Find("unYoKai/bloc image/médaillon");
            Image medalImage = medal != null ? medal.GetComponent<Image>() : null;
            if (medalImage != null) medalImage.sprite = manager.listeYoKaiAmiSurLeTerrain[i].imageFace;
            Transform level = block.transform.Find("unYoKai/Niveau");
            TextMeshProUGUI levelText = level != null ? level.GetComponent<TextMeshProUGUI>() : null;
            if (levelText != null) levelText.text = manager.listeYoKaiAmiSurLeTerrain[i].niveau.ToString();
        }
    }
}
