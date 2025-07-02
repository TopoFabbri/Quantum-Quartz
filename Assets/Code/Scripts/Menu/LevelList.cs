using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "Custom/Level List")]
public class LevelList : ScriptableObject
{
    [System.Serializable]
    public class LevelData
    {
        public string levelName; 
        [SerializeField] private SceneReference sceneReference;

        public string SceneName => sceneReference.Name;
    }

    public List<LevelData> levels = new List<LevelData>();
}