using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coffreMohand : MonoBehaviour
{
    [Range(0,5)]public int test_range;
    [Min(0)]public int id_nourriture;
    public bool joueur_dedans = false;
    private DialcodePrinted dialcodePrinted;
    public bool estClické;
    public bool estRécuperee;


    void Start()
    {
        dialcodePrinted = DialcodePrinted.Instance;
            
    }







    void Update(){
        if(joueur_dedans==true && Input.GetButtonDown("A") && estRécuperee == false)
        {
            if (estClické)
            {
                dialcodePrinted.Fermeture(false);
                estRécuperee = true;

            }
            else
            {
                GetComponent<Animator>().SetTrigger("Ouverture de Coffre");
                
                dialcodePrinted.RecevoirUnCadeau(TypeDialogue.cadeau, 1, CatégorieItem.Nouriture, 0, null, "", false);
                estClické = true;
            }

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
