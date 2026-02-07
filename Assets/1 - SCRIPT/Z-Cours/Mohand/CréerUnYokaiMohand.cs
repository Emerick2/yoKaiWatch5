using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrééUnYoKaiMohand : MonoBehaviour
{
    public GameObject monYoKai;
    public Transform position;
    public float taux = 0.01f;
    public int idDuYoKai = 8;
    void Start()
    {
        StartCoroutine(ApparitionDeYoKai(idDuYoKai));
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
        // GameObject modèle = medaliumDictionnaire._instance.ServantYoKai(id).yoKai;
        // monYoKai = Instantiate(modèle);
        monYoKai = medaliumDictionnaire._instance.InstantiateYoKai(id.ToString());
        monYoKai.AddComponent<MouvementMohand>();
        new ServantBoxCollider3D().CreateBoxCollider(taux, monYoKai,true);
        monYoKai.transform.tag = "Player";
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
        //if(Input.GetButton("fl�che gauche"))
        //{
        //    monYoKai.GetComponent<Animator>().SetBool("garde",true);

        //}
        //else
        //{
        //    monYoKai.GetComponent<Animator>().SetBool("garde",false);
        //}

    }
}