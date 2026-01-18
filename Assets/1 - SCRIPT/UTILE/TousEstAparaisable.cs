using System.Collections;
using UnityEngine;

public class TousEstAparaisable : MonoBehaviour
{
    //public float vitesseADonné = 10f;
    public Transform positionDeApparition;
    public GameObject Prefab;
    public float tempsAtendre;
    void Start()
    {
        StartCoroutine(tourneViseEnvoyer());
    }
    IEnumerator tourneViseEnvoyer()
    {
        while (true){
            yield return new WaitForSeconds(tempsAtendre);
            GameObject TV = Instantiate(Prefab);
            Vector3 newPosition1 = new Vector3(positionDeApparition.position.x, positionDeApparition.position.y, 0f);
            TV.transform.position = newPosition1;
            //TV.GetComponent<CircleCollider>().enabled=true;
            //TV.GetComponent<SpriteRenderer>().enabled=true;
            //TV.GetComponent<toutsEstDéplasable>().vitesse=vitesseADonné;
        }
    }
}
