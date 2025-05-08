using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public int pX;
    public int pY=3;
    public int pZ=7;
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    private void Update(){
        Vector3 targetPosition = target.position + offset;
        targetPosition.x += pX;
        targetPosition.y += pY;
        targetPosition.z += pZ;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}