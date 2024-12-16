using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Initial Setup")]
    [SerializeField] protected float speed;
    [SerializeField] protected float waitTime = 2f;
    [Header("Collision Info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [Space]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isWallDetected;
    private bool isUp;
    private bool isWaiting;
    private float waitTimer;


    void Start()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    void Update()
    {
        CollisionTest();
        UpdateWaiting();
        Move();
        waitTimer -= Time.deltaTime;
    }

    private void CollisionTest()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }

    private void Move()
    {
        if (!isWaiting)
            Movement();
        else
            rb.velocity = Vector2.zero;
    }

    private void Movement()
    {
        if (isUp)
        {
            if (!isWallDetected)
            {
                waitTimer = waitTime;
                isWaiting = true;
                isUp = false;
                return;
            }
            rb.velocity = new Vector2(rb.velocity.x, speed);
        }
        else
        {
            if (isGrounded)
            {
                waitTimer = waitTime;
                isWaiting = true;
                isUp = true;
                return;
            }
            rb.velocity = new Vector2(rb.velocity.x, -speed);
        }
    }

    private void UpdateWaiting()
    {
        if (waitTimer < 0)
        {
            isWaiting = false;
        }
    }
}
