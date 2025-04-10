using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouvementAléatoireYoKAi : MonoBehaviour{
    public List<Transform> pointsArrêts;
    public int vitesse;
    [Range(0,100)] int chanceDePause;
    public int MaxTempsDePauseQuandIlFaitUnePause;
    public int MinTempsDePauseQuandIlFaitUnePause;

    private bool marche=false;
    private Transform cible;
    void Update(){
        Vector2 position = transform.position;

        //commence à marcher :
        Vector2 newPosition1 = new Vector3(4f, 8f);
        StartCoroutine(MoveToPosition(transform, newPosition1));
    }
    IEnumerator MoveToPosition(Transform transform, Vector2 newPosition1){
        Vector3 currentPos = transform.position;
        float t = 0f;
        while (t < 1){
            t += Time.deltaTime / vitesse;
            transform.position = Vector2.Lerp(currentPos, newPosition1, t);
            yield return null;
        }
    }
}