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
        if (NewDataCenter.Instance.Lire("nomDuJoueur") != "0"){
            cible = positionFlameContinuer;
        }else{NouvellePartie();}
    }
    void Update(){
        if (jeSuisSurLeDéplacementMouvementPrincipal){
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)){
                if (jeSuisSurContinuer){//je vais viser la nouvelle partie
                    jeSuisSurContinuer = false;
                    cible = positionFlameNouvellePartie;
                    STOP = false;
                    continuerNonSélectionner.SetActive(true);
                    continuerSélectionner.SetActive(false);
                    nouvellePartieNonSélectionner.SetActive(false);
                    nouvellePartieSélectionner.SetActive(true);
                }
                else if (!jeSuisSurContinuer){//je vais viser continuer
                    jeSuisSurContinuer = true;
                    cible = positionFlameContinuer;
                    STOP = false;
                    continuerNonSélectionner.SetActive(false);
                    continuerSélectionner.SetActive(true);
                    nouvellePartieNonSélectionner.SetActive(true);
                    nouvellePartieSélectionner.SetActive(false);
                }
            }
            if (Input.GetKeyDown(KeyCode.E)){
                if (jeSuisSurContinuer){
                    SceneManager.LoadScene(leJeu);//plus tard il faudra mettre ici un déplassement vers la scène ou le joueur à sauvgarder
                }else{
                    NouvellePartie();
                }
            }
            if (!STOP){
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
