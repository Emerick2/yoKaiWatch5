using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YKW.Core.Item.Class;
using UnityEngine.UI;
using TMPro;
using YKW.Core.Events;
using EventBus = YKW.Core.Events.EventBus;

public class SacCombats : MonoBehaviour{
    public GameObject blocAppSac;
    private Item itemScript;
    private GameObject exempleBloc;
    public GameObject conteneur;
    private List<GameObject> listeDeLaNouriturePlacé = new List<GameObject>();
    private List<int> indicesNourriture = new List<int>();
    public Vector2 position;
    private int colonnes = 3;
    private int positionVisibleDansLeBloc;
    private bool utiliser = false;
    public TextMeshProUGUI texte;

    void Start() {
        if (blocAppSac != null) blocAppSac.SetActive(false);
        itemScript = Item.Instance;
    }

    public void Initialiser(){
        itemScript = Item.Instance;
        if (blocAppSac == null || itemScript == null)
        {
            Debug.LogWarning("Combat bag unavailable: a UI reference or the inventory is missing.");
            GetComponent<CombatActionController>()?.AbortBagSelection();
            return;
        }
        position = Vector2.zero;
        utiliser = false;
        blocAppSac.SetActive(true);
        EventBus.Publish(new CombatBagVisibilityEvent(true));
        StartCoroutine(InitialisationCoroutine());
    }

    IEnumerator InitialisationCoroutine(){
        float elapsed = 0f;
        while (itemScript != null && !itemScript.chargementTerminé){
            if (elapsed >= 30f)
            {
                Debug.LogError("Combat bag: inventory loading did not finish within 30 seconds.");
                EventBus.Publish(new CombatMessageEvent("L'inventaire n'a pas pu être chargé."));
                GetComponent<CombatActionController>()?.AbortBagSelection();
                yield break;
            }
            yield return new WaitForSecondsRealtime(0.2f);
            elapsed += 0.2f;
        }
        if (itemScript == null || conteneur == null || !PlacerLesObjets() || listeDeLaNouriturePlacé.Count == 0)
        {
            EventBus.Publish(new CombatMessageEvent("Aucun aliment disponible."));
            GetComponent<CombatActionController>()?.AbortBagSelection();
            yield break;
        }
        PlacerLaBonneLargeur();
        DefilemantAutomatique(0);
        ChangerDePosition(0,0);
        utiliser = true;
    }

    public void Arrêter(){
        utiliser = false;
        CombatBagUI bagUi = blocAppSac != null ? blocAppSac.GetComponent<CombatBagUI>() : null;
        if (bagUi != null) EventBus.Publish(new CombatBagVisibilityEvent(false));
        else
        {
            EventBus.Publish(new CombatBagVisibilityEvent(false));
            if (blocAppSac != null) blocAppSac.SetActive(false);
        }
    }

    void PlacerLaBonneLargeur() {
        if (conteneur == null) return;

        GridLayoutGroup grid = conteneur.GetComponent<GridLayoutGroup>();
        RectTransform rt = conteneur.GetComponent<RectTransform>();
        if (grid == null || rt == null) return;

        int nbElements = listeDeLaNouriturePlacé.Count;
        int nbColonnes = Mathf.CeilToInt((float)nbElements / 3f);

        float nouvelleLargeur = grid.padding.left + grid.padding.right;
        
        if (nbColonnes > 0) {
            nouvelleLargeur += (nbColonnes * grid.cellSize.x);
            nouvelleLargeur += (nbColonnes - 1) * grid.spacing.x;
        }

        rt.sizeDelta = new Vector2(nouvelleLargeur, rt.sizeDelta.y);
    }

    void DefilemantAutomatique(int direction){
        if (conteneur == null || listeDeLaNouriturePlacé.Count == 0) return;
        int cible = ConnaitreNombrePosition(position, true, false);
        bool forcer = false;
        // if (cible == 1){
        //     forcer = true;
        //     positionVisibleDansLeBloc = cible;
        // } else if (cible == listeDeLaNouriturePlacé.Count/colonnes){
        //     forcer = true;
        //     positionVisibleDansLeBloc = listeDeLaNouriturePlacé.Count/colonnes;
        // }

        if ((positionVisibleDansLeBloc < 2 && direction < 0) || (positionVisibleDansLeBloc > 4 && direction > 0)||forcer){
            GridLayoutGroup grid = conteneur.GetComponent<GridLayoutGroup>();
            RectTransform rt = conteneur.GetComponent<RectTransform>();
            if (grid == null || rt == null) return;

            float width = 0;
            // width += grid.padding.left;
            width += (grid.cellSize.x + grid.spacing.x)*(cible-positionVisibleDansLeBloc);
            width *=-1;

            Vector2 anchored = rt.anchoredPosition;
            anchored.x = width;
            rt.anchoredPosition = anchored;
        } else {
            positionVisibleDansLeBloc += direction;
        }
    }

