using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 26 romain-secondes = 1 seconde

public class GestionnaireJeuInsecte : MonoBehaviour {
    public GameObject targetBug;
    public List<GameObject> listOfLine;
    public GameObject arrow;
    public float timeForStart = 1.733f;
    public int degrees = 90;
    public float speed = 0.01f;

    private bool canPlay = false;
    private int degreesEnd = 0;
    private bool doingTheLastLaps = false;

    void Start() {
        HideLine(3);
        StartCoroutine(TheWheelOfDestinyTurns());
    }

    void Update() {
        if (canPlay && Input.GetButtonDown("A")){
            doingTheLastLaps = true;
            degreesEnd = degrees;
            if (degreesEnd >= 360) degreesEnd = 0;
            if (degreesEnd <= 0) degreesEnd = 360;
        }
    }

    private void HideLine(int number){
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
            j++;
        }
        /*
        prendre un nombre aléatoire entre 0 et listOfPossibilities.Count
        cacher le traits.
        Retirer le choix de la liste

        continuer tant que j < nombre && listOfPossibilities.Count > 1
        
        */
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
                canPlay = false;   
            }
        }
    }
}
