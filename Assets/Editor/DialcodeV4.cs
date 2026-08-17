using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class DialcodeV4 : EditorWindow{
    private int hauteurDeToutLesElement = 20;
    private int marge = 10;

    private GUIStyle monStyleChampDeSaisie;
    private GUIStyle monStyleTitreElement;
    private GUIStyle styleCadre;
    private GUIStyle monStyleHelpBox;
    private GUIStyle monStyleInfo;

    private bool styleChargé = false;
    private int nombreDeChoix = 1;
    private List<FragmentDeDialogue> fragment = new List<FragmentDeDialogue>();
    private Vector2 scrollPosition;
    private int hauteurConditionnele = 0;
    private PNJDialcode selectedPNJ = null;
    private string lastSavedFile = "";

    private int nombrePage = 1;
    private int positionPage;

    private List<ConditionOfTheDialoguePage> conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>();

    private double nextSaveTime = 0;
    private const double SAVE_INTERVAL = 10.0;
    private bool sauvegardeAutomatique = false;
    private bool aDesModificationsNonSauvegardees = false;
    private bool vientDeSauvegarder = false;

    private Dictionary<Color, Texture2D> textureCache = new Dictionary<Color, Texture2D>();
    private int lastFocusedControl = 0;
    

    [MenuItem("Tools/DialcodeV4")]
    public static void ShowWindow(){
        GetWindow<DialcodeV4>("DialcodeV4");
    }

    private Texture2D GetCachedTex(Color col) {
        if (!textureCache.ContainsKey(col)) {
            textureCache[col] = MakeTex(2, 2, col);
        }
        return textureCache[col];
    }

    private void OnSelectionChange(){
        bool nothingHappened = true;
        GameObject go = Selection.activeGameObject;
        if (go != null){
            if (go.GetComponent<PNJDialcode>() != null){
                if (!GUI.changed && !aDesModificationsNonSauvegardees) nextSaveTime = EditorApplication.timeSinceStartup + SAVE_INTERVAL;
                selectedPNJ = go.GetComponent<PNJDialcode>();
                positionPage = 0;
                LireLaSauvegarde(positionPage);
                aDesModificationsNonSauvegardees = false;
                nothingHappened = false;
            }
        }
        if(nothingHappened){
            selectedPNJ = null;
            fragment.Clear();
            nombrePage = 1;
            positionPage = 0;
            conditionOfTheDialoguePage.Clear();
            aDesModificationsNonSauvegardees = false;
        }
        Repaint();
    }

    void OnGUI(){
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.S) {
            EnregistrerPage(positionPage);
            Debug.Log("<color=grey>Sauvegarde manuelle (Ctrl+S) effectuée...</color>");
            e.Use();
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Éditeur de Dialogue : " + (selectedPNJ != null ? selectedPNJ.name : "Aucun PNJ sélectionné"));
        GUILayout.FlexibleSpace();
        sauvegardeAutomatique = GUILayout.Toggle(sauvegardeAutomatique, "Sauv. Auto (Temps)", EditorStyles.toolbarButton);
        EditorGUILayout.EndHorizontal();

        if (selectedPNJ == null){
            if (Selection.activeGameObject == null){
                EditorGUILayout.HelpBox("Veuillez sélectionner un GameObject avec le script PNJDialcode.", MessageType.Info);
            } else if (GUILayout.Button("Ajouter PNJDialcode à " + Selection.activeGameObject.name)){
                if (Selection.activeGameObject.GetComponent<PNJDialcode>() == null){
                    selectedPNJ = Selection.activeGameObject.AddComponent<PNJDialcode>();
                    OnSelectionChange();
                }
            }
        }

        DrawInterface();

        if (EditorGUI.EndChangeCheck()) {
            if (!vientDeSauvegarder) {
                aDesModificationsNonSauvegardees = true;
            }
            EditorUtility.SetDirty(this);
        }
        vientDeSauvegarder = false;

        if (sauvegardeAutomatique && (aDesModificationsNonSauvegardees || GUI.changed) && EditorApplication.timeSinceStartup > nextSaveTime) {
            EnregistrerPage(positionPage);
            Debug.Log("<color=grey>Sauvegarde automatique (temps) effectuée...</color>");
        }

        int currentFocus = GUIUtility.keyboardControl;
        if (currentFocus != lastFocusedControl) {
            if (aDesModificationsNonSauvegardees && lastFocusedControl != 0) {
                EnregistrerPage(positionPage, false);
                Debug.Log("<color=grey>Sauvegarde au changement de champ effectuée...</color>");
            }
            lastFocusedControl = currentFocus;
        }
    }

    private void OnLostFocus() {
        if (selectedPNJ != null && (aDesModificationsNonSauvegardees || GUI.changed)){
            EnregistrerPage(positionPage);
            Debug.Log("<color=grey>Sauvegarde au changement de sélection effectuée...</color>");
        }
    }

    private void OnDisable() {
        if (selectedPNJ != null) {
            try {
                EnregistrerPage(positionPage);
            } catch (Exception e) {
                Debug.LogWarning("Échec de la sauvegarde automatique à la fermeture : " + e.Message);
            }
        }
    }


    void PlacerEnTête(){
        GUILayout.BeginHorizontal();
        while (conditionOfTheDialoguePage.Count < nombrePage){
            conditionOfTheDialoguePage.Add(new ConditionOfTheDialoguePage());
        }
        positionPage = Mathf.Clamp(positionPage, 0, conditionOfTheDialoguePage.Count - 1);

        for (int i = 0; i < nombrePage; i++){
            GUILayout.BeginVertical();
            if (nombrePage > 1){
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    conditionOfTheDialoguePage.RemoveAt(i);
                    string folder = Application.dataPath + "/Resources/texte/";
                    string baseName = selectedPNJ.jsonPath;

                    string deletedPath = folder + baseName + "-page" + i + ".json";
                    if (File.Exists(deletedPath)) {
                        string archiveName = folder + baseName + "-page" + i + "-suprimerLe-" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".json";
                        File.Move(deletedPath, archiveName);
                    }

                    for (int next = i + 1; next < nombrePage; next++) {
                        string oldFile = folder + baseName + "-page" + next + ".json";
                        string newFile = folder + baseName + "-page" + (next - 1) + ".json";
        
                        if (File.Exists(oldFile)) {
                            File.Move(oldFile, newFile);
                        }
                    }

                    nombrePage--;
                    positionPage = Mathf.Clamp(positionPage, 0, nombrePage - 1);
                    SaveConditionOfTheDialoguePage();
                    LireLaSauvegarde(positionPage);
                    
                    AssetDatabase.Refresh();
                    
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUIUtility.ExitGUI(); 
                    return;
                    // GUI.color = Color.white;
                    // break; 
                }
                GUI.color = Color.white;
            }
            if (positionPage == i) GUI.color = Color.green;
            if (GUILayout.Button((i+1).ToString(), GUILayout.Width(50), GUILayout.Height(50))) {  
                if (GUI.changed || aDesModificationsNonSauvegardees) EnregistrerPage(positionPage);
                positionPage = i;      
                LireLaSauvegarde(positionPage);
                aDesModificationsNonSauvegardees = false;
                GUIUtility.ExitGUI();
                return;
            }
            if (positionPage == i) GUI.color = Color.white;
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }
        if (GUILayout.Button("+", GUILayout.Width(50), GUILayout.Height(50))) {    
            if (GUI.changed || aDesModificationsNonSauvegardees) EnregistrerPage(positionPage);
            nombrePage++;    
            conditionOfTheDialoguePage.Add(new ConditionOfTheDialoguePage());
            SaveConditionOfTheDialoguePage();
            positionPage = nombrePage-1;      
            fragment.Clear();
            LireLaSauvegarde(positionPage);
            aDesModificationsNonSauvegardees = false;
            GUIUtility.ExitGUI();
            return;
        }
        GUI.color = Color.cyan;
        if (GUILayout.Button("SAUVEGARDER", EditorStyles.toolbarButton, GUILayout.Width(100))) {
            EnregistrerPage(positionPage);
            Debug.Log("Dialogue sauvegardé manuellement !");
        }
        GUI.color = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (conditionOfTheDialoguePage.Count > 0 && positionPage < conditionOfTheDialoguePage.Count) {
            conditionOfTheDialoguePage[positionPage].visibleCharacter = AjouterUnBool("Personnage visible ?", conditionOfTheDialoguePage[positionPage].visibleCharacter);

            // condition :
            if (GUILayout.Button("+ Ajouter Condition")) {
                conditionOfTheDialoguePage[positionPage].listOfConditions.Add(new Conditionnelle());
                aDesModificationsNonSauvegardees = true;
            }
                    
            for (int j = 0; j < conditionOfTheDialoguePage[positionPage].listOfConditions.Count; j++){
                Conditionnelle c = conditionOfTheDialoguePage[positionPage].listOfConditions[j];
                Rect rectLabelTemporaire = CrééLeRect();
                GUILayout.BeginHorizontal();
                c.nomV1 =  AjoutTexte("Valeur", c.nomV1);
                c.opérateur = AjouterUnEnumGénérique("Opérateur", c.opérateur);
                c.nomV2 =  AjoutTexte("Valeur", c.nomV2);
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    conditionOfTheDialoguePage[positionPage].listOfConditions.RemoveAt(j);
                    j--;
                    aDesModificationsNonSauvegardees = true;
                }
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
                if (j+1 < conditionOfTheDialoguePage[positionPage].listOfConditions.Count){
                    c.opérateurEtape2 = AjouterUnEnumGénérique("Lien suite", c.opérateurEtape2);
                }                
            }
        }
    }

    void DrawInterface(){
        if (selectedPNJ == null) return;
        nombreDeChoix = 1;
        hauteurConditionnele = 0;
        if (!styleChargé) Style();
        // Style();
        Rect rectangleFenêtre = new Rect(0,0,position.width,position.height);
        Color backgroundColor = aDesModificationsNonSauvegardees ? new Color(0.5f, 0.5f, 0.86f) : Color.white;
        EditorGUI.DrawRect(rectangleFenêtre, backgroundColor);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();//<>
        GUILayout.Space(20);
        GUILayout.BeginVertical();//<>

        PlacerEnTête();

        for (int i = 0; i < fragment.Count; i++){
            FragmentDeDialogue f = null;
            if (fragment.Count < i+1){
                f = new FragmentDeDialogue();
                fragment.Add(f);
            } else {
                f = fragment[i];
            }
            ChoisirUneCouleurSelonLeType(f.type);
            GUILayout.BeginVertical(styleCadre); 

            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20))) {
                fragment.RemoveAt(i);
                GUI.changed = true;
                aDesModificationsNonSauvegardees = true;

                GUILayout.EndHorizontal(); 
                GUILayout.EndVertical(); 
                GUILayout.EndVertical(); 
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();

                Event.current.Use();
                return;
            }
            GUI.color = Color.white;
            GUI.color = Color.green;
            if (GUILayout.Button("V", GUILayout.Width(20))) {
                fragment.Insert(i, new FragmentDeDialogue());
                FragmentDeDialogue f1 = null;
                if (i > 1){
                    f1 = fragment[i-1];
                }
                SetingsTheNewFragment(f1, fragment[i]);
                i--;
                GUI.changed = true;
                aDesModificationsNonSauvegardees = true;

            }
            GUI.color = Color.white;          
            GUILayout.EndHorizontal();

            AjouterUnFragument(f);            
            GUILayout.EndVertical();

            GUILayout.Space(10);      
        }

        if (GUILayout.Button("Ajouter un nouveau bloc", GUILayout.Height(30))) {
            fragment.Add(new FragmentDeDialogue());
            FragmentDeDialogue f1 = null;
            if (fragment.Count > 1){
                f1 = fragment[fragment.Count-2];
            }
            SetingsTheNewFragment(f1, fragment[fragment.Count-1]);
            GUI.changed = true;
            aDesModificationsNonSauvegardees = true;
            Event.current.Use();
        }
        
        GUILayout.EndVertical();//</>
        GUILayout.EndHorizontal();//</>
        GUILayout.EndScrollView();
    }

    void Style(){
        monStyleChampDeSaisie = new GUIStyle(EditorStyles.textField);
        monStyleChampDeSaisie.fontSize = 18;
        monStyleChampDeSaisie.normal.textColor = new Color(0.9f,0.9f,0.9f);
        monStyleChampDeSaisie.normal.background = MakeTex(2, 2, new Color(0.9f, 0.9f, 0.9f));
        monStyleChampDeSaisie.padding = new RectOffset(10,10,0,5);

        // ---------
        monStyleTitreElement = new GUIStyle(EditorStyles.boldLabel);
        monStyleTitreElement.normal.textColor = Color.black;

        // Dans Style()
        styleCadre = new GUIStyle();
        styleCadre.normal.background = MakeTex(2, 2, new Color(0.8f, 0.8f, 0.8f));
        styleCadre.padding = new RectOffset(15, 15, 15, 15);
        styleCadre.margin = new RectOffset(0, 0, 10, 10);

        // --------------------------
        monStyleHelpBox = new GUIStyle(EditorStyles.helpBox);
        monStyleHelpBox.normal.textColor = Color.black;
        monStyleHelpBox.fontSize = 12;

        monStyleInfo = new GUIStyle(EditorStyles.helpBox);
        monStyleInfo.normal.textColor = Color.black;
        monStyleInfo.fontSize = 12;
        monStyleInfo.normal.background = EditorGUIUtility.Load("icons/d_console.infoicon.sml.png") as Texture2D;

        styleChargé = true;
        // Debug.Log("Style charger !");
    }

    private Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    void ChoisirUneCouleurSelonLeType(TypeDialogue type){
        Color color = new Color(0.8f, 0.8f, 0.8f);
        switch (type){
            case TypeDialogue.cadeau:
            case TypeDialogue.donDiscret:
                color = new Color(0.9f, 1f, 0f);
                break;
            case TypeDialogue.vol:
            case TypeDialogue.volDiscret:
                color = new Color(1f, 0.8f, 0f);
                break;
            case TypeDialogue.boutique:
                color = new Color(0.95f, 0.84f, 1f);
                break;
            case TypeDialogue.information:
                color = new Color(0.84f, 0.98f, 1f);
                break;
            case TypeDialogue.texte:
                color = new Color(0.8f, 1f, 0.4f);
                break;
            case TypeDialogue.nul:
                color = new Color(0.1f, 0.1f, 0.1f);
                break;
        }
        styleCadre.normal.background = GetCachedTex(color);
        int x = (type == TypeDialogue.finDeCondition && hauteurConditionnele > 0) ? 1 : 0;
        styleCadre.margin = new RectOffset(30 * (hauteurConditionnele - x), 0, 10, 10);
    }

    //Cette fonction est appeller à la fermeture de la fenêtre.
    private void OnDestroy() {
        foreach (var tex in textureCache.Values) {
            DestroyImmediate(tex);
        }
        textureCache.Clear();
    }

    void AjouterUnFragument(FragmentDeDialogue f){
        f.type = AjouterUnEnumGénérique("Type", f.type);
        switch (f.type){
            case TypeDialogue.texte :
                f.quiJeSuis = AjouterUnEnumGénérique("Qui je suis", f.quiJeSuis);
                if (f.quiJeSuis == QuiJeSuis.héroYoKai || f.quiJeSuis == QuiJeSuis.yokai){
                    f.IdÉlément = AjoutNombre("ID", f.IdÉlément);
                }
                if (f.IdÉlément == 0 || (f.quiJeSuis != QuiJeSuis.héroYoKai && f.quiJeSuis != QuiJeSuis.yokai)){
                    f.nom = AjoutTexte("NOM", f.nom);
                    f.imagePath = AjouterSprite("Image",f.imagePath);
                }
                f.phrase = AjoutTexteLong("Phrase", f.phrase);
                f.imageDePresentationPath = AjouterSprite("Image de présentation", f.imageDePresentationPath);
                break;
            case TypeDialogue.information :
                f.phrase = AjoutTexte("Information", f.phrase);
                f.cesUneExclamation = AjouterUnBool("C'est une exclamation ?", f.cesUneExclamation);
                break;
            case TypeDialogue.cadeau:
            case TypeDialogue.vol:
            case TypeDialogue.volDiscret:
            case TypeDialogue.donDiscret:
                f.typeObjet = AjouterUnEnumGénérique( "Type objet", f.typeObjet);
                f.IdÉlément = AjoutNombre("ID", f.IdÉlément);
                if (f.IdÉlément == 0){
                    f.nom = AjoutTexte("NOM", f.nom);
                    f.imagePath = AjouterSprite("Image",f.imagePath);
                }
                f.quantitée = AjouterQuantitée("Quantitée",f.quantitée);
                break;
            case TypeDialogue.choix:
                string nomCondition = "\"choix-numéro-" + nombreDeChoix.ToString()+"\"";
                GUILayout.Box("La clef de sauvegarde sera : "+nomCondition, monStyleHelpBox);
                GUILayout.Box("0 = Non, 1 = Oui", monStyleHelpBox);
                GUILayout.Box("Pour l'utiliser, créé une nouvelle condition de passage et écriver : "+nomCondition+"=1 (ou 0)",monStyleHelpBox);
                break;
            case TypeDialogue.boutique:
                EditorGUILayout.LabelField("Liste des produits (IDs)", monStyleTitreElement);
                if (f.listeProduitBoutique == null) {
                    f.listeProduitBoutique = new List<int>();
                }
                if (GUILayout.Button("+ Ajouter un produit au magasin")) {
                    f.listeProduitBoutique.Add(0);
                    aDesModificationsNonSauvegardees = true;
                }

                for (int i = 0; i < f.listeProduitBoutique.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    // f.listeProduitBoutique[i] = EditorGUILayout.IntField("ID Produit " + i, f.listeProduitBoutique[i]);
                    f.listeProduitBoutique[i] = AjoutNombre("ID Produit " + i.ToString(), f.listeProduitBoutique[i]);

                    GUI.color = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        f.listeProduitBoutique.RemoveAt(i);
                        aDesModificationsNonSauvegardees = true;
                        EditorGUILayout.EndHorizontal();
                        break; 
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                break;
            case TypeDialogue.conditionDePassage:
                hauteurConditionnele++;
                if (f.conditionnelle == null) f.conditionnelle = new List<Conditionnelle>();
                if (GUILayout.Button("+ Ajouter Condition")) {
                    f.conditionnelle.Add(new Conditionnelle());
                    aDesModificationsNonSauvegardees = true;
                }
                
                for (int j = 0; j < f.conditionnelle.Count; j++){
                    // EditorGUILayout.BeginVertical(GUI.skin.textField);
                    Conditionnelle c = f.conditionnelle[j];
                    Rect rectLabelTemporaire = CrééLeRect();
                    // EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
                    GUILayout.BeginHorizontal();
                    c.nomV1 =  AjoutTexte("Valeur", c.nomV1);
                    c.opérateur = AjouterUnEnumGénérique("Opérateur", c.opérateur);
                    c.nomV2 =  AjoutTexte("Valeur", c.nomV2);
                    GUI.color = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        f.conditionnelle.RemoveAt(j);
                        j--;
                        aDesModificationsNonSauvegardees = true;
                    }
                    GUI.color = Color.white;
                    GUILayout.EndHorizontal();
                    if (j+1 < f.conditionnelle.Count){
                        c.opérateurEtape2 = AjouterUnEnumGénérique("Lien suite", c.opérateurEtape2);
                    }                
                }
                break;
            case TypeDialogue.finDeCondition:
                hauteurConditionnele--;
                if (hauteurConditionnele < 0) f.type = TypeDialogue.nul;
                break;
            case TypeDialogue.quete:
                f.categorieQuete = AjouterUnEnumGénérique("Quel est la catégorie de cette requête ?", f.categorieQuete);
                f.nom = AjoutTexte("Nom",f.nom);
                f.endQuest = AjouterUnBool("La requête est terminé ?", f.endQuest);
                string clef = f.categorieQuete.ToString()+"-"+f.nom;
                // Debug.Log(clef);
                GUILayout.Box("La clef de sauvegarde est : "+clef, monStyleHelpBox);
                GUILayout.Box("Les valeurs possibles sont :"+clef, monStyleHelpBox);
                GUILayout.Box("0 : La requête n'as pas commancer."+clef, monStyleHelpBox);
                GUILayout.Box("1 : La requête est en cours."+clef, monStyleHelpBox);
                GUILayout.Box("2 : La requête est terminée."+clef, monStyleHelpBox);
                break;
            case TypeDialogue.infoQuest:
                f.infoQuest.name = AjoutTexte("Nom",f.infoQuest.name);
                f.infoQuest.description = AjoutTexte("Description",f.infoQuest.description);
                f.infoQuest.objective = AjoutTexte("Objectif",f.infoQuest.objective);
                f.infoQuest.level = AjoutNombre("Niveau recommandé",f.infoQuest.level);
                f.infoQuest.xp = AjoutNombre("Points d'expériences",f.infoQuest.xp);
                break;
            case TypeDialogue.variable:
                f.nom = AjoutTexte("NOM", f.nom);
                f.variableAActivée = AjouterUnBool("La variable est activées ?",f.variableAActivée);
                GUILayout.Box("0 : Non activée.", monStyleHelpBox);
                GUILayout.Box("1 : Activée.", monStyleHelpBox);
                break;
        }
    }

    Rect CrééLeRect(){
        GUILayout.Space(marge);
        return GUILayoutUtility.GetRect(0,hauteurDeToutLesElement);
    }

    // string AjouterSprite(string valeurLabel, string cheminActuel) {
    //     Rect rectLabelTemporaire = CrééLeRect();
    //     EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);

    //     Sprite spriteActuel = null;
    //     if (!string.IsNullOrEmpty(cheminActuel)) {
    //         spriteActuel = Resources.Load<Sprite>(cheminActuel);
    //         if (spriteActuel == null) {
    //             spriteActuel = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/" + cheminActuel + ".png");
    //         }
    //     }

    //     Sprite nouveauSprite = (Sprite)EditorGUILayout.ObjectField(spriteActuel, typeof(Sprite), false);

    //     if (nouveauSprite != spriteActuel) {
    //         if (nouveauSprite != null) {
    //             string assetPath = AssetDatabase.GetAssetPath(nouveauSprite);
                
    //             if (assetPath.Contains("/Resources/")) {
    //                 int index = assetPath.IndexOf("/Resources/") + 11;
    //                 string relativePath = assetPath.Substring(index);
    //                 int dotIndex = relativePath.LastIndexOf('.');
    //                 if (dotIndex > 0) relativePath = relativePath.Substring(0, dotIndex);

    //                 cheminActuel = relativePath;
    //             } else {
    //                 Debug.LogWarning($"L'image {nouveauSprite.name} doit être placée dans un dossier 'Resources' pour pouvoir être chargée dynamiquement.");
    //                 cheminActuel = nouveauSprite.name; // Fallback
    //             }
    //         } else {
    //             cheminActuel = "";
    //         }
    //         aDesModificationsNonSauvegardees = true;
    //     }

    //     return cheminActuel;
    // }
    string AjouterSprite(string label, string cheminActuel) {
        Sprite spriteAffiche = null;
        if (!string.IsNullOrEmpty(cheminActuel)) {
            spriteAffiche = Resources.Load<Sprite>(cheminActuel);
            
            if (spriteAffiche == null) {
                spriteAffiche = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/" + cheminActuel + ".png");
            }
        }

        Sprite nouveauSprite = (Sprite)EditorGUILayout.ObjectField(label, spriteAffiche, typeof(Sprite), false);

        if (nouveauSprite != spriteAffiche) {
            if (nouveauSprite != null) {
                string assetPath = AssetDatabase.GetAssetPath(nouveauSprite);
                
                if (assetPath.Contains("/Resources/")) {
                    int index = assetPath.IndexOf("/Resources/") + 11;
                    string relativePath = assetPath.Substring(index);
                    
                    int dotIndex = relativePath.LastIndexOf('.');
                    if (dotIndex > 0) relativePath = relativePath.Substring(0, dotIndex);

                    return relativePath;
                } else {
                    Debug.LogWarning($"L'image '{nouveauSprite.name}' doit se trouver dans un dossier 'Resources' pour être chargée en jeu.");
                    return nouveauSprite.name;
                }
            } else {
                return "";
            }
        }

        return cheminActuel;
    }

    int AjouterQuantitée(string valeurLabel, int valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
        return EditorGUILayout.IntSlider(valeurParDéfaut, 1, 99);
    }

    string AjoutTexte(string valeurLabel, string valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        // EditorGUI.DrawRect(rectLabelTemporaire, couleurTitre);
        EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
        return EditorGUILayout.TextField(valeurParDéfaut, monStyleChampDeSaisie, GUILayout.Height(28));
    }

    string AjoutTexteLong(string valeurLabel, string valeurParDéfaut) {
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
        if (monStyleChampDeSaisie != null) {
            monStyleChampDeSaisie.wordWrap = true;
        }
        return EditorGUILayout.TextArea(valeurParDéfaut, monStyleChampDeSaisie, GUILayout.Height(65));
    }

    int AjoutNombre(string valeurLabel, int valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
        valeurParDéfaut = EditorGUILayout.IntField(valeurParDéfaut, monStyleChampDeSaisie, GUILayout.Height(hauteurDeToutLesElement));
        valeurParDéfaut = Mathf.Max(0, valeurParDéfaut);
        return valeurParDéfaut;
    }

    T AjouterUnEnumGénérique<T>(string texteLabel, T valeurActuelle) where T : System.Enum {
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, texteLabel, monStyleTitreElement);
        return (T)EditorGUILayout.EnumPopup(valeurActuelle, monStyleChampDeSaisie, GUILayout.Height(28));
    }

    bool AjouterUnBool(string texteLabel, bool valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, texteLabel, monStyleTitreElement);
        return EditorGUILayout.Toggle(valeurParDéfaut);
    }


    void LireLaSauvegarde(int numéro) {
        if (selectedPNJ == null) return;
        ReadTheSaveOfConditionOfTheDialoguePage();
        string fullPath = Application.dataPath + "/Resources/texte/" + selectedPNJ.jsonPath + "-page" + numéro + ".json";

        if (File.Exists(fullPath)) {
            string json = File.ReadAllText(fullPath);
            DialogueData data = JsonUtility.FromJson<DialogueData>(json);            
            fragment = data.fragments ?? new List<FragmentDeDialogue>();
        } else {
            fragment = new List<FragmentDeDialogue>();
        }

        nombrePage = Mathf.Max(1, conditionOfTheDialoguePage.Count);
    
        selectedPNJ.maListeDeFraguement = new List<FragmentDeDialogue>(fragment);
    }


    void EnregistrerPage(int numéro, bool retirerFocus = true){
        if (retirerFocus) GUI.FocusControl(null); // Retire le focus pour valider les modifications de texte en cours
        aDesModificationsNonSauvegardees = false;
        vientDeSauvegarder = true;
        nextSaveTime = EditorApplication.timeSinceStartup + SAVE_INTERVAL;
        if (selectedPNJ == null) return;
        SaveConditionOfTheDialoguePage();
        string directory = Application.dataPath + "/Resources/texte";
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        if (string.IsNullOrEmpty(selectedPNJ.jsonPath)) {
            selectedPNJ.jsonPath = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        selectedPNJ.maListeDeFraguement = new List<FragmentDeDialogue>(fragment);
        DialogueData data = new DialogueData { fragments = selectedPNJ.maListeDeFraguement };
        string json = JsonUtility.ToJson(data, true);
        string fullPath = directory + "/" + selectedPNJ.jsonPath + "-page" + numéro + ".json";

        File.WriteAllText(fullPath, json);
        EditorUtility.SetDirty(selectedPNJ);
        AssetDatabase.Refresh();
        Debug.Log($"<color=cyan>Fichier sauvegardé :</color> {selectedPNJ.jsonPath}-page{numéro}.json à {DateTime.Now.ToLongTimeString()}");
    }

    void SaveConditionOfTheDialoguePage(){
        while (conditionOfTheDialoguePage.Count < nombrePage){
            conditionOfTheDialoguePage.Add(new ConditionOfTheDialoguePage());
        }
        string directory = Application.dataPath + "/Resources/texte";
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        if (string.IsNullOrEmpty(selectedPNJ.jsonPath)) {
            selectedPNJ.jsonPath = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        }

        selectedPNJ.conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>(conditionOfTheDialoguePage);
        ConditionOfTheDialoguePageData data = new ConditionOfTheDialoguePageData { conditionOfTheDialoguePage = selectedPNJ.conditionOfTheDialoguePage };
        string json = JsonUtility.ToJson(data, true);
        string fullPath = directory + "/" + selectedPNJ.jsonPath + "-listePage" + ".json";

        File.WriteAllText(fullPath, json);

        EditorUtility.SetDirty(selectedPNJ);
        AssetDatabase.Refresh();
    }

    void ReadTheSaveOfConditionOfTheDialoguePage() {
        string fullPath = Application.dataPath + "/Resources/texte/" + selectedPNJ.jsonPath + "-listePage" + ".json";

        if (File.Exists(fullPath)) {
            string json = File.ReadAllText(fullPath);
            ConditionOfTheDialoguePageData data = JsonUtility.FromJson<ConditionOfTheDialoguePageData>(json);
            conditionOfTheDialoguePage = data.conditionOfTheDialoguePage ?? new List<ConditionOfTheDialoguePage>();
        } else {
            conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>();
        }
    
        selectedPNJ.conditionOfTheDialoguePage = new List<ConditionOfTheDialoguePage>(conditionOfTheDialoguePage);
    }

    void SetingsTheNewFragment(FragmentDeDialogue f1, FragmentDeDialogue f){
        string nomDuGameObject = Selection.activeGameObject.name;
        int ID = int.Parse(System.Text.RegularExpressions.Regex.Match(nomDuGameObject, @"\d+").Value);
        if (ID > 0 && nomDuGameObject.Contains("y")){
            f.IdÉlément = ID;
            f.quiJeSuis = QuiJeSuis.yokai;
        }
    }
}
