using System.Collections.Generic;

[System.Serializable]
public class BingoKaiBigData{
    public BingoKaiData Red_Coin;
    public BingoKaiData Yellow_Coin;
    public BingoKaiData Orange_Coin;
    public BingoKaiData Pink_Coin;
    public BingoKaiData Green_Coin;
    public BingoKaiData Blue_Coin;
    public BingoKaiData Purple_Coin;
    public BingoKaiData One_Star_Coin;
    public BingoKaiData Five_Star_Coin;
    public BingoKaiData Special_Coin;
    public BingoKaiData Yo_Mystery_Coin;
    public BingoKaiData Kai_Mystery_Coin;
    public BingoKaiData Soul_Mystery_Coin;
    public BingoKaiData Sushi_Coin;
    public BingoKaiData Tempura_Coin;
    public BingoKaiData Whirlwind_Coin;
    public BingoKaiData Wanderer_Coin;
    public BingoKaiData Rusted_Coin;
    public BingoKaiData Flower_Excite_Coin;
    public BingoKaiData Bird_Excite_Coin;
    public BingoKaiData Wind_Excite_Coin;
    public BingoKaiData Moon_Excite_Coin;
    public BingoKaiData Mellow_Coin;
    public BingoKaiData Brave_Dream_Coin;
    public BingoKaiData Light_Dream_Coin;
    public BingoKaiData Platin_Dream_Coin;
    public BingoKaiData Fortun_Dream_Coin;
    public BingoKaiData Princess_Dream_Coin;
    public BingoKaiData Wisdom_Dream_Coin;
    public BingoKaiData Chivalry_Dream_Coin;
    public BingoKaiData _2nd_Helping_Coin;
    public BingoKaiData Belly_Buster_Coin;
    public BingoKaiData L_Souvenir_Coin;
    public BingoKaiData F_Souvenir_Coin;
    public BingoKaiData S_Souvenir_Coin;
    public BingoKaiData K_Souvenir_Coin;
    public BingoKaiData Joyful_Coin;
    public BingoKaiData Excellent_Coin;
    public BingoKaiData Beaming_Coin;
    public BingoKaiData Iron_Pot_Coin;
    public BingoKaiData Fancy_That_Coin;
    public BingoKaiData Black_Coin;
    public BingoKaiData Demonic_Coin;
    public BingoKaiData Underworld_Coin;
    public BingoKaiData Sealed_Coin;
    public BingoKaiData Emblem_Coin;
    public BingoKaiData Treasure_Coin;
    public BingoKaiData Legend_Coin;
    public BingoKaiData Super_Red_Coin;
    public BingoKaiData Super_Yellow_Coin;
    public BingoKaiData Super_Orange_Coin;
    public BingoKaiData Super_Pink_Coin;
    public BingoKaiData Super_Green_Coin;
    public BingoKaiData Super_Blue_Coin;
    public BingoKaiData Super_Purple_Coin;
    public BingoKaiData Super_Light_Blue_Coin;
    public BingoKaiData Ultra_Red_Coin;
    public BingoKaiData Ultra_Yellow_Coin;
    public BingoKaiData Ultra_Orange_Coin;
    public BingoKaiData Ultra_Pink_Coin;
    public BingoKaiData Ultra_Green_Coin;
    public BingoKaiData Ultra_Blue_Coin;
    public BingoKaiData Ultra_Purple_Coin;
    public BingoKaiData Ultra_Light_Blue_Coin;
    public BingoKaiData _3DS_Coin;
}

[System.Serializable]
public class BingoKaiData{
    public BingoKaiCityData Granval;
    public BingoKaiCityData USA;
}


[System.Serializable]
public class BingoKaiCityData{
    public BingoKaiPriceData First_prize;
    public BingoKaiPriceData Second_prize;
    public BingoKaiPriceData Third_prize;
    public BingoKaiPriceData Default_prize;
}


[System.Serializable]
public class BingoKaiPriceData{
    public List<string> yokais;
    public List<string> articles;
}


[System.Serializable]
public class BingoKaiPriceType{
    
}