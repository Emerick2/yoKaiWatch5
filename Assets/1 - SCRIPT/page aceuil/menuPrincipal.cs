using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuPrincipal : MonoBehaviour{
    bool jeSuisSurLeDéplacementMouvementPrincipal = true;
    public GameObject continuerNonSélectionner;
    public GameObject continuerSélectionner;
    public GameObject nouvellePartieNonSélectionner;
    public GameObject nouvellePartieSélectionner;
    public bool jeSuisSurContinuer = true;
    public RectTransform petitFlame;
    public RectTransform positionFlameContinuer;
    public RectTransform positionFlameNouvellePartie;
    private Vector2 direction;
    private RectTransform cible;
    private bool STOP;
    public int vitesse=1000;
    public string leJeu;
    public GameObject choisirLeNom;
    public GameObject menuPrincipalBloc;
    void Start(){
        if (NewDataCenter.Instance.Lire("nomDuJoueur","mesData.json") != "0"){
            cible = positionFlameContinuer;
        }else{NouvellePartie();}
    }

    IEnumerator ChangementScène(string scène) {        
        GameObject.FindWithTag("Fondu").GetComponent<Animator>().SetTrigger("faireLeFondu");
        yield return new WaitForSeconds(0.40f);
        SceneManager.LoadScene(scène);
    }
    void Update(){
        if (jeSuisSurLeDéplacementMouvementPrincipal){
            if (Input.GetButtonDown("right") || Input.GetButtonDown("left") || Input.GetButtonDown("up") || Input.GetButtonDown("down")){
                if (jeSuisSurContinuer){//je vais viser la nouvelle partie
                    jeSuisSurContinuer = false;
                    cible = positionFlameNouvellePartie;
                    STOP = false;
                    continuerNonSélectionner.SetActive(true);
                    continuerSélectionner.SetActive(false);
                    nouvellePartieNonSélectionner.SetActive(false);
                    nouvellePartieSélectionner.SetActive(true);
                }else if (!jeSuisSurContinuer){//je vais viser continuer
                    jeSuisSurContinuer = true;
                    cible = positionFlameContinuer;
                    STOP = false;
                    continuerNonSélectionner.SetActive(false);
                    continuerSélectionner.SetActive(true);
                    nouvellePartieNonSélectionner.SetActive(true);
                    nouvellePartieSélectionner.SetActive(false);
                }
            }
            if (Input.GetButtonDown("A")){
                if (jeSuisSurContinuer){
                    string p = NewDataCenter.Instance.Lire("scenne","position.json");
                    if (p != "0"){
                        leJeu = p;
                    }
                    GameObject newObject = new GameObject("livraison début de partie");
                    newObject.AddComponent<LivraisonDébutUnePartie>();
                    StartCoroutine(ChangementScène(leJeu));
                }else{
                    NouvellePartie();
                }
            }
            if (!STOP && cible != null){
                direction = (cible.anchoredPosition - petitFlame.anchoredPosition).normalized;
                petitFlame.anchoredPosition += direction * vitesse * Time.deltaTime;
                if (Vector2.Distance(petitFlame.anchoredPosition, cible.anchoredPosition) < 5f){
                    STOP = true;    
                }
            }
        }
    }
    void NouvellePartie(){
        choisirLeNom.SetActive(true);
        jeSuisSurLeDéplacementMouvementPrincipal = false;
        menuPrincipalBloc.SetActive(false);
        string filePath = Application.persistentDataPath + "/mesData.json";
        if (System.IO.File.Exists(filePath)){
            System.IO.File.Delete(filePath);
        }
        GameObject dataCenter = NewDataCenter.Instance.gameObject;
        dataCenter.SetActive(false);
        GameObject TV = Instantiate(dataCenter);
        Destroy(dataCenter);
        TV.SetActive(true);
    }
}
