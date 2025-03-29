using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Déplacement : MonoBehaviour
{
    [Header("Paramètres spécifiques")]
    public RectTransform imageSiDoitFlip;
    public bool uneFoisArrivéJeNeParPlus;
    public bool STOP;
    public bool pauseQuandJArriveSurLaPosition;
    public float tempsDAttente = 0f;
    private bool coroutineDéjaActivé = false;
    private Vector2 direction;
    
    [Header("Obligatoire")]
    [Range(0, 400)] public float vitesse=100;
    public RectTransform[] pointDePassage;

    private RectTransform rectTransform;
    public RectTransform cible;
    private int pointDeDestination = 0;
    public bool autoDestructionAuBoutDe15s=true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (pointDePassage != null && pointDePassage.Length > 0)
        {
            cible = pointDePassage[0];
        }
        if (autoDestructionAuBoutDe15s){Destroy(gameObject,15f);}
        StartCoroutine(RegardeSiIlVaAGaucheOuADroite());
    }

    IEnumerator RegardeSiIlVaAGaucheOuADroite(){
        float positionActuelleAntérieure = rectTransform.anchoredPosition.x;
        while (true){
            yield return new WaitForSeconds(0.03f);
            if (imageSiDoitFlip != null){
                if (rectTransform.anchoredPosition.x > positionActuelleAntérieure)
                {
                    imageSiDoitFlip.localScale = new Vector3(1, 1, 1);
                }
                else if (rectTransform.anchoredPosition.x < positionActuelleAntérieure)
                {
                    imageSiDoitFlip.localScale = new Vector3(-1, 1, 1);
                }
                positionActuelleAntérieure = rectTransform.anchoredPosition.x;
            }
        }
    }

    void Update(){
        if (!STOP){
            direction = (cible.anchoredPosition - rectTransform.anchoredPosition).normalized;
            rectTransform.anchoredPosition += direction * vitesse * Time.deltaTime;

            if (Vector2.Distance(rectTransform.anchoredPosition, cible.anchoredPosition) < 10f){
                if (pointDePassage.Length != 0){
                    pointDeDestination = (pointDeDestination + 1) % pointDePassage.Length;
                    cible = pointDePassage[pointDeDestination];
                }
                if (uneFoisArrivéJeNeParPlus){
                    STOP = true;    
                }
                if (autoDestructionAuBoutDe15s){Destroy(gameObject);}

                if (pauseQuandJArriveSurLaPosition && !coroutineDéjaActivé){
                    coroutineDéjaActivé = true;
                    StartCoroutine(PausePosition());
                }
            }
        }
    }

    IEnumerator PausePosition(){
        yield return new WaitForSeconds(tempsDAttente);
        STOP = false;
        coroutineDéjaActivé = false;
    }
}
