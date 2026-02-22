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
        // StartCoroutine(AffichageImageYoKai());
    }
    IEnumerator AffichageImageYoKai(){
        while(true){
            List<int> yoKaiDansEquipe = MesYoKai.Instance.yoKaiDeMonEquipe;
            for (int i = 0; i < yoKaiDansEquipe.Count; i++){
                if (i<médaille.Count && i<icôneMédaille.Count){
                    médaille[i].SetActive(true);
                    icôneMédaille[i].sprite = médailum.ServantYoKai(int.Parse(NewDataCenter.Instance.Lire(yoKaiDansEquipe[i]+"_ID"))).médaillon;
                }else{
                    Debug.LogError("Le i est en "+i);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
