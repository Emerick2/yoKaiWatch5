using UnityEngine;

public class PNJ : MonoBehaviour{
    public string monCodeDeDialogue;
    private bool dansLaZonne;
    private GestionnaireMenu gestionnaireMenu;
    void Update(){
        if (Input.GetButtonDown("A") && dansLaZonne && mouvementDuJoueur.droitDeMarcher == true){
            InfoUI._instance.ChangerTexteInfoUI("");
            if (gestionnaireMenu == null) gestionnaireMenu = FindObjectOfType<GestionnaireMenu>();
            gestionnaireMenu.OnCommenceUnDialogueICI(monCodeDeDialogue);
            mouvementDuJoueur.droitDeMarcher = false;
            dansLaZonne = false;
        }
    }
    private void OnTriggerEnter(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            dansLaZonne = true;
            InfoUI._instance.ChangerTexteInfoUI("Appuie sur 'A' pour parler");
        }
    }
    private void OnTriggerExit(Collider collision){
        if (collision.gameObject.CompareTag("Player")){
            dansLaZonne = false;
            InfoUI._instance.ChangerTexteInfoUI("");
        }
    }
}
