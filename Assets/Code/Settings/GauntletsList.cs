using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GauntletList", menuName = "Game/GauntletList")]
public class GauntletsList : ScriptableObject
{
    [Serializable]
    public class GauntletData
    {
        public string gauntletName;
        public string sceneName;
        public int costInKeys;
        public bool isUnlocked; 
    }

    public List<GauntletData> gauntlets = new List<GauntletData>();
}