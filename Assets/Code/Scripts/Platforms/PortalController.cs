using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public static event Action OnPortalEnter;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           OnPortalEnter?.Invoke();
           Debug.Log("Portal enter");
        }
    }
}
