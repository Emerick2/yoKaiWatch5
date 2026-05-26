using UnityEngine;

public class AddAnimator : MonoBehaviour {
    void Start(){
        transform.name = transform.name.Replace("(Clone)","");
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null){
            animator = gameObject.AddComponent<Animator>();
        }        
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("annimatorYoKai/"+gameObject.name);
    }
}
