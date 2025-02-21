using UnityEngine;

public class boutonRetour : MonoBehaviour
{
    public GameObject aOuvrir;
    public GameObject aFermer;
    void Update(){
        if(Input.GetKeyDown(KeyCode.B)){
            action();
        }
    }
    public void action(){
        aFermer.SetActive(false);
        aOuvrir.SetActive(true);
    }
}
