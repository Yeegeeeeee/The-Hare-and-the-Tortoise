using UnityEngine;

public class Entity : MonoBehaviour
{

    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Collision Info")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [Space]
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform attackCheck;
    [SerializeField] protected float attackCheckDistance;

    protected float damage;

    protected bool isGrounded;
    protected bool isWallDetected;

    protected int facingDir = 1;
    protected bool facingRight = true;

    protected float currentHp;

    protected bool isDead = false;
    protected int id;
    protected bool isHurt;

    public System.Action onFilpped;


    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        CollisionTest();
    }

    protected virtual void CollisionTest()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(attackCheck.position, new Vector3(attackCheck.position.x + attackCheckDistance * facingDir, attackCheck.position.y));
    }

    public float getCurrentHp()
    {
        return currentHp;
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if (onFilpped != null)
            onFilpped();
    }

    public virtual void TakeDamage(float damage)
    {
        if (!isDead)
        {
            currentHp -= damage;
            if (currentHp <= 0)
            {
                isDead = true;
            }
        }
    }

    public bool getFacingRight()
    {
        return facingRight;
    }

    public float getFacingDir()
    {
        return facingDir;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void DisableAll()
    {
        anim.SetBool("isMoving", false);
        anim.SetBool("isAttacking", false);
    }
}
