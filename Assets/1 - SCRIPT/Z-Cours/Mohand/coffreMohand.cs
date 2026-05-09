using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class coffreMohand : MonoBehaviour
{
    [Range(0,5)]public int test_range;
    [Min(0)]public int id_nourriture;
    private bool joueur_dedans = false;
    private DialcodePrinted dialcodePrinted;
    public bool estClické;
    public bool estRécuperee;
    public string clé;
    public AudioClip musique;


    void Start()
    {
        dialcodePrinted = DialcodePrinted.Instance;
        clé = transform.position.x.ToString() + "-" + transform.position.x.ToString() + "-" + SceneManager.GetActiveScene().name;
        if (NewDataCenter.Instance.Lire(clé) != "0")
        {
            estRécuperee = true;
            GetComponent<Animator>().SetTrigger("Ouverture de Coffre");


        }
            
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
                NewDataCenter.Instance.SauvegarderSTR(clé, "1");
                GestionnaireMusique._Komasan.ChangerLaMusique(musique, false);
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
