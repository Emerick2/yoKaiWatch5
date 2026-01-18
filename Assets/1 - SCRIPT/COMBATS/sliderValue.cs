using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderValue : MonoBehaviour
{
    public bool deGaucheADroite;
    public bool deHautEnBas;
    public RectTransform positionEnBas;
    public RectTransform positionEnHaut;
    public RectTransform monTaux;
    [Range(0, 100)] public int value;

    void Update(){
        if (value<=100){
            if (deGaucheADroite){
                float resultX = ((positionEnHaut.position.x - positionEnBas.position.x) * value / 100) + positionEnBas.position.x;
                Vector3 newPosition = new Vector3(resultX, monTaux.position.y, monTaux.position.z);
                monTaux.position = newPosition;
            }

            if (deHautEnBas){
                float resultY = ((positionEnHaut.position.y - positionEnBas.position.y) * value / 100) + positionEnBas.position.y;
                Vector3 newPosition = new Vector3(monTaux.position.x, resultY, monTaux.position.z);
                monTaux.position = newPosition;
            }
        }
    }
}
