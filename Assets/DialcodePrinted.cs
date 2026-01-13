/*
Les balises :
<PNAME> : nom du héro

quand on dit non à un choix, ça enregistre 0 sinon si ces oui, se sera 1
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialcodePrinted : MonoBehaviour{
    public List<GameObject> listeObjetACacherPendantDialogue;
    private List<GameObject> objetMasqueADémasquer = new List<GameObject>();

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
    private GameObject information;
    private GameObject choix;
    private GameObject choixBoutique;
    private GameObject boutiqueAcheter;
    private int auteurConditionnelle;
    private int auteurConditionnelleRéel;
    private int positionChoix;
    private bool choixEnCours;
    private bool choixEnCoursBoutique1;
    private bool choixEnCoursBoutique2;
    private int cibleChoixBoutique1;
    private GameObject imageDePrésentation;
    private int nChoix;
    public static DialcodePrinted Instance;
    public bool dansUneZoneDeDialogue;

    void Awake(){
        if (Instance == null){
            Instance = this;
        } else {
            Destroy(this);
        }
        dansUneZoneDeDialogue = false;
    }
    void Start(){
        bulle = transform.Find("-- BULLE --").gameObject;
        cadeau = transform.Find("-- OFFRE --").gameObject;
        information = transform.Find("-- INFO --").gameObject;
        choix = transform.Find("-- CHOIX --").gameObject;
        choixBoutique = transform.Find("-- CHOIX BOUTIQUE --").gameObject;
        boutiqueAcheter = transform.Find("-- ACHETER --").gameObject;
        imageDePrésentation = bulle.transform.Find("imageDePrésentation").gameObject;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        Fermeture(true);
    }

    void Update(){
        if (dansUneZoneDeDialogue){
            if (choixEnCours) {
                if (Input.GetButtonDown("flèche haut") || Input.GetButtonDown("flèche bas")){
                    positionChoix++;
                    ChangerPositionChoix();
                } else if (Input.GetButtonDown("A")){
                    string valeur = "0";
                    if (positionChoix == 0){
                        valeur = "1";
                    }
                    NewDataCenter.Instance.SauvegarderSTR("choix-numéro-"+nChoix, valeur);
                    nChoix++;
                    position++;
                    choixEnCours = false;
                }
            }
            if (choixEnCoursBoutique1) {
                if (Input.GetButtonDown("flèche haut") || Input.GetButtonDown("flèche bas")){
                    ChangerPositionCibleChoixBoutique1(1);
                } else if (Input.GetButtonDown("A")){
                    choixEnCoursBoutique1 = false;
                    if(cibleChoixBoutique1 == 0){
                        choixEnCoursBoutique2 = true;
                        boutiqueAcheter.GetComponent<BoutiqueDialcode>().produitAllimentaire = fragmentDeDialogue[position].listeProduitBoutique;
                        boutiqueAcheter.GetComponent<BoutiqueDialcode>().acheterUser = true;
                        StartCoroutine(boutiqueAcheter.GetComponent<BoutiqueDialcode>().Initialisation());
                        OuvertureFenêtre(boutiqueAcheter);
                    } else if(cibleChoixBoutique1 == 1){
                        choixEnCoursBoutique2 = true;
                        List<int> listeDesIDNouriturePossédé = new List<int>();
                        for (int i = 0; i < Item._instance.listeDesNouritures.Count; i++){
                            if (int.TryParse(NewDataCenter.Instance.Lire(Item._instance.listeDesNouritures[i].nom), out int quantite) && quantite > 0){
                                listeDesIDNouriturePossédé.Add(i);
                            }
                        }
                        boutiqueAcheter.GetComponent<BoutiqueDialcode>().produitAllimentaire = listeDesIDNouriturePossédé;
                        boutiqueAcheter.GetComponent<BoutiqueDialcode>().acheterUser = false;
                        StartCoroutine(boutiqueAcheter.GetComponent<BoutiqueDialcode>().Initialisation());
                        OuvertureFenêtre(boutiqueAcheter);
                    }
                }
            } if (choixEnCoursBoutique2){
                if (Input.GetButtonDown("B") && boutiqueAcheter.GetComponent<BoutiqueDialcode>().acheterUnProduitFenêtreOuverte == false){
                    choixEnCoursBoutique2 = false;
                    position++;
                    Action();
                }
            }
            if (!choixEnCours && !choixEnCoursBoutique1 && !choixEnCoursBoutique2){
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
        }
    }

    void Action(){
        AvancerJusqueAuProchainDialogue();
        if (fragmentDeDialogue.Count <= position){
            Fermeture(false);
            return;
        }
        FragmentDeDialogue fragment = fragmentDeDialogue[position];
        fragment.nom = fragment.nom.Replace("<PNAME>",NewDataCenter.Instance.Lire("nomDuJoueur"));
        fragment.phrase = fragment.phrase.Replace("<PNAME>",NewDataCenter.Instance.Lire("nomDuJoueur"));
        if (fragment.quiJeSuis == QuiJeSuis.yokai || fragment.quiJeSuis == QuiJeSuis.héroYoKai){
            if (fragment.nom.Length == 0) fragment.nom = medaliumDictionnaire._instance.dictionnaire[fragment.IdÉlément].nom;
            if (fragment.image == null) fragment.image = medaliumDictionnaire._instance.dictionnaire[fragment.IdÉlément].faceImage;
        }
        if (fragment.type == TypeDialogue.cadeau || fragment.type == TypeDialogue.volDiscret || fragment.type == TypeDialogue.donDiscret || fragment.type == TypeDialogue.vol){
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
            OuvertureFenêtre(bulle);
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
            OuvertureFenêtre(cadeau);
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
        } else if (fragment.type == TypeDialogue.vol){
            OuvertureFenêtre(cadeau);
            cadeau.transform.Find("cintille").gameObject.SetActive(false);
            string phrase = "Tu as remis :";
            if (fragment.nom.Length > 0){
                if (fragment.quantitée == 1) phrase = "Tu as remis : <color=#4C714F>\n"+fragment.nom+"</color>.";
                else phrase = "Tu as remis : <color=#4C714F>\n"+fragment.quantitée +" "+ fragment.nom+"</color>.";
            }
            NewDataCenter.Instance.SauvegarderINT(fragment.nom,fragment.quantitée*(-1));
            //Utiliser <C12><ITEM_NAME></C> sur qui ?
            cadeau.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>().text = phrase;
            position++;
        } else if (fragment.type == TypeDialogue.volDiscret){
            Vole(fragment.nom, fragment.quantitée);
            position++;
            AvancerJusqueAuProchainDialogue();
            Action();
        } else if (fragment.type == TypeDialogue.donDiscret){
            DonDiscret(fragment.nom, fragment.quantitée);
            position++;
            AvancerJusqueAuProchainDialogue();
            Action();
        } else if (fragment.type == TypeDialogue.information){
            OuvertureFenêtre(information);
            information.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>().text = fragment.phrase;
            Sprite image = décoration.spritePointExclamatif;
            if(!fragment.cesUneExclamation) image = décoration.spritePointIntérogatif;
            information.transform.Find("icone").gameObject.GetComponent<Image>().sprite = image;
            position++;
        } else if (fragment.type == TypeDialogue.choix){
            OuvertureFenêtre(choix);
            choixEnCours = true;
            positionChoix = 1;
            ChangerPositionChoix();
            //le choix nécessite d'une clef de sauvegarde temporaire, cette clef est ce qui est écrit dans "nom";
        } else if (fragment.type == TypeDialogue.boutique){
            OuvertureFenêtre(choixBoutique);
            choixEnCoursBoutique1 = true;
            choixEnCoursBoutique2 = false;
            cibleChoixBoutique1 = 0;
            ChangerPositionCibleChoixBoutique1(0);
        }
    }

    void ChangerPositionCibleChoixBoutique1(int suplémant){
        cibleChoixBoutique1=(((cibleChoixBoutique1+suplémant)%2)+2)%2;
        choixBoutique.transform.Find("Bouton1/fond-actif").gameObject.SetActive(cibleChoixBoutique1 == 0);
        choixBoutique.transform.Find("Bouton1/fond-inactif").gameObject.SetActive(cibleChoixBoutique1 != 0);
        choixBoutique.transform.Find("Bouton2/fond-actif").gameObject.SetActive(cibleChoixBoutique1 == 1);
        choixBoutique.transform.Find("Bouton2/fond-inactif").gameObject.SetActive(cibleChoixBoutique1 != 1);
    }

    void OuvertureFenêtre(GameObject fenêtre){
        bulle.SetActive(bulle == fenêtre);
        cadeau.SetActive(cadeau == fenêtre);
        information.SetActive(information == fenêtre);
        choix.SetActive(choix == fenêtre);
        choixBoutique.SetActive(choixBoutique == fenêtre);
        boutiqueAcheter.SetActive(boutiqueAcheter == fenêtre);
    }

    void ChangerPositionChoix(){
        positionChoix%=2;
        choix.transform.Find("Bouton1/fond-actif").gameObject.SetActive(positionChoix == 0);
        choix.transform.Find("Bouton1/fond-inactif").gameObject.SetActive(positionChoix != 0);
        choix.transform.Find("Bouton2/fond-actif").gameObject.SetActive(positionChoix == 1);
        choix.transform.Find("Bouton2/fond-inactif").gameObject.SetActive(positionChoix != 1);
    }

    void Ouverture(){
        nChoix = 1;
        boiteFermer = false;
        auteurConditionnelle = 0;
        auteurConditionnelleRéel = 0;
        choixEnCoursBoutique1 = false;
        choixEnCoursBoutique2 = false;
        cibleChoixBoutique1 = 0;
        StartCoroutine(AnnimationOuverture());
        position = 0;
        if (fragmentDeDialogue.Count == 0){
            Debug.LogWarning("Il n'y a pas de phrase pour le dialogue !");
        }
        objetMasqueADémasquer.Clear();
        foreach (GameObject item in listeObjetACacherPendantDialogue){
            if (item.activeSelf == true){
                objetMasqueADémasquer.Add(item);
                item.SetActive(false);
            }
        }
        mouvementDuJoueur.droitDeMarcher = false;
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
        foreach (GameObject item in objetMasqueADémasquer){
            item.SetActive(true);
        }
        objetMasqueADémasquer.Clear();
        mouvementDuJoueur.droitDeMarcher = true;
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
                NePasDéformerImage(espaceImage);
            } else {
                espaceImage.gameObject.SetActive(false);
            }
        } else {
            espaceNom.gameObject.SetActive(false);
            blocNomUtilisé.transform.Find("le nom").gameObject.SetActive(false);
            espaceImage.gameObject.SetActive(false);
        }

        if (fragmentDeDialogue[position].imageDePrésentation != null){
            imageDePrésentation.SetActive(true);
            imageDePrésentation.GetComponent<Image>().sprite = fragmentDeDialogue[position].imageDePrésentation;
            NePasDéformerImage(imageDePrésentation.GetComponent<Image>());
        } else {
            imageDePrésentation.SetActive(false);
        }
    }

    void NePasDéformerImage(Image image){
        // image.SetNativeSize();
        float ratio = image.sprite.rect.width / image.sprite.rect.height;
        float nouvelleLargeur = image.rectTransform.rect.height * ratio;
        image.rectTransform.sizeDelta = new Vector2(nouvelleLargeur, image.rectTransform.sizeDelta.y);
    }

    void AvancerJusqueAuProchainDialogue(){
        // cette fonction permet seulement de sauter les structures conditionelles.
        for (int i = position; i < fragmentDeDialogue.Count; i++){
            if (fragmentDeDialogue[position].type != TypeDialogue.conditionDePassage && fragmentDeDialogue[position].type != TypeDialogue.finDeCondition){
                if (fragmentDeDialogue[position].type == TypeDialogue.nul){
                    position++;
                    if (position >= fragmentDeDialogue.Count){
                        Fermeture(false);
                        peutCliquer = false;
                        return;
                    }
                }
                if (fragmentDeDialogue[position].auteurConditionnelle <= auteurConditionnelle){
                    return;
                } //else {
                //     Debug.Log("On as pas vu ça comme ma hauteur est "+auteurConditionnelle +" et la scienne est "+fragmentDeDialogue[position].auteurConditionnelle);
                // }
            }
            if (fragmentDeDialogue[position].type == TypeDialogue.conditionDePassage){
                bool valeur = RéponceALaCondition();
                if (valeur){
                    auteurConditionnelle++;
                }
                auteurConditionnelleRéel++;
            } else if (fragmentDeDialogue[position].type == TypeDialogue.finDeCondition){
                auteurConditionnelleRéel--;
                if (auteurConditionnelleRéel < auteurConditionnelle){
                    auteurConditionnelle=auteurConditionnelleRéel;
                }
            }
            position++;
        }
        if (position >= fragmentDeDialogue.Count){
            Fermeture(false);
            peutCliquer = false;
        }
    }

    bool RéponceALaCondition() {
        FragmentDeDialogue f = fragmentDeDialogue[position];
        if (f.conditionnelle == null || f.conditionnelle.Count == 0) return true;

        List<bool> valeurs = new List<bool>();
        foreach (Conditionnelle c in f.conditionnelle) {
            valeurs.Add(EvaluerConditionSimple(c));
        }

        return CalculerLogiqueComplexe(valeurs, f.conditionnelle);
    }

    bool EvaluerConditionSimple(Conditionnelle c) {
        string v1s = c.nomV1;
        string v2s = c.nomV2;
        bool nombre = int.TryParse(c.nomV1, out int tt);
        if (!nombre){
            v1s = NewDataCenter.Instance.Lire(c.nomV1);
        }
        nombre = int.TryParse(c.nomV2, out tt);
        if (!nombre){
            v2s = NewDataCenter.Instance.Lire(c.nomV2);
        }

        bool isV1Num = int.TryParse(v1s, out int v1);
        bool isV2Num = int.TryParse(v2s, out int v2);

        // Debug.Log(v1+"  -  "+v2);

        switch (c.opérateur) {
            case Opérateur.supérieur: return isV1Num && isV2Num && v1 > v2;
            case Opérateur.supérieurOuÉgale: return isV1Num && isV2Num && v1 >= v2;
            case Opérateur.inférieur: return isV1Num && isV2Num && v1 < v2;
            case Opérateur.inférieurOuÉgale: return isV1Num && isV2Num && v1 <= v2;
            case Opérateur.égale: return v1s == v2s;
            case Opérateur.different: return v1s != v2s;
            default: return false;
        }
    }

    bool CalculerLogiqueComplexe(List<bool> valeurs, List<Conditionnelle> conditions) {
        int index = 0;
        return EvaluerRecursif(valeurs, conditions, ref index);
    }

    private bool EvaluerRecursif(List<bool> valeurs, List<Conditionnelle> conditions, ref int index) {
        bool resultatAccumule = true;
        OpérateurEtape2 dernierOp = OpérateurEtape2.et;

        while (index < valeurs.Count) {
            Conditionnelle c = conditions[index];
            bool valeurActuelle;

            if (c.opérateurEtape2 == OpérateurEtape2.ouvertureParentaise) {
                index++;
                valeurActuelle = EvaluerRecursif(valeurs, conditions, ref index);
            } else {
                valeurActuelle = valeurs[index];
            }

            if (dernierOp == OpérateurEtape2.et) {
                resultatAccumule = resultatAccumule && valeurActuelle;
            } else if (dernierOp == OpérateurEtape2.ou) {
                resultatAccumule = resultatAccumule || valeurActuelle;
            }

            if (c.opérateurEtape2 == OpérateurEtape2.fermetureParentaise) {
                return resultatAccumule;
            }

            dernierOp = c.opérateurEtape2;
            index++;
        }

        return resultatAccumule;
    }

    void Vole(string nom, int quantitée){
        //ce n'est pas vraiment un vol mais le fait de prendre quelque chose, par exemple, prendre de l'argent si l'utilisateur doit payer un truc.
        NewDataCenter.Instance.SauvegarderINT(nom, quantitée*(-1));
    }
    void DonDiscret(string nom, int quantitée){
        NewDataCenter.Instance.SauvegarderINT(nom, quantitée);
    }
}

public enum TypeDialogue{
    texte,
    cadeau,
    conditionDePassage,
    finDeCondition,
    volDiscret,
    donDiscret,
    information,
    choix,
    vol,
    boutique,
    nul
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

public enum Opérateur{
    supérieur,
    supérieurOuÉgale,
    inférieur,
    inférieurOuÉgale,
    égale,
    different
}

public enum OpérateurEtape2{
    et,
    ou,
    ouvertureParentaise,
    fermetureParentaise
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
    public Sprite spritePointExclamatif;
    public Sprite spritePointIntérogatif;
}

[System.Serializable]
public class FragmentDeDialogue{
    public int auteurConditionnelle;
    public int IdÉlément;
    public TypeDialogue type;
    public string phrase;
    public QuiJeSuis quiJeSuis;
    public string nom;
    public Sprite image;
    [Min(1)]public int quantitée = 1;//si ces un cadeau, il y a une quantitée
    public TypeObjet typeObjet;
    [Header("Opération")]
    public List<Conditionnelle> conditionnelle = new List<Conditionnelle>();
    public bool cesUneExclamation;
    public Sprite imageDePrésentation;
    public List<int> listeProduitBoutique = new List<int>();
}

[System.Serializable]
public class Conditionnelle{
    public string nomV1;
    public Opérateur opérateur;
    public string nomV2;
    public OpérateurEtape2 opérateurEtape2;
}