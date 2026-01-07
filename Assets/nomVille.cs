using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nomVille : MonoBehaviour{
    public TextMeshProUGUI titre;
    void Start(){
        titre.text = SceneManager.GetActiveScene().name;
        StartCoroutine(Fermeture());
    }
    IEnumerator Fermeture(){
        yield return new WaitForSeconds(6f);
        gameObject.SetActive(false);
    }
}
