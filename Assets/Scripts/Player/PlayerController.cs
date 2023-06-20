using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb2D;
    Animator anim;
    float originalGravity;
    public Shadow shadow;

    [Header("Move and Jump")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForceFactor;

    Vector2 move;
    private bool facingRight;
    //private bool doubleJump;

    Vector2 vecGravity;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private float jumpTime;
    bool isJumping;
    float jumpCounter;

    [Header("CheckGround")]
    [SerializeField] private Transform checkGroundPoint;
    [SerializeField] private float radOfGroundCircle;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;

    [Header("WallJumping")]
    [SerializeField] private Transform checkWallTouch;
    [SerializeField] private Vector2 wallTouchSize;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private Vector2 wallJumpAngle;
    [SerializeField] private float wallJumpingduration;
    [SerializeField] private float wallSlidingSpeed = 0;
    [SerializeField] private float maxWallSlidingSpeed;
    [SerializeField] private float wallSlidingAceleration;
    [SerializeField] private float wallJumpDirection = -1f;
    private bool isWallTouch;
    bool wallJumping;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private GameObject dashEffect;
    private bool canDash = true;
    private bool isDashing;

    private void Start()
    {
        rb2D= GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalGravity = rb2D.gravityScale;
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        wallJumpAngle.Normalize();
    }

    private void FixedUpdate()
    {
        //WallJump
        if (wallJumping)
        {
            wallSlidingSpeed = 0;
            rb2D.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            wallJumping = false;
        }

        //Movement
        if (!isDashing && !isWallTouch)
        {
            rb2D.velocity = new Vector2(move.x * moveSpeed, rb2D.velocity.y);
        }

        //Slide
        if(isWallTouch && !isGrounded)
        {
            if(wallSlidingSpeed < maxWallSlidingSpeed)
            {
                wallSlidingSpeed += wallSlidingAceleration * Time.deltaTime;
            }
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }

        //Flip
        if (move.x < 0 && !facingRight) Flip();
        if (move.x > 0 && facingRight) Flip();
    }

    private void Update()
    {
        if (isGrounded) isWallTouch = false;

        if(!isWallTouch) wallSlidingSpeed = 0;

        CheckGround();
        SetAnimationState();
        JumpSystem();
    }

    public void Jump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if(isGrounded)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce);
                isJumping = true;
                //doubleJump = true;
                jumpCounter = 0;
            }
            //else if (doubleJump && !isWallTouch)
            //{
            //    rb2D.velocity = new Vector2(rb2D.velocity.x, jumpForce * doubleJumpForceFactor);
            //    doubleJump = false;
            //}
            else if (isWallTouch && !isGrounded/* && move.x == 0*/)
            {
                wallJumping = true;
            }
        }
        if(value.canceled)
        {
            isJumping = false;
            jumpCounter = 0;

            if(rb2D.velocity.y > 0)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.6f);
            }
        }
    }

    void JumpSystem()
    {
        if (rb2D.velocity.y < 0)
        {
            rb2D.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }
        if (rb2D.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) isJumping = false;

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb2D.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }
    }

    public void Movement(InputAction.CallbackContext value) 
    { 
        move = value.ReadValue<Vector2>();
    }

    public void Dash(InputAction.CallbackContext value)
    {
        if(value.started && canDash)
        {
            StartCoroutine("DashAction");
        }
    }

    private IEnumerator DashAction()
    {
        canDash = false;
        isDashing = true;
        rb2D.gravityScale = 0f;
        rb2D.velocity = new Vector2(dashSpeed * transform.localScale.x, 0f);
        dashEffect.SetActive(true);
        shadow.isShadowActive = true;

        yield return new WaitForSeconds(dashTime);

        rb2D.gravityScale = originalGravity;
        isDashing = false;
        dashEffect.SetActive(false);
        shadow.isShadowActive = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    void Flip()
    {
        if (!isWallTouch)
        {
            wallJumpDirection *= -1f;
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(checkGroundPoint.position, radOfGroundCircle, whatIsGround);
        if(!isGrounded)
            isWallTouch = Physics2D.OverlapBox(checkWallTouch.position, wallTouchSize, 90, whatIsGround);
    }

    void SetAnimationState()
    {
        anim.SetFloat("Run", Mathf.Abs(move.x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("VelocityY", rb2D.velocity.y);
        //anim.SetBool("Dashing", isDashing);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(checkGroundPoint.position, radOfGroundCircle);
        Gizmos.DrawWireCube(checkWallTouch.position, wallTouchSize);
    }
}