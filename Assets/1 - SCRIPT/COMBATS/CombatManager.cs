using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using YKW.Core.YoKai.Class;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;
using StructureYoKaiCombat = YKW.Core.YoKai.Class.YoKaiCombat;

[RequireComponent(typeof(ActionCombat))]
public class CombatManager : EventSubscriberBehaviour{
    public List<Image> lesEspacesMédaillonDansLaRoue;
    [HideInInspector] public List<YoKaiCombat> listeYoKaiSurLeTerrain = new List<YoKaiCombat>();
    [HideInInspector] public List<YoKaiCombat> listeYoKaiAmiSurLeTerrain = new List<YoKaiCombat>();
    [HideInInspector] public List<YoKaiCombat> listeYoKaiEnnemieSurLeTerrain = new List<YoKaiCombat>();
    private readonly Queue<int> rotationRequests = new Queue<int>();

    public List<Transform> troisPositionAmi;
    public List<GameObject> troisDisplateUIAmi;
    public List<GameObject> troisDisplateUIAmiDerrière;
    public List<Transform> troisPositionEnnemie;
    public List<GameObject> troisDisplateUIEnnemie;
    public List<YoKaiCombat> ordreDePassage = new List<YoKaiCombat>();

    [Header("Scene test roster")]
    public bool useSceneTestAlliesWhenRosterEmpty;
    public List<int> sceneTestAllySpeciesIds = new List<int>();
    public List<int> sceneTestAllyLevels = new List<int>();
    [Min(1)] public int sceneTestMinimumLevel = 10;

    public List<GameObject> lesOption;
    public Image imageVitesseAccéléré;
    public List<Sprite> spritePourLaVitesse;


    public DiminuerLaVieScript diminuerLaVieScript;

    public GameObject cible;
    // options :

    public GameObject blocDesBoutonGénéraux;
    // sprite
    public Sprite spriteFlâne;
    public Sprite spriteDéfance;
    public Sprite spriteRaté;

    private bool combatEnCours;
    private bool combatInitialise;
    private float combatStartedAt;
    private CombatTurnController turnController;
    private CombatActionController actionController;
    private CombatSceneController sceneController;
    private CombatProgressionController progressionController;
    private CombatResultController resultController;
    private bool modularResultScreenRequested;

    public GameObject panelVictory;
    public int stepPanelVictory = 0;
    public List<GameObject> aAfficherSeulementAvecLaCible;

    public int tauxPourRatéUneAttaque = 10;
    public bool gameInStop = false;
    public List<Image> listeImage;

    private void Awake()
    {
        if (GetComponent<ActionCombat>() == null)
            gameObject.AddComponent<ActionCombat>();

        turnController = GetComponent<CombatTurnController>();
        if (turnController == null) turnController = gameObject.AddComponent<CombatTurnController>();
        turnController.Initialize(this);

        actionController = GetComponent<CombatActionController>();
        if (actionController == null) actionController = gameObject.AddComponent<CombatActionController>();
        actionController.Initialize(this);

        sceneController = GetComponent<CombatSceneController>();
        if (sceneController == null) sceneController = gameObject.AddComponent<CombatSceneController>();
        sceneController.Initialize(this);

        progressionController = GetComponent<CombatProgressionController>();
        if (progressionController == null) progressionController = gameObject.AddComponent<CombatProgressionController>();
        progressionController.Initialize(this);

        resultController = GetComponent<CombatResultController>();
        if (resultController == null) resultController = gameObject.AddComponent<CombatResultController>();
        resultController.Initialize(this, progressionController);
        if (modularResultScreenRequested) resultController.UseModularResultScreen();
    }

    [SubscribeEvent]
    private void OnRotateRequested(CombatRotateRequestedEvent command) =>
        DemanderRotationRoue(command.Direction);

    [SubscribeEvent]
    private void OnOptionChangeRequested(CombatOptionChangeRequestedEvent command) =>
        ChangerOption(command.Direction);

    [SubscribeEvent]
    private void OnConfirmRequested(CombatConfirmRequestedEvent command) => ConfirmerAction();

    [SubscribeEvent]
    private void OnCancelRequested(CombatCancelRequestedEvent command) => AnnulerAction();

    [SubscribeEvent]
    private void OnSpeedToggleRequested(CombatSpeedToggleRequestedEvent command) => BasculerVitesseAccélérée();

    [SubscribeEvent]
    private void OnAllyTargetSelected(CombatAllyTargetSelectedEvent command) =>
        ChoieDeAmiEffectuer(command.AllyIndex);

    [SubscribeEvent]
    private void OnFoodSelected(CombatFoodSelectedEvent command) => ChoisieAllimant(command.FoodIndex);

