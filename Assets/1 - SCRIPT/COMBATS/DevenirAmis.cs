using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevenirAmis : MonoBehaviour {
    private int ID;
    private int niveau;
    private string personalité;
    private string surnom;
    public Vector3 position;
    void Start()
    {
        NeDisparaitPas neDisparaitPas = FindObjectOfType<NeDisparaitPas>();
        ID = int.Parse(neDisparaitPas.aTransporter1);
        niveau = int.Parse(neDisparaitPas.aTransporter2);
        personalité = neDisparaitPas.aTransporter3;
        surnom = neDisparaitPas.aTransporter4;
        if (surnom == "") surnom = medaliumDictionnaire._instance.ServantYoKai(ID).nom;

        GameObject TV = medaliumDictionnaire._instance.InstantiateYoKai(ID.ToString());
        TV.transform.position = position;
        neDisparaitPas.SeDétruire();
        Enregistrer();
    }
    void Enregistrer(){
        MesYoKai.Instance.AjouterUnYoKai(ID, niveau, personalité, surnom, 0);
    }
}
