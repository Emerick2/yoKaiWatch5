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
    private int nombreDeFragument = 0;
    private List<FragmentDeDialogue> fragment = new List<FragmentDeDialogue>();
    private Vector2 scrollPosition;
    private int hauteurConditionnele = 0;
    private PNJDialcode selectedPNJ = null;
    private string lastSavedFile = "";

    private int nombrePage = 1;
    private int positionPage;

    [MenuItem("Tools/Dialcode V4")]
    public static void ShowWindow(){
        GetWindow<DialcodeV4>("Dialcode V4");
    }

    private void OnSelectionChange(){
        GameObject go = Selection.activeGameObject;
        if (go != null){
            selectedPNJ = go.GetComponent<PNJDialcode>();
            positionPage = 0;
            LireLaSauvegarde(positionPage);
        }else{
            selectedPNJ = null;
        }
        Repaint();
    }

    void OnGUI(){
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Éditeur de Dialogue : " + (selectedPNJ != null ? selectedPNJ.name : "Aucun PNJ sélectionné"));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (selectedPNJ == null){
            EditorGUILayout.HelpBox("Veuillez sélectionner un GameObject avec le script PNJDialcode.", MessageType.Info);
            if ((selectedPNJ == null && Selection.activeGameObject.GetComponent<PNJDialcode>() != null) || (Selection.activeGameObject != null && GUILayout.Button("Ajouter PNJDialcode à " + Selection.activeGameObject.name))){
                if (Selection.activeGameObject.GetComponent<PNJDialcode>() == null) Selection.activeGameObject.AddComponent<PNJDialcode>();
                OnSelectionChange();
            }
            return;
        }

        DrawInterface();

        if (EditorGUI.EndChangeCheck()) {
            EnregistrerPage(positionPage); 
        }
    }


    void PlacerEnTête(){
        GUILayout.BeginHorizontal();
        for (int i = 0; i < nombrePage; i++){
            GUILayout.BeginVertical();
            if (nombrePage > 1){
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    if (nombrePage-1>0){
                        nombrePage--;
                    }
                    if (positionPage == i){
                        positionPage = 0;
                    }
                }
                GUI.color = Color.white;
            }
            if (positionPage == i) GUI.color = Color.green;
            if (GUILayout.Button((i+1).ToString(), GUILayout.Width(50), GUILayout.Height(50))) {  
                positionPage = i;      
                LireLaSauvegarde(positionPage);
            }
            if (positionPage == i) GUI.color = Color.white;
            GUILayout.EndVertical();
            GUILayout.Space(5);
        }
        if (GUILayout.Button("+", GUILayout.Width(50), GUILayout.Height(50))) {    
            nombrePage++;    
            positionPage = nombrePage-1;      
            fragment.Clear();
            LireLaSauvegarde(positionPage);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        bool personnageVisible = true;
        personnageVisible = AjouterUnBool("Personnage visible ?",personnageVisible);

        // condition :
        List<Conditionnelle> conditionnelle = new List<Conditionnelle>();
        if (conditionnelle == null) conditionnelle = new List<Conditionnelle>();
        if (GUILayout.Button("+ Ajouter Condition")) conditionnelle.Add(new Conditionnelle());
                
        for (int j = 0; j < conditionnelle.Count; j++){
            Conditionnelle c = conditionnelle[j];
            Rect rectLabelTemporaire = CrééLeRect();
            GUILayout.BeginHorizontal();
            c.nomV1 =  AjoutTexte("Valeur", c.nomV1);
            c.opérateur = AjouterUnEnumGénérique("Opérateur", c.opérateur);
            c.nomV2 =  AjoutTexte("Valeur", c.nomV2);
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20))) {
                conditionnelle.RemoveAt(j);
                j--;
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            if (j+1 < conditionnelle.Count){
                c.opérateurEtape2 = AjouterUnEnumGénérique("Lien suite", c.opérateurEtape2);
            }                
        }
    }

    void DrawInterface(){
        nombreDeChoix = 1;
        hauteurConditionnele = 0;
        // if (!styleChargé) Style();
        Style();
        Rect rectangleFenêtre = new Rect(0,0,position.width,position.height);
        EditorGUI.DrawRect(rectangleFenêtre,Color.white);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginHorizontal();//<>
        GUILayout.Space(20);
        GUILayout.BeginVertical();//<>

        PlacerEnTête();

        for (int i = 0; i < nombreDeFragument; i++){
            FragmentDeDialogue f = null;
            if (fragment.Count < i+1){
                f = new FragmentDeDialogue();
                fragment.Add(f);
            } else {
                f = fragment[i];
            }
            ChoisirUneCouleurSelonLeType(f.type);
            GUILayout.BeginVertical(styleCadre); 
            // if (GUILayout.Button("Suprimer le bloc ci-dessou", GUILayout.Height(30))) {
            //     nombreDeFragument--;
            //     fragment.RemoveAt(i);
            //     i--;
            // }
            GUILayout.BeginHorizontal();
            GUI.color = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20))) {
                nombreDeFragument--;
                fragment.RemoveAt(i);
                i--;
            }
            GUI.color = Color.white;
            GUI.color = Color.green;
            if (GUILayout.Button("V", GUILayout.Width(20))) {
                nombreDeFragument++;
                fragment.Insert(i, new FragmentDeDialogue());
                i--;
            }
            GUI.color = Color.white;          
            GUILayout.EndHorizontal();

            AjouterUnFragument(f);            
            GUILayout.EndVertical();

            GUILayout.Space(10);      
        }

        if (GUILayout.Button("Ajouter un nouveau bloc", GUILayout.Height(30))) {
            nombreDeFragument++;
        }
        
        GUILayout.EndVertical();//</>
        GUILayout.EndHorizontal();//</>
        GUILayout.EndScrollView();
    }

    void Style(){
        styleChargé = true;
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

        Debug.Log("Style charger !");
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
        styleCadre.normal.background = MakeTex(2, 2, color);

        int x = 0;
        if (type == TypeDialogue.finDeCondition && hauteurConditionnele>0) x = 1;
        styleCadre.margin = new RectOffset(30*(hauteurConditionnele-x), 0, 10, 10);
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
                    f.image = AjouterSprite("Image",f.image);
                }
                f.phrase = AjoutTexte("Phrase", f.phrase);
                f.imageDePrésentation = AjouterSprite("Image de présantation",f.imageDePrésentation);
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
                    f.image = AjouterSprite("Image",f.image);
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
                }

                for (int i = 0; i < f.listeProduitBoutique.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    // f.listeProduitBoutique[i] = EditorGUILayout.IntField("ID Produit " + i, f.listeProduitBoutique[i]);
                    f.listeProduitBoutique[i] = AjoutNombre("ID Produit " + i.ToString(), f.listeProduitBoutique[i]);

                    GUI.color = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(20))) {
                        f.listeProduitBoutique.RemoveAt(i);
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
                if (GUILayout.Button("+ Ajouter Condition")) f.conditionnelle.Add(new Conditionnelle());
                
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
        }
    }

    Rect CrééLeRect(){
        GUILayout.Space(marge);
        return GUILayoutUtility.GetRect(0,hauteurDeToutLesElement);
    }

    Sprite AjouterSprite(string valeurLabel, Sprite valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, valeurLabel, monStyleTitreElement);
        return (Sprite)EditorGUILayout.ObjectField(valeurParDéfaut, typeof(Sprite), false);
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
        return EditorGUILayout.TextField(valeurParDéfaut, monStyleChampDeSaisie, GUILayout.Height(hauteurDeToutLesElement));
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
        return (T)EditorGUILayout.EnumPopup(valeurActuelle, monStyleChampDeSaisie, GUILayout.Height(30));
    }

    bool AjouterUnBool(string texteLabel, bool valeurParDéfaut){
        Rect rectLabelTemporaire = CrééLeRect();
        EditorGUI.LabelField(rectLabelTemporaire, texteLabel, monStyleTitreElement);
        return EditorGUILayout.Toggle(valeurParDéfaut);
    }


    void LireLaSauvegarde(int numéro) {
        string fullPath = Application.dataPath + "/Resources/texte/" + selectedPNJ.jsonPath + "-page" + numéro + ".json";

        if (File.Exists(fullPath)) {
            string json = File.ReadAllText(fullPath);
            DialogueData data = JsonUtility.FromJson<DialogueData>(json);
            fragment = data.fragments ?? new List<FragmentDeDialogue>();
        } else {
            fragment = new List<FragmentDeDialogue>();
        }
    
        nombreDeFragument = fragment.Count;
        selectedPNJ.maListeDeFraguement = new List<FragmentDeDialogue>(fragment);
    }


    void EnregistrerPage(int numéro){
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
    }
}
