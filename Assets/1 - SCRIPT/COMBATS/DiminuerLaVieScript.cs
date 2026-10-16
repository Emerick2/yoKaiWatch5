using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YKW.Core.YoKai.Enums;
using YKW.Core.YoKai.Class;
using YokaiSpecies = YKW.Core.YoKai.Class.YoKai;

/// <summary>Damage rules for the currently implemented basic attack.</summary>
public class DiminuerLaVieScript : MonoBehaviour
{
    public enum AttackType { Physical, Technique }

    private static List<YoKaiCombat> GetActiveAlliesOnWheel(
        bool isFriendly, IEnumerable<YoKaiCombat> turnOrder)
    {
        if (turnOrder == null) return new List<YoKaiCombat>();
        return turnOrder
            .Where(yokai => yokai != null && yokai.hp_me > 0 && yokai.estDevant && yokai.estUnAmi == isFriendly)
            .ToList();
    }

    /// <summary>
    /// A unity is formed by living allies of the same tribe in neighboring
    /// wheel slots. Each neighbor contributes 10%, up to the two neighbors
    /// that can surround a slot on the six-position wheel.
    /// </summary>
    private static float CalculateUnityBonus(YoKaiCombat attacker, List<YoKaiCombat> allies)
    {
        if (attacker == null || allies == null) return 1f;

        int slot = ((attacker.positionRoue % 6) + 6) % 6;
        int sameTribeNeighbors = allies.Count(ally =>
        {
            if (ally == null || ally == attacker || ally.tribue != attacker.tribue) return false;
            int allySlot = ((ally.positionRoue % 6) + 6) % 6;
            int distance = Mathf.Abs(slot - allySlot);
            return distance == 1 || distance == 5;
        });

        return 1f + Mathf.Min(sameTribeNeighbors, 2) * 0.10f;
    }

    private static float GetAttackPower(YoKaiCombat attacker, AttackType attackType)
    {
        if (attacker == null) return 0f;
        return Mathf.Max(0f, attackType == AttackType.Physical
            ? attacker.Stats.Force
            : attacker.Stats.Esprit);
    }

    /// <summary>
    /// Basic attacks are neutral. The elemental multiplier is used by future
    /// elemental techniques and Soultimates, not by the physical attack action.
    /// </summary>
    public float GetElementalDamageMultiplier(
        Technique usedTechnique, bool isPhysicalAttack, YokaiSpecies defenderData)
    {
        if (isPhysicalAttack || defenderData == null || medaliumDictionnaire._instance == null)
            return 1f;

        Element attackElement = medaliumDictionnaire._instance.ObtenirElementDeLaTechnique(usedTechnique);
        if (attackElement == Element.Neutre || attackElement == Element.Soin)
            return 1f;
        if (attackElement == defenderData.elementFaiblesse)
            return 1.5f;
        if (attackElement == defenderData.elementResistance)
            return 0.5f;
        return 1f;
    }

    public int ApplyDamage(YoKaiCombat attacker, YoKaiCombat defender, List<YoKaiCombat> turnOrder)
    {
        if (attacker == null || defender == null || defender.hp_me <= 0) return 0;

        // This action is currently the basic physical attack. Keeping the
        // attack type explicit makes its stat and elemental rules unambiguous.
        const AttackType attackType = AttackType.Physical;
        float attackPower = GetAttackPower(attacker, attackType);
        float unityBonus = CalculateUnityBonus(
            attacker, GetActiveAlliesOnWheel(attacker.estUnAmi, turnOrder));

        // Strength drives physical damage; defense reduces every damage source.
        // The coefficients preserve the existing damage scale while removing
        // the unrelated, Pokémon-style second stat formula from this calculation.
        float baseDamage = attackPower * 0.5f * unityBonus - Mathf.Max(0f, defender.Stats.Défence) * 0.25f;
        int rollStep = Random.Range(0, 21);
        float variance = 0.90f + rollStep / 100f;

        YokaiSpecies defenderData = medaliumDictionnaire._instance != null
            ? medaliumDictionnaire._instance.ServantYoKai(defender.ID)
            : null;
        float elementalMultiplier = defenderData == null
            ? 1f
            : GetElementalDamageMultiplier(
                attacker.technique, attackType == AttackType.Physical, defenderData);

        float guardMultiplier = Mathf.Clamp(defender.multiplicateurGarde, 0f, 1f);
        float finalDamage = baseDamage * variance * elementalMultiplier * guardMultiplier;
        int damage = Mathf.Max(1, Mathf.FloorToInt(finalDamage));
        defender.ModifierPV(-damage);
        return damage;
    }
}
