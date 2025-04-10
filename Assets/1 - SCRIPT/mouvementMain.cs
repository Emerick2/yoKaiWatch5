using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouvementMain : MonoBehaviour
{
    public float vitesse = 800f;
    public float minXPosition = -500f;
    public float maxXPosition = 500f;
    public float minYPosition = 0f;
    public float maxYPosition = 1000f;
    private bool bloquer;
    public GameObject lePapa;

    private RectTransform rectTransform;

    void Start(){
        rectTransform = GetComponent<RectTransform>();
    }

    void Update(){
        if (!bloquer){
            Vector2 position = rectTransform.anchoredPosition;

            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && position.x < maxXPosition){
                position.x += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) && position.x > minXPosition){
                position.x -= vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) && position.y < maxYPosition){
                position.y += vitesse * Time.deltaTime;
            }
            if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && position.y > minYPosition){
                position.y -= vitesse * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space)){
                StartCoroutine(StopClic());
            }

            rectTransform.anchoredPosition = position;
        }
    }

    IEnumerator StopClic()
    {
        bloquer = true;
        List<RectTransform> autreObjet = new List<RectTransform>();

        // Cherche tous les enfants ayant le tag "Finish"
        foreach (Transform child in lePapa.transform){
            if (child.gameObject.CompareTag("Finish")){
                RectTransform rect = child.GetComponent<RectTransform>();
                if (rect != null){
                    autreObjet.Add(rect);
                }
            }
        }

        bool stoper = false;
        int i = autreObjet.Count;

        gameObject.GetComponent<Animator>().SetBool("clic", true);

        if (autreObjet.Count > 0){
            while (!stoper && i > 0){
                i--;
                if (RectsSeChevauchent(rectTransform, autreObjet[i])){
                    stoper = true;
                    Debug.Log("Collision avec l'objet Finish !");
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        gameObject.GetComponent<Animator>().SetBool("clic", false);
        bloquer = false;
    }

    bool RectsSeChevauchent(RectTransform a, RectTransform b){
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];

        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);

        Rect rectA = new Rect(aCorners[0].x, aCorners[0].y, aCorners[2].x - aCorners[0].x, aCorners[2].y - aCorners[0].y);
        Rect rectB = new Rect(bCorners[0].x, bCorners[0].y, bCorners[2].x - bCorners[0].x, bCorners[2].y - bCorners[0].y);

        return rectA.Overlaps(rectB);
    }
}
