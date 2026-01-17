using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffreMohand : MonoBehaviour
{
    private bool joueur_dedans;    // Start is called before the first frame update
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
