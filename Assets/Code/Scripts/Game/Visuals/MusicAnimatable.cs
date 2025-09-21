using Code.Scripts.Game.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Game.Visuals
{
    [RequireComponent(typeof(Animator))]
    public class MusicAnimatable : MonoBehaviour
    {
        private Animator targetAnimator;
        
        private readonly List<string> paramNames = new();

        private void OnMusicEvent(string cueName)
        {
            if (paramNames.Contains(cueName))
            {
                targetAnimator.SetTrigger(cueName);
            }
        }

        private void OnEnable()
        {
            SfxController.MusicCue += OnMusicEvent;
        }

        private void OnDisable()
        {
            SfxController.MusicCue -= OnMusicEvent;
        }

        private void Start()
        {
            targetAnimator = GetComponent<Animator>();
            
            foreach (AnimatorControllerParameter parameter in targetAnimator.parameters)
            {
                paramNames.Add(parameter.name);
            }
        }
    }
}