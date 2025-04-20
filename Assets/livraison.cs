using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class livraison : MonoBehaviour{
    public List<UneDeMesLivraison> numéroDesYoKaiPourLeCombat = new List<UneDeMesLivraison>();//le premier est l'ID et le deuxième son niveau.
    void Start(){
        DontDestroyOnLoad(gameObject);
    }
    public void ajouterUneValeur(int ID, int niveau){
        UneDeMesLivraison nouvelleLivraison = new UneDeMesLivraison();
        nouvelleLivraison.ID = ID;
        nouvelleLivraison.niveau = niveau;
        numéroDesYoKaiPourLeCombat.Add(nouvelleLivraison);
    }
}
[System.Serializable]
public class UneDeMesLivraison{
    public int ID;
    public int niveau;
}