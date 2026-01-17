using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffreMohand : MonoBehaviour
{
    [Range(0,5)]public int test_range;
    [Min(0)]public int id_nourriture;
    private bool joueur_dedans = true;    // Start is called before the first frame update
                                   //void Start()
                                   //{

    //}

    //// Update is called once per frame
    void Update(){
        if(joueur_dedans==true && Input.GetButtonDown("A"))
        {
            Debug.Log("test");
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            joueur_dedans = true;
        }

    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            joueur_dedans = false;
        }
    }
    




}
