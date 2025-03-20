using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class PNJ_script : MonoBehaviour {
    public string numéroDeDialogue;
    private string numéroDeDialogueSauvgarder;
    private GameObject canevas;
    public GameObject canevasPrefab;
    private string leNom="???";
    private string instruction;
    public List<string> lesInstructions;
    public bool atendUneRéponceALaQuestion;
    private bool isInRange;
    private bool pauseDansLaLecture;
    private int index=0;
    private Dialogue dialogue;
    public Dictionary<string, object> monDictionnaire = new Dictionary<string, object>();
    private bool jeSuisDansAttanteDeUnIf=true;

    void ComplétéLesIntructions(){
        lesInstructions.Clear();
        string filePath = Application.dataPath + "/mes dialogues.json";
        if (File.Exists(filePath)){
            string jsonText = File.ReadAllText(filePath);
            DialogueContainer data = JsonUtility.FromJson<DialogueContainer>(jsonText);
            
            foreach (Dialogue2 dialogue2 in data.dialogues) {
                if (dialogue2.numéro == numéroDeDialogue) {
                    instruction=dialogue2.texte;
                    leNom = dialogue2.nom;
                    //RetireLesEspaceDeInstruction();
                }
            }
        }
        //canevasPrefab = Resources.Load<GameObject>("CANEVAS");

        canevas = GameObject.FindWithTag("Canevas");
        if (canevas == null){
            canevas = Instantiate(canevasPrefab);
        }

        bool ilFautMettreUnEspace=false;
        string lectureTemps = "";
        string cc = "cc";
        string cc2 = "cc2";
        for (int i = 0; i < instruction.Length; i++){
            if (lesInstructions.Count > 0){
                cc = lesInstructions[lesInstructions.Count-1];
            }
            if (lesInstructions.Count >= 3){
                cc2 = lesInstructions[lesInstructions.Count-3];
            }
            if (instruction[i]=='<'){
                lesInstructions.Add(lectureTemps);
                lectureTemps="<";
            }else if (instruction[i]=='>'){
                lectureTemps+=">";
                lesInstructions.Add(lectureTemps);
                lectureTemps="";
            }
            else if (instruction[i]=='=' && (cc=="<if>" || cc=="< if>" || cc=="<elif>"||cc=="< elif>")){
                lesInstructions.Add(lectureTemps);
                lesInstructions.Add("=");
                lectureTemps="";
            }else if (instruction[i]=='%'&& (cc2=="<if>" || cc2=="< if>" || cc2=="<elif>"||cc2=="< elif>")){
                lesInstructions.Add(lectureTemps);
                lectureTemps="";
            }
            else{
                bool rienDeSpécialNeSaitPasser=true;
                if (lesInstructions.Count>0){
                    string va = lesInstructions[lesInstructions.Count-1];
                    //cas de la sauvgarde
                    if (va=="<sauv>" && instruction[i]=='='){
                        lesInstructions.Add(lectureTemps);//valeur de la clef
                        lesInstructions.Add("=");//valeur du =
                        lectureTemps="";//commencer à lire la valeur à mettre dans la clef
                        rienDeSpécialNeSaitPasser=false;
                    }
                    //cas du if/elif/else
                    if (va=="<if>" || va=="<elif>" || va=="<else>"){
                        if (instruction[i]==']' || instruction[i]=='[' || instruction[i]=='='){
                            if (instruction[i+1]==']' || instruction[i+1]=='[' || instruction[i+1]=='=') {Debug.LogWarning("Une boîte de dialogue est mal configuer, il y a une tentative de <= ou >= ou une autre erreur dans se style !");}
                            else{
                                lesInstructions.Add(lectureTemps);//valeur de la clef
                                lesInstructions.Add(instruction[i].ToString());//valeur de l'oppérateur
                                lectureTemps="";//commencer à lire la valeur à mettre dans la clef
                                rienDeSpécialNeSaitPasser=false;
                            }
                        }
                    }
                }
                if(rienDeSpécialNeSaitPasser) {
                    if (instruction[i]==' '){ilFautMettreUnEspace=true;}
                    else {
                        if (ilFautMettreUnEspace){
                            lectureTemps+=" ";
                            ilFautMettreUnEspace=false;
                        }
                        lectureTemps+=instruction[i];
                    }
                }else{ilFautMettreUnEspace=false;}
            }
        }
        for (int i = 0; i < lesInstructions.Count; i++){
            if (lesInstructions[i]==""){
                lesInstructions.RemoveAt(i);
            }
        }
        for (int i = 0; i < lesInstructions.Count; i++){
            if (lesInstructions[i][0]=='<'){
                string valeurTotal="";
                for (int j = 0; j < lesInstructions[i].Length; j++){
                    if (lesInstructions[i][j]==' '){}
                    else{valeurTotal+=lesInstructions[i][j];}
                }
                lesInstructions[i]=valeurTotal;
            }
        }
    }

    void Update(){
        if(isInRange && Input.GetKeyDown(KeyCode.E)){
            if (lesInstructions.Count==0){//premier passage}
                numéroDeDialogueSauvgarder=numéroDeDialogue;
                ComplétéLesIntructions();
            }
            if (numéroDeDialogueSauvgarder!=numéroDeDialogue){
                numéroDeDialogueSauvgarder=numéroDeDialogue;
                index=0;
                ComplétéLesIntructions();
            }
            pauseDansLaLecture=false;
            if (index < lesInstructions.Count){
                lecture();
            }else{
                index=0;
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider collision){
        if(collision.CompareTag("Player")){
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider collision){
        if (collision.CompareTag("Player")){
            isInRange = false;
            atendUneRéponceALaQuestion=false;
            DialogueManager.instance.EndDialogue();
        }
    }

    void faireLaLecture(string phraseALire) {
        pauseDansLaLecture=true;
        if (!DialogueManager.instance.dialogueEnCours){
            dialogue = new Dialogue();
            dialogue.name = leNom;
            dialogue.sentences = new string[] { phraseALire }; // Convertir la phrase en tableau
            DialogueManager.instance.StartDialogue(dialogue);
        }else{
            if (dialogue == null){
                dialogue = new Dialogue();
            }
            if (dialogue.sentences == null){
                dialogue.sentences = new string[0];
            }
            dialogue.sentences[0] = phraseALire;
            DialogueManager.instance.passerParLeNouveauPNJQuiNeSaitPasParler(dialogue);
        }
        phraseALire="";
    }
    public void enregistrerUneValeur(string clefs, string valeur, bool local){
        int valeur2;
        if (valeur[0] == '+' || valeur[0] == '-'){ 
            string valeurSansSigne = valeur.Substring(1); // Retire le premier caractère
            if (!int.TryParse(valeurSansSigne, out int valeurInt)){
                Debug.LogError("Valeur invalide : " + valeurSansSigne);
                return;
            }
            int valeurActuelle = int.Parse(récupéréValeur(clefs));
            valeur2 = (valeur[0] == '+') ? valeurActuelle + valeurInt : valeurActuelle - valeurInt;
            valeur=valeur2.ToString();
        }

        if (local){
            monDictionnaire[clefs] = valeur.ToString();
        }
        else{
            NewDataCenter.Instance.SauvegarderSTR(clefs, valeur.ToString());
        }
        print(clefs);
        Debug.Log("Valeur actuelle de monChoix : " + récupéréValeur("monChoix"));
    }
    string récupéréValeur(string clefs){
        if (monDictionnaire.ContainsKey(clefs)){
            return monDictionnaire[clefs]?.ToString() ?? "0";
        }else{
            return NewDataCenter.Instance.Lire(clefs) ?? "0";
        }
    }
    bool vérifieLaConditionDeIf(string clef, string opérateur, string valeur) {
        if (opérateur == "="){
            if (récupéréValeur(clef) == valeur){return true;}
            else {return false;}
        }else if (opérateur == "!="){
            if (récupéréValeur(clef) != valeur){return true;}
            else {return false;}
        }else if (opérateur == "]"){
            if (int.Parse(récupéréValeur(clef)) > int.Parse(valeur)){return true;}
            else {return false;}
        }else if (opérateur == "["){
            if (int.Parse(récupéréValeur(clef)) < int.Parse(valeur)){return true;}
            else {return false;}
        }else if (opérateur == "]="){
            if (int.Parse(récupéréValeur(clef)) >= int.Parse(valeur)){return true;}
            else {return false;}
        }else if (opérateur == "[="){
            if (int.Parse(récupéréValeur(clef)) <= int.Parse(valeur)){return true;}
            else {return false;}
        }else{
            return false;
        }
    }


    void lecture(){
        if (index == 0){
            pauseDansLaLecture=false;
            atendUneRéponceALaQuestion=false;
        }
        
        while (index < lesInstructions.Count && !pauseDansLaLecture) {
            if (lesInstructions[index]=="<t>"){
                faireLaLecture(lesInstructions[index+1]);
                index+=3;
            }else if (lesInstructions[index]=="<sauv>"){
                enregistrerUneValeur(lesInstructions[index+1], lesInstructions[index+3], false);
                index+=5;
            }else if (lesInstructions[index]=="<sauvLocal>"){
                enregistrerUneValeur(lesInstructions[index+1], lesInstructions[index+3], true);
                index+=5;
            }else if (lesInstructions[index]=="</if>" || lesInstructions[index]=="</elif>" || lesInstructions[index]=="</else>"){
                index++;
                if (index<lesInstructions.Count && (lesInstructions[index]!="<elif>" && lesInstructions[index]!="<else>")){
                    jeSuisDansAttanteDeUnIf=true;
                }
            }else if (lesInstructions[index]=="<if>" || lesInstructions[index]=="<elif>" || lesInstructions[index]=="<else>"){
                string v=lesInstructions[index];
                print(lesInstructions[index+1]);
                bool autorisation = vérifieLaConditionDeIf(lesInstructions[index+1],lesInstructions[index+2],lesInstructions[index+3]);
                index+=4;
                print(lesInstructions[index]);
                if (v=="<if>"){
                    if (autorisation){
                        jeSuisDansAttanteDeUnIf=false;
                    }else{
                        jeSuisDansAttanteDeUnIf = true;
                        int compteur = 0;
                        while (index < lesInstructions.Count && (lesInstructions[index]!="</if>" || compteur>0)) {
                            if (lesInstructions[index]=="<if>"){compteur++;}
                            if (lesInstructions[index]=="</if>"){compteur--;}
                            index++;
                        }
                    }
                }//prendre en compte les if dans les if quand je passe
                else if (jeSuisDansAttanteDeUnIf && (autorisation || v=="<else>")){
                    jeSuisDansAttanteDeUnIf=false;
                }else{
                    string rechercher="";
                    if (v=="<elif>"){rechercher="</elif>";}
                    if (v=="<else>"){rechercher="</else>";}
                    int compteur=0;
                    while (index < lesInstructions.Count && (lesInstructions[index]!=rechercher || compteur>0)) {
                        if (lesInstructions[index]==v){compteur++;}
                        if (lesInstructions[index]==rechercher){compteur--;}
                        index++;
                    }
                }
            }else if (lesInstructions[index]=="<choix>"){
                string choix1="";string choix2="";string choix3="";string choix4="";
                string nomDeLaClefDuChois=lesInstructions[index+1];
                print(nomDeLaClefDuChois);
                index+=2;//je me retrouve sur l'index de <choix1>
                if (lesInstructions[index]=="<choix1>"){
                    choix1 = lesInstructions[index+1];
                    index+=3;
                }if (lesInstructions[index]=="<choix2>"){
                    choix2 = lesInstructions[index+1];
                    index+=3;
                }if (lesInstructions[index]=="<choix3>"){
                    choix3 = lesInstructions[index+1];
                    index+=3;
                }if (lesInstructions[index]=="<choix4>"){
                    choix4 = lesInstructions[index+1];
                    index+=3;
                }
                canevas.GetComponent<lechoixExtension>().mettreUnchoix(choix1,choix2,choix3,choix4,gameObject,nomDeLaClefDuChois);
                pauseDansLaLecture=true;
            }
            else{index++;}
        }
        if (index > lesInstructions.Count){
            index=0;
            DialogueManager.instance.EndDialogue();
        }
    }
}
[System.Serializable]
public class Dialogue2
{
    public string numéro;
    public string nom;
    public string texte;
}

[System.Serializable]
public class DialogueContainer
{
    public Dialogue2[] dialogues;
}
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class PNJ_script : MonoBehaviour {
    public string numéroDeDialogue;
    private string instruction = "";
    private int index = 0;
    private string instructionEnCours = "";
    private string lectrueDeInstructionEnCours = "";
    private bool jeLiUneInstruction;
    private bool jeLiUneInstructionFermante;
    private string phraseALire = "";
    
    private Dictionary<string, object> monDictionnaire = new Dictionary<string, object>();
    private List<string> T = new List<string> {};
    //pour le dialogue :
    public bool isInRange;
    private Dialogue dialogue;

    private bool pauseDansLaLecture;

    //pour les if :
    private List<string> empilementDesIf = new List<string> {};
    private List<bool> attentUnIf = new List<bool> {};

    //sauvegarde
    private string keys;
    private bool sauvegardeEnCours;
    [HideInInspector]
    public bool atendUneRéponceALaQuestion;
    private GameObject canevas;
    public GameObject canevasPrefab;
    public string leNom="???";
    void Start(){
        string filePath = Application.dataPath + "/mes dialogues.json";
        if (File.Exists(filePath)){
            string jsonText = File.ReadAllText(filePath);
            DialogueContainer data = JsonUtility.FromJson<DialogueContainer>(jsonText);
            
            foreach (Dialogue2 dialogue2 in data.dialogues) {
                if (dialogue2.numéro == numéroDeDialogue) {
                    instruction=dialogue2.texte;
                    leNom = dialogue2.nom;
                    //RetireLesEspaceDeInstruction();
                }
            }
        }
        //canevasPrefab = Resources.Load<GameObject>("CANEVAS");

        canevas = GameObject.FindWithTag("Canevas");
        if (canevas == null){
            canevas = Instantiate(canevasPrefab);
        }
    }

    void RetireLesEspaceDeInstruction(){
        string résultat="";
        int ii=0;
        bool duTexte=false;
        while (ii < instruction.Length) {
            if (ii+2 < instruction.Length && instruction[ii]=='<'&&instruction[ii+1]=='t'&&instruction[ii+2]=='>'){
                duTexte=true;
                ii+=3;
            }else if (ii+3 < instruction.Length && instruction[ii]=='<'&&instruction[ii+1]=='/'&&instruction[ii+2]=='t'&&instruction[ii+3]=='>'){
                duTexte=false;
                ii+=4;
            }if (duTexte || (ii < instruction.Length && instruction[ii] != ' ')){
                résultat+=instruction[ii];
            }
            ii++;
        }instruction=résultat;
    }

    void lecture() {
        if (index == 0){
            phraseALire = "";
            pauseDansLaLecture=false;
            atendUneRéponceALaQuestion=false;
            attentUnIf.Clear();
            empilementDesIf.Clear();
        }
        
        while (index < instruction.Length && !pauseDansLaLecture) {
            if (index + 1 < instruction.Length && instruction[index] == '<' && instruction[index + 1] == '/') { 
                jeLiUneInstruction = true;
                jeLiUneInstructionFermante = true;
                index += 2;
                lectrueDeInstructionEnCours = "";
            } 
            else if (instruction[index] == '<') {
                jeLiUneInstruction = true;
                lectrueDeInstructionEnCours = "";
                index += 1;
            } else if (instruction[index] == '>') {
                jeLiUneInstruction = false;
                index += 1;
                effetDesBaliseFermante();
            }else if (instruction[index] == '=' && sauvegardeEnCours){
                keys=phraseALire;
                phraseALire="";
                index += 1;
            } else if (instruction[index] == '$'){
                attentUnIf.Clear();
                empilementDesIf.Clear();
                index += 1;
            }else if (index < instruction.Length) {
                if (jeLiUneInstruction) {
                    lectrueDeInstructionEnCours += instruction[index];
                } 
                else {
                    phraseALire += instruction[index];
                }
                index += 1;
            }
        }
        if (index > instruction.Length){
            index=0;
            DialogueManager.instance.EndDialogue();
        }
    }
    void effetDesBaliseFermante(){
        if (jeLiUneInstructionFermante){
            if (lectrueDeInstructionEnCours == "t") { 
                faireLaLecture();
            } else if ((lectrueDeInstructionEnCours == "if" || lectrueDeInstructionEnCours == "elif" || lectrueDeInstructionEnCours == "else")){
                lesBaliseIfElifElse(lectrueDeInstructionEnCours, true);
            }else if (lectrueDeInstructionEnCours=="sauv"||lectrueDeInstructionEnCours=="sauvLocal"){
                if (lectrueDeInstructionEnCours!="sauvLocal"){
                    enregistrerUneValeur(keys, phraseALire, false);
                }else{
                    enregistrerUneValeur(keys, phraseALire, true);
                }
                phraseALire="";
                sauvegardeEnCours=false;
            }else if (lectrueDeInstructionEnCours=="fin"){
                DialogueManager.instance.EndDialogue();
            }
        }else{
            if (lectrueDeInstructionEnCours == "if" || lectrueDeInstructionEnCours == "elif" || lectrueDeInstructionEnCours == "else") {
                lesBaliseIfElifElse(lectrueDeInstructionEnCours, false);
            }else if (lectrueDeInstructionEnCours=="sauv" || lectrueDeInstructionEnCours=="sauvLocal"){
                sauvegardeEnCours=true;
            }else if (lectrueDeInstructionEnCours=="t"){
                phraseALire="";
            }else if (lectrueDeInstructionEnCours=="choix"){
                proposerUnChoix();
            }
        }
        jeLiUneInstructionFermante = false;
        lectrueDeInstructionEnCours = "";
    }
    string ExtraireCondition() {
        string condition = "";
        while (index < instruction.Length && instruction[index] != '%') {
            condition += instruction[index];
            index++;
        }
        index++; // Passer le '%'
        return condition;
    }
    void SauterBloc() {
        int profondeur = 1;
        while (index < instruction.Length && profondeur > 0) {
            if (instruction[index] == '<' && instruction[index + 1] == '/') { 
                profondeur--;
            } else if (instruction[index] == '<' && instruction[index + 1] != '/') { 
                profondeur++;
            }
            index++;
        }
    }

    void faireLaLecture() {
        pauseDansLaLecture=true;
        if (!DialogueManager.instance.dialogueEnCours){
            dialogue = new Dialogue();
            dialogue.name = leNom;
            dialogue.sentences = new string[] { phraseALire }; // Convertir la phrase en tableau
            DialogueManager.instance.StartDialogue(dialogue);
        }else{
            if (dialogue == null){
                dialogue = new Dialogue();
            }
            if (dialogue.sentences == null){
                dialogue.sentences = new string[0];
            }
            dialogue.sentences[0] = phraseALire;
            DialogueManager.instance.passerParLeNouveauPNJQuiNeSaitPasParler(dialogue);
        }
        phraseALire="";
    }
    void proposerUnChoix() {
        //<choix>acheter le pain<choix1>oui</choix1><choix2>non</choix2></choix>
        atendUneRéponceALaQuestion=true;
        int i=index;
        string temps = "";string clef = "";
        bool lecturClef=true;
        string choix1="";string choix2="";string choix3="";string choix4="";
        string chaine = instruction;
        bool continuer = true;
        while (i + 7 < chaine.Length && continuer) {
            if (chaine.Substring(i, 8) == "</choix>") {
                continuer = false;
                i += 8;
                index=i;pauseDansLaLecture=true;
                canevas.GetComponent<lechoixExtension>().mettreUnchoix(choix1,choix2,choix3,choix4,gameObject,clef);
                break;
            }
            if (!continuer){
                return;
            }
            string test7 = i + 8 <= chaine.Length ? chaine.Substring(i, 8) : "";
            string test8 = i + 9 <= chaine.Length ? chaine.Substring(i, 9) : "";

            if ("<choix1>" == test7 ||"<choix2>" == test7 ||"<choix3>" == test7 || "<choix4>" == test7){
                temps="";lecturClef=false;i+=8;
            }else if("</choix1>" == test8){
                choix1=temps;i+=9;
            }else if("</choix2>" == test8){
                choix2=temps;i+=9;
            }else if("</choix3>" == test8){
                choix3=temps;i+=9;
            }else if("</choix4>" == test8){
                choix4=temps;i+=9;
            }if(chaine[i]=='%'){//"</choix%>"
                continuer=false;
                i+=2;
            }
            if (i < chaine.Length && continuer) {
                if (lecturClef) {
                    clef += chaine[i];
                } else if (continuer) {
                    if(chaine[i]=='>'){
                        temps="";
                    }else{
                        temps += chaine[i];
                    }
                }
                i++;
            }
        }index=i;pauseDansLaLecture=true;
        canevas.GetComponent<lechoixExtension>().mettreUnchoix(choix1,choix2,choix3,choix4,gameObject,clef);
    }

    void Update(){
        if(isInRange && Input.GetKeyDown(KeyCode.E)){
            print("début");
            pauseDansLaLecture=false;
            if (index < instruction.Length){
                lecture();
            }else{
                index=0;
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    private void OnTriggerEnter(Collider collision){
        if(collision.CompareTag("Player")){
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider collision){
        if (collision.CompareTag("Player")){
            isInRange = false;
            atendUneRéponceALaQuestion=false;
            DialogueManager.instance.EndDialogue();
        }
    }
    public void enregistrerUneValeur(string clefs, string valeur, bool local){
        int valeur2;
        if (valeur[0] == '+' || valeur[0] == '-'){ 
            string valeurSansSigne = valeur.Substring(1); // Retire le premier caractère
            if (!int.TryParse(valeurSansSigne, out int valeurInt)){
                Debug.LogError("Valeur invalide : " + valeurSansSigne);
                return;
            }
            int valeurActuelle = int.Parse(récupéréValeur(clefs));
            valeur2 = (valeur[0] == '+') ? valeurActuelle + valeurInt : valeurActuelle - valeurInt;
            valeur=valeur2.ToString();
        }

        if (local){
            monDictionnaire[clefs] = valeur.ToString();
        }
        else{
            NewDataCenter.Instance.SauvegarderSTR(clefs, valeur.ToString());
        }
        print(clefs);
        Debug.Log("Valeur actuelle de monChoix : " + récupéréValeur("monChoix"));
    }
    string récupéréValeur(string clefs){
        if (monDictionnaire.ContainsKey(clefs)){
            return monDictionnaire[clefs]?.ToString() ?? "0";
        }else{
            return NewDataCenter.Instance.Lire(clefs) ?? "0";
        }
    }
    bool vérifieLaConditionDeIf(string chaine) {
        décripterLaChaine(chaine);
        int x=100;
        bool valeurARenvoyer=false;
        bool valeurARenvoyerT=false;
        while (T.Count != 0 && x>=0){
            x--;
            if (T[1] == "="){
                valeurARenvoyerT = petitIf(T[0],T[2],true);
                T.RemoveAt(0);T.RemoveAt(0);T.RemoveAt(0);
            }else if (T[1] == "!="){
                bool valeur = !petitIf(T[0],T[2],true);
                valeurARenvoyerT = valeur;
                T.RemoveAt(0);T.RemoveAt(0);T.RemoveAt(0);
            }else if (T[1] == "<" || T[1] == ">" || T[1] == "<=" || T[1] == ">="){
                bool valeur = petitIf(T[0],T[2],false);
                valeurARenvoyerT = valeur;
                T.RemoveAt(0);T.RemoveAt(0);T.RemoveAt(0);
            }
        }
        return valeurARenvoyerT;
    }
    bool petitIf(string clef,string valeur, bool pasDuInt){
        bool réponce = false;
        if (pasDuInt){
            if (récupéréValeur(clef)==valeur){réponce = true;}
            else{réponce = false;}
        }else{
            int y = int.Parse(récupéréValeur(clef));
            if (T.Count >0){
                if (T[0] == ">="){if (y>=int.Parse(valeur)){réponce = true;}}
                else if (T[0] == "<="){if (y<=int.Parse(valeur)){réponce = true;}}
                else if (T[0] == ">"){if (y>int.Parse(valeur)){réponce = true;}}
                else if (T[0] == "<"){if (y<int.Parse(valeur)){réponce = true;}}
                else{réponce = false;}
            }else{
                Debug.LogWarning("ATTENTION : à la ligne 340 de PNJ_script la liste T est vide il FAUT que T ne soit pas vide !");
            }
        }
        return réponce;
    }
    void décripterLaChaine(string chaine){
        int i=0;
        string temps = "";
        while (chaine.Length > i){
            if ((chaine[i]=='=' || chaine[i]=='!' || chaine[i]=='<' || chaine[i]=='>') && temps!=""){
                T.Add(temps);
                temps="";
                if (chaine[i]=='='){T.Add("=");i++;}
                else if (chaine[i]=='!' && chaine[i+1]=='='){T.Add("!=");i+=2;}
                else if (chaine[i]=='<' && chaine[i+1]=='='){T.Add("<=");i+=2;}
                else if (chaine[i]=='>' && chaine[i+1]=='='){T.Add(">=");i+=2;}
                else if (chaine[i]=='<' && chaine[i+1]!='='){T.Add("<");i++;}
                else if (chaine[i]=='>' && chaine[i+1]!='='){T.Add(">");i++;}
            }
            temps+=chaine[i];
            i++;
        }
        if (temps!=""){T.Add(temps);}
    }

    void lesBaliseIfElifElse(string nom, bool fermeture){
        //empilementDesIf     attentUnIf
        if (nom == "if" && !fermeture){
            if (vérifieLaConditionDeIf(ExtraireCondition())) {
                empilementDesIf.Add(nom);
                attentUnIf.Add(false);
            }else{
                attentUnIf.Add(true);
                SauterBloc();
            }
        }else if (nom=="elif" && !fermeture){
            if (vérifieLaConditionDeIf(ExtraireCondition())&&attentUnIf[attentUnIf.Count-1]==true) {
                empilementDesIf.Add(nom);
                attentUnIf[attentUnIf.Count-1]=false;
            }else{
                SauterBloc();
            }
        }else if (nom=="else" && !fermeture){
            if (attentUnIf.Count >0 && attentUnIf[attentUnIf.Count-1]==true) {
                empilementDesIf.Add(nom);
                attentUnIf[attentUnIf.Count-1]=false;
            }else{
                SauterBloc();
            }
        }else if(nom == "if" && fermeture){
            if (empilementDesIf.Count>0 && empilementDesIf[empilementDesIf.Count-1]=="if"){
                if (empilementDesIf.Count>0) empilementDesIf.RemoveAt(empilementDesIf.Count - 1);
                if (attentUnIf.Count>0) attentUnIf.RemoveAt(attentUnIf.Count - 1);
            }
        }else if(nom == "elif" && fermeture){
            if (empilementDesIf.Count>0 && empilementDesIf[empilementDesIf.Count-1]=="elif"){
                if (empilementDesIf.Count>0) empilementDesIf.RemoveAt(empilementDesIf.Count - 1);
                if (attentUnIf.Count>0) attentUnIf.RemoveAt(attentUnIf.Count - 1);
            }
        }else if(nom == "else" && fermeture){
            if (empilementDesIf.Count>0 && empilementDesIf[empilementDesIf.Count-1]=="else"){
                if (empilementDesIf.Count>0) empilementDesIf.RemoveAt(empilementDesIf.Count - 1);
                if (attentUnIf.Count>0) attentUnIf.RemoveAt(attentUnIf.Count - 1);
            }
        }
    }
}
[System.Serializable]
public class Dialogue2
{
    public string numéro;
    public string nom;
    public string texte;
}

[System.Serializable]
public class DialogueContainer
{
    public Dialogue2[] dialogues;
}
*/

/*
les instruction :
<t>blabla</t> : permet d'entrer du texte
<choix>acheter le pain<choix1>oui</choix1><choix2>non</choix2></choix> : permet de demander à l'utilisateur qu'elque chose et de l'enregistre localement, le nombre de choix peut varier de 1 à 4
<sauv>pain=+1</sauv> : permet de sauvegarder une donné dans une clef (cela peut être un calcule simple pour ajouter ou retirer une valeur ou un texte (si c'est un texte il ne faut pas commencer par + ou -))
<sauvLocal>pain=+1</sauvLocal>
<afficheSauv>pain</afficheSauv> : prend une donné qui est sauvegarder et l'insère dans le dialogue
<if>pain>=2%<t>tu à beaucoup de pain.</t></if> : le if doit rester ouvert tous le long de la conversation de discusion, la valeur regarder peut être local ou sauvegarder, il peut y avoir des if dans un if ou tous ce que l'on veut dans les if
<elif>pain<=0%<t>tu n'a pas de pain !</t></elif> : comme le if mais il s'exécute si l'est if précédent n'ont pas été valider et les elif précédent n'ont pas été valider.
<else>bonjour</else> : si ni les if ni les elif n'ont été valider, fonctionne comme un if
$ : il faut en mettre un pour réinitialiser les if et pouvoir en réutiliser.
<musique>2</musique> : joue une fois la musique qui à l'identifient demander (les identifient commence à 0 et pas à 1)
<apparition>6=true</apparition> : fait apparaitre(true) ou disparaitre(false) un gameObject dans la scène
*/