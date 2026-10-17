using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YKW.Core.Item.Class;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;
using CombatYokai = YKW.Core.YoKai.Class.YoKaiCombat;
using YoKaiEspece = YKW.Core.YoKai.Class.YoKai;

/// <summary>Builds the combat roster and owns scene-facing actor, wheel, and HUD presentation.</summary>
public sealed class CombatSceneController : MonoBehaviour
{
    private CombatManager manager;
    private readonly HashSet<GameObject> clickableCards = new HashSet<GameObject>();
    private Transform battleTerrain, activeCardRow, reserveCardRow, enemyCardRow;
    private bool isRotating;
    private Coroutine actorTransition;
    private List<ActorMotion> rotationMotions = new List<ActorMotion>();
    public bool IsRotating => isRotating;
    private readonly Dictionary<CombatYokai, Vector3> actorScales = new Dictionary<CombatYokai, Vector3>();

    private sealed class ActorMotion
    {
        public GameObject Actor;
        public Vector3 StartPosition, EndPosition, StartScale, EndScale, RestScale;
        public bool ShowAtEnd;
    }
    private readonly Dictionary<Transform, Vector2> templateSizes = new Dictionary<Transform, Vector2>();
    private readonly Dictionary<CombatYokai, Vector2> alignedCardPositions = new Dictionary<CombatYokai, Vector2>();

    public void Initialize(CombatManager owner)
    {
        manager = owner;
    }

    private void OnDisable()
    {
        if (actorTransition != null) StopCoroutine(actorTransition);
        CompleteActorRotation();
    }

    private void LateUpdate()
    {
        if (manager == null || !manager.CombatEnCours || isRotating) return;
        LayoutBattleRows();
        Camera camera = Camera.main;
        if (camera == null) return;
        AlignActorsWithCards(manager.listeYoKaiAmiSurLeTerrain, manager.troisDisplateUIAmi, camera);
        AlignActorsWithCards(manager.listeYoKaiEnnemieSurLeTerrain, manager.troisDisplateUIEnnemie, camera);
    }

