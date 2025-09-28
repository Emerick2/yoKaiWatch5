using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GestionnaireMusique : MonoBehaviour{
    public static GestionnaireMusique _Komasan;
    public AudioClip musiqueDeFond;
    private AudioClip musiqueEnMémoire;
    private AudioSource audioSource;
    private List<GameObject> listeObjetMusique = new List<GameObject>();
    private bool laMusiqueVientDuLecteurMP3;

    public GameObject blocMusique;
    public TextMeshProUGUI nomMusique;
    
    private void Awake(){
        if (_Komasan == null){
            _Komasan = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (musiqueDeFond != null){
            ChangerLaMusique(musiqueDeFond, true);
        }
    }
    void Start(){
        NettoyerLaListe();
        Ecrire();
    }
    void Update(){
        if (!audioSource.isPlaying && musiqueEnMémoire != null){
            ChangerLaMusique(musiqueEnMémoire,true);
        }
    }

    void Ecrire(){
        if (blocMusique != null){
            if (laMusiqueVientDuLecteurMP3){
                blocMusique.SetActive(true);
                if (nomMusique != null && audioSource.clip != null) nomMusique.text = audioSource.clip.name;
            } else {
                blocMusique.SetActive(false);
            }
        }
    }
    public void ChangerLaMusique(AudioClip nomMusique, bool répété, bool vientDuLecteurMP3 = false,bool remaitreLeThèmePrincipale = false){
        if (remaitreLeThèmePrincipale){
            nomMusique = musiqueDeFond;
        }
        laMusiqueVientDuLecteurMP3 = vientDuLecteurMP3;
        if (audioSource == null || nomMusique == null) return;
        if (!répété){
            musiqueEnMémoire=audioSource.clip;
        }
        audioSource.clip = nomMusique;
        audioSource.loop = répété;
        audioSource.Play();
        Ecrire();
    }
    public void JouerUnEffetSonore(AudioClip nomMusique, bool répété){
        if (audioSource == null || nomMusique == null) return;
        GameObject TV = new GameObject();
        AudioSource source = TV.AddComponent<AudioSource>();
        source.clip = nomMusique;
        source.loop = répété;
        source.Play();
        listeObjetMusique.Add(TV);
    }
    IEnumerator NettoyerLaListe(){
        while (true){
            yield return new WaitForSeconds(2f);
            foreach (GameObject item in listeObjetMusique){
                if (!item.GetComponent<AudioSource>().isPlaying){
                    listeObjetMusique.Remove(item);
                    Destroy(item);
                }
            }
        }
    }
}
