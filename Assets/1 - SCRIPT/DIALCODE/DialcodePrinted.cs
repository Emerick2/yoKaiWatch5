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
    private GameObject quete;
    private int auteurConditionnelle;
    private int auteurConditionnelleRéel;
    private int positionChoix;
    private bool choixEnCours;
    private bool choixEnCoursBoutique1;
    private bool choixEnCoursBoutique2;
    private int awaitTheClicForTheEndQuest = 0;//0 : valeur par défaut / 1 : en attente de la fin de la pause / 2 : en attente du clique.
    private int cibleChoixBoutique1;
    private GameObject imageDePrésentation;
    private int nChoix;
    public static DialcodePrinted Instance;
    public bool dansUneZoneDeDialogue;
    public bool discussionInProgress;

    void Awake(){
        if (Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
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
        quete = transform.Find("-- QUÊTE --").gameObject;
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
                if (Input.GetButtonDown("up") || Input.GetButtonDown("down")){
                    positionChoix = (positionChoix + 1) % 2;
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
                    // Action();
                }
            }
            if (choixEnCoursBoutique1) {
                if (Input.GetButtonDown("up") || Input.GetButtonDown("down")){
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
            if (awaitTheClicForTheEndQuest == 2){
                if (Input.GetButtonDown("A")){
                    quete.transform.Find("super").GetComponent<Animator>().SetBool("afficher", false);
                    awaitTheClicForTheEndQuest = 0;
                }
            }
            if (awaitTheClicForTheEndQuest == 0 && !choixEnCours && !choixEnCoursBoutique1 && !choixEnCoursBoutique2){
                if (Input.GetButtonDown("A")){
                    Input.ResetInputAxes();
                    if (boiteFermer){
                        Ouverture();
                        peutCliquer = false;
                        Action();
                    } else if (position >= fragmentDeDialogue.Count){
                        Fermeture(false);
                        peutCliquer = false;
                    } else if (peutCliquer) {
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
        if ((fragment.quiJeSuis == QuiJeSuis.yokai || fragment.quiJeSuis == QuiJeSuis.héroYoKai) && fragment.IdÉlément>0){
            if (fragment.nom.Length == 0) fragment.nom = medaliumDictionnaire._instance.ServantYoKai(fragment.IdÉlément).Nom;
            if (fragment.image == null) fragment.image = medaliumDictionnaire._instance.ServantYoKai(fragment.IdÉlément).faceImage;
        }
        if (fragment.type == TypeDialogue.cadeau || fragment.type == TypeDialogue.volDiscret || fragment.type == TypeDialogue.donDiscret || fragment.type == TypeDialogue.vol){
            if (fragment.quantitée <= 0) fragment.quantitée = 1;
            if (fragment.typeObjet == CatégorieItem.Nouriture){
                fragment.nom = Item._instance.listeDesNouritures[fragment.IdÉlément].nom;
                fragment.image = Item._instance.listeDesNouritures[fragment.IdÉlément].image;
            } else if (fragment.typeObjet == CatégorieItem.Equipement){
                fragment.nom = Item._instance.listeDesÉquipements[fragment.IdÉlément].nom;
                fragment.image = Item._instance.listeDesÉquipements[fragment.IdÉlément].sprite;
            } else if (fragment.typeObjet == CatégorieItem.Animal){
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
        } else if (fragment.type == TypeDialogue.cadeau || fragment.type == TypeDialogue.donDiscret || fragment.type == TypeDialogue.vol || fragment.type == TypeDialogue.volDiscret){
            RecevoirUnCadeau(fragment.type, fragment.quantitée, fragment.typeObjet, fragment.IdÉlément, fragment.image, fragment.nom, true);
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
        } else if (fragment.type == TypeDialogue.quete){
            OuvertureFenêtre(quete);
            // la valeur de sauvegarde est :
            // 0 : La requête n'as pas commancer.
            // 1 : La requête est en cours.
            // 2 : La requête est terminée.
            quete.transform.Find("icone requête").gameObject.SetActive(fragment.categorieQuete == CatégorieQuête.Quête);
            quete.transform.Find("icone service").gameObject.SetActive(fragment.categorieQuete == CatégorieQuête.Service);

            if (!fragment.endQuest){                    
                NewDataCenter.Instance.SauvegarderSTR(fragment.categorieQuete.ToString()+"-"+fragment.nom,"1");
                if (fragment.categorieQuete == CatégorieQuête.Quête || fragment.categorieQuete == CatégorieQuête.Histoire || fragment.categorieQuete == CatégorieQuête.Clef){
                    quete.transform.Find("Texte").GetComponent<TextMeshProUGUI>().text = "\""+fragment.nom+"\" commance !\nLa quête à été ajouter au menu des requêtes.";
                } else if (fragment.categorieQuete == CatégorieQuête.Service) {
                    quete.transform.Find("Texte").GetComponent<TextMeshProUGUI>().text = "\""+fragment.nom+"\" commance !\nLe service à été ajouter au menu des requêtes.";
                } 
            } else {
                StartCoroutine(AnimationEndQuest());
                NewDataCenter.Instance.SauvegarderSTR(fragment.categorieQuete.ToString()+"-"+fragment.nom,"2");
                quete.transform.Find("Texte").GetComponent<TextMeshProUGUI>().text = "\""+fragment.nom+"\" est terminé !";
                /*
                ajouter le script movie.
                il vas :
                    - faire apparaitre un écran sur la scène
                    - bloquer les mouvement du joueur
                    - faire visioner une vidéo
                    - avoir un booléen pour savoir si la vidéo à terminé de se jouer ou non.
                                
                notre scirpt :
                    - vas faire apparaitre wisper content sur la bulle de dialogue
                    - vas jouer la musique "bien jouer"
                    - quand je joueur cliqueras sur A, le script déclancheras le movie
                    - toutes les 0.3f, le script vas regarder si la vidéo est fini
                    - si la vidéo est fini, il fait fermer le movie (movie à une variable start et end).
                    - on affiche les récompanse du joueur et les niveau gagner par l'équipe
                    - on poursuit le dialogue.
                */
            }
            position++;
        } else if (fragment.type == TypeDialogue.infoQuest) {
            QuestMenuInformation q = fragment.infoQuest;
            Debug.Log("Ici !");
            if (q.name != ""){
                if (q.description != "") NewDataCenter.Instance.SauvegarderSTR("Quête_description%"+q.name, q.description);
                if (q.objective != "") NewDataCenter.Instance.SauvegarderSTR("Quête_objectif%"+q.name, q.objective);
                if (q.level > 0) NewDataCenter.Instance.SauvegarderSTR("Quête_niveau%"+q.name, q.level.ToString());
                if (q.xp > 0) NewDataCenter.Instance.SauvegarderSTR("Quête_xp%"+q.name, q.xp.ToString());
            }
            position++;
            AvancerJusqueAuProchainDialogue();
            if (position < fragmentDeDialogue.Count) Action();
        }
    }

    /// <summary>
    /// Si ont appelle se script hors du dialogue, il faut appeller OuvertureFenêtre();
    /// </summary>
    /// <param name="type">le type de cadeau</param>
    /// <param name="quantitée">la quantitée à donnée</param>
    /// <param name="typeObjet">la catégorie de l'objet donné</param>
    /// <param name="IdÉlément">l'id de l'élément donné</param>
    /// <param name="image">l'image de l'élément (facultatif)</param>
    /// <param name="nom">le nom de l'élément (facultatif)</param>
    /// <param name="dansUnDialogue">c'est sur true si sa fait partie d'un dialogue</param>
    public void RecevoirUnCadeau(TypeDialogue type, int quantitée, CatégorieItem typeObjet, int IdÉlément, Sprite image = null, string nom = "", bool dansUnDialogue = false){
        Debug.Log(nom);
        canvasGroup.alpha = 1f;
        if (quantitée <= 0) quantitée = 1;
        if (typeObjet == CatégorieItem.Nouriture){
            nom = Item._instance.listeDesNouritures[IdÉlément].nom;
            image = Item._instance.listeDesNouritures[IdÉlément].image;
        } else if (typeObjet == CatégorieItem.Equipement){
            nom = Item._instance.listeDesÉquipements[IdÉlément].nom;
            image = Item._instance.listeDesÉquipements[IdÉlément].sprite;
        } else if (typeObjet == CatégorieItem.Animal){
            nom = Item._instance.listeDesAnnimaux[IdÉlément].nom;
            image = Item._instance.listeDesAnnimaux[IdÉlément].image;
        }

        if (type == TypeDialogue.cadeau){
            OuvertureFenêtre(cadeau);
            if (image != null){
                cadeau.transform.Find("cintille").gameObject.SetActive(true);
                cadeau.transform.Find("cintille/cadeau").GetComponent<Image>().sprite = image;
            } else {
                cadeau.transform.Find("cintille").gameObject.SetActive(false);
            }
            string phrase = "Tu as obtenu un cadeau !";
            if (nom.Length > 0){
                if (quantitée == 1) phrase = "Tu as obtenu : <color=#4C714F>\n"+nom+"</color> !";
                else phrase = "Tu as obtenu : <color=#4C714F>\n"+quantitée +" "+ nom+"</color> !";
            }
            NewDataCenter.Instance.SauvegarderINT(nom,quantitée);
            //Utiliser <C12><ITEM_NAME></C> sur qui ?
            cadeau.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>().text = phrase;
            if (dansUnDialogue) position++;
        } else if (type == TypeDialogue.vol){
            if (int.Parse(NewDataCenter.Instance.Lire(nom)) > 0){
                OuvertureFenêtre(cadeau);
                cadeau.transform.Find("cintille").gameObject.SetActive(false);
                string phrase = "Tu as remis :";
                if (nom.Length > 0){
                    if (quantitée == 1) phrase = "Tu as remis : <color=#4C714F>\n"+nom+"</color>.";
                    else phrase = "Tu as remis : <color=#4C714F>\n"+quantitée +" "+ nom+"</color>.";
                }
                NewDataCenter.Instance.SauvegarderINT(nom,quantitée*(-1));
                //Utiliser <C12><ITEM_NAME></C> sur qui ?
                cadeau.transform.Find("Texte").gameObject.GetComponent<TextMeshProUGUI>().text = phrase;
            }
            if (dansUnDialogue) position++;
        } else if (type == TypeDialogue.volDiscret){
            if (int.Parse(NewDataCenter.Instance.Lire(nom)) > 0){
                Vole(nom, quantitée);
            }
            if (dansUnDialogue) {
                position++;
                AvancerJusqueAuProchainDialogue();
                Action();
            }
        } else if (type == TypeDialogue.donDiscret){
            Debug.Log(nom);
            DonDiscret(nom, quantitée);
            if (dansUnDialogue) {
                position++;
                AvancerJusqueAuProchainDialogue();
                Action();
            }
        }
    }

    IEnumerator AnimationEndQuest(){
        awaitTheClicForTheEndQuest = 1;
        AudioClip audioClip = Resources.Load<AudioClip>("Musique/évent/Quest Complete");
        GestionnaireMusique._Komasan.JouerUnEffetSonore(audioClip,false);
        quete.transform.Find("miniBlocContinuer").gameObject.SetActive(false);
        yield return new WaitForSeconds(4f);
        //faire apparaitre le wisper
        quete.transform.Find("super").GetComponent<Animator>().SetBool("afficher", true);
        yield return new WaitForSeconds(0.5f);
        quete.transform.Find("miniBlocContinuer").gameObject.SetActive(true);
        awaitTheClicForTheEndQuest = 2;
        //permettre à l'utilisateur de cliquer pour continuer
        //new ServantMovie().StartMovie(this, "test");
    }

    void ChangerPositionCibleChoixBoutique1(int suplémant){
        cibleChoixBoutique1=(((cibleChoixBoutique1+suplémant)%2)+2)%2;
        choixBoutique.transform.Find("Bouton1/fond-actif").gameObject.SetActive(cibleChoixBoutique1 == 0);
        choixBoutique.transform.Find("Bouton1/fond-inactif").gameObject.SetActive(cibleChoixBoutique1 != 0);
        choixBoutique.transform.Find("Bouton2/fond-actif").gameObject.SetActive(cibleChoixBoutique1 == 1);
        choixBoutique.transform.Find("Bouton2/fond-inactif").gameObject.SetActive(cibleChoixBoutique1 != 1);
    }

    void OuvertureFenêtre(GameObject fenêtre = null){
        if (fenêtre != null){
            bulle.SetActive(bulle == fenêtre);
            cadeau.SetActive(cadeau == fenêtre);
            information.SetActive(information == fenêtre);
            choix.SetActive(choix == fenêtre);
            choixBoutique.SetActive(choixBoutique == fenêtre);
            boutiqueAcheter.SetActive(boutiqueAcheter == fenêtre);
            quete.SetActive(quete == fenêtre);
        } else {
            bulle.SetActive(false);
            cadeau.SetActive(false);
            information.SetActive(false);
            choix.SetActive(false);
            choixBoutique.SetActive(false);
            boutiqueAcheter.SetActive(false);
            quete.SetActive(false);
        }
    }

    void ChangerPositionChoix(){
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
        awaitTheClicForTheEndQuest = 0;
        position = 0;
        discussionInProgress = true;
        StartCoroutine(AnnimationOuverture());
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
    public void Fermeture(bool rapide){
        boiteFermer = true;
        discussionInProgress = false;
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

    void AvancerJusqueAuProchainDialogue() {
        while (position < fragmentDeDialogue.Count) {
            FragmentDeDialogue dialogueActuel = fragmentDeDialogue[position];

            if (dialogueActuel.type != TypeDialogue.conditionDePassage && dialogueActuel.type != TypeDialogue.finDeCondition) {
                if (dialogueActuel.type == TypeDialogue.nul) {
                    position++;
                    if (position >= fragmentDeDialogue.Count) break;
                    dialogueActuel = fragmentDeDialogue[position];
                }
                
                if (dialogueActuel.auteurConditionnelle <= auteurConditionnelle) {
                    return; 
                }
            }

            if (dialogueActuel.type == TypeDialogue.conditionDePassage) {
                bool valeur = RéponceALaCondition();
                
                if (valeur) {
                    auteurConditionnelle++;
                } else {
                    int niveauASauter = auteurConditionnelleRéel;
                    while (position + 1 < fragmentDeDialogue.Count && 
                        !(fragmentDeDialogue[position + 1].type == TypeDialogue.finDeCondition && 
                            fragmentDeDialogue[position + 1].auteurConditionnelle == niveauASauter)) {
                        position++;
                    }
                }
                auteurConditionnelleRéel++;
            } 
            else if (dialogueActuel.type == TypeDialogue.finDeCondition) {
                auteurConditionnelleRéel--;
                if (auteurConditionnelleRéel < auteurConditionnelle) {
                    auteurConditionnelle = auteurConditionnelleRéel;
                }
            }
            position++;
        }

        if (position >= fragmentDeDialogue.Count) {
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

    public bool EvaluerConditionSimple(Conditionnelle c) {
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

    public bool CalculerLogiqueComplexe(List<bool> valeurs, List<Conditionnelle> conditions) {
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

