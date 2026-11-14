
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class GestionnaireMusique : MonoBehaviour
{
    public static GestionnaireMusique _Komasan;

    [Header("Musique")]
    public AudioClip musiqueDeFond;

    private AudioClip musiqueEnMémoire;
    private AudioSource audioSource;
    private bool laMusiqueVientDuLecteurMP3;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup groupeMusique;
    [SerializeField] private AudioMixerGroup groupeEffetsSonores;

    [Header("Interface")]
    public GameObject blocMusique;
    public TextMeshProUGUI nomMusique;

    private readonly List<GameObject> listeObjetMusique = new();

    private void Awake()
    {
        if (_Komasan != null && _Komasan != this)
        {
            Destroy(gameObject);
            return;
        }

        _Komasan = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Routage automatique vers le groupe Music
        if (groupeMusique != null)
            audioSource.outputAudioMixerGroup = groupeMusique;

        if (musiqueDeFond != null)
            ChangerLaMusique(musiqueDeFond, true);
    }

    private void Start()
    {
        StartCoroutine(NettoyerLaListe());
        Ecrire();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && musiqueEnMémoire != null)
        {
            AudioClip musiquePrecedente = musiqueEnMémoire;
            musiqueEnMémoire = null;

            ChangerLaMusique(musiquePrecedente, true);
        }
    }

    private void Ecrire()
    {
        if (blocMusique == null)
            return;

        blocMusique.SetActive(laMusiqueVientDuLecteurMP3);

        if (laMusiqueVientDuLecteurMP3 &&
            nomMusique != null &&
            audioSource.clip != null)
        {
            nomMusique.text = audioSource.clip.name;
        }
    }

    public void ChangerLaMusique(
        AudioClip nomMusique,
        bool répété,
        bool vientDuLecteurMP3 = false,
        bool remaitreLeThèmePrincipale = false)
    {
        if (remaitreLeThèmePrincipale)
            nomMusique = musiqueDeFond;

        if (audioSource == null || nomMusique == null)
            return;

        laMusiqueVientDuLecteurMP3 = vientDuLecteurMP3;

        if (!répété)
        {
            musiqueEnMémoire = audioSource.clip;
        }
        else
        {
            musiqueEnMémoire = null;
        }

        audioSource.clip = nomMusique;
        audioSource.loop = répété;
        audioSource.Play();

        Ecrire();
    }

    public void JouerUnEffetSonore(AudioClip nomMusique, bool répété)
    {
        if (nomMusique == null)
            return;

        GameObject objetSon = new GameObject(
            $"SFX_{nomMusique.name}"
        );

        objetSon.transform.SetParent(transform);

        AudioSource source = objetSon.AddComponent<AudioSource>();

        source.clip = nomMusique;
        source.loop = répété;
        source.playOnAwake = false;
        source.spatialBlend = 0f;

        // Routage automatique vers le groupe SFX
        if (groupeEffetsSonores != null)
            source.outputAudioMixerGroup = groupeEffetsSonores;

        source.Play();

        listeObjetMusique.Add(objetSon);
    }

    private IEnumerator NettoyerLaListe()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            for (int i = listeObjetMusique.Count - 1; i >= 0; i--)
            {
                GameObject objet = listeObjetMusique[i];

                if (objet == null)
                {
                    listeObjetMusique.RemoveAt(i);
                    continue;
                }

                AudioSource source = objet.GetComponent<AudioSource>();

                if (source == null || !source.isPlaying)
                {
                    listeObjetMusique.RemoveAt(i);
                    Destroy(objet);
                }
            }
        }
    }

    public void ArreterTousLesEffetsSonores()
    {
        foreach (GameObject objet in listeObjetMusique)
        {
            if (objet != null)
                Destroy(objet);
        }

        listeObjetMusique.Clear();
    }
}
