using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_mohand : MonoBehaviour
{
    [HideInInspector]public int monNombre = 0;
    
    private string test = "test";
    // Start is called before the first frame update
    void Start()
    {
        AugmenterNombre(3);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void FixedUpdate()
    {
        
    }

        
    public void While(int valeur)
    {
        while (monNombre < valeur)
        {
            monNombre += 1;
            For(3);
        }
        
        while (monNombre > valeur)
        {
            monNombre -= 1;
        }
    } 

    public void For(int valeur)
    {
        for (int i = 0 ; i < valeur ; i++)
        {
            monNombre += 1;
        }

        for (int i = valeur ; i > 0; i--)
        {
            monNombre -= 1;
            for(int j = 3 ; j > 0 ; j--)
            {
                
            }
        }
                
    }
    public void AugmenterNombre(int valeur)
    {
        monNombre += valeur;
    }
    
}
