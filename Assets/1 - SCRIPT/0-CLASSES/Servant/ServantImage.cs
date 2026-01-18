using System.Diagnostics;
using UnityEngine;

[System.Serializable]
public class ServantImage{
    public Sprite ReadImage_Rang(Rang rang){
        string sentence = "servantImage/image (22)";
        switch (rang){
            case Rang.E :
                return FindImage(sentence+"_2");
            case Rang.D :
                return FindImage(sentence+"_3");
            case Rang.C :
                return FindImage(sentence+"_6");
            case Rang.B :
                return FindImage(sentence+"_5");
            case Rang.A :
                return FindImage(sentence+"_4");
            case Rang.S :
                return FindImage(sentence+"_7");
            default:
                return null;
        }
    }

    public Sprite ReadImage_Tribue(Tribue tribue){
        string sentence = "servantImage/image (7)";
        switch (tribue){
            case Tribue.Vaillant:
                return FindImage(sentence+"_0");
            case Tribue.Mystérieu:
                return FindImage(sentence+"_1");
            case Tribue.Costaud:
                return FindImage(sentence+"_2");
            case Tribue.Mignon:
                return FindImage(sentence+"_3");
            case Tribue.Bienveillant:
                return FindImage(sentence+"_4");
            case Tribue.Sombre:
                return FindImage(sentence+"_5");
            case Tribue.Sinistre:
                return FindImage(sentence+"_6");
            case Tribue.Insaisissable:
                return FindImage(sentence+"_7");
            case Tribue.Perfide:
                return FindImage(sentence+"_8");
            case Tribue.Enma:
                return FindImage(sentence+"_15");
            case Tribue.Wandroïd:
                return FindImage(sentence+"_16");
            default:
                return null;
        }
    }

    public Sprite ReadImage_TribueMedalion(Tribue tribue){
        string sentence = "servantImage/aspirateur";
        switch (tribue){
            case Tribue.Vaillant:
                return FindImage(sentence+"_9");
            case Tribue.Mystérieu:
                return FindImage(sentence+"_10");
            case Tribue.Costaud:
                return FindImage(sentence+"_11");
            case Tribue.Mignon:
                return FindImage(sentence+"_12");
            case Tribue.Bienveillant:
                return FindImage(sentence+"_13");
            case Tribue.Sombre:
                return FindImage(sentence+"_8");
            case Tribue.Sinistre:
                return FindImage(sentence+"_7");
            case Tribue.Insaisissable:
                return FindImage(sentence+"_6");
            case Tribue.Perfide:
                return FindImage(sentence+"_5");
            case Tribue.Enma:
                return FindImage(sentence+"_1");
            case Tribue.Wandroïd:
                return FindImage(sentence+"_4");
            default:
                return null;
        }
    }

    public Sprite ReadImage_Element(Element element){
        string sentence = "servantImage/image (7)";
        switch (element){
            case Element.Neutre:
                return FindImage(sentence+"_25");
            case Element.Feu:
                return FindImage(sentence+"_17");
            case Element.Eau:
                return FindImage(sentence+"_18");
            case Element.Foudre:
                return FindImage(sentence+"_19");
            case Element.Terre:
                return FindImage(sentence+"_20");
            case Element.Vent:
                return FindImage(sentence+"_22");
            case Element.Glace:
                return FindImage(sentence+"_21");
            case Element.Absorption:
                return FindImage(sentence+"_23");
            case Element.Soin:
                return FindImage(sentence+"_24");
            default:
                return null;
        }
    }

    private Sprite FindImage(string file){
        return Resources.Load<Sprite>(file);
    }
}