using UnityEngine;

public class CacheAnimator : MonoBehaviour{
    private Animator anim;
    private float lastTime;
    private int stateHash;

    void Awake() => anim = GetComponent<Animator>();

    void OnDisable() {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        stateHash = state.fullPathHash;
        lastTime = state.normalizedTime;
    }

    void OnEnable() {
        anim.Play(stateHash, 0, lastTime);
    }
}
