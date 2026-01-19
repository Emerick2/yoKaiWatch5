using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestionnaireMenu : MonoBehaviour{
    Dictionary<string, string> monDictionnaire = new Dictionary<string, string>();
    public List<DesDialogues> desDialogues = new List<DesDialogues>();
    //par exemple : "[nom]Puscle[/nom][p]Bonjour, je m'appelle puscle ! J'aime le chocolat, et toi ? aime tu aussi le choccocoocococolat ?       [/p][question][nouriture préféré][bannane][poire][tomate][haricot][/question][if][bannane==0][p]tu a une bannane ![/p]][/if]"
    //[if][bannane==7][p]tu a une bannane ![/p]][/if][p]blabla[/p]
    public Animator animatorDeLaBoiteDeDialogue;
    public TextMeshProUGUI espacePourEcrire;
    public TextMeshProUGUI espacePourEcrireLeNom;
    
    public List<string> lesActionsAEffectuer = new List<string>();
    private int possitionDansLaRecette;
    private string temps;
    public bool enDialogue;

    public GameObject blocQuestion;
    private bool enAttenteDeUneRéponce;
    private int numéroQuestion;
    private string question;
    private List<Image> espaceDeSélectionQuestion = new List<Image>();

    public List<GameObject> espaceDeSélectionQuestionPreparer = new List<GameObject>();

    private List<char> nombre = new List<char>(){ '0','1','2','3','4','5','6','7','8','9'};
    private List<char> signeSpéciaux = new List<char>() { '=', '+', '!', '<', '>'};
    public List<bool> listeDesIf = new List<bool>();

    void Start(){
        string chemin = Application.dataPath + "/export_point_virgule.csv";
        if (System.IO.File.Exists(chemin)){
            string[] lignes = System.IO.File.ReadAllLines(chemin);
            foreach (string ligne in lignes){
                // On suppose que chaque ligne est du type : clef;valeur
                string[] morceaux = ligne.Split(';');
                if (morceaux.Length >= 2){
                    DesDialogues dial = new DesDialogues();
                    dial.codeDuDialogue = morceaux[0].Trim();
                    dial.leContenue = morceaux[1].Trim();
                    dial.codeDuDialogue = dial.codeDuDialogue.Substring(1, dial.codeDuDialogue.Length - 2);
                    dial.leContenue = dial.leContenue.Substring(1, dial.leContenue.Length - 2);
                    desDialogues.Add(dial);
                }
            }
        }else{
            Debug.LogWarning("Fichier CSV non trouvé : " + chemin);
        }
    }

    void Update(){
        if (Input.GetButtonDown("A") && enDialogue){
            //if (possitionDansLaRecette< lesActionsAEffectuer.Count) print(lesActionsAEffectuer[possitionDansLaRecette]);
            if (enAttenteDeUneRéponce){
                enAttenteDeUneRéponce = false;
                ChoieDeUneRéponceAUneQuestion(numéroQuestion);
                possitionDansLaRecette++;
            }
            SuivreEtapeSuivante();
        }
        if (enAttenteDeUneRéponce){
            if (Input.GetButtonDown("right")) SeDéplaceDansLaSélection(1);
            if (Input.GetButtonDown("left")) SeDéplaceDansLaSélection(-1);
        }
    }
    void ChoieDeUneRéponceAUneQuestion(int choie){
        AjouterUneValeurDansLeDictionnaireLocal(question, espaceDeSélectionQuestion[numéroQuestion].gameObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text);
        blocQuestion.SetActive(false);
    }
    void SeDéplaceDansLaSélection(int n){
        espaceDeSélectionQuestion[numéroQuestion].color = new Color(1f, 1f, 1f, 1f);
        numéroQuestion = (numéroQuestion+n)%espaceDeSélectionQuestion.Count;
        if (numéroQuestion<0) numéroQuestion=espaceDeSélectionQuestion.Count-1;
        espaceDeSélectionQuestion[numéroQuestion].color = new Color(1f, 0.5f, 0.5f, 1f);
    }

    public void OnCommenceUnDialogueICI(string codeDuDialogue){
        enDialogue = true;
        listeDesIf.Clear();
        enAttenteDeUneRéponce = false;
        blocQuestion.SetActive(false);
        possitionDansLaRecette = 0;
        lesActionsAEffectuer.Clear();
        DécoderLesAction(TrouverLeContenue(codeDuDialogue));
    }
    public void SuivreEtapeSuivante(){
        if (possitionDansLaRecette >= lesActionsAEffectuer.Count - 1){
            OnFiniLeDialogueICI();
        }else{
            string valeur = lesActionsAEffectuer[possitionDansLaRecette];
            if (valeur == "[nom]"){
                EcrireSonNom(lesActionsAEffectuer[possitionDansLaRecette + 1]);
                possitionDansLaRecette += 3;
                SuivreEtapeSuivante();
            }
            else if (valeur == "[p]"){
                LireLeMessage(lesActionsAEffectuer[possitionDansLaRecette + 1]);
                possitionDansLaRecette += 3;
            }
            else if (valeur == "[sauv]"){//[sauv][nbPain]["56"][/sauve]
                Sauvgarder(lesActionsAEffectuer[possitionDansLaRecette + 1], lesActionsAEffectuer[possitionDansLaRecette + 2]);
                possitionDansLaRecette += 4;
                SuivreEtapeSuivante();
            }
            else if (valeur == "[question]"){//[question][nouriture préféré][bannane][poire][tomate][haricot][/question]
                question = lesActionsAEffectuer[possitionDansLaRecette + 1];
                possitionDansLaRecette += 2;
                List<string> proposition = new List<string>();
                string p = lesActionsAEffectuer[possitionDansLaRecette];
                while (p != "[/question]" && lesActionsAEffectuer.Count > possitionDansLaRecette){
                    proposition.Add(lesActionsAEffectuer[possitionDansLaRecette]);
                    possitionDansLaRecette++;
                    if (lesActionsAEffectuer.Count > possitionDansLaRecette) p = lesActionsAEffectuer[possitionDansLaRecette];
                }
                Question(proposition);
            }

            else if (valeur == "[if]" || valeur == "[elif]" || valeur == "[else]"){
                print(valeur);
                print(listeDesIf.Count);
                UneCondition(valeur);
            }
            else if (valeur == "[/if]" || valeur == "[/elif]" || valeur == "[/else]"){
                possitionDansLaRecette++;
                if (LesIfContinue(lesActionsAEffectuer[possitionDansLaRecette])){
                    UneCondition(lesActionsAEffectuer[possitionDansLaRecette], true);//sa va permettre de passer les if qui continue
                }else if (listeDesIf.Count > 0){
                    listeDesIf.RemoveAt(listeDesIf.Count-1);
                    print("remove !");
                }
                SuivreEtapeSuivante();
            }

            else if (valeur=="[fin]"){
                possitionDansLaRecette = lesActionsAEffectuer.Count;
            }

            else{
                Debug.Log("Problème dans l'écriture du dialogue :"+ lesActionsAEffectuer[possitionDansLaRecette]);
                possitionDansLaRecette++;
                SuivreEtapeSuivante();
            }
        }
    }
    void Question(List<string> proposition){
        enAttenteDeUneRéponce = true;
        numéroQuestion = 0;
        espaceDeSélectionQuestion.Clear();
        blocQuestion.SetActive(true);
        for (int i = 0; i < 4; i++){
            GameObject TV = espaceDeSélectionQuestionPreparer[i];
            if (TV != null){
                if (proposition.Count>i){
                    TV.SetActive(true);
                    espaceDeSélectionQuestion.Add(TV.GetComponent<Image>());
                    proposition[i] = proposition[i].Substring(1, proposition[i].Length - 2);
                    proposition[i] = RetirerEspace(proposition[i]);
                    TV.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = proposition[i];
                    TV.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }else{
                    TV.SetActive(false);
                }
            }
            else{
                Debug.LogWarning("problème avec les question du dialogue !");
            }
        }
        SeDéplaceDansLaSélection(0);
        //espaceDeSélectionQuestion
    }
    void Sauvgarder(string clef,string valeur){
        bool utiliserLeDebeug = false;
        string valeurNettoyee = valeur.Substring(1, valeur.Length - 2);
        //if (utiliserLeDebeug) print(valeurNettoyee);
        if (valeurNettoyee[0] == '"') {
            if (utiliserLeDebeug) print("passage 1");
            NewDataCenter.Instance.SauvegarderSTR(clef, valeurNettoyee);
        }
        else {
            if (utiliserLeDebeug) print("passage 2");
            NewDataCenter.Instance.SauvegarderINT(clef, int.Parse(valeurNettoyee));
        }
        if (utiliserLeDebeug) print("valeur = "+NewDataCenter.Instance.Lire(clef));
    }
    void EcrireSonNom(string nom){
        espacePourEcrireLeNom.text = nom;
    }
    void OnFiniLeDialogueICI(){
        enDialogue = false;
        animatorDeLaBoiteDeDialogue.SetTrigger("fin");
        mouvementDuJoueur.droitDeMarcher = true;
    }
    void LireLeMessage(string texteAécrire){
        espacePourEcrire.text = texteAécrire;
    }
    string TrouverLeContenue(string codeDuDialogue){
        animatorDeLaBoiteDeDialogue.SetTrigger("début");
        foreach (DesDialogues item in desDialogues){
            if (item.codeDuDialogue == codeDuDialogue){
                return item.leContenue;
            }
        }
        return "";
    }
    void DécoderLesAction(string dialogue){
        temps = "";
        foreach (char item in dialogue){
            if (item == '[') Enregistrer();
            temps+=item.ToString();
            if (item == ']') Enregistrer();
        }
        Enregistrer();
    }
    void Enregistrer(){
        temps = RetirerEspace(temps);
        if (temps != ""){
            lesActionsAEffectuer.Add(temps);
        }
        temps = "";
    }
    string RetirerEspace(string phrase){
        phrase = phrase.Trim();         // Étape 1 : Trim() enlève les espaces en début et en fin de chaîne
        return System.Text.RegularExpressions.Regex.Replace(phrase, @"\s{2,}", " ");        // Étape 2 : Regex pour remplacer les espaces multiples par un seul
    }


    void AjouterUneValeurDansLeDictionnaireLocal(string clef, string valeur){
        monDictionnaire[clef] = valeur;
    }
    string LireUneValeurDuDictionnaireLocal(string clef){
        if (monDictionnaire.ContainsKey(clef)){
            return monDictionnaire[clef];
        }else{
            return "";
        }
    }

    void UneCondition(string token, bool obligationDePartire = false){
        string attente = BalliseFermante(token);
        print(token+" : eee : "+attente);
        bool autoriser = false;
        possitionDansLaRecette++;
        if (token != "[else]" && token != "[/else]" && token != "[/if]" && token != "[/elif]"){
            autoriser = Condition(lesActionsAEffectuer[possitionDansLaRecette]);
        }
        if (obligationDePartire) autoriser = false;
        if (!obligationDePartire){
            //---début------------------
            if (token == "[if]"){
                listeDesIf.Add(autoriser);
                print("ici 258");
            }
            else if (token == "[elif]"){
                if (listeDesIf.Count > 0){
                    if (listeDesIf[listeDesIf.Count-1] == true){
                        autoriser = false;
                    }else if (autoriser){
                        listeDesIf[listeDesIf.Count-1] = true;
                    }
                }
            }
            else if (token == "[else]") {
                if (listeDesIf.Count > 0 && listeDesIf[listeDesIf.Count - 1] == true){
                    autoriser = false;
                    print("on a reffuser un else !");
                }else{
                    if (listeDesIf.Count > 0) listeDesIf[listeDesIf.Count-1] = true;
                    else listeDesIf.Add(true);
                    autoriser = true;
                    print("on a autoriser un else !");
                }
            }
        }
        //---fin--------------------

        if (autoriser) {
            if (token != "[else]") possitionDansLaRecette++;
            SuivreEtapeSuivante();
        } else {
            while (lesActionsAEffectuer.Count-1 > possitionDansLaRecette && lesActionsAEffectuer[possitionDansLaRecette] != attente && attente != "") {
                possitionDansLaRecette++;
                if (lesActionsAEffectuer[possitionDansLaRecette]==attente){
                    attente = BalliseFermante(lesActionsAEffectuer[possitionDansLaRecette]);
                }
            }
            if (listeDesIf.Count > 0){
                //print(lesActionsAEffectuer[possitionDansLaRecette]);
                if (!LesIfContinue(lesActionsAEffectuer[possitionDansLaRecette++])){
                    listeDesIf.Remove(listeDesIf[listeDesIf.Count-1]);
                }
            }
            SuivreEtapeSuivante();
        }
    }
    string BalliseFermante(string ballise){
        if (ballise == "[if]"){ return "[/if]";}
        else if (ballise == "[elif]"){ return "[/elif]";}
        else if (ballise == "[else]"){ return "[/else]";}
        else{
            Debug.LogWarning("Erreur des if dans un dialogue");
            return ""; 
        }
    }
    bool LesIfContinue(string cible){
        if (cible == "[elif]" || cible == "[else]") return true;
        else return false;
    }

    /*
    bool Condition (string texteCondition){//[pomme >= 5]
        texteCondition = texteCondition.Substring(1, texteCondition.Length - 2);
        string clef = "";
        string opérateur = "";
        string valeur = "";
        int étape = 0;
        string temps = "";
        foreach (char item in texteCondition){
            if ((signeSpéciaux.Contains(item) && étape == 0)||(étape == 1 && signeSpéciaux.Contains(item) == false)){
                if (étape == 0) clef = temps;
                else if (étape == 1) opérateur = temps;
                étape++;
                temps = "";
                temps += item.ToString();
            }
            else{
                temps += item.ToString();
            }
        }
        valeur = temps;
        clef = RetirerEspace(clef);
        opérateur = RetirerEspace(opérateur);
        valeur = RetirerEspace(valeur);
        print(clef + " ... " + opérateur + " ... " + valeur);
        if (clef == "" || opérateur == "" || valeur == ""){
            return false;
        }
        string t = LireUneValeurDuDictionnaireLocal(clef);
        if (t == "") t = NewDataCenter.Instance.Lire(clef);
        if (nombre.Contains(t[0])){
            if (opérateur == ">") return int.Parse(t) > int.Parse(valeur);
            else if (opérateur == "<") return int.Parse(t) < int.Parse(valeur);
            else if (opérateur == ">=") return int.Parse(t) >= int.Parse(valeur);
            else if (opérateur == "<=") return int.Parse(t) <= int.Parse(valeur);
        }
        if (opérateur == "==") return t == valeur;
        else if (opérateur == "!=") return t != valeur;
        return false;
    }*/

    /// <summary>
    /// Évalue une condition booléenne donnée en chaîne de caractères (avec &&, ||, parenthèses).
    /// Chaque condition atomique (ex. "clé==valeur") est évaluée via ConditionAvecV1.
    /// </summary>
    bool Condition(string texteCondition){
        texteCondition = texteCondition.Replace("[", "");
        texteCondition = texteCondition.Replace("]", "");

        if (texteCondition == null){
            Debug.LogWarning("Avertissement : la chaîne de condition est nulle.");
            return false;
        }
        // Supprimer les espaces inutiles
        string expr = texteCondition.Replace(" ", "");
        return EvaluateExpression(expr);
    }

    /// <summary>
    /// Évalue récursivement une expression booléenne sans espaces.
    /// </summary>
    bool EvaluateExpression(string expr){
        if (string.IsNullOrEmpty(expr)){
            Debug.LogWarning("Avertissement : expression vide.");
            return false;
        }

        // Vérifier l'équilibre des parenthèses
        int balance = 0;
        foreach (char c in expr){
            if (c == '(') balance++;
            else if (c == ')'){
                balance--;
                if (balance < 0){
                    Debug.LogWarning($"Avertissement : parenthèse fermante sans correspondance dans \"{expr}\"");
                    return false;
                }
            }
        }
        if (balance != 0){
            Debug.LogWarning($"Avertissement : parenthèses déséquilibrées dans \"{expr}\"");
            return false;
        }

        // Recherche d'un opérateur '||' au niveau racine (depth=0)
        int depth = 0;
        for (int i = 0; i < expr.Length - 1; i++){
            if (expr[i] == '(') depth++;
            else if (expr[i] == ')') depth--;
            else if (depth == 0 && expr[i] == '|' && expr[i + 1] == '|'){
                // Scinder sur "||"
                string left = expr.Substring(0, i);
                string right = expr.Substring(i + 2);
                return EvaluateExpression(left) || EvaluateExpression(right);
            }
        }

        // Recherche d'un opérateur '&&' au niveau racine
        depth = 0;
        for (int i = 0; i < expr.Length - 1; i++){
            if (expr[i] == '(') depth++;
            else if (expr[i] == ')') depth--;
            else if (depth == 0 && expr[i] == '&' && expr[i + 1] == '&'){
                // Scinder sur "&&"
                string left = expr.Substring(0, i);
                string right = expr.Substring(i + 2);
                return EvaluateExpression(left) && EvaluateExpression(right);
            }
        }

        // Suppression des parenthèses extérieures si elles englobent toute l'expression
        if (expr.Length >= 2 && expr[0] == '(' && expr[expr.Length - 1] == ')'){
            int depth2 = 0;
            for (int i = 0; i < expr.Length; i++){
                if (expr[i] == '(') depth2++;
                else if (expr[i] == ')') depth2--;
                if (depth2 == 0){
                    if (i == expr.Length - 1){
                        // Parenthèses englobantes externes trouvées
                        return EvaluateExpression(expr.Substring(1, expr.Length - 2));
                    }
                    break;
                }
            }
        }

        // Cas atomique : "clé op valeur"
        return EvaluateAtomic(expr);
    }

    /// <summary>
    /// Évalue une condition atomique (sans opérateurs logiques) via ConditionAvecV1.
    /// </summary>
    bool EvaluateAtomic(string atom){
        // Identifier l'opérateur parmi ceux autorisés
        string[] ops = new[] { "==", "!=", ">=", "<=", ">", "<" };
        int index = -1;
        string foundOp = null;
        foreach (string op in ops){
            index = atom.IndexOf(op);
            if (index >= 0){
                foundOp = op;
                break;
            }
        }
        if (foundOp == null){
            Debug.LogWarning($"Avertissement : opérateur logique manquant dans \"{atom}\"");
            return false;
        }

        // Séparer clé et valeur
        string key = atom.Substring(0, index);
        string value = atom.Substring(index + foundOp.Length);
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)){
            Debug.LogWarning($"Avertissement : condition atomique invalide \"{atom}\"");
            return false;
        }

        return ConditionAvecV1(key, foundOp, value);
    }

    // Fonction externe existante (V1) utilisée pour l'évaluation d'une condition atomique.
    // Implémentation fournie séparément – **ne pas modifier**.
    bool ConditionAvecV1(string clef, string opérateur, string valeur){
        //La v1 est fini :
        string t = LireUneValeurDuDictionnaireLocal(clef);
        if (t == "") t = NewDataCenter.Instance.Lire(clef);
        if (t == "") t = "0";

        if (nombre.Contains(t[0])){
            if (opérateur == ">") return int.Parse(t) > int.Parse(valeur);
            else if (opérateur == "<") return int.Parse(t) < int.Parse(valeur);
            else if (opérateur == ">=") return int.Parse(t) >= int.Parse(valeur);
            else if (opérateur == "<=") return int.Parse(t) <= int.Parse(valeur);
            else if (opérateur == "==") return int.Parse(t) == int.Parse(valeur);
            else if (opérateur == "!=") return int.Parse(t) != int.Parse(valeur);
        }
        if (opérateur == "==") return t == valeur;
        else if (opérateur == "!=") return t != valeur;
        return false;
    }
}
[System.Serializable]
public class DesDialogues{
    public string codeDuDialogue;
    public string leContenue;
    /*public void Netoyer(){
        if (codeDuDialogue[0]=='"') codeDuDialogue.Substring(1, codeDuDialogue.Length - 2);
        if (leContenue[0] == '"') leContenue.Substring(1, leContenue.Length - 2);
    }*/
}
