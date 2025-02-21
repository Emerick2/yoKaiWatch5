using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class jeuAmultime : MonoBehaviour
{
    public bool jeuTourner;
    public bool jeuDessiner;//3
    public bool jeuPointer;
    public bool jeuToucher;//6
    public GameObject CHARGER;
    public int décompte;
    public RectTransform[] positionDeApparitionDesBoules;
    public GameObject laBoule;
    public GameObject lePapa;

    public void décompter(){
        décompte-=1;
        if (décompte<=0){
            CHARGER.SetActive(true);
        }
    }
    void Start(){
        if (jeuToucher){
            décompte=6;
            StartCoroutine(tourneViseEnvoyer());
        }
    }
    IEnumerator tourneViseEnvoyer(){
        while (jeuToucher){
            yield return new WaitForSeconds(1f);
            int indexSpawn = Random.Range(0, positionDeApparitionDesBoules.Length);
            GameObject TV = Instantiate(laBoule, lePapa.transform);
            TV.GetComponent<RectTransform>().anchoredPosition = positionDeApparitionDesBoules[indexSpawn].anchoredPosition;
            TV.SetActive(true);

            int indexCible;
            if(indexSpawn < 4){
                indexCible = Random.Range(4, positionDeApparitionDesBoules.Length);
            } else {
                indexCible = Random.Range(0, 4);
            }
            TV.GetComponent<UI_Déplacement>().cible = positionDeApparitionDesBoules[indexCible].GetComponent<RectTransform>();
        }
    }
}
