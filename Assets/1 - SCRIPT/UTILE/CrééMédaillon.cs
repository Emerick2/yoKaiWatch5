using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrééMédaillon : MonoBehaviour{
    public Image espaceIcone;
    public Image espaceRare;
    public List<Image> espaceCouleurTribue;
    public Image cadreMédaillon;

    void Start(){
        StartCoroutine(AvantCréé(584));
    }

    IEnumerator AvantCréé(int ID){
        if (medaliumDictionnaire._instance != null) {
            while (!medaliumDictionnaire._instance.chargementFini){
                yield return new WaitForSeconds(0.1f);
            }
            Créé(ID);
        }
    }

    public void Créé(int id){
        if (id >= 0 && id < medaliumDictionnaire._instance.dictionnaire.Count){
            MedaliumNonPersonel yokai = medaliumDictionnaire._instance.dictionnaire[id];
            espaceIcone.sprite = yokai.faceRondImage;
            espaceRare.enabled = yokai.rare;

            Color maCouleur;
            switch (yokai.tribue){
                case (Tribue.Vaillant):
                    ColorUtility.TryParseHtmlString("#FF4444", out maCouleur);
                    break;
                case (Tribue.Mystérieu):
                    ColorUtility.TryParseHtmlString("#FFFF66", out maCouleur);
                    break;
                case (Tribue.Costaud):
                    ColorUtility.TryParseHtmlString("#FF862F", out maCouleur);
                    break;
                case (Tribue.Mignon):
                    ColorUtility.TryParseHtmlString("#ff93e6ff", out maCouleur);
                    break;
                case (Tribue.Bienveillant):
                    ColorUtility.TryParseHtmlString("#95ff49ff", out maCouleur);
                    break;
                case (Tribue.Sombre):
                    ColorUtility.TryParseHtmlString("#5144ffff", out maCouleur);
                    break;
                case (Tribue.Sinistre):
                    ColorUtility.TryParseHtmlString("#8ba6ffff", out maCouleur);
                    break;
                case (Tribue.Insaisissable):
                    ColorUtility.TryParseHtmlString("#ceffcbff", out maCouleur);
                    break;
                case (Tribue.Perfide):
                    ColorUtility.TryParseHtmlString("#7b7b7bff", out maCouleur);
                    break;
                case (Tribue.Enma):
                    ColorUtility.TryParseHtmlString("#ec526cff", out maCouleur);
                    break;
                case (Tribue.Wandroïd):
                    ColorUtility.TryParseHtmlString("#129c20ff", out maCouleur);
                    break;
                default:
                    maCouleur = Color.white;
                    break;
            }
            foreach (Image item in espaceCouleurTribue){
                item.color = maCouleur;
            }
        }
    }
}
