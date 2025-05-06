using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator _animator;

    [Header("Movement")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool _canDoubleJump;

    [Header("Buffer & Coyote Jump")] 
    [SerializeField] private float bufferJumpWindow = .25f;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float _bufferJumpActivated = -1;
    private float _coyoteJumpActivated = -1;
    
    [Header("Wall interactions")] 
    [SerializeField] private float wallJumpDuration = 0.6f;
    [SerializeField] private Vector2 wallJumpVelocity;
    private bool _isWallJumping;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockBackVelocity;
    private bool _isKnocked;
    //private bool _canBeKnocked;
    
    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool _isGrounded;
    private bool _isAirborne;
    private bool _isWallDetected;


    private float _xInput;
    private float _yInput;
    
    private bool _facingRight = true;
    private int _facingDir = 1;

    [Header("VFX")] 
    [SerializeField] private GameObject _deathVFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        
    }
    // Update is called once per frame
    void Update()
    {
        UpdateAirborneStatus();

        if (_isKnocked)
            return;
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }
    public void Knockback()
    {
        if (_isKnocked) return;
        
        StartCoroutine(KnockbackRoutine());
        rb.linearVelocity = new Vector2(knockBackVelocity.x * -_facingDir, knockBackVelocity.y);
        _animator.SetTrigger("knockBack");
    }
    private IEnumerator KnockbackRoutine()
    {
        //_canBeKnocked = false;
        _isKnocked = true;
        yield return new WaitForSeconds(knockbackDuration);
        //_canBeKnocked = true;
        _isKnocked = false;
    }

    public void Die()
    {
        GameObject deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void HandleWallSlide()
    {
        bool canWallSlide = _isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = _yInput < 0 ? 1 : 0.05f;
        if (!canWallSlide) 
            return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }
    private void UpdateAirborneStatus()
    {
        if (_isGrounded && _isAirborne)
            HandleLanding();
        if (!_isGrounded && !_isAirborne)
            BecomeAirborne();
    }
    private void BecomeAirborne()
    {
        _isAirborne = true;
        if (rb.linearVelocity.y < 0)
        {
            ActivateCoyoteJump();
        }
    }
    private void HandleLanding()
    {
        _isAirborne = false;
        _canDoubleJump = true;
        
        AttemptBufferJump();
    }
    
    private void HandleInput()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();
        }
    }
    
   #region Buffer and Coyote Jump
    private void RequestBufferJump()
    {
        if (_isAirborne)
            _bufferJumpActivated = Time.time;
    }
    private void AttemptBufferJump()
    {
        if (Time.time < _bufferJumpActivated + bufferJumpWindow)
        {
            _bufferJumpActivated = Time.time-1;
            Jump();
        }
    }
    private void ActivateCoyoteJump() => _coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => _coyoteJumpActivated = Time.time-1;
    #endregion
    
    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < _coyoteJumpActivated + coyoteJumpWindow;
        if (_isGrounded || coyoteJumpAvailable)
            Jump();
        else if (_isWallDetected && !_isGrounded)
            WallJump();
        else if (_canDoubleJump)
            DoubleJump();
        CancelCoyoteJump();
    }
    private void Jump() => rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    private void DoubleJump()
    {
        _isWallJumping = false;
        _canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }
    private void WallJump()
    {
        _canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpVelocity.x * -_facingDir, wallJumpVelocity.y);
        Flip();
        
        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }
    private IEnumerator WallJumpRoutine()
    {
        _isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        _isWallJumping = false;
    }
    
    private void HandleCollisions()
    {
        _isGrounded = Physics2D.Raycast(transform.position,
            Vector2.down, groundCheckDistance, whatIsGround);
        _isWallDetected = Physics2D.Raycast( transform.position,
            Vector2.right * _facingDir, wallCheckDistance, whatIsGround);
    }
    private void HandleAnimations()
    {
        _animator.SetFloat("xVelocity",rb.linearVelocity.x);
        _animator.SetFloat("yVelocity",rb.linearVelocity.y);
        _animator.SetBool("isGrounded",_isGrounded);
        _animator.SetBool("isWallDetected",_isWallDetected);
    }
    private void HandleMovement()
    {
        if (_isWallDetected) return;
        if (_isWallJumping) return;
        rb.linearVelocity = new Vector2(_xInput * moveSpeed, rb.linearVelocity.y);
    }
    
    private void HandleFlip()
    {
        if (_xInput < 0 && _facingRight ||
            _xInput > 0 && !_facingRight)
            Flip();
    }
    private void Flip()
    {
        _facingDir *= -1;
        transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, 
            new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * _facingDir), transform.position.y));
    }
}
