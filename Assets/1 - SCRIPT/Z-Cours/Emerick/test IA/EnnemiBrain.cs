using UnityEngine;
using UnityEngine.AI;

public class EnnemiBrain : MonoBehaviour {
    [Header("Configuration")]
    public Transform joueur;
    public float distanceVision = 15f;
    public float distanceAttaque = 2f;
    public float vitesse = 3.5f;

    private EnnemiBTContext _ctx;
    private BTNode _arbre;
    private NavMeshAgent _agent;

    void Start() {
        _agent = GetComponent<NavMeshAgent>();
        
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas)) {
            _agent.Warp(hit.position);
        }

        _ctx = new EnnemiBTContext(transform, joueur, _agent, distanceVision, distanceAttaque, vitesse);

        _arbre = new BTSequence(
            new BTConditionPeutVoirJoueur(distanceVision),
            new BTSuivreJoueur()
        );
    }

    void Update() {
        if (_arbre != null) {
            _arbre.Tick(_ctx);
        }
    }
}