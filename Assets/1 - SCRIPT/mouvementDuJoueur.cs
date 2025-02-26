using UnityEngine;

public class mouvementDuJoueur : MonoBehaviour
{
    Vector3 directionHaut = new Vector3(0, 0, 1);
    Vector3 directionBas = new Vector3(0, 0, -1);
    Vector3 directionGauche = new Vector3(-1, 0, 0);
    Vector3 directionDroit = new Vector3(1, 0, 0);
    public int vitesse = 2;


    void Update(){
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            transform.Translate(directionDroit * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }else if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)){
            transform.Translate(directionGauche * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }else if(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)){
            transform.Translate(directionHaut * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            transform.Translate(directionBas * vitesse * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        }
    }
}
