using UnityEngine;

public class boutonRetour : MonoBehaviour
{
    public GameObject aOuvrir;
    public GameObject aFermer;
    void Update(){
        if(Input.GetButtonDown("B")){
            action();
        }
    }
    public void action(){
        aFermer.SetActive(false);
        aOuvrir.SetActive(true);
    }
}
