using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesQuêtesQuiExistent : MonoBehaviour{
    public List<Quête> quête;
    public List<Sprite> imageDesCatégorie;
}

[System.Serializable]
public class Quête{
    public string nomDeLaQuête;
    public string description;
    public CatégorieQuête catégorie;
}
public enum CatégorieQuête {
    Quête,
    Service,
    Histoire,
    Clef
}