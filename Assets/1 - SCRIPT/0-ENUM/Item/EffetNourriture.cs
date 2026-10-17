namespace YKW.Core.Item.Enum
{
    /// <summary>Effet produit lorsqu'une nourriture est donnée à un allié.</summary>
    public enum EffetNourritureAllie
    {
        Inconnu = 0,
        RendTresPeuPV = 30,
        RendUnPeuPV = 60,
        RendPasMalPV = 80,
        RendBeaucoupPV = 130,
        RendEnormementPV = 180,
        RestaurePVFixe,
        RendTousLesPV,
        RendUnPeuAme = 20,
        RendPasMalAme = 40,
        RendEnormementAme = 70,
        RendTouteAme = 100,
        RecuperePourcentageAme
    }

    /// <summary>Effet produit lorsqu'une nourriture est donnée à un ennemi.</summary>
    public enum EffetNourritureEnnemi
    {
        Inconnu = 0,
        Apprecie,
        ApprecieUnPeu,
        Aime,
        ApprecieBeaucoup,
        Adore,
        DevientAmi
    }
}
