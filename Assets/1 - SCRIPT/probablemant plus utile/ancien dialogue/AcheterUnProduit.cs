using System;
using UnityEngine;
using UnityEngine.UI;

public class AcheterUnProduit : MonoBehaviour{
    public Text espaceDePrix;
    public decimal prixDeUnProduit = 0m;
    public InputField nombre;
    public decimal monArgent = 0m;
    public string nom;
    public BoutiqueDialcode vdp;
    private decimal total = 0m;
    public void Réinitialiser(float prixI, string nomI){
        nom = nomI;
        total = 0;
        monArgent = NewDataCenter.Instance.LireMonaie();
        prixDeUnProduit = (decimal)prixI;
        nombre.text="0";
        espaceDePrix.text = "0€";
        if (string.IsNullOrEmpty(espaceDePrix.text)){
            espaceDePrix.text = "0";
        }
        if (string.IsNullOrEmpty(nombre.text)){
            nombre.text = "0";
        }
    }

    void Update(){
        if (Input.GetButtonDown("left")){
            AugmenterQuantité(-1);
        } else if (Input.GetButtonDown("right")){
            AugmenterQuantité(1);
        } else if (Input.GetButtonDown("up")){
            AugmenterQuantité(10);
        } else if (Input.GetButtonDown("down")){
            AugmenterQuantité(-10);
        } else if (Input.GetButtonDown("A")){
            Payer();
        }
    }


    /// <summary>
    /// Permet d'écrire le prix que l'utilisateur devras payer s'il payent.
    /// </summary>
    void EcrirePrixTotal(){
        if (monArgent==0 || prixDeUnProduit == 0){
            Debug.Log("Division par zéro interdite");
            return;
        }
        if (int.TryParse(nombre.text, out int quantiteeSouhaitee)) {
            int quantiteeMaxPossible = (int)(monArgent / prixDeUnProduit);
            int quantiteeFinale = Math.Min(quantiteeSouhaitee, quantiteeMaxPossible);

            nombre.text = quantiteeFinale.ToString();
            total = quantiteeFinale * prixDeUnProduit;
            
            // "F2" force l'affichage à 2 chiffres après la virgule (8.80€)
            if((int)total != total) espaceDePrix.text = total.ToString("F2") + "€"; 
            else espaceDePrix.text = total.ToString("G2") + "€"; 
        }
    }

    /// <summary>
    /// Cette fonction permet de changer la quantitée de produit demander.
    /// On attent un AugmenterQuantité(1) ou un AugmenterQuantité(10) et pareille pour les négatif
    /// </summary>

    public void AugmenterQuantité(int i){
        int valeur;
        if (!int.TryParse(nombre.text, out valeur)) return;

        valeur += i;

        if (vdp.acheterUser) {
            // Mode Achat
            int quantiteeMaxPossible = (int)(monArgent / prixDeUnProduit);
            valeur = Mathf.Clamp(valeur, 0, quantiteeMaxPossible);
            valeur = Mathf.Clamp(valeur, 0, 99);
        } else {
            // Mode Vente
            int maxPossede = 0;
            if(int.TryParse(NewDataCenter.Instance.Lire(nom), out int quantite)){
                maxPossede = quantite;
            }
            if (maxPossede > 99){
                maxPossede = 99;
            }
            valeur = Mathf.Clamp(valeur, 0, maxPossede);
        }
        
        nombre.text = valeur.ToString();
        EcrirePrixTotal();
    }
    
    /// <summary>
    /// Cette fonction permet d'acheter le produit.
    /// </summary>
    public void Payer(){
        if (vdp.acheterUser){
            NewDataCenter.Instance.SauvegarderMonaie(total*(-1));
            NewDataCenter.Instance.SauvegarderINT(nom,int.Parse(nombre.text));
        } else {
            NewDataCenter.Instance.SauvegarderMonaie(total);
            NewDataCenter.Instance.SauvegarderINT(nom,int.Parse(nombre.text)*(-1));
            StartCoroutine(vdp.Initialisation(false));
        }
        if (vdp != null){ 
            vdp.acheter=false;
            vdp.ReChargerMonArgent();
        }
        vdp.acheterUnProduitFenêtreOuverte = false;
        vdp.RemplirBlocInformation();
        gameObject.SetActive(false);
    }

}
