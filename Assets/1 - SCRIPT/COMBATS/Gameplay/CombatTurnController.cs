using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YKW.Core.YoKai.Enums;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;
using CombatYokai = YKW.Core.YoKai.Class.YoKaiCombat;

/// <summary>Runs combat turns and actor behavior, leaving scene setup and UI wiring to CombatManager.</summary>
public sealed class CombatTurnController : MonoBehaviour
{
    [SerializeField, Min(1f)] private float actionPointsPerSecond = 60f;
    [SerializeField, Min(0f)] private float actionRecoverySeconds = 0.75f;
    private CombatManager manager;
    private readonly Dictionary<CombatYokai, float> remainingActionPoints =
        new Dictionary<CombatYokai, float>();
    private bool actionPointQueueInitialized;

    public void Initialize(CombatManager owner)
    {
        manager = owner;
    }

    /// <summary>
    /// Converts speed into AP using the Yo-kai Watch 2 breakpoints:
    /// SPD 0-170, 171-200, 201-500, then the fixed 90 AP cost above 500.
    /// </summary>
    public static int CalculateBaseActionPoints(float speed)
    {
        speed = Mathf.Max(0f, speed);
        if (speed <= 170f)
            return 369 - Mathf.FloorToInt(speed / 3f) * 3;
        if (speed <= 200f)
            return 198 - Mathf.FloorToInt((speed - 171f) / 5f) * 3;
        if (speed <= 500f)
            return 180 - Mathf.FloorToInt((speed - 201f) / 10f) * 3;
        return 90;
    }

    private List<CombatYokai> GetLivingActors()
    {
        if (manager == null || manager.listeYoKaiAmiSurLeTerrain == null ||
            manager.listeYoKaiEnnemieSurLeTerrain == null)
            return new List<CombatYokai>();

        return manager.listeYoKaiAmiSurLeTerrain
            .Concat(manager.listeYoKaiEnnemieSurLeTerrain)
            .Where(yokai => yokai != null && yokai.hp_me > 0 && yokai.estDevant)
            .Distinct()
            .ToList();
    }

    private void SynchronizeActionPointQueue(List<CombatYokai> livingActors)
    {
        HashSet<CombatYokai> livingSet = new HashSet<CombatYokai>(livingActors);
        foreach (CombatYokai queuedActor in remainingActionPoints.Keys.ToList())
        {
            if (!livingSet.Contains(queuedActor))
                remainingActionPoints.Remove(queuedActor);
        }

        foreach (CombatYokai actor in livingActors)
        {
            if (remainingActionPoints.ContainsKey(actor)) continue;

            float baseActionPoints = CalculateBaseActionPoints(actor.Stats.Vitesse);
            // Only the combat-opening queue uses the random 0.5-0.6 multiplier.
            float initialTurnMultiplier = actionPointQueueInitialized
                ? 1f
                : Random.Range(0.5f, 0.6f);
            remainingActionPoints.Add(actor, baseActionPoints * initialTurnMultiplier);
        }

        actionPointQueueInitialized = true;
    }

    private CombatYokai GetNextActor(List<CombatYokai> livingActors)
    {
        if (livingActors == null || livingActors.Count == 0) return null;

        float lowestActionPoints = livingActors.Min(actor => remainingActionPoints[actor]);
        // Select uniformly at random among actors tied at the front of the queue.
        List<CombatYokai> tiedActors = livingActors
            .Where(actor => Mathf.Approximately(remainingActionPoints[actor], lowestActionPoints))
            .ToList();
        return tiedActors[Random.Range(0, tiedActors.Count)];
    }

    private void ResetActorActionPoints(CombatYokai actor)
    {
        // Speed changes affect the next recharge, not AP already spent waiting.
        remainingActionPoints[actor] = CalculateBaseActionPoints(actor.Stats.Vitesse);
    }

    CombatYokai GetRandomTarget(List<CombatYokai> actors){
        if (actors == null) return null;
        List<CombatYokai> livingTargets = actors.Where(actor => actor != null && actor.hp_me > 0).ToList();
        List<CombatYokai> candidates = livingTargets.Where(actor => actor.estDevant).ToList();
        if (candidates.Count == 0) candidates = livingTargets;
        if (candidates.Count > 0) return candidates[Random.Range(0,candidates.Count)];
        else return null;
    }
    void PlayAnimation(CombatYokai yokai, string animationName){
        if (yokai != null && yokai.gameObjectSurLaScène != null){
            Animator animator = yokai.gameObjectSurLaScène.GetComponent<Animator>();
            if (HasAnimatorParameter(animator, animationName, AnimatorControllerParameterType.Trigger))
                animator.SetTrigger(animationName);
        }
    }
    void SetAnimationBool(CombatYokai yokai, string animationName, bool enabled){
        if (yokai != null && yokai.gameObjectSurLaScène != null){
            Animator animator = yokai.gameObjectSurLaScène.GetComponent<Animator>();
            if (HasAnimatorParameter(animator, animationName, AnimatorControllerParameterType.Bool))
                animator.SetBool(animationName, enabled);
        }
    }

