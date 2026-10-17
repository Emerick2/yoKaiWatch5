using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StructureYoKaiCombat = YKW.Core.YoKai.Class.YoKaiCombat;
using YoKaiEspece = YKW.Core.YoKai.Class.YoKai;

public class Cible : MonoBehaviour{
    [HideInInspector] public CombatManager CombatManager;
    public List<GameObject> yoKaiGameObjects;
    public Sprite imageSélection;
    public Sprite imageNonSélection;
    public int cibleVisée;
    public SpriteRenderer imageEstUnAmi;
    public Sprite ouiIlEstUnAmi;
    public Sprite nonIlEstPasUnAmi;
    public TextMeshPro texteAlliment;

    public SpriteRenderer imageElement;

    public float vitesseCible;
    Vector2 directionHaut = new Vector2(0, 1);
    Vector2 directionBas = new Vector2(0, -1);
    Vector2 directionGauche = new Vector2(-1, 0);
    Vector2 directionDroit = new Vector2(1, 0);
    private Vector2 positionInitial = new Vector2(0, 0);

    public List<GameObject> pointDeCibleEnnemies = new List<GameObject>();

    void Start(){
        positionInitial = transform.position;
        Initialisation();
    }

    public void Initialisation(){
        if (imageElement != null) imageElement.gameObject.SetActive(false);
        if (imageEstUnAmi != null) imageEstUnAmi.gameObject.SetActive(false);
        if (texteAlliment != null) texteAlliment.gameObject.SetActive(false);
        transform.position = positionInitial;
    }

    void Update(){
        if (Input.GetButton("right")){
            transform.Translate(directionDroit * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("left")){
            transform.Translate(directionGauche * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("up")){
            transform.Translate(directionHaut * vitesseCible * Time.deltaTime);
        }else if (Input.GetButton("down")){
            transform.Translate(directionBas * vitesseCible * Time.deltaTime);
        }

    }

    void OnTriggerEnter(Collider other){
        if (CombatManager == null || yoKaiGameObjects == null) return;
        GameObject parent = EstValide(other.gameObject);
        if (parent != null){
            SpriteRenderer cursorSprite = GetComponent<SpriteRenderer>();
            if (cursorSprite != null) cursorSprite.sprite = imageSélection;
            int IDTrouvé = -1;
            for (int i = 0; i < CombatManager.listeYoKaiEnnemieSurLeTerrain.Count; i++){
                if (CombatManager.listeYoKaiEnnemieSurLeTerrain[i].gameObjectSurLaScène == parent){
                    IDTrouvé = i;
                    break;
                }
            }
            if (IDTrouvé < 0 || IDTrouvé >= CombatManager.listeYoKaiEnnemieSurLeTerrain.Count ||
                MesYoKai.Instance == null || MesYoKai.Instance.maListe == null ||
                medaliumDictionnaire._instance == null) return;
            StructureYoKaiCombat enemy = CombatManager.listeYoKaiEnnemieSurLeTerrain[IDTrouvé];
            if (enemy == null) return;
            bool estAmi = false;
            for (int i = 0; i < MesYoKai.Instance.maListe.Count; i++){
                if (MesYoKai.Instance.maListe[i] != null && enemy.ID == MesYoKai.Instance.maListe[i].ID){
                    estAmi = true;
                    break;
                }
            }
            if (imageEstUnAmi != null && estAmi){
                imageEstUnAmi.sprite = ouiIlEstUnAmi;
            } else if (imageEstUnAmi != null){
                imageEstUnAmi.sprite = nonIlEstPasUnAmi;
            }
            if (imageEstUnAmi != null) imageEstUnAmi.transform.localScale = Vector3.one;
            YoKaiEspece species = medaliumDictionnaire._instance.ServantYoKai(enemy.ID);
            if (species == null) return;
            if (texteAlliment != null) texteAlliment.text = species.platPréféré.ToString();

            if (imageElement != null)
            {
                imageElement.sprite = new ServantImage().ReadImage_Element(species.monElement);
                imageElement.transform.localScale = Vector3.one;
                imageElement.gameObject.SetActive(true);
            }
            if (imageEstUnAmi != null) imageEstUnAmi.gameObject.SetActive(true);
            if (texteAlliment != null) texteAlliment.gameObject.SetActive(true);
            // Debug.Log(IDTrouvé);
        }
    }
    void OnTriggerExit(Collider other){
        GameObject parent = EstValide(other.gameObject);
        if (parent != null){
            SpriteRenderer cursorSprite = GetComponent<SpriteRenderer>();
            if (cursorSprite != null) cursorSprite.sprite = imageNonSélection;
            if (imageElement != null) imageElement.gameObject.SetActive(false);
            if (imageEstUnAmi != null) imageEstUnAmi.gameObject.SetActive(false);
            if (texteAlliment != null) texteAlliment.gameObject.SetActive(false);
        }
    }

    private GameObject EstValide(GameObject gameObjectAAnalyser){
        if (yoKaiGameObjects != null && yoKaiGameObjects.Contains(gameObjectAAnalyser)){
            return gameObjectAAnalyser;
        }
        Transform parentTransform = gameObjectAAnalyser.transform.parent;
        
        while (parentTransform != null){
            if (yoKaiGameObjects != null && yoKaiGameObjects.Contains(parentTransform.gameObject)){
                return parentTransform.gameObject;
            }
            parentTransform = parentTransform.parent;
        }
        return null;
    }
}
