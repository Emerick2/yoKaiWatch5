using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YKW.Core.YoKai.Class;
using YKW.Core.BingoKai.Data;

public class BingoKai : MonoBehaviour{
    public BingoKaiCoinData maPièce;
    public List<GameObject> listeBoulle = new List<GameObject>();
    private bool autorisationDeContinuer1 = true;
    private bool autorisationDeContinuer2 = true;

    [Range(0,100)] private int probabilitéDeBoulle0 = 1;//noire
    [Range(0,100)] private int probabilitéDeBoulle1 = 10;//jaune
    [Range(0,100)] private int probabilitéDeBoulle2 = 30;//rouge
    [Range(0,100)] private int probabilitéDeBoulle3 = 60;//bleu
    [Range(0,100)] private int probabilitéDeBoulle4 = 100;//gris
    [Range(0,100)] private int probabilitéDeTombéSurUnYoKai = 60;
    private int numéroDeLaBoule=0;
    private string cadeauObjet="";
    private int cadeauYoKai=0;
    private bool seCadeauEstUnObjet=false;
    public SpriteRenderer espacePourAfficherImageDeObjetCadeau;
    public Transform positionDeUnModèle3D;
    private GameObject TV;
    private Sprite spriteVide;
    public GameObject boutonE;
    public GameObject boutonB;
    private bool possibilitéDeAppuyerSurB;
    public ChoisirSaPièceDeBingoKai scripteDuChoieDeLaPièce;
    void Start(){
        spriteVide = espacePourAfficherImageDeObjetCadeau.sprite;
    }

    void Update(){
        if (Input.GetButtonDown("A")){
            BoutonE();
        }
        if (Input.GetButton("B")){
            BoutonB();
        }
    }
    public void BoutonE(){
        if (autorisationDeContinuer1){
            //scripteDuChoieDeLaPièce.PièceSélectionné();//je revindrais ici plus tard
            boutonB.SetActive(false);
            boutonE.SetActive(false);
            possibilitéDeAppuyerSurB = false;
            autorisationDeContinuer1 = false;
            ChoisirUnCadeau();
            listeBoulle[numéroDeLaBoule].GetComponent<Animator>().SetTrigger("entrée de la boulle");
            StartCoroutine(SortieDeUneBoulle(1));
        }else if (autorisationDeContinuer2){
            autorisationDeContinuer2 = false;
            StartCoroutine(SortieDeUneBoulle(2));
        }
    }

    public void BoutonB(){
        FindObjectOfType<LivraisonTp>().InverserLaDirection();
    }
    void LaBoulleCesOuverte(){
        if (seCadeauEstUnObjet){
            LesObjets obj = lesObjetsQuiExistes.Instance.TrouverUnObjetAvecSonNom(cadeauObjet);
            if (obj != null){
                espacePourAfficherImageDeObjetCadeau.sprite = obj.image;
                NewDataCenter.Instance.SauvegarderINT(cadeauObjet,1);
            }
        }else{
            TV = medaliumDictionnaire._instance.InstantiateYoKai(cadeauYoKai.ToString());
            TV.transform.position = positionDeUnModèle3D.position;

            MesYoKai.Instance.AjouterUnYoKai(cadeauYoKai, 1, "", medaliumDictionnaire._instance.ServantYoKai(cadeauYoKai).nom, 0);
        }
        autorisationDeContinuer2 = true;
    }
    IEnumerator SortieDeUneBoulle(int passage){
        if (passage==1){
            yield return new WaitForSeconds(1.7f);
            LaBoulleCesOuverte();
        }else if (passage==2){
            listeBoulle[numéroDeLaBoule].GetComponent<Animator>().SetTrigger("sortie de la boulle");
            espacePourAfficherImageDeObjetCadeau.sprite = spriteVide;
            if (TV != null){
                Destroy(TV);
            }

            //yield return new WaitForSeconds(1f);
            yield return null;
            autorisationDeContinuer1 = true;
            boutonB.SetActive(true);
            possibilitéDeAppuyerSurB = true;
        }
        boutonE.SetActive(true);
    }
    public void ChoisirUnCadeau(){
        BingoKaiPrizeData leLotsDePrix;

        int random = Random.Range(0,100);
        if (random <= probabilitéDeBoulle0){ numéroDeLaBoule=0;leLotsDePrix=maPièce.firstPrize;}
        else if (random <= probabilitéDeBoulle1){ numéroDeLaBoule=1;leLotsDePrix=maPièce.firstPrize;}
        else if (random <= probabilitéDeBoulle2){ numéroDeLaBoule=2;leLotsDePrix=maPièce.secondPrize;}
        else if (random <= probabilitéDeBoulle3){ numéroDeLaBoule=3;leLotsDePrix=maPièce.thirdPrize;}
        else{ numéroDeLaBoule=4;leLotsDePrix=maPièce.defaultPrize;}

        if (Random.Range(0,100) <= probabilitéDeTombéSurUnYoKai) seCadeauEstUnObjet = false;
        else seCadeauEstUnObjet=true;

        if (seCadeauEstUnObjet){
            cadeauObjet = leLotsDePrix.itemNames[Random.Range(0,leLotsDePrix.itemNames.Count)];
        }else{
            cadeauYoKai = leLotsDePrix.yokaiIds[Random.Range(0,leLotsDePrix.yokaiIds.Count)];
        }
    }
}
//entrée de la boulle
//sortie de la boulle

/*
objectif :
0-appuyer sur E
1-choisire un cadeau entre les 4 possibilité.
2-Regarder si je peut transformer ma boulle en boulle noire (1%).
3-faire sortire la boulle en question
4-
si ces un objet :
    -mettre l'immage
    -enregistrer l'objet
    -ajouter un texte
    -attendre l'appuie sur la touche 'E'pour fermer se panel.
si ces un yo-kai :
    -afficher le bon modèle 3D
    -enregistreer le yo-kai
    -ajouter un texte
    -attendre l'appuie sur la touche 'E'pour fermer se panel.
5-jouer l'annimation de la boulle qui disparait
6-redonner la possibilité d'appuyer sur la touche E.

Par la suite il sera possible de choissir sa pièce de bingo-kai, chaque pièce à différente probabilité qui seront écrit dans un fichier csv.






*/
