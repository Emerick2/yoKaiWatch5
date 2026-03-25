using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class mouvementDuJoueur : MonoBehaviour{
    private GameObject interfasse;
    public static bool droitDeMarcher = true;//mouvementDuJoueur.droitDeMarcher = true;
    public static GameObject player;
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
    // private CharacterController controller;
    public CameraFollow camera;
    void Start(){
        bool isAGirl = NewDataCenter.Instance.Lire("thisIsAGirl") == "True";
        GameObject katie = transform.Find("Katie").gameObject;
        GameObject nathan = transform.Find("Nathan").gameObject;
        if (isAGirl){
            Destroy(nathan);
            player = katie;
        } else {
            Destroy(katie);
            player = nathan;
        }
        camera.target = player.transform;
        player.SetActive(true);
        player.GetComponent<Rigidbody>().useGravity = true;

        // if (gameObject.GetComponent<CharacterController>() != null) controller = gameObject.GetComponent<CharacterController>();
        // else controller = gameObject.AddComponent<CharacterController>();
        LivraisonDébutUnePartie livrer = FindObjectOfType<LivraisonDébutUnePartie>();
        if (livrer != null){ 
            livrer.Déplacer(player.transform);
        }
        interfasse = GameObject.FindWithTag("interfasse");
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
                // controller.Move(maDirection * vitesse * Time.deltaTime);
                Quaternion rotationCible = Quaternion.LookRotation(maDirection);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotationCible, vitesseRotation * Time.deltaTime);
            } else {
                non = false;
            }
            if (animator != null) animator.SetBool("avence", non);
            if (caméra != null){
                Vector3 nouvellePositionCaméra = caméra.position;
                nouvellePositionCaméra.x = player.transform.position.x;
                nouvellePositionCaméra.z = player.transform.position.z;
                caméra.position = nouvellePositionCaméra;
            }
            //iconeMiniMap.position = joueur.transform.position;
        }
        if (interfasse != null && interfasse.GetComponent<CanvasGroup>() != null){
            if (droitDeMarcher){
                interfasse.GetComponent<CanvasGroup>().alpha = 1;
            } else {
                interfasse.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    } 
}
