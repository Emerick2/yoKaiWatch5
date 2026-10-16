using UnityEngine;

public class AddAnimator : MonoBehaviour {
    void Start(){
        // UI templates carry their own animation; model resource lookup is 3D-only.
        if (transform is RectTransform || GetComponentInParent<Canvas>() != null) return;
        transform.name = transform.name.Replace("(Clone)","");
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null){
            animator = gameObject.AddComponent<Animator>();
        }        
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("annimatorYoKai/" + gameObject.name);
        if (controller != null) animator.runtimeAnimatorController = controller;
        else if (animator.runtimeAnimatorController == null)
            Debug.LogWarning("No animation controller was found for model " + gameObject.name, this);
        animator.keepAnimatorStateOnDisable = true;
        animator.applyRootMotion = false;
    }
}
