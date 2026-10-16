using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gestionnaire_Tabloblablas : MonoBehaviour
{
    public int ID_Yokai;
    public string proposition_joueur;
    public string indice1;
    public string indice2;
    public string indice3;
    public string réponse;
    public string nomYokaiTexte;
    public TextMeshProUGUI texte_entrée;
    public TextMeshProUGUI texteIndice1;
    public TextMeshProUGUI texteIndice2;
    public TextMeshProUGUI texteIndice3;

    public TextMeshProUGUI nomYokai;


    void Start()
    {
        texteIndice1.text = indice1;
        texteIndice2.text = indice2;
        texteIndice3.text = indice3;
        YKW.Core.YoKai.Class.YoKai medalium = medaliumDictionnaire._instance.ServantYoKai(ID_Yokai);
        if (nomYokai != null && medalium != null) {
            nomYokai.text = medalium.nom;
            nomYokaiTexte = medalium.nom;
        }

    }

    
    void Update()
    {
        
        réponse = texte_entrée.text;
        if(réponse == nomYokaiTexte)
        {
            Debug.Log("bravo");
        }

        Debug.Log(réponse);
    }
    

}
