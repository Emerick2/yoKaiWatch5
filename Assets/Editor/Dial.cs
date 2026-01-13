using UnityEngine;
using UnityEditor;
using System;

public class Dial : EditorWindow {
    private GUIStyle styleBouton;
    private GUIStyle styleInput;
    private GUIStyle styleLabel;

    private string monNom = "";
    private string texteSaisie = "Modifie-moi";
    private int indexSelection = 0;
    private string[] options = { "Option 1", "Option 2", "Option 3" };

    [MenuItem("Tools/Dial")]
    public static void ShowWindow(){
        GetWindow<Dial>("Dial");
    }

private void InitStyles() {
    if (styleBouton != null) return;

    // --- STYLE DU BOUTON ---
    styleBouton = new GUIStyle(GUI.skin.button); 
    styleBouton.normal.textColor = Color.white;
    styleBouton.fontSize = 14;
    styleBouton.fontStyle = FontStyle.Bold;
    styleBouton.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f));

    // --- STYLE DE L'INPUT (TextField) ---
    styleInput = new GUIStyle(GUI.skin.textField);
    // IMPORTANT : On donne une couleur de fond à l'input pour qu'il se détache du blanc
    // styleInput.normal.background = MakeTex(2, 2, new Color(0.6f, 0.9f, 0.9f)); 
    // styleInput.normal.textColor = Color.white; // Texte blanc sur fond sombre
    // styleInput.focused.background = MakeTex(2, 2, new Color(0.4f, 0.4f, 0.4f));
    styleInput.focused.textColor = Color.yellow; 
    // styleInput.padding = new RectOffset(0, 0, 0, 0);
    // styleInput.margin = new RectOffset(0, 0, 0, 0);

    // --- STYLE DES LABELS ---
    styleLabel = new GUIStyle(EditorStyles.label);
    styleLabel.normal.textColor = Color.black; // On le met en noir explicitement
}

    void OnGUI(){
        InitStyles();
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Color.white);

        // --- Premier bloc ---
        Action premierBlocContenu = () => {
            // GUI.contentColor = Color.black; 
            GUILayout.Label("Contenu du premier bloc");
            monNom = EditorGUILayout.TextField("Nom", monNom, styleInput);
            GUILayout.Label("Un autre texte ici");
            if (GUILayout.Button("Bouton 1",styleBouton)) {
                Debug.Log("Bouton 1 cliqué !");
            }
        };
        DrawStyledBlock(premierBlocContenu, Color.cyan, 20);

        // --- Deuxième bloc ---
        Action deuxiemeBlocContenu = () => {
            // GUI.contentColor = Color.black;
            GUILayout.Label("Contenu du deuxième bloc");
            GUILayout.Label("Ce bloc a une marge plus grande.");
        };
        DrawStyledBlock(deuxiemeBlocContenu, new Color(1f, 0.9f, 0.9f), 50);
    }

    void DrawStyledBlock(Action contentAction, Color backgroundColor, int marginLeft){
        GUIStyle styleBloc = new GUIStyle(GUI.skin.box);
        
        styleBloc.normal.background = Texture2D.whiteTexture; 
        styleBloc.margin.left = marginLeft;
        styleBloc.padding = new RectOffset(10, 10, 10, 10); 

        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = backgroundColor;

        EditorGUILayout.BeginVertical(styleBloc);
            contentAction?.Invoke();
        EditorGUILayout.EndVertical();

        GUI.backgroundColor = oldColor;
    }

    private Texture2D MakeTex(int width, int height, Color col) {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i) pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}