    private void AlignActorsWithCards(List<CombatYokai> actors, List<GameObject> cards, Camera camera)
    {
        if (actors == null || cards == null) return;
        foreach (CombatYokai actor in actors)
        {
            if (actor == null || !actor.estDevant || actor.gameObjectSurLaScène == null ||
                actor.positionRoue < 0 || actor.positionRoue >= cards.Count) continue;
            GameObject card = cards[actor.positionRoue];
            RectTransform rect = card != null ? card.transform as RectTransform : null;
            if (rect == null || !card.activeInHierarchy) continue;
            Canvas canvas = card.GetComponentInParent<Canvas>();
            Camera uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(uiCamera, rect.TransformPoint(RectTransformUtility.CalculateRelativeRectTransformBounds(rect, rect).center));
            if (alignedCardPositions.TryGetValue(actor, out Vector2 previous) &&
                (screenPosition - previous).sqrMagnitude < 0.01f) continue;
            Renderer[] renderers = actor.gameObjectSurLaScène.GetComponentsInChildren<Renderer>();
            Vector3 center = actor.gameObjectSurLaScène.transform.position;
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
                center = bounds.center;
            }
            if (camera.WorldToScreenPoint(center).z <= 0f) continue;
            actor.gameObjectSurLaScène.transform.position += GetHorizontalAlignmentOffset(camera, center, screenPosition.x);
            alignedCardPositions[actor] = screenPosition;
        }
    }

    private static Vector3 GetHorizontalAlignmentOffset(Camera camera, Vector3 visualCenter, float screenX)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(visualCenter);
        screenPosition.x = Mathf.Clamp(screenX, camera.pixelRect.xMin + camera.pixelWidth * 0.1f,
            camera.pixelRect.xMax - camera.pixelWidth * 0.1f);
        Vector3 offset = camera.ScreenToWorldPoint(screenPosition) - visualCenter;
        return camera.transform.right * Vector3.Dot(offset, camera.transform.right);
    }

    public void AddEnemy(int id, int level)
    {
        if (medaliumDictionnaire._instance == null)
        {
            Debug.LogError("Cannot add the enemy because the Medalium data is not ready.");
            return;
        }
        id = Mathf.Max(1, id);
        level = Mathf.Clamp(level, 1, 99);
        if (IsSceneTestRosterActive())
            level = Mathf.Max(level, Mathf.Clamp(manager.sceneTestMinimumLevel, 1, 99));

        YoKaiEspece species = medaliumDictionnaire._instance.ServantYoKai(id);
        if (species == null)
        {
            Debug.LogError($"Could not find Yo-kai species with ID {id}.");
            return;
        }
        CombatYokai enemy = new CombatYokai();
        enemy.InitialiserCombatDepuis(species, level);
        enemy.estUnAmi = false;
        enemy.nom = species.nom;
        enemy.imageMédaillon = species.imageMédaillon;
        enemy.imageFace = species.faceRondImage != null ? species.faceRondImage : species.imageFace;
        enemy.positionRoue = manager.listeYoKaiEnnemieSurLeTerrain.Count;
        enemy.estDevant = true;
        manager.listeYoKaiEnnemieSurLeTerrain.Add(enemy);
    }

    public void PopulateAllies(){
        manager.listeYoKaiAmiSurLeTerrain.Clear();
        if (MesYoKai.Instance == null || MesYoKai.Instance.yoKaiDeMonEquipe == null ||
            MesYoKai.Instance.maListe == null || medaliumDictionnaire._instance == null)
        {
            Debug.LogError("Cannot prepare the team because Yo-kai data is unavailable.");
            return;
        }
        for (int i = 0; i < MesYoKai.Instance.yoKaiDeMonEquipe.Count; i++){
            int specificAllyIndex = MesYoKai.Instance.yoKaiDeMonEquipe[i];
            if (specificAllyIndex < 0 || specificAllyIndex >= MesYoKai.Instance.maListe.Count)
            {
                Debug.LogWarning($"Skipping team entry with invalid index {specificAllyIndex}.");
                continue;
            }
            int ID = MesYoKai.Instance.maListe[specificAllyIndex].ID;
            CombatYokai ally = new CombatYokai{};
            YoKaiEspece y = medaliumDictionnaire._instance.ServantYoKai(ID);
            if (y == null)
            {
                Debug.LogWarning($"Skipping ally because Yo-kai species ID {ID} was not found.");
                continue;
            }
            ally.InitialiserCombatDepuis(MesYoKai.Instance.maListe[specificAllyIndex]);
            int savedSlot = MesYoKai.Instance.maListe[specificAllyIndex].positionRoue;
            ally.positionRoue = ((savedSlot % 6) + 6) % 6;
            if (savedSlot != ally.positionRoue)
                Debug.LogWarning($"Invalid wheel slot {savedSlot} for {ally.nom}; normalized to {ally.positionRoue}.");
            ally.estUnAmi = true;
            ally.IDListeYoKai = specificAllyIndex;//Distinguishes duplicate species in the team.
            ally.nom = string.IsNullOrWhiteSpace(MesYoKai.Instance.maListe[specificAllyIndex].surnom)
                ? y.nom
                : MesYoKai.Instance.maListe[specificAllyIndex].surnom;
            ally.imageFace = y.faceRondImage != null ? y.faceRondImage : y.imageFace;
            ally.personnalité = MesYoKai.Instance.maListe[specificAllyIndex].personnalité;
            manager.listeYoKaiAmiSurLeTerrain.Add(ally);
        }

        if (manager.listeYoKaiAmiSurLeTerrain.Count == 0 &&
            MesYoKai.Instance.yoKaiDeMonEquipe.Count == 0 &&
            manager.useSceneTestAlliesWhenRosterEmpty)
        {
            PopulateSceneTestAllies();
        }
        CombatResultUI resultUI = manager.GetComponent<CombatResultUI>();
        if (resultUI != null) resultUI.CaptureInitialRoster();
    }

    private void PopulateSceneTestAllies()
    {
        if (manager.sceneTestAllySpeciesIds == null) return;
        for (int i = 0; i < manager.sceneTestAllySpeciesIds.Count && i < 6; i++)
        {
            int speciesId = manager.sceneTestAllySpeciesIds[i];
            YoKaiEspece species = medaliumDictionnaire._instance.ServantYoKai(speciesId);
            if (species == null)
            {
                Debug.LogWarning($"Skipping configured test ally: species ID {speciesId} was not found.", manager);
                continue;
            }

            int level = manager.sceneTestAllyLevels != null && i < manager.sceneTestAllyLevels.Count
                ? Mathf.Clamp(manager.sceneTestAllyLevels[i], 1, 99)
                : 1;
            level = Mathf.Max(level, Mathf.Clamp(manager.sceneTestMinimumLevel, 1, 99));
            CombatYokai ally = new CombatYokai();
            ally.InitialiserCombatDepuis(species, level);
            ally.positionRoue = i;
            ally.estUnAmi = true;
            ally.estDevant = i < 3;
            ally.IDListeYoKai = -1 - i;
            ally.IDUnique = -1 - i;
            ally.nom = species.nom;
            ally.imageMédaillon = species.imageMédaillon;
            ally.imageFace = species.faceRondImage != null ? species.faceRondImage : species.imageFace;
            manager.listeYoKaiAmiSurLeTerrain.Add(ally);
        }

        if (manager.listeYoKaiAmiSurLeTerrain.Count > 0)
            Debug.Log($"Loaded {manager.listeYoKaiAmiSurLeTerrain.Count} scene-only test allies; the player's save was not changed.", manager);
    }

    private bool IsSceneTestRosterActive()
    {
        return manager != null && manager.useSceneTestAlliesWhenRosterEmpty &&
               manager.listeYoKaiAmiSurLeTerrain != null &&
               manager.listeYoKaiAmiSurLeTerrain.Any(ally => ally != null && ally.IDListeYoKai < 0);
    }

    CombatYokai GetAllyAtWheelSlot(int position){
        for (int j = 0; j < manager.listeYoKaiAmiSurLeTerrain.Count; j++){
            if (manager.listeYoKaiAmiSurLeTerrain[j].positionRoue == position){
                return manager.listeYoKaiAmiSurLeTerrain[j];
            }
        }
        return null;
    }
    public void PlaceMedalsOnWheel(){
        if (manager.lesEspacesMédaillonDansLaRoue == null || manager.lesEspacesMédaillonDansLaRoue.Count == 0) return;
        // Reward slots belong to CombatResultUI, never to the battle wheel.
        if (manager.lesEspacesMédaillonDansLaRoue.Any(image => image != null &&
            (FindAncestor(image.transform, "WinScreen") != null || FindAncestor(image.transform, "LostScreen") != null))) return;
        Transform wheelRoot = FindAncestor(manager.lesEspacesMédaillonDansLaRoue.FirstOrDefault(image => image != null)?.transform, "YokaiRoue");
        if (wheelRoot != null) wheelRoot.gameObject.SetActive(manager.listeYoKaiAmiSurLeTerrain.Count > 1);
        foreach (Image item in manager.lesEspacesMédaillonDansLaRoue)
        {
            if (item == null) continue;
            item.enabled = true;
            item.gameObject.SetActive(true);
            ClearWheelSlotContent(item.transform);
        }
        foreach (CombatYokai item in manager.listeYoKaiAmiSurLeTerrain){
            int slot = item.positionRoue;
            if (slot < 0 || slot >= manager.lesEspacesMédaillonDansLaRoue.Count ||
                manager.lesEspacesMédaillonDansLaRoue[slot] == null)
            {
                Debug.LogWarning($"Could not place medal for {item.nom}: wheel slot {slot} is invalid.");
                continue;
            }
            Image medal = manager.lesEspacesMédaillonDansLaRoue[slot];
            medal.enabled = true;
            medal.gameObject.SetActive(true);
            SetWheelSlotContent(medal.transform, item);
        }
    }

    private static void SetWheelSlotContent(Transform slot, CombatYokai yokai)
    {
        Transform face = slot.Find("YoKaiImages");
        Image faceImage = face != null ? face.GetComponent<Image>() : null;
        if (faceImage != null)
        {
            faceImage.sprite = yokai.imageFace;
            face.gameObject.SetActive(yokai.imageFace != null);
        }

        Transform name = slot.Find("Bubble/YoKaiName");
        TMP_Text nameText = name != null ? name.GetComponent<TMP_Text>() : null;
        if (nameText != null)
        {
            nameText.text = yokai.nom ?? string.Empty;
            nameText.gameObject.SetActive(!string.IsNullOrWhiteSpace(nameText.text));
            name.parent.gameObject.SetActive(nameText.gameObject.activeSelf);
        }

        Transform levelSprite = slot.Find("LVSprite");
        if (levelSprite != null) levelSprite.gameObject.SetActive(true);
        Transform experienceBar = slot.Find("XP");
        if (experienceBar != null) experienceBar.gameObject.SetActive(true);
    }

    private static void ClearWheelSlotContent(Transform slot)
    {
        string[] contentNames = { "Bubble", "SpriteType", "YoKaiImages", "LVSprite", "XP" };
        for (int i = 0; i < contentNames.Length; i++)
        {
            Transform content = slot.Find(contentNames[i]);
            if (content != null) content.gameObject.SetActive(false);
        }
    }
    public void SpawnFriendlyActors()
    {
        alignedCardPositions.Clear();
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain)
        {
            if (ally == null) continue;
            bool visible = ally.positionRoue >= 0 && ally.positionRoue < 3;
            if (visible) EnsureFriendlyActor(ally);
            if (ally.gameObjectSurLaScène != null) ally.gameObjectSurLaScène.SetActive(visible);
        }
    }

    private void EnsureFriendlyActor(CombatYokai ally)
    {
        if (ally.gameObjectSurLaScène != null) return;
        int slot = ally.positionRoue;
        if (slot < 0 || slot >= 3 || manager.troisPositionAmi == null ||
            slot >= manager.troisPositionAmi.Count || manager.troisPositionAmi[slot] == null) return;
        GameObject actor = medaliumDictionnaire._instance.InstantiateYoKai(ally.ID.ToString());
        if (actor == null) return;
        if (actor.GetComponent<AddAnimator>() == null) actor.AddComponent<AddAnimator>();
        actor.transform.position = manager.troisPositionAmi[slot].position;
        actor.transform.Rotate(0, 180, 0);
        MatchWorldScale(actor, manager.troisPositionAmi[slot].lossyScale);
        ally.gameObjectSurLaScène = actor;
        actorScales[ally] = actor.transform.localScale;
        var colliders = new ServantBoxCollider3D();
        foreach (Transform child in actor.transform) colliders.CreateBoxCollider(0, child.gameObject, true);
        colliders.CreateBoxCollider(0, actor, false);
    }

    public void SpawnEnemyActors(){
        alignedCardPositions.Clear();
        for (int j = 0; j < manager.listeYoKaiEnnemieSurLeTerrain.Count; j++){
            if (manager.listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène != null){
                Destroy(manager.listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène);
                manager.listeYoKaiEnnemieSurLeTerrain[j].gameObjectSurLaScène = null;
            }
        }
        for (int i = 0; i < manager.listeYoKaiEnnemieSurLeTerrain.Count; i++){
            CombatYokai yk = manager.listeYoKaiEnnemieSurLeTerrain[i];
            if (yk != null){
                if (medaliumDictionnaire._instance == null) {
                    Debug.LogError("medaliumDictionnaire._instance is null.");
                    return;
                }


                GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(yk.ID.ToString());
                if (TV == null) {
                    Debug.LogError($"Could not instantiate Yo-kai with ID {yk.ID}.");
                    continue;
                }

                if (TV.GetComponent<AddAnimator>() == null) TV.AddComponent<AddAnimator>();
                if (manager.troisPositionEnnemie != null && i < manager.troisPositionEnnemie.Count && manager.troisPositionEnnemie[i] != null) {
                    TV.transform.position = manager.troisPositionEnnemie[i].position;
                }

                TV.transform.localScale = manager.troisPositionEnnemie != null && i < manager.troisPositionEnnemie.Count && manager.troisPositionEnnemie[i] != null
                    ? manager.troisPositionEnnemie[i].localScale
                    : Vector3.one;
                TV.transform.Rotate(0, 180, 0);
                yk.gameObjectSurLaScène = TV;

                if (manager.troisPositionEnnemie != null && i < manager.troisPositionEnnemie.Count && manager.troisPositionEnnemie[i] != null) {
                    MatchWorldScale(TV, manager.troisPositionEnnemie[i].lossyScale);
                }else {
                    Debug.LogError("Enemy scene positions are empty or element 0 is not assigned in the Inspector.");
                }

                ServantBoxCollider3D servantBC = new ServantBoxCollider3D();
                foreach (Transform item in yk.gameObjectSurLaScène.transform){
                    servantBC.CreateBoxCollider(0,item.gameObject,true);
                }
                servantBC.CreateBoxCollider(0,TV,false);
            }
        }
    }
    private static float CalculateFillRatio(int total, int value)
    {
        return total <= 0 ? 0f : (float)value / total;
    }

    private static void SetImage(GameObject root, string path, Sprite sprite)
    {
        if (root == null) return;
        Transform target = string.IsNullOrEmpty(path) ? root.transform : root.transform.Find(path);
        Image image = target != null ? target.GetComponent<Image>() : null;
        if (image == null)
            image = FindImageByName(root.transform, "médaillon", "medaillon", "Image", "Image (1)");
        if (image == null && string.IsNullOrEmpty(path)) image = root.GetComponent<Image>();
        if (image != null) image.sprite = sprite;
    }

    private static void SetFill(GameObject root, string path, float value, string fallbackName)
    {
        if (root == null) return;
        Transform target = root.transform.Find(path);
        Image image = target != null ? target.GetComponent<Image>() : null;
        if (image == null) image = FindImageByName(root.transform, fallbackName);
        if (image != null) image.fillAmount = value;
    }

    private static void SetAmultime(GameObject root, int value)
    {
        SetFill(root, "Stats/AmultimeSlider", Mathf.Clamp01(value / 100f), "AmultimeSlider");
        Transform fullGauge = root != null ? root.transform.Find("Stats/AmultimeFullSprite") : null;
        if (fullGauge != null) fullGauge.gameObject.SetActive(value >= 100);
    }

    private static void SetText(GameObject root, string path, string value)
    {
        if (root == null) return;
        Transform target = root.transform.Find(path);
        TextMeshProUGUI text = target != null ? target.GetComponent<TextMeshProUGUI>() : null;
        if (text == null)
        {
            TextMeshProUGUI[] candidates = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            text = candidates.FirstOrDefault(candidate =>
                candidate != null &&
                (candidate.transform.name == "nom" || candidate.transform.name == "YoKaiName"));
        }
        if (text != null) text.text = value;
    }

    private static Image FindImageByName(Transform root, params string[] candidateNames)
    {
        if (root == null || candidateNames == null) return null;
        Image[] images = root.GetComponentsInChildren<Image>(true);
        foreach (string candidateName in candidateNames)
        {
            Image match = images.FirstOrDefault(image => image != null &&
                image.transform.name == candidateName);
            if (match != null) return match;
        }
        return null;
    }

    public void RefreshCombatUI(){
        Transform combatInterface = FindSceneTransform("Interface");
        if (combatInterface != null)
        {
            combatInterface.gameObject.SetActive(true);
            Transform legacyInterface = FindSceneTransform("interface");
            if (legacyInterface != null) legacyInterface.gameObject.SetActive(false);
        }
        RefreshTemplateRosterUI();
    }

    private void RefreshTemplateRosterUI()
    {
        Transform activeRoot = FindSceneTransform("EquipeActive");
        Transform reserveRoot = FindSceneTransform("EquipeReserve");
        Transform enemyRoot = FindSceneTransform("Ennemis");

        List<CombatYokai> activeAllies = BuildSlotRoster(manager.listeYoKaiAmiSurLeTerrain, 0);
        List<CombatYokai> reserveAllies = BuildSlotRoster(manager.listeYoKaiAmiSurLeTerrain, 3);
        List<CombatYokai> enemies = BuildSlotRoster(manager.listeYoKaiEnnemieSurLeTerrain, 0);
        RenderTemplateCards(activeRoot, activeAllies);
        RenderTemplateCards(reserveRoot, reserveAllies);
        // Logical enemy 0 occupies the middle lane; subsequent enemies fill left, then right.
        CombatYokai firstEnemy = enemies[0];
        enemies[0] = enemies[1];
        enemies[1] = firstEnemy;
        RenderTemplateCards(enemyRoot, enemies);
        LayoutBattleRows();
        manager.troisDisplateUIAmi = GetCardSlots(activeRoot);
        manager.troisDisplateUIAmiDerrière = GetCardSlots(reserveRoot);
        manager.troisDisplateUIEnnemie = GetCardSlots(enemyRoot);
        GameObject firstEnemyCard = manager.troisDisplateUIEnnemie[0];
        manager.troisDisplateUIEnnemie[0] = manager.troisDisplateUIEnnemie[1];
        manager.troisDisplateUIEnnemie[1] = firstEnemyCard;
        manager.GetComponent<CombatActionController>()?.RefreshEnemyPoints();
        CombatSlotAnimator animator = manager.GetComponent<CombatSlotAnimator>();
        if (animator != null)
        {
            var cards = new List<GameObject>(manager.troisDisplateUIAmi);
            cards.AddRange(manager.troisDisplateUIAmiDerrière.AsEnumerable().Reverse());
            animator.BindCards(cards);
        }
    }

    // Fit complete templates inside the authored layout without replacing its anchors or padding.
    private void LayoutBattleRows()
    {
        if (activeCardRow == null) activeCardRow = FindSceneTransform("EquipeActive");
        if (reserveCardRow == null) reserveCardRow = FindSceneTransform("EquipeReserve");
        if (enemyCardRow == null) enemyCardRow = FindSceneTransform("Ennemis");
        if (battleTerrain == null && enemyCardRow != null) battleTerrain = enemyCardRow.parent;
        Transform terrain = battleTerrain;
        Transform active = activeCardRow;
        Transform reserve = reserveCardRow;
        Transform enemies = enemyCardRow;
        if (terrain == null || active == null || reserve == null || enemies == null) return;
        // Keep the scene's authored vertical arrangement and padding.
        // Width preferences must not propagate from three templates to the whole canvas.
        foreach (Transform root in new[] { active.parent, active, reserve, enemies })
        {
            LayoutElement width = root.GetComponent<LayoutElement>();
            if (width == null) width = root.gameObject.AddComponent<LayoutElement>();
            width.minWidth = width.preferredWidth = 0f;
            width.flexibleWidth = 1f;
        }
        Canvas.ForceUpdateCanvases();
        foreach (Transform root in new[] { active, reserve, enemies })
        {
            RectTransform row = root as RectTransform;
            HorizontalOrVerticalLayoutGroup layout = root.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (row == null || layout == null || !templateSizes.TryGetValue(root, out Vector2 size)) continue;
            layout.enabled = true;
            layout.childControlWidth = layout.childControlHeight = false;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;
            layout.childScaleWidth = layout.childScaleHeight = true;
            float available = Mathf.Max(1f, row.rect.width - layout.padding.horizontal - layout.spacing * 2f);
            float scale = Mathf.Min(1f, available / (size.x * 3f));
            foreach (RectTransform card in root)
            {
                card.sizeDelta = size;
                card.localScale = Vector3.one * scale;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(row);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(terrain as RectTransform);
    }

    private static List<CombatYokai> BuildSlotRoster(List<CombatYokai> roster, int offset)
    {
        var slots = new List<CombatYokai> { null, null, null };
        if (roster == null) return slots;
        foreach (CombatYokai actor in roster)
        {
            if (actor == null) continue;
            int slot = offset == 3 ? 5 - actor.positionRoue : actor.positionRoue;
            if (slot >= 0 && slot < slots.Count) slots[slot] = actor;
        }
        return slots;
    }

    private static List<GameObject> GetCardSlots(Transform parent)
    {
        var cards = new List<GameObject> { null, null, null };
        if (parent == null) return cards;
        for (int i = 0; i < cards.Count && i < parent.childCount; i++) cards[i] = parent.GetChild(i).gameObject;
        return cards;
    }

    private void RenderTemplateCards(Transform parent, List<CombatYokai> yokaiList)
    {
        if (parent == null || parent.childCount == 0) return;
        GameObject template = parent.GetChild(0).gameObject;
        int requiredCount = yokaiList != null ? yokaiList.Count : 0;
        Vector2 templateSize;
        if (!templateSizes.TryGetValue(parent, out templateSize))
        {
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
            bool measured = false;
            foreach (string part in new[] { "médaillon", "Stats", "nom" })
            {
                Transform content = template.transform.Find(part);
                if (content == null) continue;
                Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(template.transform, content);
                if (!measured) { bounds = contentBounds; measured = true; }
                else bounds.Encapsulate(contentBounds);
            }
            RectTransform templateRect = template.transform as RectTransform;
            templateSize = measured ? (Vector2)bounds.size : templateRect != null ? templateRect.rect.size : Vector2.one;
            if (template.GetComponent<Image>() != null)
                templateSize = Vector2.Max(templateSize, new Vector2(70f, 70f));
            templateSize = new Vector2(Mathf.Max(1f, templateSize.x), Mathf.Max(1f, templateSize.y));
            templateSizes.Add(parent, templateSize);
        }
        HorizontalOrVerticalLayoutGroup rowLayout = parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (rowLayout != null)
        {
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;
        }

        while (parent.childCount < requiredCount)
        {
            GameObject copy = Instantiate(template, parent, false);
            copy.name = template.name + " (Runtime " + parent.childCount + ")";
            CopyTemplateRectTransform(template.transform as RectTransform,
                copy.transform as RectTransform);
            copy.transform.SetSiblingIndex(parent.childCount - 1);
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject card = parent.GetChild(i).gameObject;
            if (manager != null && clickableCards.Add(card))
            {
                Button button = card.GetComponent<Button>();
                if (button == null) button = card.AddComponent<Button>();
                Navigation navigation = button.navigation;
                navigation.mode = Navigation.Mode.None;
                button.navigation = navigation;
                GameObject selectableCard = card;
                button.onClick.AddListener(() => manager.GetComponent<ChoisieYoKaiAmi>()?.SelectCard(selectableCard));
            }
            bool hasYokai = yokaiList != null && i < requiredCount && yokaiList[i] != null;
            LayoutElement dimensions = card.GetComponent<LayoutElement>();
            if (dimensions == null) dimensions = card.AddComponent<LayoutElement>();
            dimensions.minWidth = dimensions.preferredWidth = templateSize.x;
            dimensions.minHeight = dimensions.preferredHeight = templateSize.y;
            dimensions.flexibleWidth = dimensions.flexibleHeight = 0f;
            // Empty places still participate in layout so slot 1 never slides into slot 0.
            card.SetActive(i < requiredCount);
            CanvasGroup visibility = card.GetComponent<CanvasGroup>();
            if (visibility == null) visibility = card.AddComponent<CanvasGroup>();
            visibility.alpha = hasYokai ? 1f : 0f;
            visibility.interactable = visibility.blocksRaycasts = hasYokai;
            card.transform.Find("point")?.gameObject.SetActive(false);
            if (hasYokai) PopulateTemplateCard(card, yokaiList[i]);
        }

        RectTransform layout = parent as RectTransform;
        if (layout != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }
    }

    private static void CopyTemplateRectTransform(RectTransform source, RectTransform destination)
    {
        if (source == null || destination == null) return;
        destination.anchorMin = source.anchorMin;
        destination.anchorMax = source.anchorMax;
        destination.pivot = source.pivot;
        destination.sizeDelta = source.sizeDelta;
        destination.anchoredPosition = source.anchoredPosition;
        destination.localRotation = source.localRotation;
        destination.localScale = source.localScale;
    }

    private static void PopulateTemplateCard(GameObject card, CombatYokai yokai)
    {
        Image portrait = card.GetComponentsInChildren<Image>(true)
            .FirstOrDefault(image => string.Equals(image.name, "yokai", System.StringComparison.OrdinalIgnoreCase));
        if (portrait == null) portrait = card.transform.Find("médaillon")?.GetComponent<Image>();
        // The compact reserve template stores its portrait on the root Image.
        if (portrait == null) portrait = card.GetComponent<Image>();
        if (portrait == null) portrait = FindImageByName(card.transform, "medaillon", "Image", "Image (1)");
        if (portrait != null)
        {
            portrait.sprite = yokai.imageFace;
            portrait.enabled = yokai.imageFace != null;
            portrait.preserveAspect = true;
            if (portrait.rectTransform.rect.width < 1f || portrait.rectTransform.rect.height < 1f)
                portrait.rectTransform.sizeDelta = new Vector2(70f, 70f);
        }
        SetText(card, "nom", yokai.nom ?? string.Empty);
        SetFill(card, "Stats/LifeSlider", CalculateFillRatio(yokai.hp_max_me, yokai.hp_me), "LifeSlider");
        SetAmultime(card, yokai.amultime);
    }

    private Transform FindSceneTransform(string targetName)
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < transforms.Length; i++)
        {
            Transform candidate = transforms[i];
            if (candidate != null && candidate.gameObject.scene == manager.gameObject.scene && candidate.name == targetName)
                return candidate;
        }
        return null;
    }

    private static Transform FindAncestor(Transform source, string targetName)
    {
        Transform current = source;
        while (current != null)
        {
            if (current.name == targetName) return current;
            current = current.parent;
        }
        return null;
    }
    public void UpdateFrontLine(){
        foreach (CombatYokai item in manager.listeYoKaiAmiSurLeTerrain){
            if (item != null) item.estDevant = item.positionRoue >= 0 && item.positionRoue < 3;
        }
    }
    public void ApplyEquippedItemStats(CombatYokai yokai)
    {
        if (yokai == null || Item.Instance == null || Item.Instance.listeDesÉquipements == null ||
            !yokai.tienUnObjet || yokai.numéroObjetTennu < 0 ||
            yokai.numéroObjetTennu >= Item.Instance.listeDesÉquipements.Count)
            return;

        var effect = Item.Instance.listeDesÉquipements[yokai.numéroObjetTennu].effet;
        yokai.Stats.Vitesse += effect.Vitesse;
        yokai.Stats.Force += effect.Force;
        yokai.Stats.Esprit += effect.Esprit;
        yokai.Stats.Défence += effect.Défence;
    }
    public void RotateWheel(int direction)
    {
        direction = ((direction % 6) + 6) % 6;
        if (direction == 0 || isRotating || manager.listeYoKaiAmiSurLeTerrain.Count <= 1) return;
        if (!manager.listeYoKaiAmiSurLeTerrain.Any(ally => ally != null && ally.hp_me > 0 &&
            (ally.positionRoue + direction) % 6 < 3)) return;

        var visibleBefore = new HashSet<CombatYokai>(manager.listeYoKaiAmiSurLeTerrain
            .Where(ally => ally != null && ally.gameObjectSurLaScène != null &&
                ally.gameObjectSurLaScène.activeSelf && ally.estDevant));
        var changes = new List<SlotChange>();
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain)
        {
            if (ally == null) continue;
            changes.Add(new SlotChange(ally.IDUnique != 0 ? ally.IDUnique : ally.IDListeYoKai,
                ally.positionRoue, (ally.positionRoue + direction) % 6));
        }
        isRotating = true;
        // Subscribers capture the old cards before gameplay commits the new occupancy.
        EventBus.Publish(new CombatSlotsChangedEvent(changes));
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain)
            if (ally != null) ally.positionRoue = (ally.positionRoue + direction) % 6;
        UpdateFrontLine();
        RefreshCombatUI();

        var motions = new List<ActorMotion>();
        Camera camera = Camera.main;
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain)
        {
            if (ally == null) continue;
            bool entering = ally.estDevant && ally.hp_me > 0;
            bool wasVisible = visibleBefore.Contains(ally);
            if (!entering && !wasVisible) continue;
            if (entering) EnsureFriendlyActor(ally);
            GameObject actor = ally.gameObjectSurLaScène;
            if (actor == null) continue;
            Vector3 restScale = actorScales.TryGetValue(ally, out Vector3 scale) ? scale : actor.transform.localScale;
            Vector3 end = actor.transform.position;
            Vector3 retreat = (camera != null ? camera.transform.right : Vector3.right) *
                (ally.positionRoue == 2 || ally.positionRoue == 3 ? 1f : -1f) * 3f;
            if (entering && ally.positionRoue < manager.troisPositionAmi.Count && manager.troisPositionAmi[ally.positionRoue] != null)
                end = manager.troisPositionAmi[ally.positionRoue].position;
            else if (!entering) end += retreat;
            GameObject card = ally.positionRoue < 3 ? manager.troisDisplateUIAmi[ally.positionRoue]
                : manager.troisDisplateUIAmiDerrière[5 - ally.positionRoue];
            if (camera != null && card != null)
            {
                RectTransform rect = card.transform as RectTransform;
                Canvas canvas = card.GetComponentInParent<Canvas>();
                Camera uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(uiCamera, rect.TransformPoint(RectTransformUtility.CalculateRelativeRectTransformBounds(rect, rect).center));
                Renderer[] renderers = actor.GetComponentsInChildren<Renderer>(true);
                Vector3 center = actor.transform.position;
                if (renderers.Length > 0)
                {
                    Bounds bounds = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
                    center = bounds.center;
                }
                end += GetHorizontalAlignmentOffset(camera, center + end - actor.transform.position, screen.x);
            }
            Vector3 start = wasVisible ? actor.transform.position : end + retreat;
            Vector3 startScale = wasVisible ? actor.transform.localScale : Vector3.zero;
            actor.transform.position = start;
            actor.transform.localScale = startScale;
            actor.SetActive(true);
            motions.Add(new ActorMotion { Actor = actor, StartPosition = start, EndPosition = end,
                StartScale = startScale, EndScale = entering ? restScale : Vector3.zero,
                RestScale = restScale, ShowAtEnd = entering });
        }
        CombatSlotAnimator animator = manager.GetComponent<CombatSlotAnimator>();
        rotationMotions = motions;
        actorTransition = StartCoroutine(AnimateActorRotation(motions, animator != null ? animator.TransitionDuration : 0.32f));
    }

    private IEnumerator AnimateActorRotation(List<ActorMotion> motions, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / Mathf.Max(0.05f, duration)), 3f);
            foreach (ActorMotion motion in motions)
            {
                if (motion.Actor == null) continue;
                motion.Actor.transform.position = Vector3.Lerp(motion.StartPosition, motion.EndPosition, t);
                motion.Actor.transform.localScale = Vector3.Lerp(motion.StartScale, motion.EndScale, t);
            }
            yield return null;
        }
        CompleteActorRotation();
    }

    private void CompleteActorRotation()
    {
        foreach (ActorMotion motion in rotationMotions)
        {
            if (motion.Actor == null) continue;
            motion.Actor.transform.position = motion.EndPosition;
            motion.Actor.transform.localScale = motion.RestScale;
            motion.Actor.SetActive(motion.ShowAtEnd);
        }
        rotationMotions.Clear();
        actorTransition = null;
        alignedCardPositions.Clear();
        isRotating = false;
    }

    void MatchWorldScale(GameObject targetObject, Vector3 targetLossyScale) {
        targetObject.transform.localScale = Vector3.one;

        Physics.SyncTransforms();

        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) {
            Debug.LogWarning($"No Renderer found on {targetObject.name}");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers) {
            bounds.Encapsulate(rend.bounds);
        }

        float nativeHeight = bounds.size.y;
        if (nativeHeight <= 0.01f) return;
        float targetHeight = targetLossyScale.y;
        float scaleRatio = (targetHeight / nativeHeight) / 1.5f;
        targetObject.transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
    }
}
