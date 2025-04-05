using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class assignerImageDeYoKai : MonoBehaviour
{
    public List<GameObject> médaille;
    public List<Image> icôneMédaille;
    private medaliumDictionnaire médailum;
    void Start(){
        médailum = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        string yoKaiDansEquipe = NewDataCenter.Instance.Lire("yoKaiDansEquipe");
        if (yoKaiDansEquipe != "0"){
            for (int i = 0; i < yoKaiDansEquipe.Length; i++){
                médaille[i].SetActive(true);
                icôneMédaille[i].sprite = médailum.dictionnaire[int.Parse(NewDataCenter.Instance.Lire(yoKaiDansEquipe[i]+"_ID"))].médaillon;
            }
        }
    }
}
