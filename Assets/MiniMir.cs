using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniMir : MonoBehaviour
{
    public TextMeshProUGUI destinationText;
    public Image destinationImage;
    public List<TeleMir> endroitTelemir = new List<TeleMir>();
    private List<TeleMir> endroitTPlayer = new List<TeleMir>();
    void Start()
    {
      ChangerDeProposition(0);  
    }
    
    void ChangerDeProposition(int idTelemir)
    {
        if(idTelemir >= endroitTPlayer.Count || idTelemir < 0)
        {
            return;
        }
        TeleMir telemir = endroitTPlayer[idTelemir];
        destinationText.text = telemir.nom;
        destinationImage.sprite = telemir.imageNouvelle;
    }
    
    void Update()
    {
        
    }
}
[System.Serializable]
public class TeleMir
{
    public int IDTeleport;
    public string nom;
    public string nameOfScene;
    public Sprite imageNouvelle;

    public TeleMir(int id, string nomTeleMir, string scene, Sprite image)
    {
        IDTeleport = id;
        nom = nomTeleMir;
        nameOfScene = scene;
        imageNouvelle = image;
    }
}