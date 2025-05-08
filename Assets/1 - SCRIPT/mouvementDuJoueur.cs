using UnityEngine;

public class mouvementDuJoueur : MonoBehaviour{
    public static bool droitDeMarcher = true;//mouvementDuJoueur.droitDeMarcher = true;
    public GameObject joueur;
    public Animator animator;
    Vector3 directionHaut = new Vector3(0, 0, 1);
    Vector3 directionBas = new Vector3(0, 0, -1);
    Vector3 directionGauche = new Vector3(-1, 0, 0);
    Vector3 directionDroit = new Vector3(1, 0, 0);
    public float vitesse = 2;

    void Update(){
        if (droitDeMarcher){
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                transform.Translate(directionDroit * vitesse * Time.deltaTime);
                animator.SetBool("avence",true);
                joueur.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }else if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)){
                transform.Translate(directionGauche * vitesse * Time.deltaTime);
                joueur.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                animator.SetBool("avence",true);
            }else if(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)){
                transform.Translate(directionHaut * vitesse * Time.deltaTime);
                joueur.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                animator.SetBool("avence",true);
            }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
                transform.Translate(directionBas * vitesse * Time.deltaTime);
                joueur.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                animator.SetBool("avence",true);
            }else{
                animator.SetBool("avence",false);
            }
        }
    }
}
