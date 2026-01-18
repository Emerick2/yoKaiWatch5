using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class test : MonoBehaviour{
    public Image image;
    public Sprite monSprite;
    public RectTransform pos;
    public TextMeshProUGUI texte;
    public Vector3 position;

    void Start(){
        image.sprite = monSprite;
        texte.text = "ee";
        image.rectTransform.position = position;

    }
}
