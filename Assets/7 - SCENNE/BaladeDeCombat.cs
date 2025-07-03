using System.Collections.Generic;
using UnityEngine;

public class BaladeDeCombat : MonoBehaviour{
    public List<Éléments> listeYoKai = new List<Éléments>();
    public Vector3 positionJoueur;
    public string scèneOriginale;
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    public void Détruit(){
        Destroy(gameObject);
    }
    public void Ajouter(int ID, int niveau){
        Éléments nouveau = new Éléments();
        nouveau.ID = ID;
        nouveau.niveau = niveau;
        listeYoKai.Add(nouveau);
    }
}
[System.Serializable]
public class Éléments{
    public int ID;
    public int niveau;
}
