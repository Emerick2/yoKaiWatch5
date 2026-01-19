// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class lechoixExtension : MonoBehaviour
// {
//     private bool attendRelachementE = true;
//     private bool pauseDeApparition=true;
//     public GameObject[] objet;
//     public GameObject[] sélection;
//     public Text[] texte;
//     private int nombre;
//     private bool jeSuisUtile;
//     //public GameObject objetSélection;
//     private int position=0;
//     private GameObject messager;
//     private List<string> choix = new List<string> {};
//     private string clef;


//     public void mettreUnchoix(string choix1, string choix2, string choix3, string choix4, GameObject go, string clefs){
//         messager=go;
//         clef=clefs;
//         pauseDeApparition = true;
//         choix.Clear();
//         if (choix1 != ""){
//             objet[0].SetActive(true);nombre=0;jeSuisUtile=true;/*objetSélection.SetActive(true)*/;choix.Add(choix1);texte[0].text=choix1;
//         }if (choix2 != ""){
//             objet[1].SetActive(true);nombre++;choix.Add(choix2);texte[1].text=choix2;
//         }if (choix3 != ""){
//             objet[2].SetActive(true);nombre++;choix.Add(choix3);texte[2].text=choix3;
//         }if (choix4 != ""){
//             objet[3].SetActive(true);nombre++;choix.Add(choix4);texte[3].text=choix4;
//         }

//         if (nombre != 0){
//             sélection[position].SetActive(true);
//             for (int i = 0; i < sélection.Length; i++){
//                 if (i != position){
//                     sélection[i].SetActive(false);
//                 }
//             }
//             //objetSélection.transform.position=objet[position].transform.position;
//             //objetSélection.transform.position.x+=10.82f;
//         }
//         if (pauseDeApparition){
//             StartCoroutine (staupPauseDialogue());
//         }
//     }
//     IEnumerator staupPauseDialogue(){
//         yield return new WaitForSeconds(0.3f);
//         pauseDeApparition=false;
//     }
//     public void fermeture(){
//         /*objetSélection.SetActive(false)*/;jeSuisUtile=false;
//         for (int i = 0; i < sélection.Length; i++){
//             sélection[i].SetActive(false);
//             objet[i].SetActive(false);
//         }
//     }
//     void Update(){
//         if (jeSuisUtile){
//             if (Input.GetButtonDown("right")){
//                 position++;
//                 if (position > nombre){
//                     position = 0;
//                 }else if (position < 0){
//                     position = nombre;
//                 }
//                 sélection[position].SetActive(true);
//                 for (int i = 0; i < sélection.Length; i++){
//                     if (i != position){
//                         sélection[i].SetActive(false);
//                     }
//                 }
//                 //objetSélection.transform.position=objet[position].transform.position;
//                 //objetSélection.transform.position.x+=10.82f;
//             }else if (Input.GetButtonDown("left")){
//                 position--;
//                 if (position > nombre){
//                     position = 0;
//                 }else if (position < 0){
//                     position = nombre;
//                 }
//                 sélection[position].SetActive(true);
//                 for (int i = 0; i < sélection.Length; i++){
//                     if (i != position){
//                         sélection[i].SetActive(false);
//                     }
//                 }
//                 //objetSélection.transform.position=objet[position].transform.position;
//                 //objetSélection.transform.position.x+=10.82f;
//             }else if(Input.GetButtonDown("A") && !pauseDeApparition && messager!=null && !attendRelachementE && position >= 0 && position < choix.Count /*&& objetSélection != null*/ && objet[position] != null){
//                 pauseDeApparition=true;
//                 jeSuisUtile=false;
//                 messager.GetComponent<PNJ_script>().enregistrerUneValeur(clef, choix[position], true);
//                 messager.GetComponent<PNJ_script>().atendUneRéponceALaQuestion=false;
//                 fermeture();
//             }else if (Input.GetButtonUp("A")){
//                 attendRelachementE = false;
//             }
//         }
//     }
// }
