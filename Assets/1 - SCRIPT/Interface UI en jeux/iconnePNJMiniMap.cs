using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class iconnePNJMiniMap : MonoBehaviour{
    public GameObject iconeYoKai;
    public GameObject iconeHumain;
    void Start(){
        bool cesUnHumain = CesUnHumain();
        iconeHumain.SetActive(cesUnHumain);
        iconeYoKai.SetActive(!cesUnHumain);
    }

    bool CesUnHumain(){
        foreach (Transform chi in gameObject.transform){
            GameObject child = chi.gameObject;
            if (child.name[0] == 'y'){
                bool etNon = false;
                for (int i = 1; i < child.name.Length; i++){
                    if (!CesUnNombre(child.name[i])){
                        etNon = true;
                        break;
                    }
                }
                if (!etNon){
                    return false;
                }
            }
        }
        return true;
    }

    bool CesUnNombre(char caractère){
        List<char> nombre = new List<char>{'0','1','2','3','4','5','6','7','8','9'};
        foreach (char n in nombre){
            if (n == caractère){
                return true;
            }
        }
        return false;
    }
}
