using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
Se script n'a pas entièrement été créé par moi, je l'est télécharger et modifier pour l'utiliser dans se projet.
*/

public class DessineJeuAmultime : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public List<Sprite> lesDessinQuiPeuventArrivé;
    [Header("Configuration du Jeu")]
    [Tooltip("Pourcentage minimum de la zone visible à couvrir pour réussir.")]
    [Range(0.0f, 100.0f)] public float pourcentageDeLaZonneAParcourire = 80.0f; 

    [Tooltip("Marge d'erreur pour le débordement. 0 = pas de débordement autorisé.")] 
    [Range(0.0f, 100.0f)] public float margeErreurDeDébordement = 10.0f; 

    [Tooltip("Rayon du 'pinceau' en pixels (taille du marqueur de dessin).")]
    public float rayonDuPinceau = 5.0f; 

    [Tooltip("Couleur du dessin (ex: Rouge).")]
    public Color couleurDuPinceau = Color.red;

    [Tooltip("Le GameObject Image qui sera le marqueur de dessin.")]
    public GameObject imageADécalquer; 

    // --- Variables Internes ---
    private Image imageASuivre;
    private Texture2D texture2DImage;
    private RectTransform rectTransform;

    // Représente la zone dessinée (pixels rouges)
    private Texture2D textureDessinerParLeJoeur; 
    // Image affichant la zone dessinée (qui sera enfant de l'image cible)
    private Image imageDessinJoueurRésultat; 

    private int totalVisiblePixels = 0;
    private int coveredVisiblePixels = 0;
    private int overshootPixels = 0;
    private bool isDrawing = false;

    public int numéroToursMAX = 3;
    public int numéroTours;
    [HideInInspector] public FonctionnementJeuAmultime fonctionnement;
    private bool victoireDéjàAppeler;

    public void Initialisation(FonctionnementJeuAmultime f){
        fonctionnement = f;
        numéroTours = 0;
        victoireDéjàAppeler = false;
        InitialisationZoneDessin(true);
    }

    private void InitialisationZoneDessin(bool nouvelleImage){
        if (nouvelleImage){
            List<Sprite> laListe = lesDessinQuiPeuventArrivé;
            Sprite spriteActuel = GetComponent<Image>().sprite;
            laListe.RemoveAll(spriteDeListe => spriteDeListe == spriteActuel);
            Debug.Log(laListe.Count);
            
            GetComponent<Image>().sprite = laListe[Random.Range(0,laListe.Count)];
            imageASuivre = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            // IMPORTANT : Le Sprite doit être importé avec "Read/Write Enabled" coché
            if (imageASuivre.sprite == null || imageASuivre.sprite.texture == null){
                Debug.LogError("L'Image cible n'a pas de Sprite ou de Texture. Assurez-vous que l'option 'Read/Write Enabled' est cochée dans les paramètres d'import de la texture.");
                enabled = false;
                return;
            }

            texture2DImage = imageASuivre.sprite.texture;
        }    
        // 1. Compter les pixels visibles de la forme (alpha > 0)
        Color[] pixels = texture2DImage.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0.1f) // Vérifie si le pixel n'est pas complètement transparent
            {
                totalVisiblePixels++;
            }
        }
        
        if (totalVisiblePixels == 0)
        {
            Debug.LogError("L'image n'a pas de pixels visibles (alpha > 0). Vérifiez votre image source.");
            enabled = false;
            return;
        }
        
        // 2. Créer une nouvelle Texture2D transparente pour stocker le dessin
        textureDessinerParLeJoeur = new Texture2D(texture2DImage.width, texture2DImage.height, TextureFormat.RGBA32, false);
        Color[] clearColors = new Color[textureDessinerParLeJoeur.width * textureDessinerParLeJoeur.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = new Color(0, 0, 0, 0); // Totalement transparent
        }
        textureDessinerParLeJoeur.SetPixels(clearColors);
        textureDessinerParLeJoeur.Apply();

        // 3. Créer le GameObject Image pour afficher la zone dessinée
        GameObject drawingGO = new GameObject("DrawingLayer");
        drawingGO.transform.SetParent(transform, false); // Enfant de l'objet principal, garde la même position locale
        imageDessinJoueurRésultat = drawingGO.AddComponent<Image>();
        imageDessinJoueurRésultat.rectTransform.anchorMin = new Vector2(0, 0);
        imageDessinJoueurRésultat.rectTransform.anchorMax = new Vector2(1, 1);
        imageDessinJoueurRésultat.rectTransform.sizeDelta = Vector2.zero; // S'étire pour couvrir l'objet parent
        
        // Assigner la texture au Sprite
        Sprite drawingSprite = Sprite.Create(textureDessinerParLeJoeur, new Rect(0, 0, textureDessinerParLeJoeur.width, textureDessinerParLeJoeur.height), Vector2.one * 0.5f, imageASuivre.sprite.pixelsPerUnit);
        imageDessinJoueurRésultat.sprite = drawingSprite;
        imageDessinJoueurRésultat.color = couleurDuPinceau; // La couleur s'applique à l'alpha de la texture
    }

    // --- Gestion du Dessin ---

    // Lorsque l'utilisateur clique sur l'image
    public void OnPointerDown(PointerEventData eventData){
        isDrawing = true;
        Draw(eventData.position, eventData);
    }

    // Lorsque l'utilisateur glisse la souris sur l'image
    public void OnDrag(PointerEventData eventData){
        if (isDrawing){
            Draw(eventData.position, eventData);
        }
    }
    
    // Lorsque l'utilisateur relâche le clic
    public void OnPointerUp(PointerEventData eventData){
        isDrawing = false;
        CheckCompletion();
    }

    // La logique principale de dessin
    private void Draw(Vector2 screenPosition, PointerEventData eventData){
        // Convertir la position de l'écran en position locale à l'intérieur du RectTransform
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, eventData.pressEventCamera, out localPoint)){
            // Normaliser la position locale (de -width/2 à +width/2) à (0 à 1)
            Vector2 normalizedPoint = new Vector2(
                (localPoint.x / rectTransform.rect.width) + 0.5f,
                (localPoint.y / rectTransform.rect.height) + 0.5f
            );

            // Convertir à l'échelle des pixels de la texture (0 à texture.width)
            int pixelX = (int)(normalizedPoint.x * texture2DImage.width);
            int pixelY = (int)(normalizedPoint.y * texture2DImage.height);

            // Appliquer le "pinceau" (un cercle de pixels)
            int radiusInt = Mathf.CeilToInt(rayonDuPinceau * (texture2DImage.width / rectTransform.rect.width)); // Rayon à l'échelle des pixels
            
            for (int y = -radiusInt; y <= radiusInt; y++){
                for (int x = -radiusInt; x <= radiusInt; x++){
                    if (x * x + y * y <= radiusInt * radiusInt){ // Si dans le cercle
                        int pX = pixelX + x;
                        int pY = pixelY + y;

                        // Vérifier les limites de la texture
                        if (pX >= 0 && pX < texture2DImage.width && pY >= 0 && pY < texture2DImage.height){
                            MarkPixel(pX, pY);
                        }
                    }
                }
            }
            
            textureDessinerParLeJoeur.Apply(); // Mettre à jour la texture affichée
        }
    }

    // Marque un pixel et met à jour les compteurs
    private void MarkPixel(int x, int y){
        // Vérifie si le pixel n'a pas déjà été marqué
        if (textureDessinerParLeJoeur.GetPixel(x, y).a < 0.1f) {
            // Marque le pixel dans la texture de dessin
            textureDessinerParLeJoeur.SetPixel(x, y, Color.white); // On utilise Color.white, l'alpha est modulé par drawColor de drawingImage

            // 1. Vérifie si le pixel est dans la zone visible de la forme
            if (texture2DImage.GetPixel(x, y).a > 0.1f){
                coveredVisiblePixels++;
            }
            // 2. Vérifie si le pixel est en débordement (hors de la zone visible)
            else{
                overshootPixels++;
            }
        }
    }


    public void CheckCompletion(){
        if (totalVisiblePixels == 0) return;
        float completionPercentage = ((float)coveredVisiblePixels / totalVisiblePixels) * 100f;
        float overshootPercentage = ((float)overshootPixels / totalVisiblePixels) * 100f; 

        Debug.Log($"Progression: {completionPercentage:F2}% (Cible: {pourcentageDeLaZonneAParcourire}%)");
        Debug.Log($"Débordement: {overshootPercentage:F2}% (Max Autorisé: {margeErreurDeDébordement}%)");

        if (overshootPercentage > margeErreurDeDébordement){
            // Debug.Log("<color=red>Jeu PERDU ! Trop de débordement.</color>");
            isDrawing = false;
            Effacer(false);
        } else if (completionPercentage >= pourcentageDeLaZonneAParcourire){
            Debug.Log("<color=green>Jeu RÉUSSI ! Forme complétée !</color>");
            isDrawing = false;
            numéroTours++;
            fonctionnement.NouvelleValeurSliderImage((float)numéroTours/numéroToursMAX);
            if (numéroTours >= numéroToursMAX){
                if (!victoireDéjàAppeler){
                    fonctionnement.VictoireMiniJeu();
                    victoireDéjàAppeler = true;
                }
            } else {
                Effacer(true);
            }
        } else {
            Effacer(false);
        }
    }

    public void Effacer(bool nouvelleImage){
       coveredVisiblePixels = 0;
        overshootPixels = 0;
            if (imageDessinJoueurRésultat != null){
            Destroy(imageDessinJoueurRésultat.gameObject);
        }
        InitialisationZoneDessin(nouvelleImage);
        Debug.Log("<color=yellow>Le dessin a été effacé. Bonne chance !</color>");
    }
}