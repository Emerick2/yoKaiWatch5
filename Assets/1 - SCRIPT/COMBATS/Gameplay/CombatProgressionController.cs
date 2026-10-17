using System.Collections.Generic;
using UnityEngine;
using CombatYokai = YKW.Core.YoKai.Class.YoKaiCombat;

/// <summary>Calculates and persists combat experience independently of the turn and scene flows.</summary>
public sealed class CombatProgressionController : MonoBehaviour
{
    private CombatManager manager;

    public void Initialize(CombatManager owner)
    {
        manager = owner;
    }

    public int CalculateTotalExperience(List<CombatYokai> defeatedEnemies) {
        float totalExperience = 0f;
        if (defeatedEnemies == null) return 0;

        foreach (var enemy in defeatedEnemies)
        {
            if (enemy == null) continue;
            int rank = (int)enemy.rang;
            float baseExperience = enemy.niveau * 5f;
            float rankMultiplier = 1f + (rank / 4f);

            totalExperience += baseExperience * rankMultiplier;
        }
        return Mathf.RoundToInt(totalExperience);
    }
    public float CalculateExperienceMultiplier(CombatYokai yokaiState) {
        if (yokaiState == null) return 1f;
        float bonusMultiplier = 1.0f;

        if (yokaiState.aAttaque) {
            bonusMultiplier += 0.15f;
        }

        if (yokaiState.aEtePurifie) {
            bonusMultiplier += 0.20f;
        }

        return bonusMultiplier;
    }

    public void GrantExperience(List<CombatYokai> defeatedEnemies) {
        if (manager == null || manager.listeYoKaiAmiSurLeTerrain == null) return;
        int totalExperience = CalculateTotalExperience(defeatedEnemies);

        int allyIndex = 0;
        foreach (CombatYokai ally in manager.listeYoKaiAmiSurLeTerrain) {
            if (ally == null) { allyIndex++; continue; }
            ally.GagneXP(totalExperience);

            float participationMultiplier = CalculateExperienceMultiplier(ally);
            if (participationMultiplier != 0) {
                int bonusExperience = Mathf.RoundToInt(totalExperience * participationMultiplier) - totalExperience;
                ally.GagneXP(bonusExperience);
            }

            if (MesYoKai.Instance != null && NewDataCenter.Instance != null &&
                MesYoKai.Instance.maListe != null && ally.IDListeYoKai >= 0 &&
                ally.IDListeYoKai < MesYoKai.Instance.maListe.Count) {
                MesYoKai.Instance.maListe[ally.IDListeYoKai].xp = ally.xp;
                MesYoKai.Instance.maListe[ally.IDListeYoKai].niveau = ally.niveau;

                string stableId = ally.IDListeYoKai.ToString();
                NewDataCenter.Instance.SauvegarderSTR(stableId + "_xp", ally.xp.ToString());
                NewDataCenter.Instance.SauvegarderSTR(stableId + "_niveau", ally.niveau.ToString());
            }

            if (manager.listeImage != null && manager.listeImage.Count > allyIndex && manager.listeImage[allyIndex] != null) {
                float progress = (float)ally.xp / 100;
                float fillPercent = progress * ally.NombreDePointXPAGagner(ally.niveau + 1);
                manager.listeImage[allyIndex].fillAmount = fillPercent / 100;
            }
            allyIndex++;
        }
    }
}
