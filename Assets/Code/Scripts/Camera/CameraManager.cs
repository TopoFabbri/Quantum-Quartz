using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    public Vector3 CameraPosition { get; set; }

    private void Awake()
    {
        CameraPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, CameraPosition, Time.deltaTime * speed);
    }
}