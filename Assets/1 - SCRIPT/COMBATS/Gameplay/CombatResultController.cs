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
    [SerializeField, Min(0f)] private float outcomeMessageDuration = 5f;
    private bool announcementStarted;
    private bool isAnnouncing;
    private float announcementDeadline;
    private CombatEndedEvent pendingResult;
    private bool returnRequested;

    public void Initialize(CombatManager owner, CombatProgressionController progressionController)
    {
        manager = owner;
        progression = progressionController;
    }

    public void UseModularResultScreen() => usesModularResultScreen = true;

    public void ProcessEndedCombat()
    {
        if (resultEventPublished) return;
        if (announcementStarted)
        {
            if (isAnnouncing && Time.unscaledTime >= announcementDeadline) ShowResults();
            return;
        }
        if (!resultScreenIsClosed || manager.stepPanelVictory != 0) return;
        announcementStarted = true;
        isAnnouncing = true;
        manager.GetComponent<CombatTargetPointer>()?.Close();
        manager.GetComponent<ChoisieYoKaiAmi>()?.Arrêter();
        manager.GetComponent<SacCombats>()?.Arrêter();
        if (manager.blocDesBoutonGénéraux != null) manager.blocDesBoutonGénéraux.SetActive(false);
        if (manager.panelVictory != null) manager.panelVictory.SetActive(false);
        var defeatedEnemies = manager.listeYoKaiEnnemieSurLeTerrain
            .Where(enemy => enemy != null && enemy.hp_me <= 0).ToList();
        progression.GrantExperience(defeatedEnemies);
        pendingResult = new CombatEndedEvent(manager.EstVictoire(), manager.CalculerXpTotaleCombat(defeatedEnemies));
        announcementDeadline = Time.unscaledTime + outcomeMessageDuration;
        EventBus.Publish(new CombatOutcomeAnnouncementEvent(pendingResult.Victory, outcomeMessageDuration));
    }

    private void ShowResults()
    {
        if (!isAnnouncing || resultEventPublished) return;
        isAnnouncing = false;
        resultEventPublished = true;
        manager.stepPanelVictory = 1;
        EventBus.Publish(new CombatOutcomeAnnouncementDismissedEvent());
        if (!usesModularResultScreen && manager.panelVictory != null)
        {
            manager.panelVictory.SetActive(true);
            Transform page = manager.panelVictory.transform.Find("partie-1");
            if (page != null) page.gameObject.SetActive(true);
        }
        EventBus.Publish(pendingResult);
    }

    public void ConfirmResult()
    {
        if (!announcementStarted) ProcessEndedCombat();
        if (isAnnouncing) { ShowResults(); return; }
        if (!resultEventPublished) return;
        if (usesModularResultScreen)
        {
            ReturnToPreviousScene();
            return;
        }
        if (manager.stepPanelVictory < 2)
        {
            EventBus.Publish(new CombatResultDismissedEvent());
            if (manager.panelVictory == null)
            {
                ReturnToPreviousScene();
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

        ReturnToPreviousScene();
    }

    private void ReturnToPreviousScene()
    {
        if (returnRequested || pendingResult == null) return;
        if (!PlayerPositionLocally.Instance.ReturnFromCombat(pendingResult.Victory)) return;
        returnRequested = true;
        EventBus.Publish(new CombatResultDismissedEvent());
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
