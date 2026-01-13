Pour le rendre accèsible dans le menu de Unity :
``` cs
[MenuItem("Tools/Dial")]
public static void ShowWindow(){
    GetWindow<Dial>("Dial");
}
```

`void OnGUI(){}` : Cette fonction est appelée pour dessiner l'interface de la fenêtre.


`EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Color.white);` : créé un rectangle sur l'ensemble de la page.

`GUILayout.Label("Mon contenu ici");` : texte.
`GUI.contentColor = Color.black;` : changer la couleur du texte.