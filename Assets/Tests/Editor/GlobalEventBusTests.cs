using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityMeta;
using YKW.Core.Events;

public sealed class GlobalEventBusTests
{
    private static WorkerDispatchProbe activeWorkerProbe;
    private static WorkerDispatchProbe activeMethodWorkerProbe;

    private sealed class WorkerDispatchProbe
    {
        public int ReceivedThreadId;
        public Exception Failure;
    }

    private sealed class NotifyChangedProbe
    {
        [NotifyChanged] public int Health;
        [NotifyChanged] public float Speed;
        [NotifyChanged] public bool Enabled;
        [NotifyChanged] public string Name;
        [NotifyChanged] public ProbeMode Mode;
        [NotifyChanged] public ProbeStats Stats;
        [NotifyChanged] public ProbeReference Reference;
        public void SetHealth(int value) { Health = value; }
        public void SetSpeed(float value) { Speed = value; }
        public void SetEnabled(bool value) { Enabled = value; }
        public void SetName(string value) { Name = value; }
        public void SetMode(ProbeMode value) { Mode = value; }
        public void SetStats(ProbeStats value) { Stats = value; }
        public void SetReference(ProbeReference value) { Reference = value; }
    }

    [Test]
    public void OwnedYokaiCanBeConstructedOffTheMainThread()
    {
        Type ownedYokaiType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiActif");
        Assert.IsNotNull(ownedYokaiType, "The runtime YoKaiActif type must be loaded.");

        object instance = null;
        Exception constructionFailure = null;
        Thread worker = new Thread(() =>
        {
            try
            {
                instance = Activator.CreateInstance(ownedYokaiType);
            }
            catch (Exception exception)
            {
                constructionFailure = exception;
            }
        });

        worker.Start();
        worker.Join();

        Assert.IsNull(constructionFailure);
        Assert.IsNotNull(instance);
        FieldInfo hpIvField = ownedYokaiType.GetField("hp_iv");
        FieldInfo ivStatsField = ownedYokaiType.GetField("IVStats");
        Assert.IsNotNull(hpIvField);
        Assert.IsNotNull(ivStatsField);
        Assert.AreEqual(0f, (float)hpIvField.GetValue(instance));
        Assert.AreEqual(0f, SumIvStats(ivStatsField.GetValue(instance)));

        MethodInfo assignIvs = ownedYokaiType.GetMethod("AttribuerIV");
        Assert.IsNotNull(assignIvs);
        assignIvs.Invoke(instance, null);
        Assert.AreEqual(40f, (float)hpIvField.GetValue(instance) / 2f +
            SumIvStats(ivStatsField.GetValue(instance)));
    }

    private static float SumIvStats(object stats)
    {
        Assert.IsNotNull(stats);
        Type statsType = stats.GetType();
        string[] fieldNames = { "Force", "Vitesse", "Esprit", "Défence" };
        float sum = 0f;
        for (int i = 0; i < fieldNames.Length; i++)
        {
            FieldInfo field = statsType.GetField(fieldNames[i]);
            Assert.IsNotNull(field, "Missing IV stat field: " + fieldNames[i]);
            sum += Convert.ToSingle(field.GetValue(stats));
        }
        return sum;
    }

    private struct EmptyEvent { }
    private struct DispatchMutationEvent { }
    private struct NestedEvent { }
    private struct FailureEvent { }
    private struct WorkerThreadEvent { }
    private enum ProbeMode { Idle, Active }

    private struct ProbeStats : IEquatable<ProbeStats>
    {
        public int Strength;
        public bool Equals(ProbeStats other) { return Strength == other.Strength; }
        public override bool Equals(object obj) { return obj is ProbeStats && Equals((ProbeStats)obj); }
        public override int GetHashCode() { return Strength; }
    }

    private sealed class ProbeReference
    {
        public int Value;
        public ProbeReference(int value) { Value = value; }
    }

    public sealed class MethodAspectProbeEvent : MethodEvent { }

    private static class MethodAspectProbeState
    {
        public static int BeforeCalls;
    }

    public sealed class ProbeBeforeAttribute : MethodAspectAttribute
    {
        [BeforeTemplate]
        public static void Before()
        {
            MethodAspectProbeState.BeforeCalls++;
        }
    }

    private sealed class PublishedMethodProbe
    {
        [ProbeBefore]
        [PublishEvent(typeof(MethodAspectProbeEvent))]
        public void FinishAction() { }
    }

    [Test]
    public void PublishWithoutSubscribersIsSafe()
    {
        Assert.DoesNotThrow(() => EventBus.Publish(new EmptyEvent()));
    }

    [Test]
    public void Combat2SceneHasSerializedGlobalUiWiring()
    {
        string scenePath = Path.Combine(Application.dataPath, "7 - SCENNE/Combat2.unity");
        string sceneYaml = File.ReadAllText(scenePath);
        string canvasId = FindActiveGameObjectId(sceneYaml, "Canvas");
        string combatManagerId = FindActiveGameObjectId(sceneYaml, "'| Gestionnaire combat'", true);
        Assert.IsNotNull(canvasId, "Combat2 must have an active Canvas.");
        Assert.IsNotNull(combatManagerId, "Combat2 must have an active combat manager.");
        string uiManager = FindComponentByScript(sceneYaml, combatManagerId, "dc045934ca2cbbda28965bb3736c96b9");
        Assert.IsNotNull(uiManager, "The combat manager must own CombatUiManager.");
        StringAssert.Contains("messageController: {fileID: 3333000001}", uiManager);
        StringAssert.Contains("resultController: {fileID: 3333000002}", uiManager);
        StringAssert.Contains("slotController: {fileID: 3333000003}", uiManager);
        StringAssert.Contains("actorMessageController: {fileID: 3333000006}", uiManager);
        StringAssert.Contains("bagController: {fileID: 3333000005}", uiManager);
        StringAssert.Contains("messagePanel: {fileID: 518997866}", uiManager);
        StringAssert.Contains("resultPanel: {fileID: 1875514373}", uiManager);
        StringAssert.Contains("bagPanel: {fileID: 481932157}", uiManager);
        Assert.IsNotNull(FindComponentByScript(sceneYaml, combatManagerId, "c315c466e6804f90b64fa1e930300006"),
            "CombatMessageUI must be serialized on the manager object.");
        Assert.IsNotNull(FindComponentByScript(sceneYaml, combatManagerId, "c315c466e6804f90b64fa1e930300007"),
            "CombatResultUI must be serialized on the manager object.");
        Assert.IsNotNull(FindComponentByScript(sceneYaml, combatManagerId, "c315c466e6804f90b64fa1e930300008"),
            "CombatSlotAnimator must be serialized on the manager object.");
        Assert.IsNotNull(FindComponentByScript(sceneYaml, combatManagerId, "696e0118078a571689f9642266d3df2d"),
            "CombatActorMessageUI must be serialized on the manager object.");
        Assert.IsNull(FindComponentByScript(sceneYaml, combatManagerId, "c315c466e6804f90b64fa1e930300009"),
            "The static CombatUIAnimator helper must not be serialized as a scene component.");

        string canvasTransformId = FindTransformId(sceneYaml, canvasId);
        Assert.IsNotNull(canvasTransformId);
        Assert.IsNotNull(FindChildId(sceneYaml, canvasTransformId, "Message"));
        Assert.IsNotNull(FindChildId(sceneYaml, canvasTransformId, "Sac"));
        Assert.IsNotNull(FindChildId(sceneYaml, canvasTransformId, "\"R\\xE9sultat\""));
        string resultsId = FindChildId(sceneYaml, canvasTransformId, "\"R\\xE9sultat\"");
        string resultsTransform = FindTransformId(sceneYaml, resultsId);
        foreach (string screenName in new[] { "WinScreen", "LostScreen" })
        {
            string screenId = FindChildId(sceneYaml, resultsTransform, screenName);
            string screenTransform = FindTransformId(sceneYaml, screenId);
            string contentId = FindChildId(sceneYaml, screenTransform, "Content");
            string contentTransform = FindTransformId(sceneYaml, contentId);
            string lootId = FindChildId(sceneYaml, contentTransform, "Loot");
            string lootTransform = FindTransformId(sceneYaml, lootId);
            string itemContainerId = FindChildId(sceneYaml, lootTransform, "ItemContainer");
            string itemContainerTransform = FindTransformId(sceneYaml, itemContainerId);
            string viewportId = FindChildId(sceneYaml, itemContainerTransform, "Content");
            Assert.IsNotNull(FindComponentByScript(sceneYaml, viewportId,
                "1aa08ab6e0800fa44ae55d278d1423e3"),
                screenName + " must provide its serialized ScrollRect loot viewport.");
        }
        string bagId = FindChildId(sceneYaml, canvasTransformId, "Sac");
        Assert.IsNotNull(bagId);
        Assert.IsNotNull(FindComponentByScript(sceneYaml, bagId, "c315c466e6804f90b64fa1e93030000a"),
            "CombatBagUI must be attached to the serialized bag panel.");
        Assert.IsNotNull(FindActiveChildId(sceneYaml, canvasTransformId, "'| La roue'"));

        string combat = FindComponentByScript(sceneYaml, combatManagerId, "93b5899dc999e874b9725ec9edc4e013");
        Assert.IsNotNull(combat, "The combat manager component must be serialized on the active root.");
        Match slots = Regex.Match(combat,
            @"lesEspacesM[^\r\n]*daillonDansLaRoue[^\r\n]*\r?\n(?<slots>(?:  - \{fileID:.*\r?\n)+)");
        Assert.IsTrue(slots.Success, "The six wheel Image references must be serialized.");
        Assert.AreEqual(6, Regex.Matches(slots.Groups["slots"].Value, @"(?m)^  - \{fileID:").Count);
    }

    [Test]
    public void Combat2SceneOpensAndResolvesTheSerializedUiControllers()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/7 - SCENNE/Combat2.unity", OpenSceneMode.Additive);
        try
        {
            Assert.IsTrue(scene.IsValid());
            Assert.IsTrue(scene.isLoaded);

            Type uiManagerType = FindRuntimeType("CombatUiManager");
            Assert.IsNotNull(uiManagerType);
            Component uiManager = null;
            GameObject[] roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length && uiManager == null; i++)
            {
                Component[] components = roots[i].GetComponentsInChildren(uiManagerType, true);
                if (components.Length > 0) uiManager = components[0];
            }

