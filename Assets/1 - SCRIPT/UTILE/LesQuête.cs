using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LesQuête : MonoBehaviour{
    //mouvement :
    private int IDVisibleLaPlusBasse=0;//nombre de fois ou je suis décendu, se nombre pourait aller jusqua -5
    public GameObject cible;
    public GameObject cibleDeDépart;
    public UI_Déplacement monDéplacementScript;
    public RectTransform bloc;
    private int nombreMaximumDeArcticleAfficher = 4;
    //spacifique à se programme :
    private int sélection;
    private List<GameObject> espaceDeQuête = new List<GameObject>();

    public GameObject élémentQuêtePréfab;
    private LesQuêtesQuiExistent listeDeQuête;
    public GameObject blocDeAffichage;
    void Start(){
        listeDeQuête = NewDataCenter.Instance.transform.Find("DICTIONNAIRE").GetComponent<LesQuêtesQuiExistent>();
        for (int i = 0; i < listeDeQuête.quête.Count-1; i++){
            GameObject TV = Instantiate(élémentQuêtePréfab);
            TV.transform.parent = bloc;
            TV.transform.localScale = new Vector3(1,1,1);
            espaceDeQuête.Add(TV);
            TV.transform.Find("nom").GetComponent<Text>().text = listeDeQuête.quête[i].nomDeLaQuête;
            if (listeDeQuête.quête[i].catégorie.ToString() == "Quête"){
                TV.transform.Find("Image").GetComponent<Image>().sprite = listeDeQuête.imageDesCatégorie[0];
            } else if (listeDeQuête.quête[i].catégorie.ToString() == "Service"){
                TV.transform.Find("Image").GetComponent<Image>().sprite = listeDeQuête.imageDesCatégorie[1];
            } else if (listeDeQuête.quête[i].catégorie.ToString() == "Histoire"){
                TV.transform.Find("Image").GetComponent<Image>().sprite = listeDeQuête.imageDesCatégorie[2];
            } else if (listeDeQuête.quête[i].catégorie.ToString() == "Clef"){
                TV.transform.Find("Image").GetComponent<Image>().sprite = listeDeQuête.imageDesCatégorie[3];
            }
            if (NewDataCenter.Instance.Lire(listeDeQuête.quête[i].nomDeLaQuête)=="1"){
                TV.transform.Find("Terminé").gameObject.SetActive(true);
            }
        }
        Choisir();
    }

    void Update(){
        if (Input.GetButtonDown("up")){
            espaceDeQuête[sélection].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            sélection--;
            Choisir();
        }
        else if (Input.GetButtonDown("down")){
            espaceDeQuête[sélection].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            sélection++;
            Choisir();
        }
    }

    void Choisir(){
        if (sélection<0){
            sélection = espaceDeQuête.Count-1;
        }else if (sélection >= espaceDeQuête.Count){
            sélection = 0;
        }
        espaceDeQuête[sélection].GetComponent<Image>().color = new Color(0.986f, 1f, 0.617f, 1f);
        blocDeAffichage.transform.Find("Nom").GetComponent<Text>().text = listeDeQuête.quête[sélection].nomDeLaQuête;
        blocDeAffichage.transform.Find("Description").GetComponent<Text>().text = listeDeQuête.quête[sélection].description;
        if (NewDataCenter.Instance.Lire(listeDeQuête.quête[sélection].nomDeLaQuête)=="1"){
            blocDeAffichage.transform.Find("Réusie").gameObject.SetActive(true);
        }else{
            blocDeAffichage.transform.Find("Réusie").gameObject.SetActive(false);
        }
        //mouvement :
        int temps = IDVisibleLaPlusBasse;
        IDVisibleLaPlusBasse = sélection-nombreMaximumDeArcticleAfficher;
        if (IDVisibleLaPlusBasse<0){IDVisibleLaPlusBasse=0;}
        if (IDVisibleLaPlusBasse>espaceDeQuête.Count-1){IDVisibleLaPlusBasse=espaceDeQuête.Count-nombreMaximumDeArcticleAfficher-3;}
        if (temps != IDVisibleLaPlusBasse){
            mouvementDesArcticle();
        }
    }
    
    void mouvementDesArcticle(){
        int nombreMaximumDeArcticleAfficher = 4;
        float décalageVersLeBas = espaceDeQuête[0].transform.position.y - espaceDeQuête[1].transform.position.y;
        float positionDeDépart = cibleDeDépart.transform.position.y;
        cible.transform.position = new Vector2(bloc.transform.position.x, (IDVisibleLaPlusBasse*décalageVersLeBas)+positionDeDépart);
        monDéplacementScript.STOP = false;
    }
}
