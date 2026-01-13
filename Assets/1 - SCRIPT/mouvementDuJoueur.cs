using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class mouvementDuJoueur : MonoBehaviour{
    public static bool droitDeMarcher = true;//mouvementDuJoueur.droitDeMarcher = true;
    public GameObject joueur;
    public Animator animator;
    public Animator oeuilSauvegarde;
    Vector3 directionHaut = new Vector3(0, 0, 1);
    Vector3 directionBas = new Vector3(0, 0, -1);
    Vector3 directionGauche = new Vector3(-1, 0, 0);
    Vector3 directionDroit = new Vector3(1, 0, 0);
    public float vitesse = 2;
    public float vitesseRotation = 10f;
    public Transform caméra;
    public Transform iconeMiniMap;


    void Start(){
        LivraisonDébutUnePartie livrer = FindObjectOfType<LivraisonDébutUnePartie>();
        if (livrer != null){ 
            livrer.Déplacer(joueur.transform);
        }
    }

    void Update(){
        if (droitDeMarcher){
            bool non = true;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 maDirection = new Vector3(h, 0, v);
            // if ((h > 0.03f || h < -0.03f) && (v > 0.03f || v < -0.03f)){
            if (maDirection.magnitude > 0.1f){
                transform.Translate(maDirection.normalized * vitesse * Time.deltaTime, Space.World);
                Quaternion rotationCible = Quaternion.LookRotation(maDirection);
                joueur.transform.rotation = Quaternion.Slerp(joueur.transform.rotation, rotationCible, vitesseRotation * Time.deltaTime);
            } else {
                non = false;
            }

            // if (Input.GetButton("flèche droite")){
            //     transform.Translate(directionDroit * vitesse * Time.deltaTime);
            //     joueur.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            // }else if (Input.GetButton("flèche gauche")){
            //     transform.Translate(directionGauche * vitesse * Time.deltaTime);
            //     joueur.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            // }else if (Input.GetButton("flèche haut")){
            //     transform.Translate(directionHaut * vitesse * Time.deltaTime);
            //     joueur.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            // }else if (Input.GetButton("flèche bas")){
            //     transform.Translate(directionBas * vitesse * Time.deltaTime);
            //     joueur.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // }else{
            //     non = false;
            // }
            if (animator != null) animator.SetBool("avence", non);
            if (caméra != null){
                Vector3 nouvellePositionCaméra = caméra.position;
                nouvellePositionCaméra.x = joueur.transform.position.x;
                nouvellePositionCaméra.z = joueur.transform.position.z;
                caméra.position = nouvellePositionCaméra;
            }
            //iconeMiniMap.position = joueur.transform.position;
        }
        if (Input.GetKeyDown(KeyCode.N)||Input.GetButtonDown("+") && NewDataCenter.Instance != null){
            Sauvgarder();
        }
    } 

    public void Sauvgarder(){
        if (oeuilSauvegarde != null){
            oeuilSauvegarde.SetTrigger("ouvre");
        }
        NewDataCenter.Instance.SauvegarderSTR("pos1", joueur.transform.position.x.ToString(),"position");
        NewDataCenter.Instance.SauvegarderSTR("pos2", joueur.transform.position.y.ToString(),"position");
        NewDataCenter.Instance.SauvegarderSTR("pos3", joueur.transform.position.z.ToString(),"position");
        NewDataCenter.Instance.SauvegarderSTR("scenne", SceneManager.GetActiveScene().name,"position");
    }
}
