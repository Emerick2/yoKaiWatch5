using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 26 romain-secondes = 1 seconde

public class GestionnaireJeuInsecte : MonoBehaviour {
    public GameObject targetBug;
    public List<GameObject> listOfLine;
    public GameObject arrow;
    public RectTransform targetBugMouving;
    public float timeForStart = 1.733f;
    private int degrees = 90;
    public float speed = 0.01f;

    private bool canPlay = false;
    private bool canMouve = true;
    private int degreesEnd = 0;
    private bool doingTheLastLaps = false;
    public float speedMouve = 3f;
    public float minXPosition;
    public float minYPosition;
    public float maxXPosition;
    public float maxYPosition;

    public List<RectTransform> badBug;
    public GameObject targetBugForTheMoveImage;
    public GameObject targetBugForPlayImage;
    public Sprite targetTouche;
    public Sprite targetNotTouche;
    private int idBugTargetForThisGame;

    public GameObject dialogueBubble;
    private bool dialogueBubbleInProgress;
    private bool itMustBeSaidThatThereIsNothingLeft = false;
    public List<GameObject> listImageHideLine = new List<GameObject>();
    public GameObject iconeSelectBug;


    void Start() {
        Initialisation();
    }

    private void Initialisation(){
        canPlay = false;
        canMouve = true;
        targetBugForPlayImage.SetActive(false);
        targetBugForTheMoveImage.SetActive(true);
        degrees = 90;
        doingTheLastLaps = false;

        if (badBug.Count == 0) {
            itMustBeSaidThatThereIsNothingLeft = true;
        }
    }

    void Update() {
        if (dialogueBubbleInProgress){
            if (Input.GetButtonDown("A")){
                dialogueBubble.SetActive(false);
                dialogueBubbleInProgress = false;
            }
            return;
        }

        if (itMustBeSaidThatThereIsNothingLeft) {
            OpenTheMessage(false, 0, true, "Il n'y a plus rien ici...");
            itMustBeSaidThatThereIsNothingLeft = false;
        }

        if (canPlay && Input.GetButtonDown("A")){
            doingTheLastLaps = true;
            degreesEnd = degrees;
            if (degreesEnd >= 360) degreesEnd = 0;
            if (degreesEnd <= 0) degreesEnd = 360;
        } else if (canMouve) {
            Vector2 movement = Vector2.zero;

            if (Input.GetButton("left") && targetBugMouving.anchoredPosition.x > minXPosition) 
                movement.x = -1;
            else if (Input.GetButton("right") && targetBugMouving.anchoredPosition.x < maxXPosition) 
                movement.x = 1;

            if (Input.GetButton("up") && targetBugMouving.anchoredPosition.y < maxYPosition) 
                movement.y = 1;
            else if (Input.GetButton("down") && targetBugMouving.anchoredPosition.y > minYPosition) 
                movement.y = -1;

            targetBugMouving.anchoredPosition += movement * speedMouve * Time.deltaTime;

            if (badBug.Count == 0){
                targetBugForTheMoveImage.GetComponent<Image>().sprite = targetNotTouche;
            }

            for (int i = 0; i < badBug.Count; i++){
                RectTransform bug = badBug[i];

                if (CheckOverlap(targetBugMouving, bug, 0.4f)) {
                    targetBugForTheMoveImage.GetComponent<Image>().sprite = targetTouche;
                    targetBugForTheMoveImage.transform.localScale = Vector3.Lerp(targetBugForTheMoveImage.transform.localScale, new Vector3(1f, 1f, 1f), 10f * Time.deltaTime);
                    iconeSelectBug.SetActive(true);

                    if (Input.GetButtonDown("A")){
                        targetBugMouving.anchoredPosition = bug.anchoredPosition;
                        targetBugForTheMoveImage.SetActive(false);
                        targetBugForPlayImage.SetActive(true);
                        canMouve = false;
                        idBugTargetForThisGame = i;
                        HideLine(Random.Range(3,6));
                        StartCoroutine(TheWheelOfDestinyTurns());
                    }
                    break;
                } else {
                    targetBugForTheMoveImage.GetComponent<Image>().sprite = targetNotTouche;
                    iconeSelectBug.SetActive(false);
                    targetBugForTheMoveImage.transform.localScale = Vector3.Lerp(targetBugForTheMoveImage.transform.localScale, new Vector3(0.8f, 0.8f, 1f), 10f * Time.deltaTime);
                }
            }
        }
    }

    private void HideLine(int number){
        listImageHideLine.Clear();
        foreach (GameObject item in listOfLine){
            item.SetActive(true);
        }
        List<int> listOfPossibilities = new List<int>();
        for (int i = 0; i < listOfLine.Count; i++){
            listOfPossibilities.Add(i);
        }
        int j = 0;
        while (j < number && listOfPossibilities.Count > 1){
            int randomID = Random.Range(0,listOfPossibilities.Count);
            int random = listOfPossibilities[randomID];
            listOfPossibilities.Remove(randomID);
            listOfLine[random].SetActive(false);
            listImageHideLine.Add(listOfLine[random]);
            j++;
        }
    }

