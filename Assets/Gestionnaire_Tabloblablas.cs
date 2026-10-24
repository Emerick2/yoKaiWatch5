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
    

        
    }
    

    public void ValiderReponse()
    {
        réponse = texte_entrée.text;
        Debug.Log(UniformiserTexte(réponse));
        Debug.Log(UniformiserTexte(nomYokaiTexte));
        if (UniformiserTexte(réponse) == UniformiserTexte(nomYokaiTexte))
        {
            Debug.Log("validation");

        }
        else
        {
            Debug.Log("incorrect");
        } 
        
        
    }

    string UniformiserTexte(string nom)
    {
        nom = nom.ToLower();
        nom = nom.Replace(" ", "");
        nom = nom.Trim();
        return nom;
    }


    string FonctionA()
    {
        string text = "text";
        return text;
    }

    string FonctionB()
    {
        string text = "text";
        return text + FonctionA();
    }

    string C(string text)
    {
        text = "text";
        return text * 2;
    }

     int D()
     { 
        int monNombre = 1;
        for(int i = 0; i < 5; i++)
        {
            monNombre+=i;
        }

        return monNombre;
     }



}
