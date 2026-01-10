using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrééUnYoKaiMohand : MonoBehaviour
{
    public GameObject monYoKai;
    public Transform position;
    void Start()
    {
        StartCoroutine(ApparitionDeYoKai(8));
        monYoKai.transform.position = position.position;
    }
    IEnumerator ApparitionDeYoKai(int id)
    {
        // if (medaliumDictionnaire._instance == null){
        //     return;
        // }
        while (medaliumDictionnaire._instance.chargementFini == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        GameObject modèle = medaliumDictionnaire._instance.dictionnaire[id].yoKai;
        monYoKai = Instantiate(modèle);
        monYoKai.AddComponent<MouvementMohand>();

    }

    void Update()
    {
        if (Input.GetButtonDown("A"))
        {
            monYoKai.GetComponent<Animator>().SetTrigger("coup");
        }

        if (Input.GetButtonDown("B"))
        {
            monYoKai.GetComponent<Animator>().SetTrigger("attaque");
        }
        //if(Input.GetButton("flèche gauche"))
        //{
        //    monYoKai.GetComponent<Animator>().SetBool("garde",true);

        //}
        //else
        //{
        //    monYoKai.GetComponent<Animator>().SetBool("garde",false);
        //}

    }
}