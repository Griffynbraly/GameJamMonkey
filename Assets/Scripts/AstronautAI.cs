using UnityEngine;
using System.Collections;

public class AstronautAI : MonoBehaviour
{
    private enum AstronautState { Patrolling, Chasing, Stunned, Scared, Attacking }
    private AstronautState state;

    private bool stateComplete;
    private bool stunned;
    private bool chasing;
    private bool isFacingRight;
    private bool playerInView;

    private float moveSpeed;
    private float velocityXSmooth = 0f;
    private float timeToHide = 999999 * 999999f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        state = AstronautState.Patrolling;
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
    }

    void FixedUpdate()
    {
        if (state == AstronautState.Patrolling || state == AstronautState.Chasing)
        {
            int direction = isFacingRight ? 1 : -1;
            float targetVelocityX = direction * moveSpeed;
            float smoothTime = state == AstronautState.Chasing ? 0.2f : 0f;

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
            return;
        }

        if (TouchingPlayer())
        {
            state = AstronautState.Attacking;
        }
        else if (chasing)
        {
            state = AstronautState.Chasing;
        }
        else if (playerInView)
        {
            if (state == AstronautState.Patrolling)
                state = AstronautState.Scared;
        }
        else
        {
            state = AstronautState.Patrolling;
        }
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
        if (playerInView) chasing = true;
        stunned = false;
        stateComplete = true;
    }

    void UpdateAttack()
    {
        StartCoroutine(ChargeAttack());
    }

    IEnumerator ChargeAttack()
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);

        if (TouchingPlayer())
        {
            // Player hit logic here
        }

        chasing = true;
        stateComplete = true;
    }

    void UpdateChase()
    {
        moveSpeed = 8f;
        if (!chasing || TouchingPlayer() || Time.time > timeToHide)
            chasing = false;
            stateComplete = true;
    }
    void UpdatePatrol()
    {
        WallCheck();
        moveSpeed = 4f;
        if (playerInView || stunned)
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
                    playerInView = isFacingRight ? angle > 120 : angle < 60;
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
}
