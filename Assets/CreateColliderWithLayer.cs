using UnityEngine;
using System.Collections.Generic;

public class CreateColliderWithLayer : MonoBehaviour
{
    public float hauteurMur = 10f; 

    void Start()
    {
        CreateColliderWithLayerScript();
    }

    public void CreateColliderWithLayerScript()
    {
        // 1. Trouver tous les objets avec le tag "ZonneMur"
        GameObject[] murs = GameObject.FindGameObjectsWithTag("ZonneMur");
        
        foreach (GameObject mur in murs)
        {
            SpriteRenderer sr = mur.GetComponent<SpriteRenderer>();
            
            if (sr != null)
            {
                // 2. Ajouter ou récupérer le BoxCollider
                BoxCollider bc = mur.GetComponent<BoxCollider>();
                if (bc == null) bc = mur.AddComponent<BoxCollider>();

                // 3. Calcul de la taille du collider
                // sr.size donne la taille du sprite sans l'échelle. 
                // Pour un sprite à plat (90° sur X) :
                // x = Largeur du sprite
                // y = Hauteur du mur (on remplace la dimension 'y' du sprite)
                // z = Hauteur du sprite (ce qui est sa "profondeur" au sol)
                
                Vector2 spriteSize = sr.size;
                
                // On définit la taille du BoxCollider
                // Attention : On divise la hauteur par le scale Y de l'objet 
                // pour que la hauteur finale soit exactement 10 unités mondiales.
                bc.size = new Vector3(spriteSize.x, spriteSize.y, hauteurMur / mur.transform.localScale.y);

                // 4. Ajustement du centre
                // On décale le centre sur l'axe Z local (qui pointe vers le haut à cause de la rotation 90°)
                // pour que le mur "sorte" du sol vers le haut.
                bc.center = new Vector3(0, 0, (hauteurMur / 2f) / mur.transform.localScale.y);
            }
        }
    }
}