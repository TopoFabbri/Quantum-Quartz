using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTrigger : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private CameraManager camera;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            camera.CameraPosition = cameraPosition.position;
        }
    }
    
}