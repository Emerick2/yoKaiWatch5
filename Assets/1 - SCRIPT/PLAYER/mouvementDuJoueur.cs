using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class mouvementDuJoueur : MonoBehaviour {
    private GameObject interfasse;
    public static bool droitDeMarcher = true;
    public static GameObject player;
    private Rigidbody rb;

    public Animator animator;
    public Animator oeuilSauvegarde;
    private float vitesse = 5f;
    public float vitesseNormal = 5f;
    public float vitesseCourse = 10f;
    private bool isRuning = false;
    public float vitesseRotation = 10f;
    public Transform caméra;
    public CameraFollow cameraFollow;
    public float distanceObstacle = 0.3f;
    public float distanceObstaclePNJ = 0.05f;

    void Start() {
        bool isAGirl = NewDataCenter.Instance.Lire("thisIsAGirl") == "True";
        GameObject katie = transform.Find("Katie").gameObject;
        GameObject nathan = transform.Find("Nathan").gameObject;

        if (isAGirl) {
            Destroy(nathan);
            player = katie;
        } else {
            Destroy(katie);
            player = nathan;
        }

        cameraFollow.target = player.transform;
        player.SetActive(true);

        rb = player.GetComponent<Rigidbody>();
        if (rb == null) rb = player.AddComponent<Rigidbody>();
        
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        LivraisonDébutUnePartie livrer = FindObjectOfType<LivraisonDébutUnePartie>();
        if (livrer != null) { 
            livrer.Déplacer(player.transform);
        }
        interfasse = GameObject.FindWithTag("interfasse");
    }

    void Update(){
        if (droitDeMarcher){
            if (Input.GetButtonDown("B")){
                isRuning = true;
                vitesse = vitesseCourse;
            } else if (Input.GetButtonUp("B")){
                isRuning = false;
                vitesse = vitesseNormal;
            }
        } else if (isRuning) {
            isRuning = false;
            vitesse = vitesseNormal;
        }
    }

    private bool DetectionObstacle(Vector3 maDirection){
        Vector3 origine = player.transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origine, maDirection, distanceObstacle, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);    
    }

    void FixedUpdate() {
        if (droitDeMarcher && rb != null) {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 maDirection = new Vector3(h, 0, v).normalized;

            if (maDirection.magnitude > 0.1f) {
                rb.velocity = new Vector3(maDirection.x * vitesse, rb.velocity.y, maDirection.z * vitesse);
                
                if (animator != null) animator.SetBool("avence", true);

                Quaternion rotationCible = Quaternion.LookRotation(maDirection);
                rb.rotation = Quaternion.Slerp(rb.rotation, rotationCible, vitesseRotation * Time.fixedDeltaTime);
            } else {
                if (animator != null) animator.SetBool("avence", false);
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                rb.angularVelocity = Vector3.zero;
            }
        } else if (rb != null) {
            if (animator != null) animator.SetBool("avence", false);
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }

        UpdateCameraPosition();
        UpdateInterface();
    }

    void UpdateCameraPosition() {
        if (caméra != null && player != null) {
            Vector3 nouvellePositionCaméra = caméra.position;
            nouvellePositionCaméra.x = player.transform.position.x;
            nouvellePositionCaméra.z = player.transform.position.z;
            caméra.position = nouvellePositionCaméra;
        }
    }

    void UpdateInterface() {
        if (interfasse != null && interfasse.GetComponent<CanvasGroup>() != null) {
            interfasse.GetComponent<CanvasGroup>().alpha = droitDeMarcher ? 1 : 0;
        }
    }
}