    private static bool HasAnimatorParameter(Animator animator, string parameterName,
        AnimatorControllerParameterType parameterType)
    {
        if (animator == null || !animator.isActiveAndEnabled || !animator.isInitialized || animator.runtimeAnimatorController == null ||
            string.IsNullOrEmpty(parameterName)) return false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.name == parameterName && parameter.type == parameterType) return true;
        return false;
    }
    IEnumerator PerformBasicAttack(CombatYokai actor){
        CombatYokai target = null;
        if (actor.estUnAmi) target = GetRandomTarget(manager.listeYoKaiEnnemieSurLeTerrain);
        else target = GetRandomTarget(manager.listeYoKaiAmiSurLeTerrain);
        if (target == null)
        {
            Debug.LogWarning($"{actor.nom} skipped an attack because the opposing team has no living target.");
            yield break;
        }

        actor.aAttaque = true;
        bool missed = Random.Range(0,100)<Mathf.Clamp(manager.tauxPourRatéUneAttaque, 0, 100);
        if (missed){
            ShowMissMessage(target);
            yield break;
        }
        if (target != null){
            PlayAnimation(actor, "attaque");
            yield return new WaitForSeconds(0.2f);
            if (manager.diminuerLaVieScript == null)
            {
                Debug.LogError("CombatTurnController: DiminuerLaVieScript is not assigned.");
                yield break;
            }
            int damage = manager.diminuerLaVieScript.ApplyDamage(actor, target, manager.ordreDePassage);
            if (target.hp_me>0){
                PlayAnimation(target, "coup");
            } else {
                PlayAnimation(target, "mort");
            }
            GameObject display = null;
            if(target.positionRoue<3) {
                if (target.estUnAmi && target.positionRoue < manager.troisDisplateUIAmi.Count)
                    display = manager.troisDisplateUIAmi[target.positionRoue];
                else if (!target.estUnAmi && target.positionRoue < manager.troisDisplateUIEnnemie.Count)
                    display = manager.troisDisplateUIEnnemie[target.positionRoue];
            }
            if(display != null) {
                Transform number = display.transform.Find("nombre");
                EcrireUneQuantitéAvecDesImages writer = number != null ? number.GetComponent<EcrireUneQuantitéAvecDesImages>() : null;
                if (writer != null) writer.EcrireLeNombre(damage);
                Animator animator = number != null ? number.GetComponent<Animator>() : null;
                if (HasAnimatorParameter(animator, "ouvert", AnimatorControllerParameterType.Trigger)) animator.SetTrigger("ouvert");
            }
        }
    }
    bool ShouldLoaf(Personalité personality){
        float chancePercent = 0;
        switch (personality){
            case Personalité.Sérieux:
            chancePercent = 0f;
            break;
            case Personalité.Ferme:
            chancePercent = 1.75f;
            break;
            case Personalité.Détendu:
            chancePercent = 3.5f;
            break;
            case Personalité.Frivole:
            chancePercent = 5f;
            break;
            case Personalité.Distrait:
            chancePercent = 9f;
            break;
            case Personalité.Maladroit:
            chancePercent = 12f;
            break;
            default:
            Debug.LogWarning("Unknown personality; using the default loaf chance.");
            chancePercent = 5f;
            break;
        }
        return Random.Range(0,100)<chancePercent;
    }
    bool ShouldGuard(Personalité personality){
        return Random.Range(0,100)<30;
    }
    void PlayLoafAnimation(CombatYokai actor, bool isActive){
        if (actor.gameObjectSurLaScène != null){
            SetAnimationBool(actor, "fatigue", isActive);
            if (actor.estUnAmi){
                float currentX = actor.gameObjectSurLaScène.transform.eulerAngles.x;
                float currentZ = actor.gameObjectSurLaScène.transform.eulerAngles.z;
                if (isActive){
                    actor.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 180f, currentZ);
                } else {
                    actor.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 0f, currentZ);
                }
            }
            EventBus.Publish(new CombatActorMessageEvent(actor.estUnAmi, actor.positionRoue,
                isActive, isActive ? "Pfiou..." : null, isActive ? manager.spriteFlâne : null));
        }
    }
    void ShowMissMessage(CombatYokai actor){
        if (actor == null) return;
        EventBus.Publish(new CombatActorMessageEvent(actor.estUnAmi, actor.positionRoue,
            true, "Au zut !", manager.spriteRaté));
    }
    void PlayGuardAnimation(CombatYokai actor, bool isActive){
        if (actor.gameObjectSurLaScène != null){
            SetAnimationBool(actor, "protect", isActive);
            if (actor.estUnAmi){
                float currentX = actor.gameObjectSurLaScène.transform.eulerAngles.x;
                float currentZ = actor.gameObjectSurLaScène.transform.eulerAngles.z;
                if (isActive){
                    actor.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 180f, currentZ);
                } else {
                    actor.gameObjectSurLaScène.transform.rotation = Quaternion.Euler(currentX, 0f, currentZ);
                }
            }
            EventBus.Publish(new CombatActorMessageEvent(actor.estUnAmi, actor.positionRoue,
                isActive, isActive ? "En garde !" : null, isActive ? manager.spriteDéfance : null));
        }
    }
    private static int GetAutomaticWheelRotation(List<CombatYokai> allies)
    {
        if (allies == null) return 0;
        bool livingFront = allies.Any(ally => ally != null && ally.hp_me > 0 &&
            ally.positionRoue >= 0 && ally.positionRoue < 3);
        bool livingReserve = allies.Any(ally => ally != null && ally.hp_me > 0 &&
            ally.positionRoue >= 3 && ally.positionRoue < 6);
        return !livingFront && livingReserve ? 3 : 0;
    }

    public IEnumerator RunTurns(){
        manager.RefreshCombatUI();
        for (int i = 0; i < manager.troisDisplateUIAmi.Count; i++)
            EventBus.Publish(new CombatActorMessageEvent(true, i, false));
        for (int i = 0; i < manager.troisDisplateUIEnnemie.Count; i++)
            EventBus.Publish(new CombatActorMessageEvent(false, i, false));
        yield return new WaitForSeconds(0.5f); // Let scene animators finish loading before the first turn.
        SynchronizeActionPointQueue(GetLivingActors());
        while (manager.CombatEnCours){
            if (manager.GameInStop || manager.SlotTransitionInProgress)
            {
                yield return null;
                continue;
            }
            int automaticRotation = GetAutomaticWheelRotation(manager.listeYoKaiAmiSurLeTerrain);
            if (automaticRotation != 0)
            {
                manager.ApplyWheelRotation(automaticRotation);
                yield return null;
                continue;
            }
            int requestedRotation = manager.TakeRotationRequest();
            if (requestedRotation != 0)
            {
                manager.ApplyWheelRotation(requestedRotation);
                // A rotation changes the active roster. Re-select after its animation.
                yield return null;
                continue;
            }

            List<CombatYokai> livingActors = GetLivingActors();
            if (livingActors.Count == 0)
            {
                yield return null;
                continue;
            }
            SynchronizeActionPointQueue(livingActors);
            manager.ordreDePassage = livingActors
                .OrderBy(actor => remainingActionPoints[actor])
                .ToList();

            CombatYokai actor = GetNextActor(livingActors);
            if (actor == null) { yield return null; continue; }
            float remaining = remainingActionPoints[actor];
            if (remaining > 0f)
            {
                // Tick the active queue without blocking input for an entire AP recharge.
                float spent = Mathf.Min(remaining, Mathf.Max(1f, actionPointsPerSecond) * Time.deltaTime);
                foreach (CombatYokai waiting in livingActors)
                    remainingActionPoints[waiting] = Mathf.Max(0f, remainingActionPoints[waiting] - spent);
                yield return null;
                continue;
            }

            bool shouldAttack = true; // Other action types will be selected here when implemented.
            bool shouldLoaf = ShouldLoaf(actor.personnalité);
            bool shouldGuard = ShouldGuard(actor.personnalité);
            if (!shouldLoaf && actor.estEnTrainDeFlâner){
                actor.estEnTrainDeFlâner = false;
                PlayLoafAnimation(actor, false);
            }
            if (!shouldGuard && actor.estEnTrainDeFaireUneGarde){
                actor.estEnTrainDeFaireUneGarde = false;
                actor.multiplicateurGarde = 1;
                PlayGuardAnimation(actor, false);
            }
            if (shouldLoaf){
                actor.estEnTrainDeFlâner = true;
                PlayLoafAnimation(actor, true);
            } else if (shouldGuard){
                actor.estEnTrainDeFaireUneGarde = true;
                actor.multiplicateurGarde = 0.5f;
                PlayGuardAnimation(actor, true);
            }else if (shouldAttack){
                yield return StartCoroutine(PerformBasicAttack(actor));
            }
            actor.amultime = Mathf.Clamp(actor.amultime + 10, 0, 100);
            manager.RefreshCombatUI();
            manager.RefreshCombatState();
            if (!manager.CombatEnCours)
                yield break;

            ResetActorActionPoints(actor);
            yield return new WaitForSeconds(actionRecoverySeconds);
        }
    }
}
