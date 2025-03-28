using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class assignerImageDeYoKai : MonoBehaviour
{
    public List<GameObject> médaille;
    public List<Image> icôneMédaille;
    public medaliumDictionnaire médailum;
    void Start(){
        string yoKaiDansEquipe = NewDataCenter.Instance.Lire("yoKaiDansEquipe");
        if (yoKaiDansEquipe != "0"){
            for (int i = 0; i < yoKaiDansEquipe.Length; i++){
                médaille[i].SetActive(true);
                icôneMédaille[i].sprite = médailum.dictionnaire[int.Parse(NewDataCenter.Instance.Lire(yoKaiDansEquipe[i]+"_ID"))].médaillon;
            }
        }
    }
}
