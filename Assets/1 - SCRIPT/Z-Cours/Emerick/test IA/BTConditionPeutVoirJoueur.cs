using UnityEngine;

public class BTConditionPeutVoirJoueur : BTNode {
    private float _distanceMax;

    public BTConditionPeutVoirJoueur(float distance) {
        _distanceMax = distance;
    }

    public override BTStatus Tick(EnnemiBTContext ctx) {
        if (ctx.Player == null) return BTStatus.Failure;

        Vector3 direction = ctx.Player.position - ctx.Self.position;
        
        if (Physics.Raycast(ctx.Self.position, direction.normalized, out RaycastHit hit, _distanceMax)) {
            if (hit.transform == ctx.Player) {
                return BTStatus.Success;
            }
        }
        return BTStatus.Failure;
    }
}