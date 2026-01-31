using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ServantGrille{
    private List<GameObject> gameObjectInTheBlock = new List<GameObject>();
    public Vector2 position;
    [Min(1)] private int maxInHorizontale = 1; // ne doit jamais être <= 0
    [Min(1)] private int maxInVerticale = 1; // ne doit jamais être <= 0
    [Min(1)] private int maxVisibleInHorizontale = 1; // ne doit jamais être <= 0
    [Min(1)] private int maxVisibleInVerticale = 1; // ne doit jamais être <= 0
    [Min(0)] public int highestVisibleLine = 0; 
    private bool inTheHorizontalAxe = true; //ça signifie que les élément vont vers le bas et pas vers la droite.
    public GameObject conteneur;
    private GameObject exemple;
    private GridLayoutGroup grid;
    private RectTransform rt;
    private RectTransform rtMasque;
    private bool start = false;
    private int marge = 1;
    private MonoBehaviour runner;
    private Coroutine coroutineMouvement;
    private Func<GameObject, GameObject, int, GameObject> maFonctionDePlacement;
    private Func<GameObject> appllerAuChangemantDePosition;
    public void Initialisation(int numberOfItems, GameObject conteneur2, RectTransform rtMasque2, Func<GameObject, GameObject, int, GameObject> laFonction, MonoBehaviour runner2, Func<GameObject> appllerAuChangemantDePosition2){
        conteneur = conteneur2;
        position = new Vector2(0,0);
        rtMasque = rtMasque2;
        maFonctionDePlacement = laFonction;
        runner = runner2;
        appllerAuChangemantDePosition = appllerAuChangemantDePosition2;
        grid = conteneur.GetComponent<GridLayoutGroup>();
        rt = conteneur.GetComponent<RectTransform>();
        rt.pivot = new Vector2(1, 1); // le pivot du conteneur est en haut (1) à gauche (0).

        start = true;
        FillTheItems(numberOfItems);
        ConfigureGridLayoutGroup();
        ChangePosition();
        CalculateMaxVisible();
        ChangeHeightBlock(true);
    }

    private void CalculateMaxVisible() {
        float maskW = rtMasque.rect.width;
        float maskH = rtMasque.rect.height;
        float stepX = grid.cellSize.x + grid.spacing.x;
        float stepY = grid.cellSize.y + grid.spacing.y;
        // Debug.Log(maskH);
        // Debug.Log(stepY);

        maxVisibleInHorizontale = Mathf.FloorToInt((maskW + grid.spacing.x) / stepX)-1;
        maxVisibleInVerticale = Mathf.FloorToInt((maskH + grid.spacing.y) / stepY);

        // Sécurité pour éviter le 0
        maxVisibleInHorizontale = Mathf.Max(1, maxVisibleInHorizontale);
        maxVisibleInVerticale = Mathf.Max(1, maxVisibleInVerticale);

        // Debug.Log($"Visibles -> H: {maxVisibleInHorizontale}, V: {maxVisibleInVerticale}");
    }

    private void FillTheItems(int numberOfItems){
        if (!start) return;
        gameObjectInTheBlock.Clear();
        foreach (Transform child in conteneur.transform){
            if (exemple == null){
                exemple = child.gameObject;
                exemple.SetActive(false);
            }
            if (child.gameObject != exemple){
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
        if (maFonctionDePlacement != null) {
            for (int i = 0; i < numberOfItems; i++){
                GameObject valeur = maFonctionDePlacement(exemple, conteneur, i);
                if (valeur != null) gameObjectInTheBlock.Add(valeur);   
            }
        }
    }

    private void ConfigureGridLayoutGroup(){
        if (!start) return;
        /*
        Ici on vas paramétré :
        - Le nombre d'élément maximum sur l'axe x ou l'axe y (en fonction de la configuration du component).
        - La hauteur ou la largeur de la grille.
        */
        float cellWidth = rt.rect.width;
        float cellHeight = rt.rect.height;
        float containerWidth = grid.cellSize.x + grid.spacing.x;
        float containerHeight = grid.cellSize.y + grid.spacing.y;
        Vector2 sizeDelta = rt.sizeDelta;

        inTheHorizontalAxe = grid.startAxis == GridLayoutGroup.Axis.Horizontal;
        // horizontale :
        if (grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount){
            maxInHorizontale = grid.constraintCount;
        } else {
            int total = grid.padding.left + grid.padding.right;
            total += (int)(cellWidth/containerWidth); 
            maxInHorizontale = (int)sizeDelta.y / total;
            if (sizeDelta.y == 0){
                maxInHorizontale = total;
            }
        }
        if (grid.constraint == GridLayoutGroup.Constraint.FixedRowCount){
            maxInVerticale = grid.constraintCount;
        } else {
            int total = grid.padding.top + grid.padding.bottom;
            total += (int)(cellHeight/containerHeight); 
            maxInVerticale = (int)sizeDelta.x / total;
            if (sizeDelta.x == 0){
                maxInVerticale = total;
            }
        }
    }

    public void Update(){
        if (!start) return;
        if (Input.GetButtonDown("left")){
            position.x--;
            ChangePosition();
            if (coroutineMouvement != null) runner.StopCoroutine(coroutineMouvement);
            coroutineMouvement = runner.StartCoroutine(Bouge(new Vector2(-1,0)));
        } else if (Input.GetButtonDown("right")){
            position.x++;
            ChangePosition();
            if (coroutineMouvement != null) runner.StopCoroutine(coroutineMouvement);
            coroutineMouvement = runner.StartCoroutine(Bouge(new Vector2(1,0)));
        } else if (Input.GetButtonDown("down")){
            position.y++;
            ChangePosition();
            if (coroutineMouvement != null) runner.StopCoroutine(coroutineMouvement);
            coroutineMouvement = runner.StartCoroutine(Bouge(new Vector2(0,1)));
        } else if (Input.GetButtonDown("up")){
            position.y--;
            ChangePosition();
            if (coroutineMouvement != null) runner.StopCoroutine(coroutineMouvement);
            coroutineMouvement = runner.StartCoroutine(Bouge(new Vector2(0,-1)));
        } else if (Input.GetButtonUp("up") || Input.GetButtonUp("down") || Input.GetButtonUp("left") || Input.GetButtonUp("right")) {
            if (coroutineMouvement != null){
                runner.StopCoroutine(coroutineMouvement);
                coroutineMouvement = null;
            }
        }
    }

    private void ChangePosition(){
        if (!start) return;
        bool mandatoryChange = false;
        if (maxInVerticale<=0){
            maxInVerticale = 1;
            Debug.LogWarning("Attention à la division par 0 avec nombreImageParLigne !");
        }
        int totalItems = gameObjectInTheBlock.Count;
        if (position.x >= maxInHorizontale){
            position.x = 0;
            position.y++;
        } else if (position.x < 0){
            position.x = maxInHorizontale-1;
            position.y--;
        }
        position.x %= maxInHorizontale;
        if (position.y != 0 && position.y % Mathf.CeilToInt((float)totalItems / maxInHorizontale) == 0){
            mandatoryChange = true;
        }
        position.y %= Mathf.CeilToInt((float)totalItems / maxInHorizontale);

        if (position.x < 0){
            position.x = maxInHorizontale+position.x;
        }
        if (position.y < 0){
            position.y = (int)(totalItems+position.y)/maxInHorizontale;
            mandatoryChange = true;
        }
        for (int i = 0; i < totalItems; i++){
            bool laCible = i == position.y * maxInHorizontale + position.x;
            gameObjectInTheBlock[i].GetComponent<Image>().enabled = laCible;
        }
        appllerAuChangemantDePosition();
        ChangeHeightBlock(mandatoryChange);
    }

    void ChangeHeightBlock(bool mandatoryChange) {
        if (!start) return;

        float heightLine = grid.cellSize.y + grid.spacing.y; 

        int currentTopLimit = highestVisibleLine + marge+1;
        int currentBottomLimit = highestVisibleLine + maxVisibleInVerticale - marge;

        bool isTooHigh = position.y < currentTopLimit;
        if (position.y <= 0){
            isTooHigh=false;
        }
        bool isTooLow  = position.y >= currentBottomLimit;
        if (highestVisibleLine > maxInVerticale-maxVisibleInVerticale+marge+1){
            isTooLow=false;
        }

        if (isTooHigh || isTooLow || mandatoryChange) {
            int newSummit = highestVisibleLine;
            if (mandatoryChange){
                newSummit = (int)position.y-1;
            }

            if (isTooHigh) {
                newSummit = (int)position.y - marge-1;
            }
            else if (isTooLow) {
                newSummit = (int)position.y - maxVisibleInVerticale + marge;
            }

            highestVisibleLine = Mathf.Max(0, newSummit);            


            float nouvelleHauteurY = grid.padding.top + (newSummit * heightLine);
            nouvelleHauteurY+=grid.cellSize.y;
            // if (isTooHigh) nouvelleHauteurY-=grid.cellSize.y;
            // else nouvelleHauteurY+=grid.cellSize.y;
            RectTransform rt = conteneur.GetComponent<RectTransform>();    
            Vector2 newAnchoredPos = rt.anchoredPosition;
            newAnchoredPos.y = nouvelleHauteurY;
            rt.anchoredPosition = newAnchoredPos;
        }
    }

    public int MyPosition(){
        return (int)position.y * maxVisibleInHorizontale + (int)position.x;
    }

    IEnumerator Bouge(Vector2 pos){
        yield return new WaitForSeconds(1f);
        while (true){
            Debug.Log(position);
            position+=pos;
            Debug.Log(position);
            ChangePosition();  
            yield return new WaitForSeconds(0.3f);
        }
    }
}