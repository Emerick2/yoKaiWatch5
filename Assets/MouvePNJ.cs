using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvePNJ : MonoBehaviour {
    public List<StopMouveStruct> listOfStopMouveStruct;
    public float speed = 3f;
    public bool loop;

    [Header("Détection d'obstacle")]
    public float detectionDistance = 1.5f;
    public float sphereRadius = 0.5f;
    public LayerMask obstacleLayer;
    public LayerMask obstacleLayerPlayer;

    private Vector3 target;
    private int index = 0;
    private bool weAreMakingProgress = true;
    private float y = 0;


    void Start() {
        y = transform.position.y;

        if (listOfStopMouveStruct.Count > 0) {
            SetTarget();
            StartCoroutine(UpdateMouve());
        } else {
            Debug.LogWarning("La liste de points de déplacement est vide !");
        }
    }

    IEnumerator UpdateMouve() {
        while (true) {
            if (IsObstacleInFront(obstacleLayerPlayer)) {
                yield return null;
                continue; 
            } if (IsObstacleInFront(obstacleLayer)) {
                Vector3 avoidanceDirection = Vector3.zero;

                if (IsSideClear(transform.right)) {
                    avoidanceDirection = transform.right;
                } else if (IsSideClear(-transform.right)) {
                    avoidanceDirection = -transform.right;
                }

                if (avoidanceDirection != Vector3.zero) {
                    Vector3 newMovement = (transform.forward + avoidanceDirection * 0.7f).normalized;
                    transform.position += newMovement * speed * Time.deltaTime;
                    transform.forward = newMovement;

                    yield return null;
                    continue;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            Vector3 direction = target - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero) {
                transform.forward = direction.normalized;
            }

            bool weHaveArrived = Vector3.Distance(transform.position, target) < 0.1f;

            if (weHaveArrived) {
                if (listOfStopMouveStruct[index].waitingTimeSeconds > 0) {
                    yield return new WaitForSeconds(listOfStopMouveStruct[index].waitingTimeSeconds);
                }

                CalculNextIndex();
                SetTarget();
            }

            yield return null;
        }
    }

    private bool IsSideClear(Vector3 sideDirection) {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 checkDirection = (transform.forward + sideDirection).normalized;
        return !Physics.SphereCast(origin, sphereRadius, checkDirection, out _, detectionDistance, obstacleLayer);
    }

    private bool IsObstacleInFront(LayerMask layer) {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f; 

        if (Physics.SphereCast(origin, sphereRadius, transform.forward, out hit, detectionDistance, layer)) {
            return true; 
        }

        return false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        
        Gizmos.DrawWireSphere(origin, sphereRadius);
        Gizmos.DrawWireSphere(origin + transform.forward * detectionDistance, sphereRadius);
        Gizmos.DrawLine(origin + transform.right * sphereRadius, origin + transform.forward * detectionDistance + transform.right * sphereRadius);
        Gizmos.DrawLine(origin - transform.right * sphereRadius, origin + transform.forward * detectionDistance - transform.right * sphereRadius);
    }

    private void SetTarget() {
        if (listOfStopMouveStruct[index].position == null) {
            Debug.LogError("Une position n'a pas été référencée dans le script MouvePNJ à l'index " + index);
            return;
        }
        target = listOfStopMouveStruct[index].position.position;
        target.y = y;
    }

    private void CalculNextIndex() {
        if (weAreMakingProgress) {
            index++;
        } else {
            index--;
        }

        if (index >= listOfStopMouveStruct.Count || index < 0) {
            if (loop) {
                weAreMakingProgress = !weAreMakingProgress;
                
                if (weAreMakingProgress) index = 1; 
                else index = listOfStopMouveStruct.Count - 2;
                
                if (index < 0 || index >= listOfStopMouveStruct.Count) index = 0;
            } else {
                index = Mathf.Clamp(index, 0, listOfStopMouveStruct.Count - 1);
            }
        }
    }
}

[System.Serializable]
public class StopMouveStruct {
    public Transform position;
    public float waitingTimeSeconds = 0;
}