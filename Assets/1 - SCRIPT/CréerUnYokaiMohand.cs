using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CréerUnYoKaiMohand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApparitionDeYokai(8));          
    }
    IEnumerator ApparitionDeYokai(int id){ 
    
        while (medaliumDictionnaire._instance.chargementFini == false){ 
        
            yield return new WaitForSeconds(0.1f); 
        }
        GameObject modèle = medaliumDictionnaire._instance.dictionnaire[id].yoKai;
        GameObject monYokai = Instantiate(modèle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
