using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireDeCombas : MonoBehaviour
{
    public Gestionnaire g;
    public float tempsAttanteEntreLesTours=0.5f;
    private int numéroDuTour = 1;
    private GameObject yoKaiUtiliser;
    private SliderValue yoKaiUtiliser_Vie;
    void Start()
    {
        StartCoroutine(boucleDuJeu());
    }
    IEnumerator boucleDuJeu(){
        while (true){
            trouverPersonnage();
            yield return new WaitForSeconds(tempsAttanteEntreLesTours);
        }
    }
    void trouverPersonnage(){
        if (numéroDuTour==1 && g.J1 != null && g.J1_Vie != null){
            //récupérer l'ID du personnage, sa force etc... tous doit être charger au début de la partie dans un script que tien le gameObject du personnage (ce script lui est assigner par le gestionnaire de combat)
            g.J1_Vie.value-=10;
        }
    }
}
[System.Serializable]
public class Gestionnaire{
    public GameObject J1;
    public SliderValue J1_Vie;
    public GameObject J2;
    public SliderValue J2_Vie;
    public GameObject J3;
    public SliderValue J3_Vie;
    public GameObject J4;
    public SliderValue J4_Vie;
    public GameObject J5;
    public SliderValue J5_Vie;
    public GameObject J6;
    public SliderValue J6_Vie;

    public GameObject E1;
    public SliderValue E1_Vie;
    public GameObject E2;
    public SliderValue E2_Vie;
    public GameObject E3;
    public SliderValue E3_Vie;
}
