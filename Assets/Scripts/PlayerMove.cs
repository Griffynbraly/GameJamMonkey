using UnityEngine;
using System;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float speed = 11f;
    private float climbSpeed = 10f;
    private float jumpForce = 18f;
    private bool isFacingRight;
    private float lastHorizontal;
    private bool touchingLadder;
    private bool climbingLadder;
    private bool canWalkOff;
    private bool dead = false;
    private Vector3 currentLadderLocation;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    private Transform groundCheck;

    public static event Action OnPlayerDamaged;

    private enum PlayerState { Idle, Running, Airborne, Climbing }

    PlayerState state;
    private Animator animator;
    private void Start()
    {
        groundCheck = GameObject.FindGameObjectWithTag("GroundCheck").transform;

        animator = GetComponentInChildren<Animator>();
    }
    private bool stateComplete;
    void Update()
    {
        if (!dead)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        if (horizontal != lastHorizontal)
        {
            Flip();
        }

        HandleJump();

        if (stateComplete)
        {
            SelectState();
        }

        HandleClimb();

        lastHorizontal = horizontal;

        UpdateState();

        if (!climbingLadder)
        {
            rb.gravityScale = 4;
        }

        if (IsOnGuard())
        {
            Jump(jumpForce / 2);
        }

    }

    void UpdateState()
    {
        switch (state)
        {
            case PlayerState.Climbing:
                UpdateClimb();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Running:
                UpdateRun();
                break;
            case PlayerState.Airborne:
                UpdateAir();
                break;

        }
    }

    void SelectState()
    {
        stateComplete = false;
        if (dead)
        {
            StartDeath();
        }
        else
        {
            if (climbingLadder)
            {
                state = PlayerState.Climbing;
                StartClimb();
            }
            else
            {
                if (IsGrounded())
                {
                    if (rb.linearVelocity.x == 0)
                    {
                        state = PlayerState.Idle;
                        StartIdle();
                    }
                    else
                    {
                        state = PlayerState.Running;
                        StartRun();
                    }
                }
                else
                {
                    state = PlayerState.Airborne;
                    StartAir();
                }
            }
        }        
    }
    void UpdateClimb()
    {
        if (climbingLadder && !touchingLadder || !climbingLadder)
        {
            rb.gravityScale = 4f;
            stateComplete = true;
        }
    }
    void UpdateIdle()
    {
        if (rb.linearVelocity.x != 0 || !IsGrounded() || climbingLadder)
        {
            stateComplete = true;
        }
    }
    void UpdateRun()
    {
        if (rb.linearVelocity.x == 0 || !IsGrounded() || climbingLadder)
        {
            stateComplete = true;
        }
    }
    void UpdateAir()
    {
        if (IsGrounded() || climbingLadder)
        {
            stateComplete = true;
        }
    }


    void StartIdle()
    {
        animator.Play("Idle");
    }
    void StartRun()
    {
        animator.Play("Runing");
    }
    void StartAir()
    {
        animator.Play("In Air");
    }
    void StartClimb()
    {
        animator.Play("Climbing");
    }
    void StartDeath()
    {
        animator.Play("ELectricuted");
        
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, .4f, groundLayer);

    }

    private bool IsOnGuard()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.transform.position, .4f);
        if (collider != null)
        {
            return collider.CompareTag("Guard");
        }
        else return false;

    }

    private void FixedUpdate()
    {
        if (climbingLadder)
        {
            if (vertical != 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, vertical * climbSpeed);
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }

        }
        else
        {
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        }


    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() || Input.GetKeyDown(KeyCode.Space) && climbingLadder && horizontal != 0)
        {
            Jump(jumpForce);
            climbingLadder = false;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SlowFall();
        }
    }
    private void HandleClimb()
    {
        if (!climbingLadder)
        {
            if (vertical > 0 && touchingLadder)
            {
                canWalkOff = false;
                climbingLadder = true;
                transform.position = new Vector2(currentLadderLocation.x, transform.position.y);
                rb.linearVelocity = new Vector2(0f, 0f);

            }
        }
        if (climbingLadder)
        {

            if (!IsGrounded() && climbingLadder)
            {
                canWalkOff = true;
            }
            rb.gravityScale = 0;
            if (!touchingLadder || canWalkOff && IsGrounded())
            {
                climbingLadder = false;
            }
        }

    }


    private void Jump(float jumpStrength)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
    }
    private void SlowFall()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.6f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            touchingLadder = true;
            currentLadderLocation = other.transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (touchingLadder)
        {
            touchingLadder = false;
        }

    }

    public void Killed()
    {
        if (!dead)
        {
            OnPlayerDamaged?.Invoke();
            StartCoroutine(DeathDelay());
        }
        
        dead = true;
        stateComplete = true;
        Debug.Log("AHHH OH MY GOD YOU FUKCING TAZED ME");
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}