    bool PlacerLesObjets(){
        if (conteneur == null || itemScript == null || itemScript.listeDesNouritures == null) return false;
        foreach (Transform child in conteneur.transform){
            if (child.name == "exemple" || child.name == "Item"){
                child.gameObject.SetActive(false);
                exempleBloc = child.gameObject;
            } else {
                Destroy(child.gameObject);
            }
        }
        listeDeLaNouriturePlacé.Clear();
        indicesNourriture.Clear();
        if (exempleBloc == null || NewDataCenter.Instance == null)
        {
            Debug.LogError("Combat bag: item template or save system is missing.");
            return false;
        }
        for (int index = 0; index < itemScript.listeDesNouritures.Count; index++){
            Nouriture item = itemScript.listeDesNouritures[index];
            if (item == null) continue;
            int count;
            if (!int.TryParse(NewDataCenter.Instance.Lire(item.nom), out count) || count <= 0) continue;
            GameObject TV = Instantiate(exempleBloc);
            if (TV == null) continue;
            Transform imageTransform = TV.transform.Find("ItemImage");
            Image itemImage = imageTransform != null ? imageTransform.GetComponent<Image>() : TV.GetComponent<Image>();
            if (itemImage != null) itemImage.sprite = item.image;
            Transform quantityTransform = TV.transform.Find("ItemImage/nombre");
            if (quantityTransform == null) quantityTransform = TV.transform.Find("nombre");
            if (quantityTransform != null)
            {
                EcrireUneQuantitéAvecDesImages quantity = quantityTransform.GetComponent<EcrireUneQuantitéAvecDesImages>();
                if (quantity != null)
                {
                    quantity.EcrireLeNombre(count);
                    quantity.EcrireSi1 = false;
                }
            }
            Transform held = TV.transform.Find("ItemImage/tient");
            if (held == null) held = TV.transform.Find("tient");
            if (held != null) held.gameObject.SetActive(false);
            TV.transform.SetParent(conteneur.transform, false);
            TV.transform.localScale = new Vector3(1,1,1);
            TV.transform.name = item.nom;
            listeDeLaNouriturePlacé.Add(TV);
            TV.SetActive(true);
            indicesNourriture.Add(index);
        }
        return true;
    }

    void Update() {
        if (utiliser) {
            if (listeDeLaNouriturePlacé.Count == 0) return;
            if (Input.GetButtonDown("left")){
                ChangerDePosition(-1,0);
            } else if (Input.GetButtonDown("right")){
                ChangerDePosition(1,0);
            } else if (Input.GetButtonDown("up")){
                ChangerDePosition(0,-1);
            } else if (Input.GetButtonDown("down")){
                ChangerDePosition(0,1);
            } else if (Input.GetButtonDown("A")){
                Input.ResetInputAxes();
                int selected = ConnaitreNombrePosition(position,false,false);
                if (selected >= 0 && selected < indicesNourriture.Count)
                    EventBus.Publish(new CombatFoodSelectedEvent(indicesNourriture[selected]));
            }
        }
    }

    int ConnaitreNombrePosition(Vector2 laPosition, bool sansY, bool sansX){
        if (listeDeLaNouriturePlacé.Count == 0) return -1;
        int row = sansY ? 0 : Mathf.Max(0, Mathf.RoundToInt(laPosition.x));
        int column = sansX ? 0 : Mathf.Max(0, Mathf.RoundToInt(laPosition.y));
        int index = row * colonnes + column;
        return Mathf.Clamp(index, 0, listeDeLaNouriturePlacé.Count - 1);
    }

    GameObject AvoirElementPosition(Vector2 laPosition){
        int p = ConnaitreNombrePosition(laPosition, false, false);
        if (p < 0 || p >= listeDeLaNouriturePlacé.Count){
            Debug.Log($"Invalid food index {p}; the displayed list contains {listeDeLaNouriturePlacé.Count} entries.");
            return null;
        }
        return listeDeLaNouriturePlacé[p];
    }

    void ChangerDePosition(int x, int y){
        if (listeDeLaNouriturePlacé.Count == 0) return;
        int nombreDeLignes = Mathf.CeilToInt((float)listeDeLaNouriturePlacé.Count / colonnes);
        if (nombreDeLignes <= 0 || colonnes <= 0) return;
        GameObject obj = AvoirElementPosition(position);
        if (obj != null) obj.transform.Find("tient")?.gameObject.SetActive(false);
        position.x=((position.x+x%nombreDeLignes)+nombreDeLignes)%nombreDeLignes;
        position.y=((position.y+y%colonnes)+colonnes)%colonnes;
        if (ConnaitreNombrePosition(position, false, false) != (int)position.x * colonnes + (int)position.y)
            position.y = Mathf.Max(0, listeDeLaNouriturePlacé.Count - 1 - (int)position.x * colonnes);
        obj = AvoirElementPosition(position);
        if (obj != null) obj.transform.Find("tient")?.gameObject.SetActive(true);
        DefilemantAutomatique(x);
        if (obj != null && texte != null) texte.text = "Utiliser "+obj.name+" ?";
    }
}
