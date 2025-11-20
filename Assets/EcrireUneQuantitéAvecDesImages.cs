using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EcrireUneQuantitéAvecDesImages : MonoBehaviour{
    public List<Sprite> lesNombres;
    public List<Image> emplacementNombre;
    public bool EcrireSi0 = false;
    public bool EcrireSi1 = true;
    // void Start(){
    //     EcrireLeNombre(1000);
    // }

    public void EcrireLeNombre(int nombre){
        foreach (Image item in emplacementNombre){
            item.gameObject.SetActive(false);
        }
        if ((nombre == 0 && !EcrireSi0)||(nombre == 1 && !EcrireSi1)){
            return;
        }
        if (nombre.ToString().Length > emplacementNombre.Count){
            int n = 1;
            for (int i = 1; i <= emplacementNombre.Count; i++){
                n*=10;
            }
            nombre = n-1;
        }
        string nombreTexte = nombre.ToString();
        for (int i = 0; i < nombreTexte.Length; i++){
            int p = emplacementNombre.Count-1-i;
            emplacementNombre[p].gameObject.SetActive(true);
            emplacementNombre[p].sprite = lesNombres[int.Parse(nombreTexte[i].ToString())];
        }
    }
}
