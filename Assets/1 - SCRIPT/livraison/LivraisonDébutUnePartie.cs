using UnityEngine;

public class LivraisonDébutUnePartie : MonoBehaviour{
    public Vector3 position;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }

    public void Déplacer(Transform transform){
        string pos1 = NewDataCenter.Instance.Lire("pos1","position");
        string pos2 = NewDataCenter.Instance.Lire("pos2","position");
        string pos3 = NewDataCenter.Instance.Lire("pos3","position");
        if (pos1 == "0" || pos2 == "0" || pos3 == "0"){
            position = transform.position;
        }else{
            position = new Vector3(float.Parse(pos1),float.Parse(pos2+200f),float.Parse(pos3));
        }

        transform.position = position;
        //Time.timeScale = 0.001f;
        Détruire();
    }

    public void Détruire(){
        Destroy(gameObject);
    }
}
