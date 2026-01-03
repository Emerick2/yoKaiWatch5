/*
Les balises :
<PNAME> : nom du héro
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialcodePrinted : MonoBehaviour{
    public List<FragmentDeDialogue> fragmentDeDialogue = new List<FragmentDeDialogue>();
    private TextMeshProUGUI espaceDialogue;
    private GameObject blocNomUtilisé;
    private TextMeshProUGUI espaceNom;
    private Image espaceImage;
    private int position = 0;
    public float vitesseApparitionLettre = 0.05f;
    private Coroutine coroutineApparitionLettre;
    private bool boiteFermer;
    public float dureeApparitionOuvertureFermeture = 0.1f;
    private CanvasGroup canvasGroup;
    private bool peutCliquer = false;
    public Décoration décoration;
    private GameObject bulle;
    private GameObject cadeau;
    void Start(){
        bulle = transform.Find("-- BULLE --").gameObject;
        cadeau = transform.Find("-- OFFRE --").gameObject;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        Fermeture(true);
    }

    void Update(){
        if (Input.GetButtonDown("A")){
            if (boiteFermer){
                Ouverture();
                peutCliquer = false;
                Action();
            }
            if (position >= fragmentDeDialogue.Count){
                Fermeture(false);
                peutCliquer = false;
            }else if (peutCliquer) {
                Action();
            }
        }
    }

    void Action(){
        AvancerJusqueAuProchainDialogue();
        FragmentDeDialogue fragment = fragmentDeDialogue[position];
        fragment.nom = fragment.nom.Replace("<PNAME>",NewDataCenter.Instance.Lire("nomDuJoueur"));
        fragment.phrase = fragment.phrase.Replace("<PNAME>",NewDataCenter.Instance.Lire("nomDuJoueur"));
        if (fragment.quiJeSuis == QuiJeSuis.yokai || fragment.quiJeSuis == QuiJeSuis.héroYoKai){
            if (fragment.nom.Length == 0) fragment.nom = medaliumDictionnaire._instance.dictionnaire[fragment.IdÉlément].nom;
            if (fragment.image == null) fragment.image = medaliumDictionnaire._instance.dictionnaire[fragment.IdÉlément].faceImage;
        }
        if (fragment.type == TypeDialogue.cadeau){
            if (fragment.quantitée <= 0) fragment.quantitée = 1;
            if (fragment.typeObjet == TypeObjet.nouriture){
                fragment.nom = Item._instance.listeDesNouritures[fragment.IdÉlément].nom;
                fragment.image = Item._instance.listeDesNouritures[fragment.IdÉlément].image;
            } else if (fragment.typeObjet == TypeObjet.équipement){
                fragment.nom = Item._instance.listeDesÉquipements[fragment.IdÉlément].nom;
                fragment.image = Item._instance.listeDesÉquipements[fragment.IdÉlément].sprite;
            } else if (fragment.typeObjet == TypeObjet.annimaux){
                fragment.nom = Item._instance.listeDesAnnimaux[fragment.IdÉlément].nom;
                fragment.image = Item._instance.listeDesAnnimaux[fragment.IdÉlément].image;
            }
        }
        //la je m'ocupe des id pour écrire le nom et l'image
        if (fragment.type == TypeDialogue.texte){
            bulle.SetActive(true);
            cadeau.SetActive(false);
            if (coroutineApparitionLettre == null){
                if (!boiteFermer) coroutineApparitionLettre = StartCoroutine(PrintSentence(fragment.phrase));
            } else {
                if (!boiteFermer){
                    StopCoroutine(coroutineApparitionLettre);
                    coroutineApparitionLettre = null;
                    espaceDialogue.text = fragment.phrase;
                    position++;
                }
            }
        } else if (fragment.type == TypeDialogue.cadeau){
            bulle.SetActive(false);
            cadeau.SetActive(true);
            if (fragment.image != null){
                cadeau.transform.Find("cintille").gameObject.SetActive(true);
                cadeau.transform.Find("cintille/cadeau").GetComponent<Image>().sprite = fragment.image;
            } else {
                cadeau.transform.Find("cintille").gameObject.SetActive(false);
            }
            string phrase = "Tu as obtenu un cadeau !";
            if (fragment.nom.Length > 0){
                if (fragment.quantitée == 1) phrase = "Tu as obtenu : <color=#4C714F>\n"+fragment.nom+"</color> !";
                else phrase = "Tu as obtenu : <color=#4C714F>\n"+fragment.quantitée +" "+ fragment.nom+"</color> !";
            }
            NewDataCenter.Instance.SauvegarderINT(fragment.nom,fragment.quantitée);
            //Utiliser <C12><ITEM_NAME></C> sur qui ?
            cadeau.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>().text = phrase;
            position++;
        }
    }

    void Ouverture(){
        boiteFermer = false;
        StartCoroutine(AnnimationOuverture());
        position = 0;
        if (fragmentDeDialogue.Count == 0){
            Debug.LogWarning("Il n'y a pas de phrase pour le dialogue !");
        }
    }

    IEnumerator AnnimationOuverture(){
        float tempsEcoule = 0f;
        while (tempsEcoule < dureeApparitionOuvertureFermeture){
            tempsEcoule += Time.deltaTime;
            float nouvelAlpha = Mathf.Lerp(0f, 1f, tempsEcoule / dureeApparitionOuvertureFermeture);
            canvasGroup.alpha = nouvelAlpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        peutCliquer = true;
    }
    void Fermeture(bool rapide){
        boiteFermer = true;
        if (rapide){
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        } else {
            StartCoroutine(AnnimationFermeture());
        }
    }
    IEnumerator AnnimationFermeture(){
        float tempsEcoule = 0f;
        while (tempsEcoule < dureeApparitionOuvertureFermeture){
            tempsEcoule += Time.deltaTime;
            float nouvelAlpha = Mathf.Lerp(1f, 0f, tempsEcoule / dureeApparitionOuvertureFermeture);
            canvasGroup.alpha = nouvelAlpha;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        peutCliquer = true;
    }

    IEnumerator PrintSentence(string sentence){
        AjouterLeNomEtImagePersonnage();
        TextMeshProUGUI espace = espaceDialogue;
        espace.text = "";
        for (int i = 0; i < sentence.Length; i++){
            espace.text += sentence[i];
            yield return new WaitForSeconds(vitesseApparitionLettre);
        }
        position++;
        coroutineApparitionLettre = null;
    }

    void AjouterLeNomEtImagePersonnage(){
        if (fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.héro || fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.héroYoKai){
            blocNomUtilisé = bulle.transform.Find("-- D --").gameObject;
            bulle.transform.Find("-- G --").gameObject.SetActive(false);
            if (fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.héroYoKai){
                blocNomUtilisé.transform.Find("le nom").gameObject.GetComponent<Image>().sprite = décoration.nomY2;
            } else {
                blocNomUtilisé.transform.Find("le nom").gameObject.GetComponent<Image>().sprite = décoration.nomH2;
            }
        } else {
            blocNomUtilisé = bulle.transform.Find("-- G --").gameObject;
            bulle.transform.Find("-- D --").gameObject.SetActive(false);
            if (fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.yokai){
                blocNomUtilisé.transform.Find("le nom").gameObject.GetComponent<Image>().sprite = décoration.nomY1;
            } else {
                blocNomUtilisé.transform.Find("le nom").gameObject.GetComponent<Image>().sprite = décoration.nomH1;
            }
        }
        if (fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.yokai || fragmentDeDialogue[position].quiJeSuis == QuiJeSuis.héroYoKai){
            bulle.transform.Find("boutDroit").GetComponent<Image>().sprite = décoration.boutDroiteGris;
            bulle.transform.Find("intérieur").GetComponent<Image>().sprite = décoration.centreGris;
            bulle.transform.Find("boutGauche").GetComponent<Image>().sprite = décoration.boutGaucheGris;
        } else {
            bulle.transform.Find("boutDroit").GetComponent<Image>().sprite = décoration.boutDroiteRose;
            bulle.transform.Find("intérieur").GetComponent<Image>().sprite = décoration.centreRose;
            bulle.transform.Find("boutGauche").GetComponent<Image>().sprite = décoration.boutGaucheRose;   
        }



        blocNomUtilisé.SetActive(true);
        espaceDialogue = bulle.transform.Find("texte dialogue").gameObject.GetComponent<TextMeshProUGUI>();
        espaceNom = blocNomUtilisé.transform.Find("texte nom").gameObject.GetComponent<TextMeshProUGUI>();
        espaceImage = blocNomUtilisé.transform.Find("image yo-kai si possible").gameObject.GetComponent<Image>();
        if (espaceDialogue == null || espaceNom == null || espaceImage == null){
            Debug.LogWarning("Un des espace nécessaire est manquant");
        }

        if (fragmentDeDialogue[position].nom.Length > 0){
            espaceNom.gameObject.SetActive(true);
            blocNomUtilisé.transform.Find("le nom").gameObject.SetActive(true);
            espaceNom.text = fragmentDeDialogue[position].nom;
            if (fragmentDeDialogue[position].image != null){
                espaceImage.gameObject.SetActive(true);
                espaceImage.sprite = fragmentDeDialogue[position].image;
                // espaceImage.SetNativeSize();
                float ratio = espaceImage.sprite.rect.width / espaceImage.sprite.rect.height;
                float nouvelleLargeur = espaceImage.rectTransform.rect.height * ratio;
                espaceImage.rectTransform.sizeDelta = new Vector2(nouvelleLargeur, espaceImage.rectTransform.sizeDelta.y);
            } else {
                espaceImage.gameObject.SetActive(false);
            }
        } else {
            espaceNom.gameObject.SetActive(false);
            blocNomUtilisé.transform.Find("le nom").gameObject.SetActive(false);
            espaceImage.gameObject.SetActive(false);
        }
    }

    void AvancerJusqueAuProchainDialogue(){
        // cette fonction permet seulement de sauter les structures conditionelles.
        for (int i = position; i < fragmentDeDialogue.Count; i++){
            if (fragmentDeDialogue[position].type != TypeDialogue.conditionDePassage){
                return;
            }
            position++;
        }
        if (position >= fragmentDeDialogue.Count){
            Fermeture(false);
            peutCliquer = false;
        }
    }
}

public enum TypeDialogue{
    texte,
    cadeau,
    conditionDePassage
}

public enum QuiJeSuis{
    humain,
    yokai,
    héro,
    héroYoKai
}

public enum TypeObjet{
    nouriture,
    équipement,
    annimaux
}


[System.Serializable]
public class Décoration{
    public Sprite boutGaucheGris;
    public Sprite centreGris;
    public Sprite boutDroiteGris;

    public Sprite boutGaucheRose;
    public Sprite centreRose;
    public Sprite boutDroiteRose;

    public Sprite nomY1;
    public Sprite nomY2;
    public Sprite nomH1;
    public Sprite nomH2;
}

[System.Serializable]
public class FragmentDeDialogue{
    public int IdÉlément;
    public TypeDialogue type;
    public string phrase;
    public QuiJeSuis quiJeSuis;
    public string nom;
    public Sprite image;
    [Min(1)]public int quantitée = 1;//si ces un cadeau, il y a une quantitée
    public TypeObjet typeObjet;
}