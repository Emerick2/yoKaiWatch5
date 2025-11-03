using UnityEngine;
using TMPro;

/*
InfoUI._istance.ChangerTexteInfoUI("");
*/
public class InfoUI : MonoBehaviour{
    public static InfoUI _instance;
    
    private void Awake(){
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public void ChangerTexteInfoUI(string texte){
        gameObject.GetComponent<TextMeshProUGUI>().text = texte;
    }
}
