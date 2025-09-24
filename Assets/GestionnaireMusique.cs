using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GestionnaireMusique : MonoBehaviour{
    public static GestionnaireMusique _Komasan;
    public AudioClip musiqueDeFond;
    private AudioClip musiqueEnMémoire;
    private AudioSource audioSource;
    private List<GameObject> listeObjetMusique = new List<GameObject>();
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
    }
    void Update(){
        if (!audioSource.isPlaying && musiqueEnMémoire != null){
            ChangerLaMusique(musiqueEnMémoire,true);
        }
    }

    public void ChangerLaMusique(AudioClip nomMusique, bool répété){
        if (audioSource == null || nomMusique == null) return;
        if (!répété){
            musiqueEnMémoire=audioSource.clip;
        }
        audioSource.clip = nomMusique;
        audioSource.loop = répété;
        audioSource.Play();
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