    IEnumerator TheWheelOfDestinyTurns(){
        float radius = 100f;
        Vector2 center = new Vector2(0, 0);
        yield return new WaitForSeconds(timeForStart);
        canPlay = true;

        while (canPlay) {
            yield return new WaitForSeconds(speed);     

            float radians = degrees * Mathf.Deg2Rad;

            float arrowX = center.x + Mathf.Cos(radians) * radius;
            float arrowY = center.y + Mathf.Sin(radians) * radius;

            Vector2 vectorArrow = new Vector2(arrowX, arrowY);

            // transform.localPosition = vectorArrow;

            arrow.transform.localPosition = vectorArrow;
            arrow.transform.rotation = Quaternion.Euler(arrow.transform.rotation.x, arrow.transform.rotation.y, degrees-90);

            if (!doingTheLastLaps) degrees--;
            else degrees-=2;
            // else degrees-=(int)(1 + (Mathf.Abs(90 - degrees) * 0.02f));;
            
            if (degrees >= 360) degrees = 0;
            if (degrees <= 0) degrees = 360;

            if (doingTheLastLaps && (degrees == degreesEnd || degrees+1 == degreesEnd)){
                if (idBugTargetForThisGame >= 0 && idBugTargetForThisGame < badBug.Count){
                    if (badBug[idBugTargetForThisGame] != null) Destroy(badBug[idBugTargetForThisGame].gameObject);
                    badBug.RemoveAt(idBugTargetForThisGame);
                }
                if (IsWin()){
                    OpenTheMessage(true, 0);
                } else {
                    OpenTheMessage(false, 0);
                }
                Initialisation();
            }
        }
    }

    private bool CheckOverlap(RectTransform rect1, RectTransform rect2, float percentage) {
        Vector3[] corners1 = new Vector3[4];
        Vector3[] corners2 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        rect2.GetWorldCorners(corners2);

        Rect r1 = new Rect(corners1[0].x, corners1[0].y, corners1[2].x - corners1[0].x, corners1[2].y - corners1[0].y);
        Rect r2 = new Rect(corners2[0].x, corners2[0].y, corners2[2].x - corners2[0].x, corners2[2].y - corners2[0].y);

        float xMin = Mathf.Max(r1.xMin, r2.xMin);
        float xMax = Mathf.Min(r1.xMax, r2.xMax);
        float yMin = Mathf.Max(r1.yMin, r2.yMin);
        float yMax = Mathf.Min(r1.yMax, r2.yMax);

        if (xMax <= xMin || yMax <= yMin) return false;

        float intersectionArea = (xMax - xMin) * (yMax - yMin);
        float rect1Area = r1.width * r1.height;

        return (intersectionArea / rect1Area) >= percentage;
    }

    private void OpenTheMessage(bool victory, int idBugDictionary, bool forInformation = false, string textInformation = ""){
        dialogueBubble.SetActive(true);
        dialogueBubbleInProgress = true;
        TextMeshProUGUI text = dialogueBubble.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>();
        GameObject espaceImage  = dialogueBubble.transform.Find("cintille").gameObject;
        Image imageBug  = dialogueBubble.transform.Find("cintille/cadeau").gameObject.GetComponent<Image>();

        if (forInformation){
            espaceImage.SetActive(false);
            text.text = textInformation;
        } else if (victory){
            Annimal animal = null;
            if (Item.Instance != null && idBugDictionary >= 0 && idBugDictionary < Item.Instance.listeDesAnnimaux.Count){
                animal = Item.Instance.listeDesAnnimaux[idBugDictionary];
            }
            if (animal == null){
                text.text = "Vous l'avez eu !";
                espaceImage.SetActive(false);
            } else {
                text.text = "Obtenu : "+animal.nom;
                if (animal.image != null){
                    espaceImage.SetActive(true);
                    imageBug.sprite = animal.image;
                } else {
                    espaceImage.SetActive(false);
                }
            }
        } else if (!victory){
            espaceImage.SetActive(false);
            text.text = "Vous avez rater !";
        } else {
            dialogueBubble.SetActive(false);
            dialogueBubbleInProgress = false;
            return;
        }
    }

    float GetIntersectionArea(RectTransform rt1, RectTransform rt2) {
        Vector3[] c1 = new Vector3[4];
        Vector3[] c2 = new Vector3[4];
        rt1.GetWorldCorners(c1);
        rt2.GetWorldCorners(c2);

        Rect r1 = Rect.MinMaxRect(c1[0].x, c1[0].y, c1[2].x, c1[2].y);
        Rect r2 = Rect.MinMaxRect(c2[0].x, c2[0].y, c2[2].x, c2[2].y);

        float xMin = Mathf.Max(r1.xMin, r2.xMin);
        float xMax = Mathf.Min(r1.xMax, r2.xMax);
        float yMin = Mathf.Max(r1.yMin, r2.yMin);
        float yMax = Mathf.Min(r1.yMax, r2.yMax);

        if (xMax > xMin && yMax > yMin) {
            return (xMax - xMin) * (yMax - yMin);
        }

        return 0f;
    }

    private bool IsWin() {
        float totalWinArea = 0f;
        float totalLossArea = 0f;

        foreach (GameObject lineObject in listOfLine) {
            RectTransform lineRect = lineObject.GetComponent<RectTransform>();
            if (lineRect == null) continue;

            float intersectionArea = GetIntersectionArea(targetBugMouving, lineRect);

            if (intersectionArea > 0) {
                if (!listImageHideLine.Contains(lineObject)) {
                    totalWinArea += intersectionArea;
                } 
                else {
                    totalLossArea += intersectionArea;
                }
            }
        }

        return totalWinArea > totalLossArea;
    }
}
