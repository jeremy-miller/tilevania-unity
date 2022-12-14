using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] private Transform gun;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    private Animator _animator;
    private BoxCollider2D _feetCollider;
    private CapsuleCollider2D _bodyCollider;
    private float _initialGravityScale;
    private bool _isAlive = true;
    private Vector2 _moveInput;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bodyCollider = GetComponent<CapsuleCollider2D>();
        _feetCollider = GetComponent<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _initialGravityScale = _rigidbody2D.gravityScale;
    }

    private void Update()
    {
        if (!_isAlive) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void Run()
    {
        Vector2 playerVelocity = new Vector2(_moveInput.x * runSpeed, _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = playerVelocity;
        
        bool playerHasHorizontalSpeed = Mathf.Abs(_rigidbody2D.velocity.x) > Mathf.Epsilon;  // use Mathf.Epsilon instead of 0 to avoid issues with comparing floats
        _animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(_rigidbody2D.velocity.x) > Mathf.Epsilon;  // use Mathf.Epsilon instead of 0 to avoid issues with comparing floats
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(_rigidbody2D.velocity.x), 1f);
        }
    }

    private void ClimbLadder()
    {
        if (!_feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            _rigidbody2D.gravityScale = _initialGravityScale;  // reset gravity when not climbing
            _animator.SetBool("isClimbing", false);
            return;
        }
        
        Vector2 climbVelocity = new Vector2(_rigidbody2D.velocity.x, _moveInput.y * climbSpeed);
        _rigidbody2D.velocity = climbVelocity;
        _rigidbody2D.gravityScale = 0f;  // so player doesn't float down when climbing
        
        bool playerHasVerticalSpeed = Mathf.Abs(_rigidbody2D.velocity.y) > Mathf.Epsilon;  // use Mathf.Epsilon instead of 0 to avoid issues with comparing floats
        _animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    private void Die()
    {
        if (_bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            _isAlive = false;
            _animator.SetTrigger("Dying");
            _rigidbody2D.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
    
    private void OnMove(InputValue value)
    {
        if (!_isAlive) return;
        _moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        if (!_isAlive) return;
        
        if (!_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;  // don't allow double-jumping
        
        if (value.isPressed)
        {
            _rigidbody2D.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void OnFire(InputValue value)
    {
        if (!_isAlive) return;
        Instantiate(bullet, gun.position, transform.rotation);
    }
}
