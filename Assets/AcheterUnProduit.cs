using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcheterUnProduit : MonoBehaviour{
    public Text espaceDePrix;
    public float prixDeUnProduit;
    public InputField nombre;
    public int monArgent;
    public string nom;
    public vendreDesProduits vdp;
    int total;
    void Awake(){
        Réinitialiser();
    }
    public void Réinitialiser(){
        nombre.text="0";
        monArgent = int.Parse(NewDataCenter.Instance.Lire("€"));
        espaceDePrix.text = prixDeUnProduit.ToString()+"€";
        if (string.IsNullOrEmpty(espaceDePrix.text)){
            espaceDePrix.text = "0";
        }
        if (string.IsNullOrEmpty(nombre.text)){
            nombre.text = "0";
        }
    }

    void Update(){
        total = (int)prixDeUnProduit*int.Parse(nombre.text);
        if(monArgent<total){
            int x = 0;
            for (int i = 0; i <= 99; i++){
                if (x*prixDeUnProduit<monArgent){
                    x++;
                }
            }
            nombre.text = x.ToString();
        }
        espaceDePrix.text = (int.Parse(nombre.text) * prixDeUnProduit).ToString()+"€";
    }
    public void AugmenterQuantité(int i){
        int valeur;
        if (int.TryParse(nombre.text, out valeur)){
            valeur += i;
            valeur = Mathf.Clamp(valeur, 0, 99);
            nombre.text = valeur.ToString();
        }
    }

    public void DiminuerQuantité(int i){
        int valeur;
        if (int.TryParse(nombre.text, out valeur)){
            valeur -= i;
            valeur = Mathf.Clamp(valeur, 0, 99);
            nombre.text = valeur.ToString();
        }
    }
    public void payer(){
        NewDataCenter.Instance.SauvegarderINT("€", total*(-1));
        NewDataCenter.Instance.SauvegarderINT(nom,int.Parse(nombre.text));
        if (vdp != null){ 
            vdp.acheter=false;
            vdp.ReChargerMonArgent();
        }
        gameObject.SetActive(false);
    }
}
