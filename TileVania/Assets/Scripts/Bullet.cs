using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    
    private PlayerMovement _player;
    private Rigidbody2D _rigidbody2D;
    private float _xSpeed;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerMovement>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _xSpeed = _player.transform.localScale.x * bulletSpeed;
    }
    
    private void Update()
    {
        _rigidbody2D.velocity = new Vector2(_xSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