    [SubscribeEvent]
    private void OnMinigameCompleted(CombatMinigameCompletedEvent result)
    {
        if (result == null || !result.Succeeded) return;
        StructureYoKaiCombat ally = listeYoKaiAmiSurLeTerrain.FirstOrDefault(
            yokai => yokai.IDListeYoKai == result.AllyId);
        if (ally == null) return;
        if (result.Kind == CombatMinigameKind.Soultimate) ally.aAttaque = true;
        if (result.Kind == CombatMinigameKind.Purification) ally.aEtePurifie = true;
    }

    public void UseModularResultScreen()
    {
        modularResultScreenRequested = true;
        if (resultController != null) resultController.UseModularResultScreen();
    }

    private void Start(){
        if (imageVitesseAccéléré != null) imageVitesseAccéléré.gameObject.SetActive(false);
        actionController.InitializeUI();
        if (MesYoKai.Instance == null)
            Debug.LogError("CombatManager: MesYoKai.Instance is missing; combat cannot start.", this);
        else
            StartCoroutine(CoroutineDeDémarrage());
        if (cible != null) cible.SetActive(false);
        if (blocDesBoutonGénéraux != null) blocDesBoutonGénéraux.SetActive(true);
        if (panelVictory != null) panelVictory.SetActive(false);
        if (aAfficherSeulementAvecLaCible != null)
            foreach (GameObject gameObject in aAfficherSeulementAvecLaCible)
                if (gameObject != null) gameObject.SetActive(false);
    }
    private IEnumerator CoroutineDeDémarrage(){
        float elapsed = 0f;
        while (MesYoKai.Instance != null &&
               (!MesYoKai.Instance.finDuChargement || !MesYoKai.Instance.yoKaiDeMonEquipeChargé)){
            if (elapsed >= 30f)
            {
                Debug.LogError("CombatManager: team data did not finish loading within 30 seconds.", this);
                yield break;
            }
            yield return new WaitForSecondsRealtime(0.2f);
            elapsed += 0.2f;
        }
        if (MesYoKai.Instance == null)
        {
            Debug.LogError("CombatManager: MesYoKai was destroyed while combat was loading.", this);
            yield break;
        }
        if (cible == null || cible.GetComponent<Cible>() == null)
        {
            Debug.LogError("CombatManager: the 'cible' reference must contain a Cible component.", this);
            yield break;
        }
        Cible targetSelector = cible.GetComponent<Cible>();
        if (targetSelector.yoKaiGameObjects == null)
            targetSelector.yoKaiGameObjects = new List<GameObject>();
        else
            targetSelector.yoKaiGameObjects.Clear();
        sceneController.PopulateAllies();
        PopulateEncounterEnemies();
        sceneController.PlaceMedalsOnWheel();
        sceneController.SpawnFriendlyActors();
        sceneController.SpawnEnemyActors();
        sceneController.UpdateFrontLine();
        foreach (StructureYoKaiCombat item in listeYoKaiAmiSurLeTerrain){
            sceneController.ApplyEquippedItemStats(item);
        }
        foreach (StructureYoKaiCombat item in listeYoKaiEnnemieSurLeTerrain){
            sceneController.ApplyEquippedItemStats(item);
        }
        for (int i = 0; i < listeYoKaiEnnemieSurLeTerrain.Count; i++){
            if (listeYoKaiEnnemieSurLeTerrain[i].gameObjectSurLaScène != null)
                targetSelector.yoKaiGameObjects.Add(listeYoKaiEnnemieSurLeTerrain[i].gameObjectSurLaScène);
        }
        targetSelector.CombatManager = this;
        sceneController.RefreshCombatUI();
        if (listeYoKaiAmiSurLeTerrain.Count == 0)
        {
            Debug.LogError(
                $"CombatManager: no valid allies were loaded. The saved team contains " +
                $"{MesYoKai.Instance.yoKaiDeMonEquipe.Count} entries, but the owned Yo-kai roster contains " +
                $"{MesYoKai.Instance.maListe.Count}. Scene actors and UI were initialized, but turns cannot start until an owned Yo-kai is assigned to the team.",
                this);
            yield break;
        }
        if (listeYoKaiEnnemieSurLeTerrain == null ||
            !listeYoKaiEnnemieSurLeTerrain.Any(enemy => enemy != null && enemy.hp_me > 0))
        {
            Debug.LogError("CombatManager: combat was not started because no living enemies were loaded.", this);
            yield break;
        }
        Debug.Log("Combat roster: allies " + string.Join(", ", listeYoKaiAmiSurLeTerrain
            .Where(yokai => yokai != null).Select(yokai => $"{yokai.nom} Lv{yokai.niveau} {yokai.hp_me}/{yokai.hp_max_me} HP")) +
            "; enemies " + string.Join(", ", listeYoKaiEnnemieSurLeTerrain
            .Where(yokai => yokai != null).Select(yokai => $"{yokai.nom} Lv{yokai.niveau} {yokai.hp_me}/{yokai.hp_max_me} HP")), this);
        combatInitialise = true;
        combatStartedAt = Time.unscaledTime;
        combatEnCours = true;
        StartCoroutine(turnController.RunTurns());
        EventBus.Publish(new CombatStartedEvent());
    }

