using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class AstronautAI : MonoBehaviour
{

    private enum AstronautState { Patrolling, Chasing, Stunned, Scared, Attacking }

    AstronautState state;

    private bool stateComplete;
    [SerializeField] GameObject player;
    [SerializeField] GameObject rayOrigin;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;

    private float chaseTime;
    private float moveSpeed;
    private bool chaseTimerOn = false;
    private bool stunned;
    private bool chasing = false;
    private float velocityXSmooth = 0f;

    private bool isFacingRight = false;
    [SerializeField] bool playerInView = false;

    void Start()
    {
        state = AstronautState.Patrolling;

    }

    void Update()
    {
        SpawnRaycast();
        UpdateState();

        if (playerInView && state == AstronautState.Patrolling || stunned)
        {
            StartCoroutine(StandStill());
        }
        if (chasing && !playerInView && !chaseTimerOn)
        {
            chaseTime = Time.time + 3f;
            chaseTimerOn = true;
        }

        if (Time.time > chaseTime)
        {
            if (!playerInView)
            {
                chasing = false;
            }
            chaseTimerOn = false;
        }

        if (stateComplete)
        {
            SelectState();
        }

        if (!stunned && !chasing)
        {
            FlipCheck();
        }

        //Debug.Log(TouchingPlayer());
        Debug.Log(state);
        //Debug.Log(isFacingRight);
    }


    private void FixedUpdate()
    {

        if (state == AstronautState.Patrolling || state == AstronautState.Chasing)
        {
            int direction = isFacingRight ? 1 : -1;
            float newVelocityX;
            if (state == AstronautState.Chasing)
            {
                
                float smoothTime = 0.2f;
                float targetVelocityX = direction * moveSpeed;
                newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXSmooth, smoothTime);

                Vector2 playerDirection = (new Vector2(player.transform.position.x, 0) - new Vector2(transform.position.x, 0)).normalized;
                if (playerDirection.x > 0 && !isFacingRight || playerDirection.x < 0 && isFacingRight)
                {
                    Flip();
                }
            }
            else
            {
                newVelocityX = moveSpeed * direction;
            }
            
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case AstronautState.Stunned:
                UpdateStun();
                break;
            case AstronautState.Scared:
                UpdateScared();
                break;
            case AstronautState.Attacking:
                UpdateAttack();
                break;
            case AstronautState.Chasing:
                UpdateChase();
                break;
            case AstronautState.Patrolling:
                UpdatePatrol();
                break;

        }
        
    }
    void SelectState()
    {
        stateComplete = false;
        if (stunned)
        {
            state = AstronautState.Stunned;
            StartStun();
        }
        else
        {
            if (chasing)
            {
                state = AstronautState.Chasing;
                StartChase();

                if (TouchingPlayer())
                {
                    state = AstronautState.Attacking;
                    StartAttack();
                }
            }
            else if (playerInView && state == AstronautState.Patrolling && !chasing)
            {
                state = AstronautState.Scared;
                StartScared();
            }
            else
            {
                if (!chasing)
                {
                    state = AstronautState.Patrolling;
                    StartPatrol();
                }
                
            }
        }
    }

    void UpdateStun()
    {
        rb.linearVelocity = Vector2.zero;
        StandStill();
    }
    void UpdateScared()
    {
        rb.linearVelocity = Vector2.zero;
        StandStill();
    }

    IEnumerator StandStill()
    {
        yield return new WaitForSeconds(1f);
        if (playerInView)
        {
            chasing = true;
        }
        stunned = false;
        stateComplete = true;
        
    }
    void UpdateAttack()
    {
        ChargeAttack();
    }
    IEnumerator ChargeAttack()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(.2f);
        if (TouchingPlayer())
        {
            Debug.LogWarning("playerDied");
            
        }
        else
        {
            Debug.LogWarning("IMissedFuckYou");
        }
        chasing = true;
        stateComplete = true;
    }
    void UpdateChase()
    {
        moveSpeed = 6f;

        if (!chasing || TouchingPlayer())
        {
            stateComplete = true;
        }
    }
    void UpdatePatrol()
    {
        moveSpeed = 3f;
        WallCheck();
        if (playerInView || stunned)
        {
            stateComplete = true;
        }
        
    }

    void StartStun()
    {
        //animation;
    }
    void StartScared()
    {
        //animation;
    }
    void StartAttack()
    {
        //animation;
    }
    void StartChase()
    {
        //animation;
    }
    void StartPatrol()
    {
        //animation;
    }


    private void FlipCheck()
    {
        if (isFacingRight && rb.linearVelocity.x < 0f || !isFacingRight && rb.linearVelocity.x > 0f)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void WallCheck()
    {
        if (TouchingWall())
        {
            Flip();
        }
    }

    private bool TouchingWall()
    {
        return Physics2D.OverlapCircle(rayOrigin.transform.position, 1f, groundLayer);

    }
    private bool TouchingPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(rayOrigin.transform.position, 1f);
        return collider != null && collider.gameObject.name == "Player";
    }
    private void SpawnRaycast()
    {
        Vector2 direction = (player.transform.position - rayOrigin.transform.position).normalized;
        float distance = Vector2.Distance(rayOrigin.transform.position, player.transform.position);
        float angle = Vector2.SignedAngle(transform.right, direction);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.transform.position, direction, distance);

        Debug.DrawRay(rayOrigin.transform.position, direction * distance, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.name == "Player")
            {
                if (!chasing)
                {
                    if (isFacingRight)
                    {
                        if (angle > 40)
                        {
                            playerInView = true;
                        }
                        else
                        {
                            playerInView = false;
                        }
                    }
                    if (!isFacingRight)
                    {
                        if (angle < 140)
                        {
                            playerInView = true;
                        }
                        else
                        {
                            playerInView = false;
                        }
                    }
                }
                else
                {
                    playerInView = true;
                }
            }
        }
    }


    
    
        

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Banana"))
        {
            StartCoroutine(StandStill());
            stunned = true;
        }
    }
}
