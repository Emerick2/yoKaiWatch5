using System.Collections.Generic;

[System.Serializable]
public class ConditionOfTheDialoguePage{
    public bool visibleCharacter = true;
    public List<Conditionnelle> listOfConditions = new List<Conditionnelle>();
}