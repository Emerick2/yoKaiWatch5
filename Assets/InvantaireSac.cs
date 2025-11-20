using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class InvantaireSac : MonoBehaviour{
    public List<GameObject> laListeDesCatégorie;
    private int cible = 0;
    public Sprite spriteRSélection;
    public Sprite spriteLSélection;
    public Sprite spriteRNonSélection;
    public Sprite spriteLNonSélection;
    public Image L;
    public Image R;
    public float tempsTouche = 0.3f;
    private Coroutine coroutineCliqueLR;
    private List<ObjetDansLeSacInformation> lesItems = new List<ObjetDansLeSacInformation>();
    public GameObject conteneur;
    private GameObject exemple;
    public bool étatDeDebug = true;
    void Start(){
        Initialiser();
    }
    public void Initialiser(){
        cible = 0;
        ChangerPositionCible();
    }

    void Update(){
        if (Input.GetButtonDown("L")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(true));
            R.sprite=spriteRNonSélection;
            cible--;
            ChangerPositionCible();
        } else if (Input.GetButtonDown("R")){
            if (coroutineCliqueLR != null){
                StopCoroutine(coroutineCliqueLR);
            }
            coroutineCliqueLR = StartCoroutine(CliqueLR(false));
            L.sprite=spriteLNonSélection;
            cible++;
            ChangerPositionCible();
        }
    }

    void ChangerPositionCible(){
        cible %= laListeDesCatégorie.Count;
        if (cible < 0){
            cible = laListeDesCatégorie.Count+cible;
        }
        for (int i = 0; i < laListeDesCatégorie.Count; i++){
            if (i == cible){
                laListeDesCatégorie[i].GetComponent<Image>().enabled = true;
            } else {
                laListeDesCatégorie[i].GetComponent<Image>().enabled = false;
            }
        }
        lesItems.Clear();
        switch (cible){
            case 1:
                PlacerLaNouriture();
                break;
        }
        PlacerLesItems();
    }

    IEnumerator CliqueLR(bool cesLeL){
        if (cesLeL) L.sprite=spriteLSélection;
        else R.sprite=spriteRSélection;
        yield return new WaitForSeconds(tempsTouche);
        if (cesLeL) L.sprite=spriteLNonSélection;
        else R.sprite=spriteRNonSélection;
    }

    void PlacerLaNouriture(){
        if (étatDeDebug){
            foreach (Nouriture item in Item._instance.listeDesNouritures){
                lesItems.Add(new ObjetDansLeSacInformation{
                    nom = item.nom,
                    sprite = item.image,
                    infoNouriture = item,
                    quantitée = 1,
                });
            }
        } else {
            foreach (Nouriture item in Item._instance.listeDesNouritures){
                int quantitées = int.Parse(NewDataCenter.Instance.Lire(item.nom));
                if (quantitées > 0){
                    lesItems.Add(new ObjetDansLeSacInformation{
                        nom = item.nom,
                        sprite = item.image,
                        infoNouriture = item,
                        quantitée = quantitées,
                    });
                }
            }
        }
    }

    void PlacerLesItems(){
        foreach (Transform child in conteneur.transform){
            if (exemple == null){
                exemple = child.gameObject;
                exemple.SetActive(false);
            }
            if (child.gameObject != exemple){
                Destroy(child.gameObject);
            }
        }
        foreach (ObjetDansLeSacInformation item in lesItems){
            GameObject TV = Instantiate(exemple);
            TV.transform.Find("img").GetComponent<Image>().sprite = item.sprite;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi0 = false;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireSi1 = false;
            TV.transform.Find("nombre").GetComponent<EcrireUneQuantitéAvecDesImages>().EcrireLeNombre(item.quantitée);
            TV.transform.parent = conteneur.transform;
            TV.transform.localScale = new Vector3(1,1,1);
            TV.SetActive(true);
        }
    }
}

[System.Serializable]
public class ObjetDansLeSacInformation{
    public int quantitée;
    public string nom;
    public Sprite sprite;
    public Nouriture infoNouriture = null;
}