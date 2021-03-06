﻿using UnityEngine;

/// <summary>
/// Destruye el fruto al contacto. Usado para prevenir frutos caidos. Actualmente en desuso.
/// </summary>
public class FruitDestroyer : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ItemA") || other.CompareTag("ItemB"))
        {
            Debug.LogWarning("FruitDestroyer: This shouldn't have happened. WTF?");
            Destroy(other.gameObject);
        }
    }
}
