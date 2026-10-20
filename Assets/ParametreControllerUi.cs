using UnityEngine;

public class ParametreControllerUi : MonoBehaviour
{
    [Header("Menu principal")]
    public écrireLesPetitesValeursDuMenu menu;

    void Update()
    {
        if (Input.GetButtonDown("B"))
        {
            RevenirEnArriere();
        }
    }

    public void RevenirEnArriere()
    {
        if (menu != null)
        {
            menu.RevenirAuMenu();
        }
    }
}