using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] private AudioClip coinPickupSfx;
    [SerializeField] private int pointsForCoinPickup = 100;

    private bool _wasCollected = false;  // used to avoid double coin pickups

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_wasCollected)
        {
            _wasCollected = true;
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
            
            // Use PlayClipAtPoint to continue playing clip after gameObject is deleted below.
            // Play audio at the camera position, since this is a 2D game (don't need to play to the right of the player, etc)
            AudioSource.PlayClipAtPoint(coinPickupSfx, Camera.main.transform.position);
            
            Destroy(gameObject);
        }
    }
}
