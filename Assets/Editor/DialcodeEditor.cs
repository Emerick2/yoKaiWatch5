using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using YKW.Core.Item.Enum;

public class DialcodeEditor : EditorWindow{
    private PNJDialcode selectedPNJ;
    private Vector2 scrollPos;
    private string lastSavedFile = "";
    private int tauxIndentation = 50;

    [MenuItem("Tools/Dialcode V3")]
    public static void ShowWindow(){
        GetWindow<DialcodeEditor>("Dialcode V3");
    }

    private void OnSelectionChange(){
        GameObject go = Selection.activeGameObject;
        if (go != null){
            selectedPNJ = go.GetComponent<PNJDialcode>();
        }else{
            selectedPNJ = null;
        }
        Repaint();
    }

    private void OnGUI(){
        EditorGUI.BeginChangeCheck();
        // Rect windowRect = new Rect(0, 0, position.width, position.height);
        // EditorGUI.DrawRect(windowRect, new Color(1f, 1f, 1f));


        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Éditeur de Dialogue : " + (selectedPNJ != null ? selectedPNJ.name : "Aucun PNJ sélectionné"));
        GUILayout.FlexibleSpace();
        // if (selectedPNJ != null && GUILayout.Button("Sauvegarder JSON", EditorStyles.toolbarButton)){
        //     SauvegarderEnJson();
        // }
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
            // Si quelque chose a changé, on sauvegarde en silence (sans popup)
            SauvegarderEnJson(false); 
        }
    }

    private void DrawInterface() {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        int currentIndent = 0;

        for (int i = 0; i < selectedPNJ.maListeDeFraguement.Count; i++) {
            FragmentDeDialogue fragment = selectedPNJ.maListeDeFraguement[i];

            // Couleurs par type
            switch (fragment.type) {
                case TypeDialogue.texte: GUI.backgroundColor = Color.cyan; break;
                case TypeDialogue.cadeau: GUI.backgroundColor = Color.green; break;
                case TypeDialogue.conditionDePassage: GUI.backgroundColor = Color.gray; break;
                case TypeDialogue.choix: GUI.backgroundColor = new Color(1f, 0.5f, 0f); break;
                case TypeDialogue.finDeCondition: GUI.backgroundColor = Color.gray; break;
                default: GUI.backgroundColor = Color.white; break;
            }

            if (fragment.type == TypeDialogue.finDeCondition) currentIndent = Mathf.Max(0, currentIndent - 1);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(currentIndent * tauxIndentation);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            bool wasRemoved = DrawFragment(fragment, i);
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (wasRemoved){
                GUIUtility.ExitGUI();
                break;
            }

            GUI.backgroundColor = Color.white;

            if (fragment.type == TypeDialogue.conditionDePassage) currentIndent++;
            fragment.auteurConditionnelle = currentIndent;
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Ajouter un nouveau bloc", GUILayout.Height(30))) {
            selectedPNJ.maListeDeFraguement.Add(new FragmentDeDialogue());
        }

        EditorGUILayout.EndScrollView();
    }

    private bool DrawFragment(FragmentDeDialogue f, int index){
        EditorGUILayout.BeginHorizontal();
        f.type = (TypeDialogue)EditorGUILayout.EnumPopup("Type :", f.type);
        GUI.color = Color.red;
        bool removed = GUILayout.Button("X", GUILayout.Width(20));
        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        if (removed) {
            selectedPNJ.maListeDeFraguement.RemoveAt(index);
            return true;
        }

        EditorGUI.indentLevel++;

        switch (f.type){
            case TypeDialogue.texte:
                f.quiJeSuis = (QuiJeSuis)EditorGUILayout.EnumPopup("Qui parle ?", f.quiJeSuis);
                f.IdÉlément = EditorGUILayout.IntField("ID (si Yokai)", f.IdÉlément);
                f.nom = EditorGUILayout.TextField("Nom manuel", f.nom);
                f.image = (Sprite)EditorGUILayout.ObjectField("Image", f.image, typeof(Sprite), false);
                f.phrase = EditorGUILayout.TextArea(f.phrase, GUILayout.Height(40));
                f.imageDePrésentation = (Sprite)EditorGUILayout.ObjectField("Image Présentation", f.imageDePrésentation, typeof(Sprite), false);
                break;

            case TypeDialogue.cadeau:
            case TypeDialogue.vol:
            case TypeDialogue.volDiscret:
            case TypeDialogue.donDiscret:
                f.typeObjet = (CatégorieItem)EditorGUILayout.EnumPopup("Type Objet", f.typeObjet);
                f.IdÉlément = EditorGUILayout.IntField("ID Objet", f.IdÉlément);
                f.nom = EditorGUILayout.TextField("Nom Objet", f.nom);
                f.image = (Sprite)EditorGUILayout.ObjectField("Image", f.image, typeof(Sprite), false);
                f.quantitée = EditorGUILayout.IntSlider("Quantité", f.quantitée, 1, 99);
                break;

            case TypeDialogue.information:
                f.phrase = EditorGUILayout.TextField("Information", f.phrase);
                f.cesUneExclamation = EditorGUILayout.Toggle("Exclamation ?", f.cesUneExclamation);
                break;

            case TypeDialogue.choix:
                int nChoix = 0;
                for (int k = 0; k < index; k++)
                {
                    if (selectedPNJ.maListeDeFraguement[k].type == TypeDialogue.choix)
                    {
                        nChoix++;
                    }
                }
                EditorGUILayout.HelpBox("La clef de sauvegarde sera : choix-dialogue-" + nChoix.ToString(), MessageType.Info);
                EditorGUILayout.HelpBox("0 = Non, 1 = Oui", MessageType.None);
                break;

            case TypeDialogue.conditionDePassage:
                if (f.conditionnelle == null) f.conditionnelle = new List<Conditionnelle>();
                if (GUILayout.Button("+ Ajouter Condition")) f.conditionnelle.Add(new Conditionnelle());
                
                for (int j = 0; j < f.conditionnelle.Count; j++)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.textField);
                    Conditionnelle c = f.conditionnelle[j];
                    c.nomV1 = EditorGUILayout.TextField("Var 1", c.nomV1);
                    c.opérateur = (Opérateur)EditorGUILayout.EnumPopup(c.opérateur);
                    c.nomV2 = EditorGUILayout.TextField("Var 2", c.nomV2);
                    c.opérateurEtape2 = (OpérateurEtape2)EditorGUILayout.EnumPopup("Lien suite", c.opérateurEtape2);
                    
                    if (GUILayout.Button("Supprimer Condition")) { f.conditionnelle.RemoveAt(j); break; }
                    EditorGUILayout.EndVertical();
                }
                break;

            case TypeDialogue.finDeCondition:
                EditorExtensions.LabelHelper("--- Fin du bloc conditionnel ---");
                break;
            
            case TypeDialogue.boutique:
                EditorGUILayout.LabelField("Liste des produits (IDs)", EditorStyles.boldLabel);
                if (f.listeProduitBoutique == null) {
                    f.listeProduitBoutique = new List<int>();
                }

                if (GUILayout.Button("+ Ajouter un produit au magasin")) {
                    f.listeProduitBoutique.Add(0);
                }

                for (int i = 0; i < f.listeProduitBoutique.Count; i++) {
                    EditorGUILayout.BeginHorizontal();
                    f.listeProduitBoutique[i] = EditorGUILayout.IntField("ID Produit " + i, f.listeProduitBoutique[i]);

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
        }

        EditorGUI.indentLevel--;
        return false;
    }

    // private void SauvegarderEnJson()
    // {
    //     string directory = Application.dataPath + "/Resources/texte";
    //     if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

    //     string fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".json";
    //     string fullPath = directory + "/" + fileName;

    //     DialogueData data = new DialogueData();
    //     data.fragments = selectedPNJ.maListeDeFraguement;

    //     string json = JsonUtility.ToJson(data, true);
    //     File.WriteAllText(fullPath, json);

    //     selectedPNJ.jsonPath = fileName.Replace(".json", "");
    //     AssetDatabase.Refresh();
        
    //     Debug.Log("Dialogue sauvegardé sous : " + fullPath);
    //     EditorUtility.DisplayDialog("Succès", "Fichier sauvegardé avec succès !\nNom : " + fileName, "OK");
    // }
    private void SauvegarderEnJson(bool showDialog) {
        string directory = Application.dataPath + "/Resources/texte";
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

        // --- LOGIQUE DE NOM DE FICHIER ---
        string fileName = selectedPNJ.jsonPath;

        // Si le PNJ n'a pas encore de fichier, on crée le nom unique
        if (string.IsNullOrEmpty(fileName)) {
            fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            selectedPNJ.jsonPath = fileName;
            EditorUtility.SetDirty(selectedPNJ); // Marque l'objet pour qu'Unity sauve le nom
        }

        string fullPath = directory + "/" + fileName + ".json";

        DialogueData data = new DialogueData();
        data.fragments = selectedPNJ.maListeDeFraguement;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);

        // On ne refresh la DB que si nécessaire (c'est lourd)
        if (showDialog) {
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Succès", "Sauvegarde effectuée dans : " + fileName, "OK");
        }
    }
}

public static class EditorExtensions 
{
    public static void LabelHelper(string text)
    {
        GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        style.normal.textColor = Color.cyan;
        GUILayout.Label(text, style);
    }
}
