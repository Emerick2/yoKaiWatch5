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
        bool pasSinon = true;
        bool ilFautMettreUnEspace=false;
        string lectureTemps = "";
        string cc = "cc";
        string cc2 = "cc2";
        for (int i = 0; i < instruction.Length; i++){
            pasSinon = true;
            if (lesInstructions.Count > 0){cc = lesInstructions[lesInstructions.Count-1];}
            if (lesInstructions.Count >= 3){cc2 = lesInstructions[lesInstructions.Count-3];}
            if (instruction[i]=='<'){
                lesInstructions.Add(lectureTemps);
                lectureTemps="<";
            }else if (instruction[i]=='>'){
                lectureTemps+=">";
                lesInstructions.Add(lectureTemps);
                lectureTemps="";
            }
            else if ((cc=="<if>" || cc=="< if>" || cc=="<elif>"||cc=="< elif>") || (cc2=="<if>" || cc2=="< if>" || cc2=="<elif>"||cc2=="< elif>")){
                if ((instruction[i]=='=' || instruction[i]==']' || instruction[i]=='[' || instruction[i]=='!') && instruction[i+1]=='='){
                    lesInstructions.Add(lectureTemps);
                    lesInstructions.Add((instruction[i]+instruction[i+1]).ToString());
                    i+=1;
                    lectureTemps="";
                }else if (instruction[i]=='=' || instruction[i]==']' || instruction[i]=='[' || instruction[i]=='!'){
                    lesInstructions.Add(lectureTemps);
                    lesInstructions.Add(instruction[i].ToString());
                    lectureTemps="";
                }else if (instruction[i]==')') {
                    lesInstructions.Add(lectureTemps);
                    print ("ici en"+lectureTemps+ "!");
                    lectureTemps="";
                }
                else{
                    if (instruction[i]!='('){
                        lectureTemps+=instruction[i];
                    }
                }
            }
            else if (cc=="<sauv>" && instruction[i]=='='){
                lesInstructions.Add(lectureTemps);//valeur de la clef
                lesInstructions.Add("=");//valeur du =
                lectureTemps="";//commencer à lire la valeur à mettre dans la clef
            }
            else{
                pasSinon = false;
                if (instruction[i]==' '){ilFautMettreUnEspace=true;}
                else {
                    if (ilFautMettreUnEspace){
                        lectureTemps+=" ";
                        ilFautMettreUnEspace=false;
                    }
                    lectureTemps+=instruction[i];
                }
            }
            if (pasSinon){
                ilFautMettreUnEspace=false;
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
        if(isInRange && Input.GetButtonDown("A")){
            if (lesInstructions.Count==0){//premier passage}
                mouvementDuJoueur.droitDeMarcher = false;
                numéroDeDialogueSauvgarder=numéroDeDialogue;
                ComplétéLesIntructions();
            }
            if (numéroDeDialogueSauvgarder!=numéroDeDialogue){
                mouvementDuJoueur.droitDeMarcher = false;
                numéroDeDialogueSauvgarder=numéroDeDialogue;
                index=0;
                ComplétéLesIntructions();
            }
            pauseDansLaLecture=false;
            if (index < lesInstructions.Count){
                mouvementDuJoueur.droitDeMarcher = false;
                lecture();
            }else{
                mouvementDuJoueur.droitDeMarcher = true;
                index=0;
                DialogueManager.instance.EndDialogue();
            }
        }
    }
    /*
    //si le jeu est cours larvuce (en 2D) : (suivez "la plume numérique d'émerick" sur YouTube pour en savoir plus !)
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            isInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision){
        if (collision.CompareTag("Player")){
            isInRange = false;
            atendUneRéponceALaQuestion=false;
            DialogueManager.instance.EndDialogue();
        }
    }*/
    //si le jeu est yo-kai watch 5 (en 3D) :
    void OnTriggerEnter(Collider collision){
        if(collision.CompareTag("Player")){
            isInRange = true;
        }
    }

    void OnTriggerExit(Collider collision){
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
            }else if (lesInstructions[index]=="<nom>"){
                if (lesInstructions[index+1]=="%"){
                    dialogue.name = récupéréValeur("nomDuJoueur");
                    index+=3;
                }else{
                    dialogue.name = lesInstructions[index+1];
                    index+=3;
                }
            }
            else if (lesInstructions[index]=="</if>" || lesInstructions[index]=="</elif>" || lesInstructions[index]=="</else>"){
                index++;
                if (index<lesInstructions.Count && (lesInstructions[index]!="<elif>" && lesInstructions[index]!="<else>")){
                    jeSuisDansAttanteDeUnIf=true;
                }
            }else if (lesInstructions[index]=="<if>" || lesInstructions[index]=="<elif>" || lesInstructions[index]=="<else>"){
                string v=lesInstructions[index];
                bool autorisation = vérifieLaConditionDeIf(lesInstructions[index+1],lesInstructions[index+2],lesInstructions[index+3]);
                index+=4;
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
        if ((index > lesInstructions.Count) || (index == lesInstructions.Count && lesInstructions[index-1]!="</t>")){
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
les instruction :
<t>blabla</t> : permet d'entrer du texte
<choix>acheter le pain<choix1>oui</choix1><choix2>non</choix2></choix> : permet de demander à l'utilisateur qu'elque chose et de l'enregistre localement, le nombre de choix peut varier de 1 à 4
<sauv>pain=+1</sauv> : permet de sauvegarder une donné dans une clef (cela peut être un calcule simple pour ajouter ou retirer une valeur ou un texte (si c'est un texte il ne faut pas commencer par + ou -))
<sauvLocal>pain=+1</sauvLocal>
<if>(pain=2)<t>tu à beaucoup de pain.</t></if> : le if doit rester ouvert tous le long de la conversation de discusion, la valeur regarder peut être local ou sauvegarder, il peut y avoir des if dans un if ou tous ce que l'on veut dans les if
<elif>(pain[0)<t>tu n'a pas de pain !</t></elif> : comme le if mais il s'exécute si l'est if précédent n'ont pas été valider et les elif précédent n'ont pas été valider.
<else><t>bonjour</t></else> : si ni les if ni les elif n'ont été valider, fonctionne comme un if

pas encor créé : 
<afficheSauv>pain</afficheSauv> : prend une donné qui est sauvegarder et l'insère dans le dialogue
<musique>2</musique> : joue une fois la musique qui à l'identifient demander (les identifient commence à 0 et pas à 1)
<apparition>6=true</apparition> : fait apparaitre(true) ou disparaitre(false) un gameObject dans la scène
*/