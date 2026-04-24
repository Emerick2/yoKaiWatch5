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
    public float vitesse = 5f;
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
                bool obstacleDevant = DetectionObstacle(maDirection);

                if (!obstacleDevant) {
                    rb.MovePosition(rb.position + maDirection * vitesse * Time.fixedDeltaTime);
                    
                    
                    if (animator != null) animator.SetBool("avence", true);
                } else  {
                    if (animator != null) animator.SetBool("avence", false);
                }
                Quaternion rotationCible = Quaternion.LookRotation(maDirection);
                rb.rotation = Quaternion.Slerp(rb.rotation, rotationCible, vitesseRotation * Time.fixedDeltaTime);
            } else {
                if (animator != null) animator.SetBool("avence", false);
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        } else {
            if (animator != null) animator.SetBool("avence", false);
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