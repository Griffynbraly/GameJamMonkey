using UnityEngine;
using System.Collections;

public class AstronautAI : MonoBehaviour
{
    private enum AstronautState { Patrolling, Chasing, Stunned, Scared, Attacking }
    private AstronautState state;

    private bool stateComplete;
    public bool stunned;
    private bool chasing;
    private bool isFacingRight;
    private bool playerInView;
    private bool isShooting;

    private float moveSpeed;
    private float velocityXSmooth = 0f;
    private float timeToHide = 999999 * 999999f;

    private GameObject player;
    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private GameObject bulletOrigin;
    [SerializeField] private GameObject tazeBullet;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        state = AstronautState.Patrolling;

        if (transform.localScale.x == -1)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }
    }

    void Update()
    {
        SpawnRaycast();
        UpdateState();

        if ((playerInView && state == AstronautState.Patrolling) || stunned)
            StartCoroutine(StandStill());

        if (stateComplete)
            SelectState();

        if (!stunned && !chasing)
            FlipCheck();
        if (playerInView)
            timeToHide = Time.time + 3f;
        if (stunned)
        {
            stateComplete = true;
        }
    }

    void FixedUpdate()
    {
        if (state == AstronautState.Patrolling || state == AstronautState.Chasing)
        {
            int direction = isFacingRight ? 1 : -1;
            float targetVelocityX = direction * moveSpeed;
            float smoothTime = state == AstronautState.Chasing ? 0.1f : 0f;

            float newVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXSmooth, smoothTime);
            rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);

            if (state == AstronautState.Chasing)
                AdjustFacingDirection();
        }
    }

    void UpdateState()
    {
        switch (state)
        {
            case AstronautState.Stunned: UpdateStun(); break;
            case AstronautState.Scared: UpdateScared(); break;
            case AstronautState.Attacking: UpdateAttack(); break;
            case AstronautState.Chasing: UpdateChase(); break;
            case AstronautState.Patrolling: UpdatePatrol(); break;
        }
    }

    void SelectState()
    {
        stateComplete = false;

        if (stunned)
        {
            state = AstronautState.Stunned;
            StartStun();
            return;
        }

        if (TouchingPlayer() && playerInView)
        {
            state = AstronautState.Attacking;
            StartAttack();
        }
        else if (chasing)
        {
            state = AstronautState.Chasing;
            StartChase();
        }
        else if (playerInView && state == AstronautState.Patrolling)
        {
            state = AstronautState.Scared;
            StartScared();
        }
        else
        {
            state = AstronautState.Patrolling;
            StartPatrol();
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
    void UpdateStun()
    {
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(StandStill());
    }

    void UpdateScared()
    {
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(StandStill());
    }

    IEnumerator StandStill()
    {
        yield return new WaitForSeconds(1f);
        if (TouchingPlayer())
        {
            chasing = false;
            stateComplete = true;
        }
        else if (playerInView) 
        {
            chasing = true;
        }
        stunned = false;
        stateComplete = true;
    }

    void UpdateAttack()
    {
        if (!isShooting)
        StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack()
    {
        isShooting = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        Instantiate(tazeBullet, bulletOrigin.transform.position, Quaternion.LookRotation(Vector3.forward, player.transform.position - bulletOrigin.transform.position));

        if (!TouchingPlayer()) chasing = true;
        isShooting = false;
        stateComplete = true;
    }

    void UpdateChase()
    {
        moveSpeed = 6f;
        if (!chasing || TouchingPlayer() || Time.time > timeToHide)
            chasing = false;
            stateComplete = true;
    }
    void UpdatePatrol()
    {
        WallCheck();
        moveSpeed = 4f;
        if (playerInView)
            stateComplete = true;
    }

    void FlipCheck()
    {
        if ((isFacingRight && rb.linearVelocity.x < 0) || (!isFacingRight && rb.linearVelocity.x > 0))
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    void WallCheck()
    {
        if (TouchingWall())
            Flip();
    }

    private bool TouchingWall()
    {
        return Physics2D.OverlapCircle(rayOrigin.transform.position, 0.55f, groundLayer);
    }

    private bool TouchingPlayer()
    {
        Collider2D collider = Physics2D.OverlapCircle(rayOrigin.transform.position, 11f);
        return collider != null && collider.gameObject.CompareTag("Player");
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
                    playerInView = isFacingRight ? angle < 140 : angle < 30;
                }
                else
                {
                    playerInView = true;
                }
            }
            else
            {
                if (hit.collider.gameObject.layer != 6)
                playerInView = false;
            }
        }
    }

    private void AdjustFacingDirection()
    {
        Vector2 playerDirection = (new Vector2(player.transform.position.x, 0) - new Vector2(transform.position.x, 0)).normalized;
        if ((playerDirection.x > 0 && !isFacingRight) || (playerDirection.x < 0 && isFacingRight))
            Flip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Banana"))
        {
            StartCoroutine(StandStill());
            stunned = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider != null)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                PlayerMove playerMove = GetComponent<Collider2D>().GetComponent<PlayerMove>();
                if (playerMove != null)
                {
                    if (state == AstronautState.Chasing || state == AstronautState.Attacking)
                    {
                        //playerMove.Killed();
                    }
                }
            }
            if (collider.gameObject.CompareTag("Guard"))
            {
                Flip();
            }
        }
    }
}
