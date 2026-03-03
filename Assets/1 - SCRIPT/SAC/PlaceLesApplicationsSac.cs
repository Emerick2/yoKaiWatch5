using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlaceLesApplicationsSac : MonoBehaviour{
    public écrireLesPetitesValeursDuMenu menu;
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
        if (menu != null){
            for (int i = 0; i < listeImageApplication.Count; i++){
                bool delet = true;
                for (int j = 0; j < menu.applicationMenus.Count; j++){
                    if (menu.applicationMenus[j].sprite == listeImageApplication[i]){
                        delet = false;
                        break;
                    }
                }
                if (delet){
                    listeImageApplication.RemoveAt(i);
                    i--;
                }
            }
        }
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
        if (Input.GetButtonDown("right")){
            ChangerPositionCurseur((cible+1)%lesLieux.Count);
        }
        else if (Input.GetButtonDown("left")){
            cible -= 1;
            if (cible < 0){
                cible = lesLieux.Count+cible;
            }
            ChangerPositionCurseur(cible);
        } else if (Input.GetButtonDown("down")){
            ChangerPositionCurseur((cible+nombreApplicationParEtage)%lesLieux.Count);
        }
        else if (Input.GetButtonDown("up")){
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
