using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using YKW.Core.YoKai.Class;
using YKW.Core.YoKai.Struct;
using MedaliumNonPersonel = YKW.Core.YoKai.Class.MedaliumYoKai;

public class DiminuerLaVieScript : MonoBehaviour{
    public enum TypeAttaque { Physique, Technique }

    List<YoKaiCombat> TousLesYoKaiSurRoue(bool ami, List<YoKaiCombat> ordreDePassage){
        List<YoKaiCombat> tousLesYoKaiSurRoue = new List<YoKaiCombat>();
        tousLesYoKaiSurRoue = ordreDePassage.Where(e => e.hp_me > 0 && e.estDevant && e.estUnAmi == ami).ToList();
        return tousLesYoKaiSurRoue;
    }

    float CalculerBonusTribu(YoKaiCombat yoKai, List<YoKaiCombat> tousLesYoKaiSurRoue){
        float valeur = 1;
        foreach (YoKaiCombat item in tousLesYoKaiSurRoue){
            if(item != yoKai && item.tribue == yoKai.tribue){
                valeur+=0.1f;
            }
        }
        // Dans Yo-kai Watch, l'unité augmente souvent la stat de 10% à 20%
        // Imaginons ici que bonusTribu renvoie un multiplicateur (ex: 1.20 pour +20%)
        return valeur;
    }

    float ObtenirMultiplicateurEnvoutement(YoKaiCombat yoKai, TypeAttaque typeAttaque){
        return 1f;//je ne prend pas encore en compte les envoutement. Je reviendrais ici plus tard
        //attention, on vas multiplier cette valeur ça doit donc être un pourcentage au format 1.2 par exemple.
    }

    float CalculerCorrection(YoKaiCombat yoKai, TypeAttaque typeAttaque, List<YoKaiCombat> tousLesYoKaiSurRoue) {
        float valeurBase = 0;

        if (typeAttaque == TypeAttaque.Physique) {
            valeurBase = yoKai.Stats.Force;
        } else {
            valeurBase = yoKai.Stats.Esprit;
        }

        // StatFinale = ((2*base+iv+(ev/2)*niveau)/100)+5
        float iv = 0;
        float ev = 1;

        float statFinale = ((2*valeurBase+iv+(ev/2)*yoKai.niveau)/100)+5;

        float bonusTribu = CalculerBonusTribu(yoKai, tousLesYoKaiSurRoue);
        if (bonusTribu > 1) {
            statFinale *= bonusTribu;
        }

        float multiplicateurEnvoutement = ObtenirMultiplicateurEnvoutement(yoKai, typeAttaque);
        statFinale *= multiplicateurEnvoutement;

        return (int)statFinale;
    }

    public float CalculerMultiplicateurElementaire(Technique techniqueUtilisee, bool estAttaquePhysique, MedaliumYoKai dataDefenseur) {
        if (estAttaquePhysique){
            return 1f; 
        }

        Element elementAttaque = medaliumDictionnaire._instance.ObtenirElementDeLaTechnique(techniqueUtilisee);

        if (elementAttaque == dataDefenseur.elementFaiblesse){
            return 1.5f;
        } else if (elementAttaque == dataDefenseur.elementResistance) {
            return 0.5f;
        }
        return 1f;
    }

    public int DiminueLaVie(YoKaiCombat attaquant, YoKaiCombat cible, List<YoKaiCombat> ordreDePassage){
        TypeAttaque typeAttaque = TypeAttaque.Physique;//cette variable changeras en fonction du yo-kai plus tard
        float correction = CalculerCorrection(attaquant, typeAttaque, TousLesYoKaiSurRoue(attaquant.estUnAmi,ordreDePassage));
        int rollStep = Random.Range(0, 21);
        float rouleau = 0.90f + rollStep / 100f;
        MedaliumNonPersonel dataAttaquant = medaliumDictionnaire._instance.ServantYoKai(attaquant.ID);
        MedaliumNonPersonel dataCible = medaliumDictionnaire._instance.ServantYoKai(attaquant.ID);
        float faiblesseOuRésistance = CalculerMultiplicateurElementaire(dataAttaquant.technique, typeAttaque == TypeAttaque.Physique, dataCible);
        float multiplicateurDeCompétance = 1;
        float gardeEnnemie = cible.multiplicateurGarde;

        float dégâtBase = correction / 2f + attaquant.Stats.Force / 2f - cible.Stats.Défence / 4f;
        float dégâtFinal = dégâtBase * rouleau * faiblesseOuRésistance * multiplicateurDeCompétance * gardeEnnemie;

        int taux = (int)Mathf.Max(1f, dégâtFinal);
        cible.hp_me -= taux;
        return taux;
    }
}
