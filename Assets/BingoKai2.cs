// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BingoKai2 : MonoBehaviour {



//     void Start() {
//         string itemGagne = ObtenirElementAleatoire(itemsPieceJaune);
//         Debug.Log($"L'objet obtenu est : {itemGagne}");
//     }

//     public string ObtenirElementAleatoire(Dictionary<string, int> dictionnaire) {
//         int sommeTotale = 0;
//         foreach (var poids in dictionnaire.Values) {
//             sommeTotale += poids;
//         }

//         int nombreAleatoire = Random.Range(0, sommeTotale);

//         int sommeActuelle = 0;
//         foreach (var element in dictionnaire) {
//             sommeActuelle += element.Value;
            
//             if (nombreAleatoire < sommeActuelle) {
//                 return element.Key;
//             }
//         }

//         return "Aucun objet";
//     }
// }
