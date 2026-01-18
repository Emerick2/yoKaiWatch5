using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrééUnYoKaiEmerick : MonoBehaviour{
    public GameObject monYoKai;
    void Start(){
        StartCoroutine(ApparitionDeYoKai(8));
    }
    IEnumerator ApparitionDeYoKai(int id){
        // if (medaliumDictionnaire._instance == null){
        //     return;
        // }
        while (medaliumDictionnaire._instance.chargementFini == false){
            yield return new WaitForSeconds(0.1f);
        }
        GameObject modèle = medaliumDictionnaire._instance.dictionnaire[id].yoKai;
        monYoKai = Instantiate(modèle);
        
    }

    void Update(){
        if (Input.GetButtonDown("A")){
            monYoKai.GetComponent<Animator>().SetTrigger("coup");
        }
    }
}
