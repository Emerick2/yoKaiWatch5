using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.Scripting.APIUpdating;

namespace YKW.Core.BingoKai.Data
{
    /// <summary>Shared rewards for a prize tier, using species IDs and item names.</summary>
    [Serializable]
    [MovedFrom(true, sourceClassName: "UnDesPrix")]
    public sealed class BingoKaiPrizeData
    {
        [FormerlySerializedAs("objetPrix")] public List<string> itemNames = new List<string>();
        [FormerlySerializedAs("yoKaiPrix")] public List<int> yokaiIds = new List<int>();

        public int Count => itemNames.Count + yokaiIds.Count;
    }
}
