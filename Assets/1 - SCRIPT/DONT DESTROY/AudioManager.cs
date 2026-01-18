using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour{
    public AudioClip[] listeDeMusiques;
    public AudioSource sourceMusicale;
    private int index = 0;

    public AudioMixerGroup soundEffectMixer;
    public static AudioManager instance;

    private void Awake(){
        if (instance != null){
            Debug.LogWarning("Il y a plus d'une instance de AudioManager dans la scène");
            return;
        }
        instance = this;
    }

    void Start(){
        if (listeDeMusiques.Length != 0){
            sourceMusicale.clip = listeDeMusiques[0];
            sourceMusicale.Play();
        }
    }

    void Update(){
        if (!sourceMusicale.isPlaying){
            PlayNextSong();
        }
    }

    void PlayNextSong(){
        index = (index + 1) % listeDeMusiques.Length;
        sourceMusicale.clip = listeDeMusiques[index];
        sourceMusicale.Play();
    }
/*
    public sourceMusicale PlayClipAt(AudioClip clip, Vector3 pos){
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;
        sourceMusicale sourceMusicale = tempGO.AddComponent<sourceMusicale>();
        sourceMusicale.clip = clip;
        sourceMusicale.outputAudioMixerGroup = soundEffectMixer;
        sourceMusicale.Play();
        Destroy(tempGO, clip.length);
        return sourceMusicale;
    }
    */
}
