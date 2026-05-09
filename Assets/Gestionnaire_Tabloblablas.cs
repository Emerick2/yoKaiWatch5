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
    public TextMeshProUGUI texteIndice1;
    public TextMeshProUGUI texteIndice2;
    public TextMeshProUGUI texteIndice3;

    void Start()
    {
        texteIndice1.text = indice1;
        texteIndice2.text = indice2;
        texteIndice3.text = indice3;
    }

    
    void Update()
    {
        
    }
}
