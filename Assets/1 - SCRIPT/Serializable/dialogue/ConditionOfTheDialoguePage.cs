using System.Collections.Generic;
using System.Reflection.Emit;

[System.Serializable]
public class ConditionOfTheDialoguePage{
    public bool visibleCharacter = true;
    public List<Conditionnelle> listOfConditions = new List<Conditionnelle>();
}