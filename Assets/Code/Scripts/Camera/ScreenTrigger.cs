using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTrigger : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private float speed;

    private Transform mainCamera;
    private bool isMoving = false;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
        Vector3 cameraPositionPosition = cameraPosition.position;
        cameraPositionPosition.z = -10;
        cameraPosition.position = cameraPositionPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isMoving = true;
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            mainCamera.position =
                Vector2.MoveTowards(mainCamera.position, cameraPosition.position, Time.deltaTime * speed);

            if (mainCamera.position == cameraPosition.position)
                isMoving = false;
        }
    }
}