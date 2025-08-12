using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonsAnimation : MonoBehaviour
{
    private Animator animator;
        private GameObject thisButton;
    
        void Awake()
        {
            animator = GetComponent<Animator>();
            thisButton = gameObject;
        }
    
        void Update()
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
    
            if (current == thisButton)
            {
                animator.SetBool("Selected", true);
            }
            else
            {
                animator.SetBool("Selected", false);
            }
        }
}
