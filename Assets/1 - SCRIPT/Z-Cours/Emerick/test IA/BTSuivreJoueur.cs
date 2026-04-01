using UnityEngine;

public class BTSuivreJoueur : BTNode {
    public override BTStatus Tick(EnnemiBTContext ctx) {
        if (ctx.Player == null || ctx.Agent == null) return BTStatus.Failure;

        if (!ctx.Agent.isOnNavMesh) {
            Debug.LogWarning($"{ctx.Self.name} n'est pas sur le NavMesh !");
            return BTStatus.Failure;
        }

        ctx.Agent.SetDestination(ctx.Player.position);
        ctx.Agent.speed = ctx.VitesseDeplacement;

        if (ctx.Agent.remainingDistance <= ctx.PorteeAttaque) return BTStatus.Success;
        
        return BTStatus.Running;
    }
}