    private void PopulateEncounterEnemies()
    {
        if (PlayerPositionLocally._instance == null) return;
        if (PlayerPositionLocally._instance.idYokai1 > 0)
            sceneController.AddEnemy(PlayerPositionLocally._instance.idYokai1, PlayerPositionLocally._instance.niveauYokai1);
        if (PlayerPositionLocally._instance.idYokai2 > 0)
            sceneController.AddEnemy(PlayerPositionLocally._instance.idYokai2, PlayerPositionLocally._instance.niveauYokai2);
        if (PlayerPositionLocally._instance.idYokai3 > 0)
            sceneController.AddEnemy(PlayerPositionLocally._instance.idYokai3, PlayerPositionLocally._instance.niveauYokai3);
    }
    internal bool CombatEnCours => combatEnCours;
    internal bool GameInStop => gameInStop;

    internal int TakeRotationRequest()
    {
        return rotationRequests.Count > 0 ? rotationRequests.Dequeue() : 0;
    }

    internal void ApplyWheelRotation(int direction) => sceneController.RotateWheel(direction);
    internal bool SlotTransitionInProgress
    {
        get
        {
            if (sceneController != null && sceneController.IsRotating) return true;
            CombatSlotAnimator animator = GetComponent<CombatSlotAnimator>();
            return animator != null && animator.IsTransitioning;
        }
    }
    internal void RefreshCombatUI() => sceneController.RefreshCombatUI();
    internal void RefreshCombatState()
    {
        if (combatInitialise && combatEnCours && UnGroupeDeYoKaiEstMort())
        {
            string defeatedSide = !listeYoKaiAmiSurLeTerrain.Any(yokai => yokai != null && yokai.hp_me > 0)
                ? "allies"
                : "enemies";
            Debug.Log($"Combat ended after {Time.unscaledTime - combatStartedAt:F1}s: all {defeatedSide} are defeated.", this);
            combatEnCours = false;
            actionController.StopSpeed();
        }
    }

    private void Update()
    {
        if (combatInitialise && !combatEnCours)
            resultController.ProcessEndedCombat();

        RefreshCombatState();
    }
    public void BasculerVitesseAccélérée() => actionController.ToggleFastForward();
    public void DemanderRotationRoue(int direction)
    {
        if (combatEnCours && !gameInStop && direction % 6 != 0) rotationRequests.Enqueue(direction);
    }
    public void ChangerOption(int direction) => actionController.ChangeSelectedOption(direction);
    public void ConfirmerAction() => actionController.ConfirmAction();
    public void AnnulerAction() => actionController.CancelAction();
    public void ChoisieAllimant(int id) => actionController.SelectFood(id);
    public void ChoieDeAmiEffectuer(int target) => actionController.CompleteAllySelection(target);
    internal void ConfirmCombatResult() => resultController.ConfirmResult();

    internal bool EstVictoire()
    {
        if (listeYoKaiAmiSurLeTerrain.Count == 0 || listeYoKaiEnnemieSurLeTerrain.Count == 0) return false;
        bool unAlliéVivant = listeYoKaiAmiSurLeTerrain.Any(yoKai => yoKai.hp_me > 0);
        bool unEnnemiVivant = listeYoKaiEnnemieSurLeTerrain.Any(yoKai => yoKai.hp_me > 0);
        return unAlliéVivant && !unEnnemiVivant;
    }
    internal bool UnGroupeDeYoKaiEstMort() =>
        !listeYoKaiAmiSurLeTerrain.Any(yoKai => yoKai.hp_me > 0) ||
        !listeYoKaiEnnemieSurLeTerrain.Any(yoKai => yoKai.hp_me > 0);
    internal StructureYoKaiCombat GetAllyAtWheelSlot(int slot) =>
        listeYoKaiAmiSurLeTerrain.FirstOrDefault(ally => ally.positionRoue == slot);
    public int CalculerXpTotaleCombat(List<StructureYoKaiCombat> ennemisBattus) =>
        progressionController.CalculateTotalExperience(ennemisBattus);

    public float CalculerXpPourUnYoKai(StructureYoKaiCombat yokai) =>
        progressionController.CalculateExperienceMultiplier(yokai);

}
