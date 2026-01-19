using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoisieYoKaiAmi : MonoBehaviour{
    private GestionnaireCombat gestionnaireCombat = null;
    private int cible = 0;
    private GameObject dernierCible = null;
    private bool activé = false;
    void Start(){
        gestionnaireCombat = GetComponent<GestionnaireCombat>();
        for (int i = 0; i < gestionnaireCombat.troisDisplateUIAmi.Count; i++){
            gestionnaireCombat.troisDisplateUIAmi[i].transform.Find("point").gameObject.SetActive(false);
        }
        for (int i = 0; i < gestionnaireCombat.troisDisplateUIEnnemie.Count; i++){
            gestionnaireCombat.troisDisplateUIEnnemie[i].transform.Find("point").gameObject.SetActive(false);
        }
    }
    public void Initialiser(){
        cible = 1;
        activé = true;
        ChangerCible();
    }
    public void Arrêter(){
        activé = false;
        if (dernierCible != null) dernierCible.SetActive(false);
        dernierCible = null;
    }
    void Update(){
        if (activé){
            if (Input.GetButtonDown("left")){
                cible--;
                ChangerCible();
            } else if (Input.GetButtonDown("right")){
                cible++;
                ChangerCible();
            } else if (Input.GetButtonDown("A")){
                Input.ResetInputAxes();
                gestionnaireCombat.ChoieDeAmiEffectuer(cible);
            }
        }
    }

    void ChangerCible(){
        int n = 3;
        cible = ((cible%n)+n)%n;
        gestionnaireCombat.troisDisplateUIAmi[cible].transform.Find("point").gameObject.SetActive(true);
        if (dernierCible != null) dernierCible.SetActive(false);
        dernierCible = gestionnaireCombat.troisDisplateUIAmi[cible].transform.Find("point").gameObject;
    }
}
