
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceLesApplicationsSac : MonoBehaviour
{
    [Header("Menu")]
    public écrireLesPetitesValeursDuMenu menu;

    [Header("Prefab / Modèle")]
    public GameObject blocOrigine;

    [Header("Applications")]
    public List<Sprite> listeImageApplication = new();
    public List<GameObject> lesLieux = new();

    [Header("Sélection")]
    public Sprite spriteVisé;
    public Sprite spriteNonVisé;

    [HideInInspector]
    public int cible;

    [SerializeField, Min(1)]
    private int nombreApplicationParEtage = 3;

    private void Start()
    {
        Initialisation();
    }

    private void Initialisation()
    {
        if (blocOrigine == null)
        {
            Debug.LogError(
                "[Applications] blocOrigine n'est pas assigné !",
                this
            );
            return;
        }

        lesLieux.Clear();

        if (blocOrigine.scene.IsValid())
        {
            blocOrigine.SetActive(false);
        }

        int index = 0;

        foreach (Sprite sprite in listeImageApplication)
        {
            if (sprite == null)
                continue;

            // Vérifier si l'application est autorisée
            if (!ApplicationDisponible(sprite))
                continue;

            // Générer directement sous le parent UI
            GameObject instance = Instantiate(
                blocOrigine,
                transform,
                false
            );

            instance.name = $"Application_{index}_{sprite.name}";

            // IMPORTANT : activer l'objet cloné
            instance.SetActive(true);

            // Récupérer et activer son icône
            Transform icone = instance.transform.Find("icone");

            if (icone != null)
            {
                icone.gameObject.SetActive(true);

                Image image = icone.GetComponent<Image>();

                if (image != null)
                {
                    image.enabled = true;
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning(
                        $"[Applications] Image absente sur {icone.name}",
                        instance
                    );
                }
            }
            else
            {
                Debug.LogWarning(
                    $"[Applications] Enfant 'icone' introuvable dans {instance.name}",
                    instance
                );
            }

            // Activer également l'image de fond
            Image fond = instance.GetComponent<Image>();

            if (fond != null)
                fond.enabled = true;

            // Sélection par clic si un Button est présent
            Button bouton = instance.GetComponent<Button>();

            if (bouton != null)
            {
                int position = index;

                bouton.interactable = true;
                bouton.onClick.AddListener(
                    () => ChangerPositionCurseur(position)
                );
            }

            lesLieux.Add(instance);
            index++;
        }

        // Sélectionner la première application
        if (lesLieux.Count > 0)
        {
            ChangerPositionCurseur(0);
        }
        else
        {
            cible = 0;
            Debug.LogWarning(
                "[Applications] Aucune application générée.",
                this
            );
        }

        Debug.Log(
            $"[Applications] {lesLieux.Count} applications générées.",
            this
        );
    }

    private bool ApplicationDisponible(Sprite sprite)
    {
        // Sans menu, toutes les applications sont affichées
        if (menu == null || menu.applicationMenus == null)
            return true;

        for (int i = 0; i < menu.applicationMenus.Count; i++)
        {
            if (menu.applicationMenus[i].sprite == sprite)
                return true;
        }

        return false;
    }

    private void Update()
    {
        if (lesLieux.Count == 0)
            return;

        if (Input.GetButtonDown("right"))
        {
            ChangerPositionCurseur(cible + 1);
        }
        else if (Input.GetButtonDown("left"))
        {
            ChangerPositionCurseur(cible - 1);
        }
        else if (Input.GetButtonDown("down"))
        {
            ChangerPositionCurseur(
                cible + nombreApplicationParEtage
            );
        }
        else if (Input.GetButtonDown("up"))
        {
            ChangerPositionCurseur(
                cible - nombreApplicationParEtage
            );
        }
    }

    public void ChangerPositionCurseur(int position)
    {
        if (lesLieux.Count == 0)
            return;

        // Bouclage correct, même avec peu d'applications
        cible = ((position % lesLieux.Count)
                 + lesLieux.Count) % lesLieux.Count;

        // Désélectionner tous les objets
        foreach (GameObject objet in lesLieux)
        {
            if (objet == null)
                continue;

            Image image = objet.GetComponent<Image>();

            if (image != null)
                image.sprite = spriteNonVisé;
        }

        // Sélectionner l'application ciblée
        GameObject selection = lesLieux[cible];

        if (selection != null)
        {
            Image image = selection.GetComponent<Image>();

            if (image != null)
                image.sprite = spriteVisé;
        }
    }
}
