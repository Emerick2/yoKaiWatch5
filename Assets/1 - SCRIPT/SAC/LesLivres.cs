using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LesLivres : MonoBehaviour{
    public List<UnLivre> mesLivre;
    public List<Image> emplacementPourLesPages;
    private int positionActuelle = 0;
    public ScrollRect monScrollRect;
    public void ChangerDeLivre(int n){
        if (n > 0){
            positionActuelle=(positionActuelle+1)%emplacementPourLesPages.Count;
        } else {
            positionActuelle--;
            if (positionActuelle < 0){
                positionActuelle = emplacementPourLesPages.Count-1;
            }
        }
        if (mesLivre.Count > positionActuelle && emplacementPourLesPages.Count == mesLivre[positionActuelle].lesPages.Count){
            for (int i = 0; i < emplacementPourLesPages.Count; i++){
                emplacementPourLesPages[i].sprite = mesLivre[positionActuelle].lesPages[i];
            }
        }
        if (monScrollRect != null) {
            monScrollRect.verticalNormalizedPosition = 1f; // 1 = haut, 0 = bas
        }
    }
}

