using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffreMohand : MonoBehaviour
{
    [Range(0,5)]public int test_range;
    [Min(0)]public int id_nourriture;
    public bool joueur_dedans = false;

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
        } else {
            Debug.Log("non");
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
