using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

[CreateAssetMenu(fileName = "GauntletList", menuName = "Assets/Code/Settings/GauntletList")]
public class GauntletsList : ScriptableObject
{
    [Serializable]
    public class GauntletData
    {
        public string gauntletName;
        public SceneReference sceneReference;
        public int costInKeys;
        public bool isUnlocked; 
    }

    public List<GauntletData> gauntlets = new List<GauntletData>();
}