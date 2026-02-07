using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouvementAléatoireYoKAi : MonoBehaviour{
    private medaliumDictionnaire MedaliumDictionnaire;

    public List<Transform> pointsArrêts;
    [Range(1,100)]public float vitesse;
    [Range(0,100)]public int chanceDePause;
    [Range(0,3)]public float MinTempsDePauseQuandIlFaitUnePause;
    [Range(0,3)]public float MaxTempsDePauseQuandIlFaitUnePause;
    private int randomNumber;

    private bool actionEnCours=false;
    private Transform cible;
    void Start(){            
        MedaliumDictionnaire = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<medaliumDictionnaire>();
        BaladeDeCombat livraison = FindObjectOfType<BaladeDeCombat>();
        GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(livraison.listeYoKai[0].ID.ToString());
        
        TV.transform.localScale = new Vector3(200,200,200);
        TV.transform.parent = transform;
        yoKaiMModel3D anim = TV.AddComponent<yoKaiMModel3D>();
        anim.animController1 = transform.GetComponent<yoKaiMModel3D>().animController1;
        StartCoroutine(Fonctionnement());
    }
    IEnumerator Fonctionnement(){
        while (true){
            Vector3 position = transform.position;
            if (!actionEnCours){
                //commence à marcher :
                randomNumber = Random.Range(0, 100);
                if (chanceDePause<randomNumber){
                    int r = Random.Range(0,pointsArrêts.Count-1);
                    cible = pointsArrêts[r];
                    Vector3 newPosition1 = new Vector3(pointsArrêts[r].position.x, transform.position.y, pointsArrêts[r].position.z);
                    actionEnCours=true;
                    StartCoroutine(MoveToPosition(transform, newPosition1));
                }else{
                    float tempsDePauseT = Random.Range(MaxTempsDePauseQuandIlFaitUnePause, MinTempsDePauseQuandIlFaitUnePause);
                    yield return new WaitForSeconds(tempsDePauseT);
                }
            }else{
                if (transform.position.x == cible.position.x && transform.position.z == cible.position.z){
                    actionEnCours=false;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    IEnumerator MoveToPosition(Transform transform, Vector3 newPosition1){
        Vector3 currentPos = transform.position;
        float t = 0f;
        while (t < 1){
            t += Time.deltaTime / vitesse;
            transform.position = Vector3.Lerp(currentPos, newPosition1, t);
            yield return null;
        }
    }
}