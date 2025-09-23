using Eflatun.SceneReference;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Menu
{
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
}
