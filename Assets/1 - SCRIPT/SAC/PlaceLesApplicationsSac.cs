using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlaceLesApplicationsSac : MonoBehaviour{
    public GameObject blocOrigine;
    public List<Sprite> listeImageApplication = new List<Sprite>();
    public List<GameObject> lesLieux = new List<GameObject>();
    [HideInInspector]public int cible;
    public Sprite spriteVisé;
    public Sprite spriteNonVisé;
    private int nombreApplicationParEtage = 3;
    void Start(){
        Initialisation();
    }

    void Initialisation(){
        for (int i = 0; i < listeImageApplication.Count; i++){
            GameObject TV = Instantiate(blocOrigine);
            TV.transform.Find("icone").GetComponent<Image>().sprite = listeImageApplication[i];
            TV.transform.parent = transform;
            TV.transform.localScale = new Vector3(1,1,1);
            lesLieux.Add(TV);
        }
        Destroy(blocOrigine);
        ChangerPositionCurseur(0);
    }

    void Update(){
        if (Input.GetButtonDown("flèche droite")){
            ChangerPositionCurseur((cible+1)%lesLieux.Count);
        }
        else if (Input.GetButtonDown("flèche gauche")){
            cible -= 1;
            if (cible < 0){
                cible = lesLieux.Count+cible;
            }
            ChangerPositionCurseur(cible);
        } else if (Input.GetButtonDown("flèche bas")){
            ChangerPositionCurseur((cible+nombreApplicationParEtage)%lesLieux.Count);
        }
        else if (Input.GetButtonDown("flèche haut")){
            cible -= nombreApplicationParEtage;
            if (cible < 0){
                cible = lesLieux.Count+cible;
            }
            ChangerPositionCurseur(cible);
        } 
    }

    void ChangerPositionCurseur(int position){
        cible = position;
        if (cible > lesLieux.Count){
            cible = lesLieux.Count-1;
        } else if (cible < 0){
            cible = 0;
        }
        foreach (GameObject item in lesLieux){
            item.GetComponent<Image>().sprite = spriteNonVisé;
        }
        lesLieux[cible].GetComponent<Image>().sprite = spriteVisé;
    }
}
