using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dedur : MonoBehaviour
{
    public Sprite monSprite;
    public Sprite monSprite2;
    private GameObject objet;
    private int position = 0;

    void Start()
    {
        objet = new GameObject("le nom l'élement");
        objet.AddComponent<SpriteRenderer>();
        objet.GetComponent<SpriteRenderer>().sprite = monSprite;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)){
            if (objet != null && objet.GetComponent<SpriteRenderer>() != null){
                position=(position+1)%2;
                if (position == 0){
                    objet.GetComponent<SpriteRenderer>().sprite = monSprite;
                } else {
                    objet.GetComponent<SpriteRenderer>().sprite = monSprite2;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B)){
            position -= 1;
            if (position < 0){
                position = 1;
            }
        }
    }

    // Modulo(10,3);
    // 10%3;
    int Modulo(int valeurBase, int valeurModulo){
        if (valeurModulo > 0){
            while (valeurBase >= valeurModulo) {
                valeurBase -= valeurModulo;
            }
        }
        return valeurBase;
    }
}
