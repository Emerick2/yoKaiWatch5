using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Mohand_2012 : MonoBehaviour
{
    public Image image;
    public int incrémentation = 1;
    IEnumerator DémarrageDuScript()
    {
        yield return new WaitForSeconds(0.2f);
        while (medaliumDictionnaire._instance.dictionnaire.Count>incrémentation)
        {
            image.sprite = medaliumDictionnaire._instance.dictionnaire[incrémentation].médaillon;
            incrémentation++;
            yield return new WaitForSeconds(1f);
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DémarrageDuScript()); 
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
