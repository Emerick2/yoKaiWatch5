namespace YKW.Core.YoKai.Enums
{
    /// <summary>
    /// Semantic contexts in which a Yo-kai voice line can be played.
    ///
    /// Important:
    /// The original games do NOT appear to expose one dedicated audio cue
    /// for every value below.
    ///
    /// Yo-kai Watch 1 / YW1 Switch generally stores battle voices under a
    /// BattleVc bank associated with the Yo-kai ID, while the game decides
    /// which sample/variation is used for a given gameplay action.
    ///
    /// Sources:
    /// - Yo-kai Watch 1 / YW1 Switch audio data:
    ///   yw_sound.acf, yw_chara.acb, yw_boss.acb, yw_medal.acb,
    ///   yw_vc.acb, yw_vevent.acb, partvoice.acb.
    /// - yo-docs: "Adding Sounds" / "Adding Voices (Medallium)"
    /// - Yo-kai Watch Wiki quote categories and individual Yo-kai quote pages.
    /// </summary>
    public enum YoKaiSoundVoiceType
    {
        /// <summary>
        /// Generic encounter dialogue.
        /// Documented for several special encounters, including Wanted Yo-kai.
        /// Source: Yo-kai Watch Wiki.
        /// </summary>
        Encounter = 0,

        /// <summary>
        /// Dialogue played before starting a battle.
        /// Source: documented "Before battle" quotes for Yo-kai such as
        /// Topanyan, Dianyan and Sapphinyan.
        /// </summary>
        PreBattle = 1,

        /// <summary>
        /// Short battle voice used when attacking.
        /// Source: documented "Attacking" quotes, e.g. Hissfit.
        ///
        /// Game-data note:
        /// Not confirmed as a dedicated Attack audio cue.
        /// Usually part of the Yo-kai's BattleVc data.
        /// </summary>
        Attack = 2,

        /// <summary>
        /// Voice accompanying a Technique.
        ///
        /// Technique is an official battle action.
        /// No dedicated "TechniqueVoice" cue name has been confirmed in YW1.
        /// Expected to use BattleVc variants.
        /// </summary>
        Technique = 3,

        /// <summary>
        /// Voice accompanying an Inspirit action.
        /// Source: explicit "Inspiriting" quotes exist for Yo-kai such as Copyled.
        ///
        /// No dedicated Inspirit audio cue name has been confirmed in YW1.
        /// </summary>
        Inspirit = 4,

        /// <summary>
        /// Voice accompanying a Soultimate Move.
        ///
        /// Soultimate is an official battle action.
        /// No dedicated SoultimateVoice cue name has been confirmed in YW1;
        /// battle voice data and WAZA/Special Attack audio are stored separately.
        /// </summary>
        Soultimate = 5,

        /// <summary>
        /// Voice/reaction while guarding.
        ///
        /// Guard is an official battle action.
        /// No dedicated GuardVoice audio cue has currently been confirmed.
        /// </summary>
        Guard = 6,

        /// <summary>
        /// Voice played when a Yo-kai loafs during its turn.
        /// Source: official gameplay context and standard Yo-kai Wiki quote category.
        /// </summary>
        Loafing = 7,

        /// <summary>
        /// Generic damage reaction.
        ///
        /// Gameplay event is confirmed, but no dedicated HurtVoice cue name
        /// has currently been identified in the original YW1 audio data.
        /// </summary>
        Hurt = 8,

        /// <summary>
        /// Stronger damage reaction, intended for critical/heavy damage.
        ///
        /// YKW5 semantic extension.
        /// Not currently confirmed as a separate Level-5 voice category.
        /// </summary>
        CriticalHurt = 9,

        /// <summary>
        /// Voice/reaction when the Yo-kai is knocked out.
        ///
        /// KO is a real gameplay state, but a dedicated KOVoice cue has not
        /// currently been confirmed in YW1.
        /// </summary>
        KO = 10,

        /// <summary>
        /// Reaction after being purified from a negative Inspirit.
        ///
        /// Purification is an official gameplay mechanic.
        /// Dedicated voice category not currently confirmed.
        /// </summary>
        Purified = 11,

        /// <summary>
        /// Victory reaction after a battle.
        ///
        /// Useful YKW5 semantic event.
        /// Dedicated Level-5 VictoryVoice slot not currently confirmed.
        /// </summary>
        Victory = 12,

        /// <summary>
        /// Dialogue played after a battle.
        /// Source: documented "Post battle" quotes for Yo-kai such as
        /// Topanyan, Dianyan and Sapphinyan.
        /// </summary>
        PostBattle = 13,

        /// <summary>
        /// Voice played when the Yo-kai becomes the player's friend.
        /// Source: standard "Befriended" quote category.
        /// </summary>
        Befriend = 14,

        /// <summary>
        /// Reaction when receiving the Yo-kai's favourite food.
        /// Source: standard "Receiving food (favorite)" quote category.
        /// </summary>
        FoodFavorite = 15,

        /// <summary>
        /// Reaction when receiving neutral/normal food.
        /// Source: standard "Receiving food (normal)" quote category.
        /// </summary>
        FoodNormal = 16,

        /// <summary>
        /// Reaction when receiving disliked food.
        /// Source: standard "Receiving food (disliked)" quote category.
        /// </summary>
        FoodDisliked = 17,

        /// <summary>
        /// Voice played after being freed from the Crank-a-kai.
        /// Source: standard "Freed from Crank-a-kai" quote category.
        /// </summary>
        CrankAKai = 18,

        /// <summary>
        /// Voice played when received through a trade.
        /// Source: standard "Being traded" quote category.
        /// </summary>
        Trade = 19,

        /// <summary>
        /// Yo-kai voice used by the Medallium.
        ///
        /// This one is directly confirmed by game audio/modding data.
        /// The audio entry uses the Yo-kai Model ID and the VOICE player.
        ///
        /// Source:
        /// - yo-docs, "Adding Voices (Medallium)"
        /// - YW1 / YW1 Switch audio data.
        /// </summary>
        Medallium = 20,

        /// <summary>
        /// Story/event/cutscene dialogue.
        ///
        /// Confirmed by dedicated event/voice banks such as:
        /// yw_vc.acb, yw_vevent.acb and partvoice.acb.
        /// </summary>
        EventDialogue = 21,

        /// <summary>
        /// Wanted Yo-kai encounter dialogue.
        /// Source: YW3 Wanted Yo-kai quotes, e.g. Copyled "Encounter".
        /// </summary>
        WantedEncounter = 22,

        /// <summary>
        /// Wanted Yo-kai dialogue after being caught.
        /// Source: YW3 Wanted Yo-kai quotes, e.g. Copyled "Caught".
        /// </summary>
        WantedCaught = 23,

        /// <summary>
        /// Wanted Yo-kai dialogue after escaping.
        /// Source: YW3 Wanted Yo-kai quotes, e.g. Copyled "Escape".
        /// </summary>
        WantedEscape = 24,
    }

    /// <summary>
    /// Semantic sound-effect contexts associated with a Yo-kai.
    ///
    /// Original Level-5 audio data uses broader banks/categories such as
    /// BattleSE, WAZA_STRM, Footstep and EventSE rather than exposing
    /// one archive category for every semantic gameplay event below.
    ///
    /// Sources:
    /// - YW1 Switch: yw_sound.acf and associated ACB banks.
    /// - YW1 3DS: yw_sound_*.bcsar and streamed BCSTM audio.
    /// </summary>
    public enum YoKaiSoundSFXType
    {
        /// <summary>
        /// Generic SFX emitted by a normal physical attack.
        /// Mapped primarily from the original BattleSE audio family.
        /// </summary>
        Attack = 0,

        /// <summary>
        /// Impact generated when a normal attack connects.
        /// Semantic YKW5 subdivision of BattleSE.
        /// </summary>
        AttackImpact = 1,

        /// <summary>
        /// SFX generated when casting/using a Technique.
        ///
        /// Original games use battle/move sound banks rather than a
        /// confirmed dedicated "TechniqueSFX" archive category.
        /// </summary>
        Technique = 2,

        /// <summary>
        /// Impact/result of a Technique.
        /// Semantic YKW5 subdivision of BattleSE/WAZA audio.
        /// </summary>
        TechniqueImpact = 3,

        /// <summary>
        /// SFX emitted when applying an Inspirit.
        /// Semantic mapping from battle/move SFX.
        /// </summary>
        Inspirit = 4,

        /// <summary>
        /// SFX belonging to a Soultimate/Special Attack.
        ///
        /// Strongly supported by the original WAZA_STRM / special-attack
        /// audio data and yw_spl_atk.acb.
        /// </summary>
        Soultimate = 5,

        /// <summary>
        /// Impact/result produced by the Soultimate.
        /// Semantic subdivision of the original special-attack audio.
        /// </summary>
        SoultimateImpact = 6,

        /// <summary>
        /// Guard/block SFX.
        /// Stored as part of battle SFX rather than a known dedicated bank.
        /// </summary>
        Guard = 7,

        /// <summary>
        /// Generic damage impact received by the Yo-kai.
        /// Mapped from BattleSE.
        /// </summary>
        Hurt = 8,

        /// <summary>
        /// Knock-out/fainting SFX.
        /// Gameplay semantic category mapped from BattleSE.
        /// </summary>
        KO = 9,

        /// <summary>
        /// Character movement/footstep SFX.
        ///
        /// Directly supported by the "Footstep" category found in
        /// the YW1 Switch audio configuration.
        /// </summary>
        Footstep = 10,

        /// <summary>
        /// Yo-kai appearance/spawn SFX.
        /// Semantic event SFX; generally maps to BattleSE/EventSE.
        /// </summary>
        Spawn = 11,

        /// <summary>
        /// Yo-kai disappearance/despawn SFX.
        /// Semantic event SFX; generally maps to BattleSE/EventSE.
        /// </summary>
        Disappear = 12,

        /// <summary>
        /// Yo-kai Medal insertion sound.
        ///
        /// Confirmed YW1 internal cues include:
        /// G_medal_insert and G_medal_insert2.
        /// </summary>
        MedalInsert = 13,

        /// <summary>
        /// Crank-a-kai machine/result SFX.
        ///
        /// Confirmed YW1 internal cues include:
        /// gacya_demo and gacya_demo2.
        /// </summary>
        CrankAKai = 14,

        /// <summary>
        /// Evolution SFX/jingle.
        ///
        /// Confirmed YW1 internal cues include:
        /// J13_SHINKA and J16_LV_UP__SHINKA.
        /// </summary>
        Evolution = 15,

        /// <summary>
        /// Generic event-specific SFX.
        ///
        /// Directly supported by the EventSE category and event sound banks.
        /// </summary>
        Event = 16,
    }
}