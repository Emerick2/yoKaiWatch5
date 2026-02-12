using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour{
    public GameObject example;
    public RectTransform panelBackground;
    public Vector2 direction = new Vector2(1, 0);
    public float vitesse = 100f;
    private List<RectTransform> allBlocks = new List<RectTransform>();
    private Vector2 blockSize;
    private Vector2 gridFullSize;

    void Start(){
        blockSize = example.GetComponent<RectTransform>().sizeDelta;
        
        int cols = Mathf.CeilToInt(panelBackground.rect.width / blockSize.x) + 2;
        int rows = Mathf.CeilToInt(panelBackground.rect.height / blockSize.y) + 2;

        gridFullSize = new Vector2(cols * blockSize.x, rows * blockSize.y);

        for (int x = 0; x < cols; x++) {
            for (int y = 0; y < rows; y++) {
                RectTransform newBlock = Instantiate(example.GetComponent<RectTransform>(), transform);
                Vector2 startPos = new Vector2(
                    (x * blockSize.x) - (gridFullSize.x / 2),
                    (y * blockSize.y) - (gridFullSize.y / 2)
                );
                newBlock.anchoredPosition = startPos;
                allBlocks.Add(newBlock);
            }
        }

        example.GetComponent<RectTransform>().gameObject.SetActive(false);
    }

    void Update() {
        Vector2 deplacement = direction.normalized * vitesse * Time.deltaTime;

        foreach (RectTransform block in allBlocks) {
            block.anchoredPosition += deplacement;
            Vector2 pos = block.anchoredPosition;
            if (pos.x > gridFullSize.x / 2) pos.x -= gridFullSize.x;
            else if (pos.x < -gridFullSize.x / 2) pos.x += gridFullSize.x;

            if (pos.y > gridFullSize.y / 2) pos.y -= gridFullSize.y;
            else if (pos.y < -gridFullSize.y / 2) pos.y += gridFullSize.y;

            block.anchoredPosition = pos;
        }
    }
    void CreateBlock(Vector2 pos){
        GameObject TV = Instantiate(example, transform);
        RectTransform rt = TV.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        allBlocks.Add(rt);
    }
}