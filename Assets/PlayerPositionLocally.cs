using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositionLocally : MonoBehaviour {
    public static PlayerPositionLocally _instance;
    private Vector3 vector;
    private string nameOfTheScene;
    private bool whitTheButonB;
    private bool mustChangeThePositionDirectly;

    public static PlayerPositionLocally Instance{
        get{
            if (_instance == null) {
                GameObject obj = new GameObject("NewDataCenter");
                _instance = obj.AddComponent<PlayerPositionLocally>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
    
    private void Awake(){
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    void Update() {
        if (whitTheButonB && Input.GetButtonDown("B") && nameOfTheScene != "" && nameOfTheScene != null){
            whitTheButonB = false;
            mustChangeThePositionDirectly = true;
            StartCoroutine(Teleport(nameOfTheScene));
        }
    }

    public void StoreThePlayerPositionLocally(bool newWhitTheButonB){
        vector = mouvementDuJoueur.player.transform.position;
        nameOfTheScene = SceneManager.GetActiveScene().name;
        whitTheButonB = newWhitTheButonB;
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if (mustChangeThePositionDirectly){
            mustChangeThePositionDirectly = false;
            StartCoroutine(Patiante());
        }
    }

    IEnumerator Patiante(){
        while (mouvementDuJoueur.player == null){
            yield return new WaitForSeconds(0.02f);
        }
        mouvementDuJoueur.player.transform.position = vector;
    }

    public IEnumerator Teleport(string sceneName){
        mouvementDuJoueur.droitDeMarcher = false;

        bool lancer = false;
        GameObject theInterface = GameObject.FindWithTag("interfasse");
        if (theInterface != null){
            theInterface = theInterface.transform.Find("fondu").gameObject;
            if (theInterface != null && theInterface.GetComponent<Animator>() != null){
                theInterface.GetComponent<Animator>().SetBool("fonduActif",true);
                lancer = true;
            }
        }

        if (lancer) yield return new WaitForSeconds(1f);
        else Debug.Log("Le fond de chargement n'est pas correctement référancer.");

        SceneManager.LoadScene(sceneName);
    }
}
