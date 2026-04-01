using UnityEngine;
using UnityEngine.AI;

public sealed class EnnemiBTContext {
    public Transform Self { get; }
    public Transform Player { get; private set; }
    public NavMeshAgent Agent { get; }
    public float PorteeDetection { get; }
    public float PorteeAttaque { get; }
    public float VitesseDeplacement { get; }

    public EnnemiBTContext(Transform self, Transform player, NavMeshAgent agent, float detection, float attaque, float vitesse) {
        Self = self;
        Player = player;
        Agent = agent;
        PorteeDetection = detection;
        PorteeAttaque = attaque;
        VitesseDeplacement = vitesse;
    }

    public void RafraichirJoueur(Transform joueurExplicite, string tagJoueur) {
        if (joueurExplicite != null) { Player = joueurExplicite; return; }
        if (Player == null && !string.IsNullOrEmpty(tagJoueur)) {
            var go = GameObject.FindGameObjectWithTag(tagJoueur);
            if (go != null) Player = go.transform;
        }
    }
}