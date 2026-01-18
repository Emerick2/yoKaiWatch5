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
    public Image charge;
    public int décompte;
    public RectTransform[] positionDeApparitionDesBoules;
    public GameObject laBoule;
    public GameObject lePapa;
    [HideInInspector] public contrôleMain contrôle;
    [HideInInspector] public int position;
    void Start(){
        Initialisation();
    }

    public void Initialisation(){
        if (jeuToucher){
            décompte=6;
            StartCoroutine(TourneViseEnvoyer());
        }
    }

    void Décompter(){
        décompte-=1;
        charge.fillAmount = 1f-((float)décompte/6f);
        if (décompte<=1){
            contrôle.AmultimeFinie(position);
            CHARGER.SetActive(true);
        }
    }
    IEnumerator TourneViseEnvoyer(){
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
            TV.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
    public void BoulleToucher(GameObject go){
        Destroy(go);
        Décompter();
    }
}
