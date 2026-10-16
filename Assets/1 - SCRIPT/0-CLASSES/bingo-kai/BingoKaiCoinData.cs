using System;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

namespace YKW.Core.BingoKai.Data
{
    /// <summary>One coin's four prize tiers, shared by both Bingo-kai controllers.</summary>
    [Serializable]
    [MovedFrom(true, sourceClassName: "PossibilitéDesPièce")]
    public sealed class BingoKaiCoinData
    {
        [FormerlySerializedAs("prix1")] public BingoKaiPrizeData firstPrize = new BingoKaiPrizeData();
        [FormerlySerializedAs("prix2")] public BingoKaiPrizeData secondPrize = new BingoKaiPrizeData();
        [FormerlySerializedAs("prix3")] public BingoKaiPrizeData thirdPrize = new BingoKaiPrizeData();
        [FormerlySerializedAs("prix4")] public BingoKaiPrizeData defaultPrize = new BingoKaiPrizeData();

        public static BingoKaiCoinData FromJson(string json, string coinName, string cityName,
            Func<string, int?> resolveYokaiId)
        {
            if (resolveYokaiId == null) throw new ArgumentNullException(nameof(resolveYokaiId));
            if (string.IsNullOrWhiteSpace(coinName) || string.IsNullOrWhiteSpace(cityName)) return null;
            JObject city = JObject.Parse(json)[coinName]?[cityName] as JObject;
            if (city == null) return null;
            return new BingoKaiCoinData
            {
                firstPrize = ReadPrize(city["First_prize"], resolveYokaiId),
                secondPrize = ReadPrize(city["Second_prize"], resolveYokaiId),
                thirdPrize = ReadPrize(city["Third_prize"], resolveYokaiId),
                defaultPrize = ReadPrize(city["Default_prize"], resolveYokaiId)
            };
        }

        private static BingoKaiPrizeData ReadPrize(JToken source, Func<string, int?> resolveYokaiId)
        {
            var prize = new BingoKaiPrizeData();
            if (!(source is JObject)) return prize;
            if (source?["articles"] is JArray items)
                foreach (JToken item in items)
                    if (item.Type == JTokenType.String && !string.IsNullOrWhiteSpace((string)item))
                        prize.itemNames.Add((string)item);
            if (source?["yokais"] is JArray yokais)
                foreach (JToken yokai in yokais)
                {
                    if (yokai.Type != JTokenType.String) continue;
                    int? id = resolveYokaiId((string)yokai);
                    if (id.HasValue && id.Value > 0) prize.yokaiIds.Add(id.Value);
                }
            return prize;
        }
    }
}
