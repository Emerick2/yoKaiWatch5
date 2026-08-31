namespace YKW.Core.Item.Enum
{
    /// <summary>Effet produit lorsqu'une nourriture est donnée à un allié.</summary>
    public enum EffetNourritureAllie
    {
        Inconnu = 0,
        RendTresPeuPV,
        RendUnPeuPV,
        RendPasMalPV,
        RendBeaucoupPV,
        RendEnormementPV,
        RestaurePVFixe,
        RendTousLesPV,
        RendUnPeuAme,
        RendPasMalAme,
        RendEnormementAme,
        RendTouteAme,
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
