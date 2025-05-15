using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Editor
{
    public class BuildIncrementer : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
    
        public void OnPreprocessBuild(BuildReport report)
        {
            PlayerSettings.bundleVersion = IncrementBuildNumber(PlayerSettings.bundleVersion);
        }
        
        private static string IncrementBuildNumber(string buildNumber)
        {
            List<int> numbers = buildNumber.Split('.').Select(int.Parse).ToList();

            numbers[^1]++;
            
            return string.Join(".", numbers);
        }
    }
}
