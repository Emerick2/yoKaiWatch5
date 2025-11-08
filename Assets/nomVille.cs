using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nomVille : MonoBehaviour{
    public TextMeshProUGUI titre;
    void Start(){
        titre.text = SceneManager.GetActiveScene().name;
    }
}