            Assert.IsNotNull(uiManager, "The opened scene must contain its CombatUiManager component.");
            string[] references =
            {
                "combatManager", "sceneCanvas", "messageController", "resultController",
                "bagController", "slotController", "actorMessageController", "messagePanel",
                "resultPanel", "bagPanel"
            };
            for (int i = 0; i < references.Length; i++)
            {
                FieldInfo field = uiManagerType.GetField(references[i], BindingFlags.Instance | BindingFlags.NonPublic);
                Assert.IsNotNull(field, "Missing serialized field " + references[i]);
                Assert.IsNotNull(field.GetValue(uiManager), "Unresolved scene reference " + references[i]);
            }
        }
        finally
        {
            if (scene.IsValid() && scene.isLoaded) EditorSceneManager.CloseScene(scene, true);
        }
    }

    [UnityTest]
    public IEnumerator RewardSlotsCopyWholeTemplateWithoutMovingAuthoredSlots()
    {
        yield return new EnterPlayMode();
        GameObject screen = new GameObject("Reward screen", typeof(RectTransform));
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        GameObject first = new GameObject("YoKaiSlot", typeof(RectTransform), imageType);
        GameObject second = new GameObject("YoKaiSlot", typeof(RectTransform), imageType);
        first.transform.SetParent(screen.transform, false);
        second.transform.SetParent(screen.transform, false);
        RectTransform secondRect = (RectTransform)second.transform;
        secondRect.anchoredPosition = new Vector2(180f, -90f);
        secondRect.pivot = Vector2.one;
        secondRect.anchorMin = secondRect.anchorMax = Vector2.one;
        Vector3 authoredCenter = secondRect.TransformPoint(secondRect.rect.center);
        RectTransform portrait = EnsurePath(first.transform, "YoKaiImages") as RectTransform;
        portrait.anchoredPosition = new Vector2(8f, 12f);
        EnsurePath(first.transform, "Bubble/YoKaiName");
        EnsurePath(first.transform, "XP");
        EnsurePath(second.transform, "Wrong old content");
        try
        {
            Type resultType = FindRuntimeType("CombatResultUI");
            resultType.GetMethod("NormalizeRewardSlots", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { screen.transform });
            yield return null;
            Assert.AreEqual(1, second.transform.childCount);
            Assert.AreEqual(3, second.transform.Find("Content").childCount);
            Assert.IsNotNull(second.transform.Find("Content/Bubble/YoKaiName"));
            Assert.IsNull(second.transform.Find("Wrong old content"));
            Assert.AreEqual(portrait.anchoredPosition,
                ((RectTransform)second.transform.Find("Content/YoKaiImages")).anchoredPosition);
            Assert.Less(Vector3.Distance(authoredCenter, secondRect.TransformPoint(secondRect.rect.center)), 0.001f);
            Assert.AreEqual(Vector2.one, secondRect.pivot);
            Assert.AreEqual(Vector2.one, secondRect.anchorMin);
            Assert.AreEqual(Vector2.one, secondRect.anchorMax);
            Assert.IsTrue(((Behaviour)first.GetComponent(imageType)).enabled);
            Assert.IsTrue(((Behaviour)second.GetComponent(imageType)).enabled);
            foreach (Transform child in second.transform) Assert.IsFalse(child.gameObject.activeSelf);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(screen);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator BattleCardsClonePortraitsAndReserveNonzeroLayoutSpace()
    {
        yield return new EnterPlayMode();
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        Type layoutType = FindRuntimeType("UnityEngine.UI.VerticalLayoutGroup");
        Type controllerType = FindRuntimeType("CombatSceneController");
        Type yokaiType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        GameObject row = new GameObject("Battle cards", typeof(RectTransform), layoutType);
        ((RectTransform)row.transform).sizeDelta = new Vector2(400f, 400f);
        GameObject template = new GameObject("Info-Yo-Kai", typeof(RectTransform));
        template.transform.SetParent(row.transform, false);
        ((RectTransform)template.transform).sizeDelta = Vector2.zero;
        RectTransform medal = (RectTransform)EnsurePath(template.transform, "médaillon");
        medal.sizeDelta = new Vector2(40f, 40f);
        medal.gameObject.AddComponent(imageType);
        RectTransform stats = (RectTransform)EnsurePath(template.transform, "Stats");
        stats.sizeDelta = new Vector2(120f, 30f);
        GameObject host = new GameObject("Battle card controller");
        Component controller = host.AddComponent(controllerType);
        IList roster = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(yokaiType));
        Texture2D texture = new Texture2D(2, 2);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
        roster.Add(null); // A vacant first slot must keep its place in the layout.
        for (int i = 0; i < 2; i++)
        {
            object yokai = Activator.CreateInstance(yokaiType);
            yokaiType.GetField("imageFace").SetValue(yokai, sprite);
            roster.Add(yokai);
        }
        try
        {
            controllerType.GetMethod("RenderTemplateCards", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(controller, new object[] { row.transform, roster });
            yield return null;
            Assert.AreEqual(3, row.transform.childCount);
            RectTransform first = (RectTransform)row.transform.GetChild(0);
            RectTransform second = (RectTransform)row.transform.GetChild(1);
            Assert.Greater(first.rect.height, 0f);
            Assert.Greater(second.rect.height, 0f);
            Assert.AreNotEqual(first.anchoredPosition.y, second.anchoredPosition.y);
            foreach (Transform card in row.transform)
            {
                Assert.IsTrue(card.gameObject.activeSelf);
                CanvasGroup group = card.GetComponent<CanvasGroup>();
                if (card.GetSiblingIndex() == 0) Assert.AreEqual(0f, group.alpha);
                else
                {
                    Assert.AreEqual(1f, group.alpha);
                    Assert.AreSame(sprite, imageType.GetProperty("sprite").GetValue(
                        card.Find("médaillon").GetComponent(imageType), null));
                }
            }

        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(row);
            UnityEngine.Object.DestroyImmediate(sprite);
            UnityEngine.Object.DestroyImmediate(texture);
        }
        yield return new ExitPlayMode();
    }

    [Test]
    public void AutomaticWheelFlipRequiresAnEmptyLivingFrontAndLivingReserve()
    {
        Type actorType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        Type controllerType = FindRuntimeType("CombatTurnController");
        MethodInfo rotation = controllerType.GetMethod("GetAutomaticWheelRotation",
            BindingFlags.Static | BindingFlags.NonPublic);
        IList roster = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(actorType));
        object front = Activator.CreateInstance(actorType);
        object reserve = Activator.CreateInstance(actorType);
        actorType.GetField("positionRoue").SetValue(front, 0);
        actorType.GetField("positionRoue").SetValue(reserve, 4);
        actorType.GetField("hp_me").SetValue(front, 0);
        actorType.GetField("hp_me").SetValue(reserve, 10);
        roster.Add(null);
        roster.Add(front);
        roster.Add(reserve);
        Assert.AreEqual(3, rotation.Invoke(null, new object[] { roster }));
        actorType.GetField("hp_me").SetValue(front, 1);
        Assert.AreEqual(0, rotation.Invoke(null, new object[] { roster }));
        actorType.GetField("hp_me").SetValue(front, 0);
        actorType.GetField("hp_me").SetValue(reserve, 0);
        Assert.AreEqual(0, rotation.Invoke(null, new object[] { roster }));
        roster.Clear();
        Assert.AreEqual(0, rotation.Invoke(null, new object[] { roster }));
    }

    [Test]
    public void ActorAlignmentUsesCardScreenXWithoutChangingDepthOrHeight()
    {
        GameObject host = new GameObject("Alignment camera", typeof(Camera));
        Camera camera = host.GetComponent<Camera>();
        camera.transform.position = new Vector3(0f, 2f, -10f);
        try
        {
            Vector3 center = new Vector3(-2f, 1f, 0f);
            Vector3 before = camera.WorldToScreenPoint(center);
            float targetX = camera.pixelRect.center.x;
            Type controller = FindRuntimeType("CombatSceneController");
            Vector3 offset = (Vector3)controller.GetMethod("GetHorizontalAlignmentOffset",
                BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { camera, center, targetX });
            Vector3 after = camera.WorldToScreenPoint(center + offset);
            Assert.AreEqual(targetX, after.x, 0.01f);
            Assert.AreEqual(before.y, after.y, 0.01f);
            Assert.AreEqual(before.z, after.z, 0.001f);
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [UnityTest]
    public IEnumerator ToonShaderPreservesUrpTextureAndMappingReachesEveryMaterial()
    {
        yield return new EnterPlayMode();
        Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");
        Assert.IsNotNull(unlit);
        Texture2D originalTexture = new Texture2D(2, 2);
        Material source = new Material(unlit);
        source.SetTexture("_BaseMap", originalTexture);
        GameObject actor = new GameObject("Texture test actor", typeof(MeshRenderer));
        GameObject child = new GameObject("Inactive mesh", typeof(MeshRenderer));
        child.transform.SetParent(actor.transform, false);
        child.SetActive(false);
        actor.GetComponent<MeshRenderer>().sharedMaterials = new[] { source, source };
        child.GetComponent<MeshRenderer>().sharedMaterials = new[] { source, source };
        var instances = new HashSet<Material>();
        try
        {
            Type shaderType = FindRuntimeType("MakeAShader");
            Component appearance = actor.AddComponent(shaderType);
            shaderType.GetField("choixForcer").SetValue(appearance, true);
            shaderType.GetField("forcerToonShader").SetValue(appearance, true);
            shaderType.GetMethod("ApplyYokaiShader").Invoke(appearance, null);
            foreach (Renderer renderer in actor.GetComponentsInChildren<Renderer>(true))
            foreach (Material material in renderer.materials)
            {
                instances.Add(material);
                Assert.AreSame(originalTexture, material.GetTexture("_MainTex"));
                Assert.AreEqual(1f, material.GetFloat("_DetailTex"));
                Assert.IsTrue(material.IsKeywordEnabled("_TEX_ON"));
            }
            Type mappingType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiModele3D");
            object mapping = Activator.CreateInstance(mappingType);
            mappingType.GetField("textureFiles").SetValue(mapping, new[] { "texture_y3_01.png" });
            Texture2D mappedTexture = Resources.Load<Texture2D>("yo-kai-API/data/_3D/textures/texture_y3_01");
            Assert.IsNotNull(mappedTexture);
            Type loaderType = FindRuntimeType("MakeATexture");
            Component loader = actor.AddComponent(loaderType);
            loaderType.GetMethod("Initialize").Invoke(loader, new[] { mapping });
            yield return null;
            foreach (Renderer renderer in actor.GetComponentsInChildren<Renderer>(true))
            foreach (Material material in renderer.materials)
            {
                instances.Add(material);
                Assert.AreSame(mappedTexture, material.GetTexture("_MainTex"));
            }
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(actor);
            foreach (Material material in instances) UnityEngine.Object.DestroyImmediate(material);
            UnityEngine.Object.DestroyImmediate(source);
            UnityEngine.Object.DestroyImmediate(originalTexture);
        }
        yield return new ExitPlayMode();
    }

    [Test]
    public void RotationInputsAreQueuedInOrderAndIgnoredDuringMenus()
    {
        GameObject host = new GameObject("Inactive rotation manager");
        host.SetActive(false);
        Type type = FindRuntimeType("CombatManager");
        Component manager = host.AddComponent(type);
        type.GetField("combatEnCours", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(manager, true);
        MethodInfo request = type.GetMethod("DemanderRotationRoue");
        MethodInfo take = type.GetMethod("TakeRotationRequest", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            request.Invoke(manager, new object[] { 1 });
            request.Invoke(manager, new object[] { -1 });
            request.Invoke(manager, new object[] { 1 });
            Assert.AreEqual(1, take.Invoke(manager, null));
            Assert.AreEqual(-1, take.Invoke(manager, null));
            Assert.AreEqual(1, take.Invoke(manager, null));
            Assert.AreEqual(0, take.Invoke(manager, null));
            type.GetField("gameInStop").SetValue(manager, true);
            request.Invoke(manager, new object[] { 1 });
            Assert.AreEqual(0, take.Invoke(manager, null));
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [Test]
    public void CompactAndFullCardsUseTheSameYokaiPortrait()
    {
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        Type actorType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        GameObject compact = new GameObject("InfoYo-KaiSmall", typeof(RectTransform), imageType);
        ((RectTransform)compact.transform).sizeDelta = Vector2.zero;
        ((Behaviour)compact.GetComponent(imageType)).enabled = false;
        GameObject full = new GameObject("InfoYo-Kai", typeof(RectTransform));
        GameObject portrait = new GameObject("médaillon", typeof(RectTransform), imageType);
        portrait.transform.SetParent(full.transform, false);
        Texture2D texture = new Texture2D(2, 2);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
        try
        {
            object actor = Activator.CreateInstance(actorType);
            actorType.GetField("imageFace").SetValue(actor, sprite);
            MethodInfo populate = FindRuntimeType("CombatSceneController").GetMethod("PopulateTemplateCard",
                BindingFlags.Static | BindingFlags.NonPublic);
            populate.Invoke(null, new object[] { compact, actor });
            populate.Invoke(null, new object[] { full, actor });
            Assert.AreSame(sprite, imageType.GetProperty("sprite").GetValue(compact.GetComponent(imageType)));
            Assert.IsTrue(((Behaviour)compact.GetComponent(imageType)).enabled);
            Assert.Greater(((RectTransform)compact.transform).rect.width, 0f);
            Assert.AreSame(sprite, imageType.GetProperty("sprite").GetValue(portrait.GetComponent(imageType)));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(compact);
            UnityEngine.Object.DestroyImmediate(full);
            UnityEngine.Object.DestroyImmediate(sprite);
            UnityEngine.Object.DestroyImmediate(texture);
        }
    }

    [Test]
    public void ResultRosterKeepsOriginalSlotsAfterCombatRotations()
    {
        GameObject host = new GameObject("Original team order");
        host.SetActive(false);
        Type managerType = FindRuntimeType("CombatManager");
        Type resultType = FindRuntimeType("CombatResultUI");
        Type actorType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        Component manager = host.AddComponent(managerType);
        Component result = host.AddComponent(resultType);
        object actor = Activator.CreateInstance(actorType);
        actorType.GetField("positionRoue").SetValue(actor, 1);
        ((IList)managerType.GetField("listeYoKaiAmiSurLeTerrain").GetValue(manager)).Add(actor);
        try
        {
            resultType.GetMethod("BindRoster").Invoke(result, new object[] { manager });
            resultType.GetMethod("CaptureInitialRoster").Invoke(result, null);
            actorType.GetField("positionRoue").SetValue(actor, 4);
            IDictionary slots = (IDictionary)resultType.GetField("initialSlots",
                BindingFlags.NonPublic | BindingFlags.Instance).GetValue(result);
            Assert.AreEqual(1, slots[actor]);
        }
        finally { UnityEngine.Object.DestroyImmediate(host); }
    }

    [Test]
    public void SparseBattleRosterKeepsActualWheelSlotIndices()
    {
        Type actorType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        IList roster = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(actorType));
        object first = Activator.CreateInstance(actorType);
        object second = Activator.CreateInstance(actorType);
        actorType.GetField("positionRoue").SetValue(first, 1);
        actorType.GetField("positionRoue").SetValue(second, 3);
        roster.Add(first);
        roster.Add(second);
        MethodInfo build = FindRuntimeType("CombatSceneController").GetMethod("BuildSlotRoster", BindingFlags.Static | BindingFlags.NonPublic);
        IList front = (IList)build.Invoke(null, new object[] { roster, 0 });
        IList reserve = (IList)build.Invoke(null, new object[] { roster, 3 });
        Assert.AreEqual(3, front.Count);
        Assert.IsNull(front[0]);
        Assert.AreSame(first, front[1]);
        Assert.IsNull(front[2]);
        Assert.IsNull(reserve[0]);
        Assert.IsNull(reserve[1]);
        Assert.AreSame(second, reserve[2]);
    }

    [Test]
    public void FrontReserveChangesReuseAlreadyLoadedActors()
    {
        GameObject managerObject = new GameObject("Inactive combat manager");
        managerObject.SetActive(false);
        Type managerType = FindRuntimeType("CombatManager");
        Component manager = managerObject.AddComponent(managerType);
        GameObject host = new GameObject("Scene controller");
        Type sceneType = FindRuntimeType("CombatSceneController");
        Component controller = host.AddComponent(sceneType);
        sceneType.GetMethod("Initialize").Invoke(controller, new object[] { manager });
        Type actorType = FindRuntimeType("YKW.Core.YoKai.Class.YoKaiCombat");
        object first = Activator.CreateInstance(actorType);
        object second = Activator.CreateInstance(actorType);
        GameObject firstModel = new GameObject("First loaded model");
        GameObject secondModel = new GameObject("Second loaded model");
        actorType.GetField("gameObjectSurLaScène").SetValue(first, firstModel);
        actorType.GetField("gameObjectSurLaScène").SetValue(second, secondModel);
        actorType.GetField("positionRoue").SetValue(first, 0);
        actorType.GetField("positionRoue").SetValue(second, 3);
        IList roster = (IList)managerType.GetField("listeYoKaiAmiSurLeTerrain").GetValue(manager);
        roster.Add(first);
        roster.Add(second);
        try
        {
            sceneType.GetMethod("SpawnFriendlyActors").Invoke(controller, null);
            Assert.AreSame(firstModel, actorType.GetField("gameObjectSurLaScène").GetValue(first));
            Assert.AreSame(secondModel, actorType.GetField("gameObjectSurLaScène").GetValue(second));
            Assert.IsTrue(firstModel.activeSelf);
            Assert.IsFalse(secondModel.activeSelf);
            actorType.GetField("positionRoue").SetValue(first, 3);
            actorType.GetField("positionRoue").SetValue(second, 0);
            sceneType.GetMethod("SpawnFriendlyActors").Invoke(controller, null);
            Assert.IsFalse(firstModel.activeSelf);
            Assert.IsTrue(secondModel.activeSelf);
            Assert.AreSame(secondModel, actorType.GetField("gameObjectSurLaScène").GetValue(second));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(managerObject);
            UnityEngine.Object.DestroyImmediate(firstModel);
            UnityEngine.Object.DestroyImmediate(secondModel);
        }
    }

    [UnityTest]
    public IEnumerator CardRotationMovesWholeTemplatesAndRestoresEmptySlotVisibility()
    {
        yield return new EnterPlayMode();
        GameObject canvasObject = new GameObject("Card rotation canvas", typeof(RectTransform), typeof(Canvas));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var cards = new List<GameObject>();
        for (int i = 0; i < 6; i++)
        {
            GameObject card = new GameObject("Card " + i, typeof(RectTransform), typeof(CanvasGroup));
            card.transform.SetParent(canvasObject.transform, false);
            ((RectTransform)card.transform).anchoredPosition = new Vector2((i % 3) * 100f, i < 3 ? 0f : -100f);
            EnsurePath(card.transform, "Portrait");
            EnsurePath(card.transform, "Name");
            EnsurePath(card.transform, "Gauges");
            card.GetComponent<CanvasGroup>().alpha = i < 2 ? 1f : 0f;
            cards.Add(card);
        }
        GameObject host = new GameObject("Card animator host");
        Type animatorType = FindRuntimeType("CombatSlotAnimator");
        Component animator = host.AddComponent(animatorType);
        animatorType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(animator, 0.2f);
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        IList images = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(imageType));
        animatorType.GetMethod("Initialize").Invoke(animator, new object[] { images, canvas });
        animatorType.GetMethod("BindCards").Invoke(animator, new object[] { cards });
        try
        {
            EventBus.Publish(new CombatSlotsChangedEvent(new[] { new SlotChange(10, 0, 1), new SlotChange(20, 1, 2) }));
            RectTransform overlay = null;
            int count = 0;
            foreach (Transform child in canvasObject.transform)
                if (child.name == "Combat card transition") { overlay = (RectTransform)child; count++; }
            Assert.AreEqual(2, count);
            Assert.IsNotNull(overlay.Find("Portrait"));
            Assert.IsNotNull(overlay.Find("Name"));
            Assert.IsNotNull(overlay.Find("Gauges"));
            Vector2 start = overlay.anchoredPosition;
            // Commit the new roster, just as RefreshCombatUI does after the event.
            cards[0].GetComponent<CanvasGroup>().alpha = 0f;
            cards[1].GetComponent<CanvasGroup>().alpha = 1f;
            cards[2].GetComponent<CanvasGroup>().alpha = 1f;
            animatorType.GetMethod("BindCards").Invoke(animator, new object[] { cards });
            Assert.AreEqual(0f, cards[1].GetComponent<CanvasGroup>().alpha);
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.Greater(Vector2.Distance(start, overlay.anchoredPosition), 0.1f);
            yield return new WaitForSecondsRealtime(0.25f);
            Assert.AreEqual(0f, cards[0].GetComponent<CanvasGroup>().alpha);
            Assert.AreEqual(1f, cards[1].GetComponent<CanvasGroup>().alpha);
            Assert.AreEqual(1f, cards[2].GetComponent<CanvasGroup>().alpha);
            Assert.IsFalse((bool)animatorType.GetProperty("IsTransitioning").GetValue(animator, null));
            foreach (Transform child in canvasObject.transform) Assert.AreNotEqual("Combat card transition", child.name);

            EventBus.Publish(new CombatSlotsChangedEvent(new[] { new SlotChange(10, 1, 0), new SlotChange(20, 2, 1) }));
            cards[0].GetComponent<CanvasGroup>().alpha = 1f;
            cards[1].GetComponent<CanvasGroup>().alpha = 1f;
            cards[2].GetComponent<CanvasGroup>().alpha = 0f;
            animatorType.GetMethod("BindCards").Invoke(animator, new object[] { cards });
            host.SetActive(false);
            Assert.AreEqual(1f, cards[0].GetComponent<CanvasGroup>().alpha);
            Assert.AreEqual(1f, cards[1].GetComponent<CanvasGroup>().alpha);
            Assert.AreEqual(0f, cards[2].GetComponent<CanvasGroup>().alpha);
            yield return null;
            foreach (Transform child in canvasObject.transform) Assert.AreNotEqual("Combat card transition", child.name);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(canvasObject);
        }
        yield return new ExitPlayMode();
    }

    [Test]
    public void DuplicateDelegateIsDispatchedOnce()
    {
        int received = 0;
        Action<EmptyEvent> handler = _ => received++;
        IDisposable first = EventBus.Subscribe(handler);
        IDisposable duplicate = EventBus.Subscribe(handler);
        try
        {
            EventBus.Publish(new EmptyEvent());
            Assert.AreEqual(1, received);
        }
        finally
        {
            duplicate.Dispose();
            first.Dispose();
        }
    }

    [Test]
    public void SubscribersReceiveEventsAndCanUnsubscribe()
    {
        int received = 0;
        Action<EmptyEvent> first = _ => received++;
        Action<EmptyEvent> second = _ => received += 10;
        IDisposable firstSubscription = EventBus.Subscribe(first);
        IDisposable secondSubscription = EventBus.Subscribe(second);
        try
        {
            EventBus.Publish(new EmptyEvent());
            Assert.AreEqual(11, received);
            firstSubscription.Dispose();
            EventBus.Publish(new EmptyEvent());
            Assert.AreEqual(21, received);
        }
        finally
        {
            firstSubscription.Dispose();
            secondSubscription.Dispose();
        }
    }

    [Test]
    public void SubscribeAndUnsubscribeDuringDispatchUseStableSnapshot()
    {
        List<string> calls = new List<string>();
        Action<DispatchMutationEvent> late = _ => calls.Add("late");
        Action<DispatchMutationEvent> second = _ => calls.Add("second");
        IDisposable secondSubscription = null;
        Action<DispatchMutationEvent> first = _ =>
        {
            calls.Add("first");
            secondSubscription.Dispose();
            EventBus.Subscribe(late);
        };
        IDisposable firstSubscription = EventBus.Subscribe(first);
        secondSubscription = EventBus.Subscribe(second);
        try
        {
            EventBus.Publish(new DispatchMutationEvent());
            CollectionAssert.AreEqual(new[] { "first", "second" }, calls);
            calls.Clear();
            firstSubscription.Dispose();
            EventBus.Publish(new DispatchMutationEvent());
            CollectionAssert.AreEqual(new[] { "late" }, calls);
        }
        finally
        {
            firstSubscription.Dispose();
            secondSubscription.Dispose();
            EventBus.Unsubscribe(late);
        }
    }

    [Test]
    public void NestedEventsAreDispatchedSynchronously()
    {
        int value = 0;
        Action<NestedEvent> child = _ => value++;
        Action<EmptyEvent> parent = _ => EventBus.Publish(new NestedEvent());
        IDisposable childSubscription = EventBus.Subscribe(child);
        IDisposable parentSubscription = EventBus.Subscribe(parent);
        try
        {
            EventBus.Publish(new EmptyEvent());
            Assert.AreEqual(1, value);
        }
        finally
        {
            childSubscription.Dispose();
            parentSubscription.Dispose();
        }
    }

    [Test]
    public void HandlerFailureDoesNotPreventOtherSubscribers()
    {
        int secondHandlerRan = 0;
        Action<FailureEvent> throws = _ => throw new InvalidOperationException("expected");
        Action<FailureEvent> second = _ => secondHandlerRan++;
        IDisposable firstSubscription = EventBus.Subscribe(throws);
        IDisposable secondSubscription = EventBus.Subscribe(second);
        try
        {
            Assert.Throws<AggregateException>(() => EventBus.Publish(new FailureEvent()));
            Assert.AreEqual(1, secondHandlerRan);
        }
        finally
        {
            firstSubscription.Dispose();
            secondSubscription.Dispose();
        }
    }

    [Test]
    public void MethodEventPublisherSetsOriginMetadata()
    {
        MethodAspectProbeEvent observed = null;
        Action<MethodAspectProbeEvent> handler = value => observed = value;
        IDisposable subscription = EventBus.Subscribe(handler);
        try
        {
            MethodEventPublisher.Publish(typeof(MethodAspectProbeEvent), "Example.Gameplay", "FinishAction");
            Assert.IsNotNull(observed);
            Assert.AreEqual("Example.Gameplay", observed.DeclaringType);
            Assert.AreEqual("FinishAction", observed.MethodName);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void PublishEventAspectRunsAfterNormalMethodReturn()
    {
        MethodAspectProbeEvent observed = null;
        IDisposable subscription = EventBus.Subscribe<MethodAspectProbeEvent>(value => observed = value);
        try
        {
            new PublishedMethodProbe().FinishAction();
            Assert.IsNotNull(observed);
            Assert.AreEqual("GlobalEventBusTests/PublishedMethodProbe", observed.DeclaringType);
            Assert.AreEqual("FinishAction", observed.MethodName);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [UnityTest]
    public IEnumerator PublishEventAspectFromWorkerDispatchesOnUnityMainThread()
    {
        yield return new EnterPlayMode();

        int unityThreadId = Thread.CurrentThread.ManagedThreadId;
        WorkerDispatchProbe probe = new WorkerDispatchProbe();
        activeMethodWorkerProbe = probe;
        IDisposable subscription = EventBus.Subscribe<MethodAspectProbeEvent>(ObserveMethodWorkerEvent);
        Thread worker = new Thread(PublishMethodWorkerEvent);

        try
        {
            worker.Start();
            float timeout = Time.realtimeSinceStartup + 2f;
            while (worker.IsAlive && Time.realtimeSinceStartup < timeout) yield return null;
            while (Volatile.Read(ref probe.ReceivedThreadId) == 0 && Time.realtimeSinceStartup < timeout)
                yield return null;

            worker.Join(1000);
            Assert.IsFalse(worker.IsAlive, "The worker method did not complete.");
            Assert.IsNull(probe.Failure);
            Assert.AreEqual(unityThreadId, Volatile.Read(ref probe.ReceivedThreadId));
        }
        finally
        {
            subscription.Dispose();
            activeMethodWorkerProbe = null;
        }

        yield return new ExitPlayMode();
    }

    [Test]
    public void MethodAspectsComposeOnOneMethod()
    {
        MethodAspectProbeState.BeforeCalls = 0;
        int published = 0;
        IDisposable subscription = EventBus.Subscribe<MethodAspectProbeEvent>(_ => published++);
        try
        {
            new PublishedMethodProbe().FinishAction();
            Assert.AreEqual(1, MethodAspectProbeState.BeforeCalls);
            Assert.AreEqual(1, published);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void NotifyChangedPublishesTypedOldAndNewFieldValues()
    {
        var owner = new NotifyChangedProbe();
        ValueChangedEvent<int> observed = null;
        int eventCount = 0;
        IDisposable subscription = EventBus.Subscribe<ValueChangedEvent<int>>(evt =>
        {
            observed = evt;
            eventCount++;
        });

        try
        {
            owner.SetHealth(42);
            Assert.AreSame(owner, observed.Source);
            Assert.AreEqual("Health", observed.MemberName);
            Assert.AreEqual(typeof(NotifyChangedProbe).FullName, observed.SourceType);
            Assert.AreEqual(0, observed.OldValue);
            Assert.AreEqual(42, observed.NewValue);

            owner.SetHealth(42);
            Assert.AreEqual(1, eventCount, "Assigning the same final value does not publish again.");
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void NotifyChangedSupportsStringsEnumsStructsAndReferences()
    {
        var owner = new NotifyChangedProbe();
        var floatChanges = new List<ValueChangedEvent<float>>();
        var boolChanges = new List<ValueChangedEvent<bool>>();
        var stringChanges = new List<ValueChangedEvent<string>>();
        var enumChanges = new List<ValueChangedEvent<ProbeMode>>();
        var structChanges = new List<ValueChangedEvent<ProbeStats>>();
        var referenceChanges = new List<ValueChangedEvent<ProbeReference>>();
        IDisposable floatSubscription = EventBus.Subscribe<ValueChangedEvent<float>>(floatChanges.Add);
        IDisposable boolSubscription = EventBus.Subscribe<ValueChangedEvent<bool>>(boolChanges.Add);
        IDisposable stringSubscription = EventBus.Subscribe<ValueChangedEvent<string>>(stringChanges.Add);
        IDisposable enumSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeMode>>(enumChanges.Add);
        IDisposable structSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeStats>>(structChanges.Add);
        IDisposable referenceSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeReference>>(referenceChanges.Add);

        try
        {
            owner.SetSpeed(1.25f);
            owner.SetSpeed(1.25f);
            owner.SetEnabled(true);
            owner.SetEnabled(true);
            owner.SetName("Jibanyan");
            owner.SetName(new string("Jibanyan".ToCharArray()));
            owner.SetMode(ProbeMode.Active);
            owner.SetMode(ProbeMode.Active);
            owner.SetStats(new ProbeStats { Strength = 8 });
            owner.SetStats(new ProbeStats { Strength = 8 });
            ProbeReference first = new ProbeReference(1);
            owner.SetReference(first);
            owner.SetReference(first);
            owner.SetReference(new ProbeReference(1));

            Assert.AreEqual(1, floatChanges.Count);
            Assert.AreEqual(0f, floatChanges[0].OldValue);
            Assert.AreEqual(1.25f, floatChanges[0].NewValue);
            Assert.AreEqual(1, boolChanges.Count);
            Assert.IsFalse(boolChanges[0].OldValue);
            Assert.IsTrue(boolChanges[0].NewValue);
            Assert.AreEqual(1, stringChanges.Count);
            Assert.IsNull(stringChanges[0].OldValue);
            Assert.AreEqual("Jibanyan", stringChanges[0].NewValue);
            Assert.AreEqual(1, enumChanges.Count);
            Assert.AreEqual(ProbeMode.Idle, enumChanges[0].OldValue);
            Assert.AreEqual(ProbeMode.Active, enumChanges[0].NewValue);
            Assert.AreEqual(1, structChanges.Count);
            Assert.AreEqual(0, structChanges[0].OldValue.Strength);
            Assert.AreEqual(8, structChanges[0].NewValue.Strength);
            Assert.AreEqual(2, referenceChanges.Count,
                "Reference equality reports only changes between different object references.");
            Assert.AreSame(first, referenceChanges[0].NewValue);
        }
        finally
        {
            floatSubscription.Dispose();
            boolSubscription.Dispose();
            stringSubscription.Dispose();
            enumSubscription.Dispose();
            structSubscription.Dispose();
            referenceSubscription.Dispose();
        }
    }

    [Test]
    public void ObservableValueIncludesOwnerAndUsesValueEquality()
    {
        object owner = new object();
        ObservableValue<string> value = new ObservableValue<string>(owner, "Name", "Yo-kai");
        ValueChangedEvent<string> observed = null;
        int eventCount = 0;
        Action<ValueChangedEvent<string>> handler = evt => { observed = evt; eventCount++; };
        IDisposable subscription = EventBus.Subscribe(handler);
        try
        {
            value.Value = new string(new[] { 'Y', 'o', '-', 'k', 'a', 'i' });
            value.Value = "Jibanyan";
            Assert.AreEqual(1, eventCount);
            Assert.AreSame(owner, observed.Source);
            Assert.AreEqual("Name", observed.MemberName);
            Assert.AreEqual("Yo-kai", observed.OldValue);
            Assert.AreEqual("Jibanyan", observed.NewValue);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void ObservableValueSupportsEnumsStructsAndReferenceTypes()
    {
        ObservableValue<ProbeMode> mode = new ObservableValue<ProbeMode>(this, "Mode", ProbeMode.Idle);
        ObservableValue<ProbeStats> stats = new ObservableValue<ProbeStats>(this, "Stats",
            new ProbeStats { Strength = 4 });
        ProbeReference initialReference = new ProbeReference(9);
        ObservableValue<ProbeReference> reference = new ObservableValue<ProbeReference>(this, "Reference",
            initialReference);

        int enumChanges = 0;
        int structChanges = 0;
        int referenceChanges = 0;
        IDisposable enumSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeMode>>(_ => enumChanges++);
        IDisposable structSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeStats>>(_ => structChanges++);
        IDisposable referenceSubscription = EventBus.Subscribe<ValueChangedEvent<ProbeReference>>(_ => referenceChanges++);
        try
        {
            mode.Value = ProbeMode.Active;
            mode.Value = ProbeMode.Active;
            stats.Value = new ProbeStats { Strength = 4 };
            stats.Value = new ProbeStats { Strength = 8 };
            reference.Value = initialReference;
            reference.Value = new ProbeReference(9);

            Assert.AreEqual(1, enumChanges, "Enums use their normal value equality.");
            Assert.AreEqual(1, structChanges, "Structs can provide their own value equality.");
            Assert.AreEqual(1, referenceChanges,
                "Reference types follow EqualityComparer<T>.Default; distinct references are distinct values.");
        }
        finally
        {
            enumSubscription.Dispose();
            structSubscription.Dispose();
            referenceSubscription.Dispose();
        }
    }

    [Test]
    public void ObservableListPublishesCollectionMutationsAndSkipsEqualReplacement()
    {
        ObservableList<string> items = new ObservableList<string>();
        List<CollectionChangedEvent<string>> changes = new List<CollectionChangedEvent<string>>();
        IDisposable subscription = EventBus.Subscribe<CollectionChangedEvent<string>>(changes.Add);
        try
        {
            items.Add("A");
            items[0] = new string(new[] { 'A' });
            items[0] = "B";
            items.RemoveAt(0);
            items.Add("C");
            items.Clear();

            Assert.AreEqual(5, changes.Count);
            for (int i = 0; i < changes.Count; i++) Assert.AreSame(items, changes[i].Source);
            CollectionAssert.AreEqual(new[]
            {
                CollectionChangeKind.Added,
                CollectionChangeKind.Replaced,
                CollectionChangeKind.Removed,
                CollectionChangeKind.Added,
                CollectionChangeKind.Reset
            }, changes.ConvertAll(change => change.Kind));
            Assert.AreEqual("A", changes[1].PreviousItem);
            Assert.AreEqual("B", changes[1].Item);
            Assert.AreEqual("B", changes[2].PreviousItem);
            Assert.AreEqual(-1, changes[4].Index);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void YoKaiCombatHpSetterPublishesOwnerAndClampedChangedValue()
    {
        Assembly gameplay = null;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
            if (assemblies[i].GetName().Name == "Assembly-CSharp") gameplay = assemblies[i];
        Assert.IsNotNull(gameplay);

        Type combatType = gameplay.GetType("YKW.Core.YoKai.Class.YoKaiCombat", true);
        object combat = Activator.CreateInstance(combatType);
        combatType.GetField("hp_max_me").SetValue(combat, 20);
        combatType.GetField("hpMe", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(combat, 4);
        PropertyInfo hp = combatType.GetProperty("hp_me");
        List<ValueChangedEvent<int>> changes = new List<ValueChangedEvent<int>>();
        IDisposable subscription = EventBus.Subscribe<ValueChangedEvent<int>>(changes.Add);
        try
        {
            hp.SetValue(combat, 10, null);
            hp.SetValue(combat, 10, null);
            hp.SetValue(combat, 30, null);

            Assert.AreEqual(2, changes.Count);
            Assert.AreSame(combat, changes[0].Source);
            Assert.AreEqual("hp_me", changes[0].MemberName);
            Assert.AreEqual(10, changes[1].OldValue);
            Assert.AreEqual(20, changes[1].NewValue);
        }
        finally
        {
            subscription.Dispose();
        }
    }

    [Test]
    public void CombatActionPointFormulaMatchesSpeedBreakpoints()
    {
        Assembly gameplay = null;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
            if (assemblies[i].GetName().Name == "Assembly-CSharp") gameplay = assemblies[i];
        Assert.IsNotNull(gameplay);

        Type turnControllerType = gameplay.GetType("CombatTurnController", true);
        MethodInfo calculateActionPoints = turnControllerType.GetMethod("CalculateBaseActionPoints");
        Assert.IsNotNull(calculateActionPoints);

        Assert.AreEqual(201, calculateActionPoints.Invoke(null, new object[] { 170f }));
        Assert.AreEqual(198, calculateActionPoints.Invoke(null, new object[] { 171f }));
        Assert.AreEqual(183, calculateActionPoints.Invoke(null, new object[] { 200f }));
        Assert.AreEqual(180, calculateActionPoints.Invoke(null, new object[] { 201f }));
        Assert.AreEqual(180, calculateActionPoints.Invoke(null, new object[] { 210f }));
        Assert.AreEqual(177, calculateActionPoints.Invoke(null, new object[] { 211f }));
        Assert.AreEqual(93, calculateActionPoints.Invoke(null, new object[] { 500f }));
        Assert.AreEqual(90, calculateActionPoints.Invoke(null, new object[] { 501f }));
    }

    [UnityTest]
    public IEnumerator EventSubscriberUnsubscribesOnDisableAndDestroy()
    {
        bool previousOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
        EnterPlayModeOptions previousOptions = EditorSettings.enterPlayModeOptions;
        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
        try
        {
            yield return new EnterPlayMode();

            int initialCount = CountHandlers<CombatMessageEvent>();
            Scene subscriberScene = SceneManager.CreateScene("EventSubscriberLifecycleProbe-" + Guid.NewGuid().ToString("N"));
            GameObject host = new GameObject("Event subscriber lifecycle probe");
            SceneManager.MoveGameObjectToScene(host, subscriberScene);
            Type componentType = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length && componentType == null; i++)
                componentType = assemblies[i].GetType("CombatMessageUI", false);
            Assert.IsNotNull(componentType);
            Behaviour subscriber = (Behaviour)host.AddComponent(componentType);
            yield return null;
            Assert.AreEqual(initialCount + 1, CountHandlers<CombatMessageEvent>());
            subscriber.enabled = false;
            Assert.AreEqual(initialCount, CountHandlers<CombatMessageEvent>());
            subscriber.enabled = true;
            Assert.AreEqual(initialCount + 1, CountHandlers<CombatMessageEvent>());

            // EnterPlayMode above uses Unity's actual no-domain-reload path. The
            // SubsystemRegistration callback must clear old channels and rebind
            // active lifecycle subscribers without a manual reset.
            Assert.AreEqual(initialCount + 1, CountHandlers<CombatMessageEvent>());

            AsyncOperation unload = SceneManager.UnloadSceneAsync(subscriberScene);
            Assert.IsNotNull(unload);
            yield return unload;
            yield return null;
            Assert.AreEqual(initialCount, CountHandlers<CombatMessageEvent>());
            yield return new ExitPlayMode();
        }
        finally
        {
            EditorSettings.enterPlayModeOptionsEnabled = previousOptionsEnabled;
            EditorSettings.enterPlayModeOptions = previousOptions;
        }
    }

    [UnityTest]
    public IEnumerator WorkerCanPublishThroughCapturedUnityContext()
    {
        yield return new EnterPlayMode();

        int unityThreadId = Thread.CurrentThread.ManagedThreadId;
        WorkerDispatchProbe probe = new WorkerDispatchProbe();
        activeWorkerProbe = probe;
        IDisposable subscription = EventBus.Subscribe<WorkerThreadEvent>(ObserveWorkerEvent);
        try
        {
            Thread worker = new Thread(PublishWorkerEvent);
            worker.Start(probe);
            float timeout = Time.realtimeSinceStartup + 2f;
            while (worker.IsAlive && Time.realtimeSinceStartup < timeout) yield return null;
            while (Volatile.Read(ref probe.ReceivedThreadId) == 0 && Time.realtimeSinceStartup < timeout)
                yield return null;

            worker.Join(1000);
            Assert.IsFalse(worker.IsAlive, "Worker thread did not complete.");
            Assert.IsNull(probe.Failure);
            Assert.AreEqual(unityThreadId, Volatile.Read(ref probe.ReceivedThreadId));
        }
        finally
        {
            subscription.Dispose();
            activeWorkerProbe = null;
        }
        yield return new ExitPlayMode();
    }

    private static void ObserveWorkerEvent(WorkerThreadEvent unused)
    {
        WorkerDispatchProbe probe = Volatile.Read(ref activeWorkerProbe);
        if (probe != null) Interlocked.Exchange(ref probe.ReceivedThreadId, Thread.CurrentThread.ManagedThreadId);
    }

    private static void ObserveMethodWorkerEvent(MethodAspectProbeEvent unused)
    {
        WorkerDispatchProbe probe = Volatile.Read(ref activeMethodWorkerProbe);
        if (probe != null) Interlocked.Exchange(ref probe.ReceivedThreadId, Thread.CurrentThread.ManagedThreadId);
    }

    private static void PublishMethodWorkerEvent(object unused)
    {
        WorkerDispatchProbe probe = Volatile.Read(ref activeMethodWorkerProbe);
        if (probe == null) return;
        try { new PublishedMethodProbe().FinishAction(); }
        catch (Exception exception) { probe.Failure = exception; }
    }

    private static void PublishWorkerEvent(object state)
    {
        WorkerDispatchProbe probe = (WorkerDispatchProbe)state;
        try { EventBus.PublishOnMainThread(new WorkerThreadEvent()); }
        catch (Exception exception) { probe.Failure = exception; }
    }

    [UnityTest]
    public IEnumerator CombatBagTransitionCanBeInterruptedAndClosed()
    {
        yield return new EnterPlayMode();

        GameObject parentObject = new GameObject("Bag transition parent", typeof(RectTransform));
        RectTransform parent = parentObject.GetComponent<RectTransform>();
        parent.sizeDelta = new Vector2(200f, 200f);
        GameObject panelObject = new GameObject("Bag transition panel", typeof(RectTransform));
        RectTransform panel = panelObject.GetComponent<RectTransform>();
        panel.SetParent(parent, false);
        panel.sizeDelta = new Vector2(100f, 80f);
        Type bagType = FindRuntimeType("CombatBagUI");
        Assert.IsNotNull(bagType);
        Behaviour bag = (Behaviour)panelObject.AddComponent(bagType);
        bagType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(bag, 0.5f);
        bagType.GetField("transitionEasing", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(bag, AnimationCurve.Linear(0f, 0f, 1f, 1f));
        bagType.GetMethod("Initialize").Invoke(bag, new object[] { panel });

        try
        {
            EventBus.Publish(new CombatBagVisibilityEvent(true));
            Assert.IsTrue(panel.gameObject.activeSelf);
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.Less(panel.anchoredPosition.y, -180f,
                "The configured linear Inspector curve should control the transition progress.");
            EventBus.Publish(new CombatBagVisibilityEvent(false));
            Vector2 positionWhileClosing = panel.anchoredPosition;
            EventBus.Publish(new CombatBagVisibilityEvent(true));
            Assert.Less(Vector2.Distance(panel.anchoredPosition, positionWhileClosing), 50f,
                "Reopening should reverse from the current position instead of jumping below the screen.");
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.IsTrue(panel.gameObject.activeSelf);
            Assert.Less(Vector2.Distance(panel.anchoredPosition, Vector2.zero), 0.1f);

            EventBus.Publish(new CombatBagVisibilityEvent(false));
            yield return new WaitForSecondsRealtime(0.6f);
            Assert.IsFalse(panel.gameObject.activeSelf);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(parentObject);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator CombatMessageQueueShowsTextThenClosesPanel()
    {
        yield return new EnterPlayMode();

        GameObject parentObject = new GameObject("Message parent", typeof(RectTransform));
        RectTransform parent = parentObject.GetComponent<RectTransform>();
        parent.sizeDelta = new Vector2(200f, 200f);
        GameObject panelObject = new GameObject("Message panel", typeof(RectTransform), typeof(CanvasRenderer));
        RectTransform panel = panelObject.GetComponent<RectTransform>();
        panel.SetParent(parent, false);
        panel.sizeDelta = new Vector2(100f, 40f);
        GameObject textObject = new GameObject("Message text", typeof(RectTransform), typeof(CanvasRenderer));
        textObject.transform.SetParent(panel, false);
        Type textType = FindRuntimeType("UnityEngine.UI.Text");
        Assert.IsNotNull(textType);
        Component text = textObject.AddComponent(textType);

        GameObject host = new GameObject("Message UI host");
        Type messageType = FindRuntimeType("CombatMessageUI");
        Assert.IsNotNull(messageType);
        Behaviour messageUi = (Behaviour)host.AddComponent(messageType);
        messageType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(messageUi, 0.5f);
        messageType.GetField("defaultHoldDuration", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(messageUi, 0.05f);
        messageType.GetMethod("Initialize").Invoke(messageUi, new object[] { panel });
        panelObject.SetActive(false);

        try
        {
            EventBus.Publish(new CombatMessageEvent("Message de test", 1f));
            Assert.IsTrue(panel.gameObject.activeSelf);
            Assert.AreEqual("Message de test", textType.GetProperty("text").GetValue(text, null));
            CanvasGroup textGroup = textObject.GetComponent<CanvasGroup>();
            Assert.IsNotNull(textGroup);
            Assert.Less(textGroup.alpha, 0.1f, "Text begins nearly transparent while the panel descends.");
            float startY = panel.anchoredPosition.y;
            Assert.Greater(startY, parent.rect.height, "The message panel begins above its viewport.");
            yield return new WaitForSecondsRealtime(0.04f);
            Assert.Greater(textGroup.alpha, 0f);
            Assert.Less(textGroup.alpha, 1f, "Text fades independently during panel motion.");
            Assert.Less(panel.anchoredPosition.y, startY);
            bool observedBounce = false;
            float animationElapsed = 0f;
            while (animationElapsed < 0.42f)
            {
                if (panel.anchoredPosition.y < -1f) observedBounce = true;
                animationElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.IsTrue(observedBounce, "The message panel should briefly pass its target before settling.");
            yield return new WaitForSecondsRealtime(0.5f);
            Assert.Less(Vector2.Distance(panel.anchoredPosition, Vector2.zero), 0.1f,
                "The message panel settles at its layout position.");
            messageType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(messageUi, 0.1f);

            host.SetActive(false);
            Assert.IsFalse(panel.gameObject.activeSelf);
            host.SetActive(true);
            parent.sizeDelta = new Vector2(200f, 400f);
            EventBus.Publish(new CombatMessageEvent("Message après réactivation", 0.05f));
            Assert.IsTrue(panel.gameObject.activeSelf);
            Assert.AreEqual("Message après réactivation", textType.GetProperty("text").GetValue(text, null));
            Assert.AreEqual(parent.rect.height + panel.rect.height, panel.anchoredPosition.y, 5f,
                "The offscreen start should follow the current viewport height.");

            yield return new WaitForSecondsRealtime(0.35f);
            Assert.IsFalse(panel.gameObject.activeSelf);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(parentObject);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator ActorBattleBubblesUpdateFromGlobalEvents()
    {
        yield return new EnterPlayMode();

        GameObject display = new GameObject("Actor display");
        GameObject bubble = new GameObject("bulle", typeof(RectTransform), typeof(CanvasRenderer));
        bubble.transform.SetParent(display.transform, false);
        GameObject textObject = new GameObject("t", typeof(RectTransform), typeof(CanvasRenderer));
        textObject.transform.SetParent(bubble.transform, false);
        Type tmpType = FindRuntimeType("TMPro.TextMeshProUGUI");
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        Assert.IsNotNull(tmpType);
        Assert.IsNotNull(imageType);
        Component text = textObject.AddComponent(tmpType);
        GameObject iconObject = new GameObject("Image", typeof(RectTransform), typeof(CanvasRenderer));
        iconObject.transform.SetParent(bubble.transform, false);
        Component iconImage = iconObject.AddComponent(imageType);

        GameObject combatObject = new GameObject("Combat actor-message source");
        Type combatType = FindRuntimeType("CombatManager");
        Assert.IsNotNull(combatType);
        Behaviour combat = (Behaviour)combatObject.AddComponent(combatType);
        combat.enabled = false;
        combatType.GetField("troisDisplateUIAmi").SetValue(combat, new List<GameObject> { display });
        combatType.GetField("troisDisplateUIEnnemie").SetValue(combat, new List<GameObject>());

        GameObject host = new GameObject("Actor message UI host");
        Type actorMessageType = FindRuntimeType("CombatActorMessageUI");
        Assert.IsNotNull(actorMessageType);
        Behaviour actorMessageUi = (Behaviour)host.AddComponent(actorMessageType);
        actorMessageType.GetMethod("Initialize").Invoke(actorMessageUi, new object[] { combat });

        Texture2D texture = new Texture2D(2, 2);
        Sprite icon = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
        try
        {
            EventBus.Publish(new CombatActorMessageEvent(true, 0, true, "Au zut !", icon));
            Assert.IsTrue(bubble.activeSelf);
            Assert.AreEqual("Au zut !", tmpType.GetProperty("text").GetValue(text, null));
            Assert.AreSame(icon, imageType.GetProperty("sprite").GetValue(iconImage, null));

            EventBus.Publish(new CombatActorMessageEvent(true, 0, false));
            Assert.IsFalse(bubble.activeSelf);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(combatObject);
            UnityEngine.Object.DestroyImmediate(display);
            UnityEngine.Object.DestroyImmediate(icon);
            UnityEngine.Object.DestroyImmediate(texture);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator ModularResultScreenDefersLegacyPartyPageUntilDismissal()
    {
        yield return new EnterPlayMode();

        GameObject legacyPanel = new GameObject("Legacy victory panel");
        legacyPanel.SetActive(false);
        Transform firstPage = EnsurePath(legacyPanel.transform, "partie-1");
        EnsurePath(firstPage, "win");
        EnsurePath(firstPage, "lose");
        Transform secondPage = EnsurePath(legacyPanel.transform, "partie-2");
        for (int i = 0; i < 6; i++) EnsurePath(secondPage, i.ToString());

        GameObject combatObject = new GameObject("Modular result lifecycle probe");
        Type combatType = FindRuntimeType("CombatManager");
        Assert.IsNotNull(combatType);
        Behaviour combat = (Behaviour)combatObject.AddComponent(combatType);
        combat.enabled = false;
        combatType.GetField("panelVictory").SetValue(combat, legacyPanel);
        combatType.GetField("combatEnCours", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(combat, false);
        combatType.GetField("panelVictoryIsClose", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(combat, true);
        combatType.GetField("stepPanelVictory").SetValue(combat, 0);

        try
        {
            combatType.GetMethod("UseModularResultScreen").Invoke(combat, null);
            combatType.GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(combat, null);
            Assert.IsFalse(legacyPanel.activeSelf,
                "The legacy win/loss page must stay hidden under the new result screen.");
            Assert.IsFalse(firstPage.gameObject.activeSelf);
            Assert.AreEqual(1, combatType.GetField("stepPanelVictory").GetValue(combat));

            combatType.GetMethod("ConfirmerAction").Invoke(combat, null);
            Assert.IsTrue(legacyPanel.activeSelf,
                "The legacy party progress page should appear after dismissing the new result screen.");
            Assert.IsFalse(firstPage.gameObject.activeSelf);
            Assert.IsTrue(secondPage.gameObject.activeSelf);
            Assert.AreEqual(2, combatType.GetField("stepPanelVictory").GetValue(combat));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(combatObject);
            UnityEngine.Object.DestroyImmediate(legacyPanel);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator SlotSwapMovesBothImagesAndCleansTemporaryOverlays()
    {
        yield return new EnterPlayMode();

        GameObject canvasObject = new GameObject("Slot animation canvas", typeof(RectTransform), typeof(Canvas));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(400f, 300f);
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        Assert.IsNotNull(imageType);
        Type imageListType = typeof(List<>).MakeGenericType(imageType);
        IList images = (IList)Activator.CreateInstance(imageListType);
        Texture2D texture = new Texture2D(2, 2);
        texture.SetPixels(new[] { Color.red, Color.red, Color.red, Color.red });
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
        GameObject firstSlot = CreateImageSlot("Slot 0", canvasRect, imageType, sprite, new Vector2(-50f, 0f));
        GameObject secondSlot = CreateImageSlot("Slot 1", canvasRect, imageType, sprite, new Vector2(50f, 0f));
        images.Add(firstSlot.GetComponent(imageType));
        images.Add(secondSlot.GetComponent(imageType));

        GameObject host = new GameObject("Slot animator host");
        Type animatorType = FindRuntimeType("CombatSlotAnimator");
        Assert.IsNotNull(animatorType);
        Behaviour animator = (Behaviour)host.AddComponent(animatorType);
        FieldInfo transitionDuration = animatorType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic);
        transitionDuration.SetValue(animator, 1.5f);
        animatorType.GetMethod("Initialize").Invoke(animator, new object[] { images, canvas });

        try
        {
            SlotChange[] swap = { new SlotChange(10, 0, 1), new SlotChange(20, 1, 0) };
            EventBus.Publish(new CombatSlotsChangedEvent(swap));
            Assert.AreEqual(2, CountOverlayChildren(canvasRect));
            RectTransform firstOverlay = FindOverlay(canvasRect, 0);
            Assert.IsNotNull(firstOverlay);
            Vector2 initialOverlayPosition = firstOverlay.anchoredPosition;
            yield return new WaitForSecondsRealtime(0.02f);
            Assert.Greater(Vector2.Distance(initialOverlayPosition, firstOverlay.anchoredPosition), 0.1f,
                "The medal overlay should move continuously between slots.");
            Assert.AreEqual(1f, ((Color)imageType.GetProperty("color").GetValue(firstOverlay.GetComponent(imageType), null)).a,
                "Slot movement should not fade the medal out.");
            RectTransform inFlightOverlay = firstOverlay;
            Vector2 positionBeforeInterruption = inFlightOverlay.anchoredPosition;
            transitionDuration.SetValue(animator, 0.08f);
            SlotChange[] reverseSwap = { new SlotChange(10, 1, 0), new SlotChange(20, 0, 1) };
            EventBus.Publish(new CombatSlotsChangedEvent(reverseSwap));
            firstOverlay = FindOverlay(canvasRect, 0);
            Assert.AreSame(inFlightOverlay, firstOverlay,
                "A new permutation should retarget the visible medal instead of replacing its visual object.");
            Assert.Less(Vector2.Distance(positionBeforeInterruption, firstOverlay.anchoredPosition), 0.1f,
                "Interrupting a slot animation must continue from its current position without a snap.");
            yield return null;
            Assert.AreEqual(2, CountOverlayChildren(canvasRect));
            yield return new WaitForSecondsRealtime(0.12f);
            yield return null;
            Assert.AreEqual(0, CountOverlayChildren(canvasRect));

            transitionDuration.SetValue(animator, 0.4f);
            EventBus.Publish(new CombatSlotsChangedEvent(swap));
            yield return new WaitForSecondsRealtime(0.03f);
            RectTransform movedDestination = secondSlot.GetComponent<RectTransform>();
            movedDestination.anchoredPosition = new Vector2(110f, 30f);
            yield return new WaitForSecondsRealtime(0.3f);
            firstOverlay = FindOverlay(canvasRect, 0);
            Assert.IsNotNull(firstOverlay);
            Assert.Less(Vector2.Distance(firstOverlay.anchoredPosition, movedDestination.anchoredPosition), 20f,
                "An in-flight slot animation must follow its destination when the layout moves.");
            yield return new WaitForSecondsRealtime(0.15f);
            yield return null;
            Assert.AreEqual(0, CountOverlayChildren(canvasRect));

            PropertyInfo colorProperty = imageType.GetProperty("color");
            Assert.AreEqual(1f, ((Color)colorProperty.GetValue(firstSlot.GetComponent(imageType), null)).a);
            Assert.AreEqual(1f, ((Color)colorProperty.GetValue(secondSlot.GetComponent(imageType), null)).a);

            EventBus.Publish(new CombatSlotsChangedEvent(swap));
            Assert.AreEqual(2, CountOverlayChildren(canvasRect));
            host.SetActive(false);
            Assert.AreEqual(1f, ((Color)colorProperty.GetValue(firstSlot.GetComponent(imageType), null)).a);
            Assert.AreEqual(1f, ((Color)colorProperty.GetValue(secondSlot.GetComponent(imageType), null)).a);
            yield return null;
            Assert.AreEqual(0, CountOverlayChildren(canvasRect));

            host.SetActive(true);
            transitionDuration.SetValue(animator, 0.08f);
            EventBus.Publish(new CombatSlotsChangedEvent(swap));
            Assert.AreEqual(2, CountOverlayChildren(canvasRect), "The subscriber should rebind after reactivation.");
            yield return new WaitForSecondsRealtime(0.12f);
            yield return null;
            Assert.AreEqual(0, CountOverlayChildren(canvasRect));
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(canvasObject);
            UnityEngine.Object.DestroyImmediate(sprite);
            UnityEngine.Object.DestroyImmediate(texture);
        }
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator ResultScreenRefreshesRewardsAndHandlesNextCombatWithoutLoot()
    {
        yield return new EnterPlayMode();

        GameObject rootObject = new GameObject("Résultat", typeof(RectTransform), typeof(Canvas));
        RectTransform root = rootObject.GetComponent<RectTransform>();
        root.sizeDelta = new Vector2(400f, 300f);
        Transform win = EnsurePath(root, "WinScreen");
        Transform lost = EnsurePath(root, "LostScreen");
        EnsurePath(lost, "Money");
        Transform lostItemContent = EnsurePath(lost, "Content/Loot/ItemContainer/Content");
        Transform experienceText = EnsurePath(win, "Content/Loot/Container1/Content/Bars/Text");
        Transform moneyText = EnsurePath(win, "Money/Text (TMP)");
        Transform itemContent = EnsurePath(win, "Content/Loot/ItemContainer/Content");
        ((RectTransform)itemContent).sizeDelta = new Vector2(150f, 60f);
        Type scrollType = FindRuntimeType("UnityEngine.UI.ScrollRect");
        Assert.IsNotNull(scrollType);
        Component scroll = itemContent.gameObject.AddComponent(scrollType);
        Component lostScroll = lostItemContent.gameObject.AddComponent(scrollType);
        Transform lostItemTemplate = EnsurePath(lostItemContent, "Icon");
        ((RectTransform)lostItemTemplate).sizeDelta = new Vector2(100f, 50f);
        Transform lostItemIcon = EnsurePath(lostItemTemplate, "ItemIcon");
        Transform itemTemplate = EnsurePath(itemContent, "Icon");
        ((RectTransform)itemTemplate).sizeDelta = new Vector2(100f, 50f);
        Transform itemIcon = EnsurePath(itemTemplate, "ItemIcon");
        Type imageType = FindRuntimeType("UnityEngine.UI.Image");
        Type tmpType = FindRuntimeType("TMPro.TextMeshProUGUI");
        Assert.IsNotNull(imageType);
        Assert.IsNotNull(tmpType);
        Component iconImage = itemIcon.gameObject.AddComponent(imageType);
        lostItemIcon.gameObject.AddComponent(imageType);
        Component experience = AddTmpText(experienceText.gameObject, tmpType);
        Component money = AddTmpText(moneyText.gameObject, tmpType);
        Component itemLabel = AddTmpText(EnsurePath(itemTemplate, "Bars/Text").gameObject, tmpType);
        Component lostItemLabel = AddTmpText(EnsurePath(lostItemTemplate, "Bars/Text").gameObject, tmpType);

        GameObject host = new GameObject("Combat result UI host");
        Type resultType = FindRuntimeType("CombatResultUI");
        Assert.IsNotNull(resultType);
        Behaviour resultUi = (Behaviour)host.AddComponent(resultType);
        resultType.GetField("transitionDuration", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(resultUi, 0.05f);
        resultType.GetMethod("Initialize").Invoke(resultUi, new object[] { root });
        Transform lootRows = itemContent.Find("Items");
        Transform lostLootRows = lostItemContent.Find("Items");
        Assert.IsNotNull(lootRows);
        Assert.IsNotNull(lostLootRows);
        Assert.AreSame(itemContent, scrollType.GetProperty("viewport").GetValue(scroll, null));
        Assert.AreSame(lootRows, scrollType.GetProperty("content").GetValue(scroll, null));
        Assert.AreSame(lostItemContent, scrollType.GetProperty("viewport").GetValue(lostScroll, null));
        Assert.AreSame(lostLootRows, scrollType.GetProperty("content").GetValue(lostScroll, null));
        Assert.IsNotNull(itemContent.GetComponent(FindRuntimeType("UnityEngine.UI.RectMask2D")));
        Assert.IsNotNull(lootRows.GetComponent(FindRuntimeType("UnityEngine.UI.VerticalLayoutGroup")));
        Assert.IsNotNull(lootRows.GetComponent(FindRuntimeType("UnityEngine.UI.ContentSizeFitter")));

        Texture2D texture = new Texture2D(2, 2);
        texture.SetPixels(new[] { Color.blue, Color.blue, Color.blue, Color.blue });
        texture.Apply();
        Sprite icon = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
        List<CombatLootItem> loot = new List<CombatLootItem>
        {
            new CombatLootItem("item-a", "Pomme", 2, icon),
            new CombatLootItem("item-b", "Lait", 1, icon)
        };

        try
        {
            EventBus.Publish(new CombatEndedEvent(true, 1234, 50, loot));
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.IsTrue(win.gameObject.activeSelf);
            Assert.IsFalse(lost.gameObject.activeSelf);
            Assert.AreEqual(1234.ToString("N0"), tmpType.GetProperty("text").GetValue(experience, null));
            Assert.AreEqual(50.ToString("N0"), tmpType.GetProperty("text").GetValue(money, null));
            Assert.AreEqual(2, lootRows.childCount);
            Assert.AreEqual("Pomme × 2", tmpType.GetProperty("text").GetValue(itemLabel, null));
            Assert.AreSame(icon, imageType.GetProperty("sprite").GetValue(iconImage, null));
            Canvas.ForceUpdateCanvases();
            Assert.Greater(lootRows.GetComponent<RectTransform>().rect.height,
                itemContent.GetComponent<RectTransform>().rect.height,
                "Loot rows must grow beyond the clipped viewport so long lists can scroll.");

            EventBus.Publish(new CombatResultDismissedEvent());
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.IsFalse(win.gameObject.activeSelf);

            EventBus.Publish(new CombatEndedEvent(true, 0, null, null));
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.IsTrue(win.gameObject.activeSelf);
            Assert.AreEqual(Vector2.zero, ((RectTransform)win).anchoredPosition);
            Assert.AreEqual("0", tmpType.GetProperty("text").GetValue(experience, null));
            Assert.IsFalse(win.Find("Money").gameObject.activeSelf);
            Assert.IsFalse(win.Find("Content/Loot/ItemContainer").gameObject.activeSelf);
            Assert.AreEqual(2, lootRows.childCount, "Loot rows should be retained for reuse.");

            List<CombatLootItem> singleItem = new List<CombatLootItem>
            {
                new CombatLootItem("item-c", "Poire", 3, icon)
            };
            EventBus.Publish(new CombatEndedEvent(true, 12, null, singleItem));
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.AreEqual(12.ToString("N0"), tmpType.GetProperty("text").GetValue(experience, null));
            Assert.AreEqual("Poire × 3", tmpType.GetProperty("text").GetValue(itemLabel, null));
            Assert.IsFalse(lootRows.GetChild(1).gameObject.activeSelf);
            Assert.AreEqual(2, lootRows.childCount, "Existing rows should be reused when loot shrinks.");

            EventBus.Publish(new CombatResultDismissedEvent());
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.IsFalse(win.gameObject.activeSelf);

            EventBus.Publish(new CombatEndedEvent(false, 0, null, null));
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.IsTrue(lost.gameObject.activeSelf);
            Assert.IsFalse(win.gameObject.activeSelf);
            Assert.IsFalse(root.Find("LostScreen/Money").gameObject.activeSelf);
            Assert.IsFalse(root.Find("LostScreen/Content/Loot/ItemContainer").gameObject.activeSelf);

            EventBus.Publish(new CombatEndedEvent(false, 4, null,
                new List<CombatLootItem> { new CombatLootItem("item-loss", "Soda", 1, icon) }));
            yield return new WaitForSecondsRealtime(0.08f);
            Assert.AreEqual("Soda × 1", tmpType.GetProperty("text").GetValue(lostItemLabel, null));
            Assert.AreEqual(1, lostLootRows.childCount);
            Assert.IsTrue(lostItemContent.GetComponent(FindRuntimeType("UnityEngine.UI.RectMask2D")) != null);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(host);
            UnityEngine.Object.DestroyImmediate(rootObject);
            UnityEngine.Object.DestroyImmediate(icon);
            UnityEngine.Object.DestroyImmediate(texture);
        }
        yield return new ExitPlayMode();
    }

    private static Transform EnsurePath(Transform root, string path)
    {
        string[] segments = path.Split('/');
        Transform current = root;
        for (int i = 0; i < segments.Length; i++)
        {
            Transform child = current.Find(segments[i]);
            if (child == null)
            {
                GameObject childObject = new GameObject(segments[i], typeof(RectTransform));
                child = childObject.transform;
                child.SetParent(current, false);
            }
            current = child;
        }
        return current;
    }

    private static Component AddTmpText(GameObject target, Type tmpType)
    {
        if (target.GetComponent<CanvasRenderer>() == null) target.AddComponent<CanvasRenderer>();
        return target.AddComponent(tmpType);
    }

    private static GameObject CreateImageSlot(string name, RectTransform parent, Type imageType,
        Sprite sprite, Vector2 position)
    {
        GameObject slot = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        RectTransform rect = slot.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = new Vector2(32f, 32f);
        rect.anchoredPosition = position;
        Component image = slot.AddComponent(imageType);
        imageType.GetProperty("sprite").SetValue(image, sprite, null);
        return slot;
    }

    private static int CountOverlayChildren(RectTransform parent)
    {
        int count = 0;
        for (int i = 0; i < parent.childCount; i++)
            if (parent.GetChild(i).name == "Combat medal transition") count++;
        return count;
    }

    private static RectTransform FindOverlay(RectTransform parent, int index)
    {
        int found = 0;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name != "Combat medal transition") continue;
            if (found++ == index) return child as RectTransform;
        }
        return null;
    }

    private static Type FindRuntimeType(string fullName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type type = assemblies[i].GetType(fullName, false);
            if (type != null) return type;
        }
        return null;
    }

    private static string FindActiveGameObjectId(string sceneYaml, string serializedName, bool requireRoot = false)
    {
        MatchCollection documents = Regex.Matches(sceneYaml,
            @"(?ms)^--- !u!1 &(?<id>-?\d+)\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        for (int i = 0; i < documents.Count; i++)
        {
            string body = documents[i].Groups["body"].Value;
            if (!body.Contains("m_Name: " + serializedName + "\n") || !body.Contains("m_IsActive: 1"))
                continue;
            string objectId = documents[i].Groups["id"].Value;
            if (!requireRoot) return objectId;
            string transformId = FindTransformId(sceneYaml, objectId);
            if (transformId != null && FindTransformParentId(sceneYaml, transformId) == "0") return objectId;
        }
        return null;
    }

    private static string FindChildId(string sceneYaml, string parentTransformId, string serializedName)
    {
        MatchCollection documents = Regex.Matches(sceneYaml,
            @"(?ms)^--- !u!1 &(?<id>-?\d+)\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        for (int i = 0; i < documents.Count; i++)
        {
            string body = documents[i].Groups["body"].Value;
            if (!body.Contains("m_Name: " + serializedName + "\n")) continue;
            string objectId = documents[i].Groups["id"].Value;
            string transformId = FindTransformId(sceneYaml, objectId);
            if (transformId != null && FindTransformParentId(sceneYaml, transformId) == parentTransformId)
                return objectId;
        }
        return null;
    }

    private static string FindActiveChildId(string sceneYaml, string parentTransformId, string serializedName)
    {
        string childId = FindChildId(sceneYaml, parentTransformId, serializedName);
        if (childId == null) return null;
        Match document = Regex.Match(sceneYaml,
            @"(?ms)^--- !u!1 &" + Regex.Escape(childId) + @"\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        return document.Success && document.Groups["body"].Value.Contains("m_IsActive: 1") ? childId : null;
    }

    private static string FindTransformId(string sceneYaml, string gameObjectId)
    {
        MatchCollection documents = Regex.Matches(sceneYaml,
            @"(?ms)^--- !u!(?:4|224) &(?<id>-?\d+)\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        for (int i = 0; i < documents.Count; i++)
            if (documents[i].Groups["body"].Value.Contains("m_GameObject: {fileID: " + gameObjectId + "}"))
                return documents[i].Groups["id"].Value;
        return null;
    }

    private static string FindTransformParentId(string sceneYaml, string transformId)
    {
        Match document = Regex.Match(sceneYaml,
            @"(?ms)^--- !u!(?:4|224) &" + Regex.Escape(transformId) + @"\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        Match parent = Regex.Match(document.Groups["body"].Value, @"m_Father: \{fileID: (?<id>-?\d+)\}");
        return parent.Success ? parent.Groups["id"].Value : null;
    }

    private static string FindComponentByScript(string sceneYaml, string gameObjectId, string scriptGuid)
    {
        MatchCollection documents = Regex.Matches(sceneYaml,
            @"(?ms)^--- !u!114 &(?<id>-?\d+)\r?\n(?<body>.*?)(?=^--- !u!|\z)");
        for (int i = 0; i < documents.Count; i++)
        {
            string body = documents[i].Groups["body"].Value;
            if (body.Contains("m_GameObject: {fileID: " + gameObjectId + "}") &&
                body.Contains("guid: " + scriptGuid + ",")) return body;
        }
        return null;
    }

    private static int CountHandlers<TEvent>()
    {
        FieldInfo channelsField = typeof(EventBus).GetField("channels", BindingFlags.Static | BindingFlags.NonPublic);
        IDictionary channels = (IDictionary)channelsField.GetValue(null);
        object channel;
        if (!channels.Contains(typeof(TEvent))) return 0;
        channel = channels[typeof(TEvent)];
        FieldInfo handlersField = channel.GetType().GetField("handlers", BindingFlags.Instance | BindingFlags.NonPublic);
        return ((ICollection)handlersField.GetValue(channel)).Count;
    }
}
