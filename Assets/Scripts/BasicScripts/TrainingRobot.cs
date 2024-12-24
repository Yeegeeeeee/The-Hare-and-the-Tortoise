using UnityEngine;

public class TrainingRobot : Entity
{
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [Header("Player Detection Info")]
    [SerializeField] private LayerMask whatIsPlayer;
    [Header("Enemy Info")]
    [SerializeField] private float MaxHP = 200;
    [SerializeField] private float BaseDamage = 10;
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected float offset;

    private RaycastHit2D isPlayerDetected;

    private bool isAttacking = false;

    private bool isMoving;

    private GameObject item;

    private SpriteRenderer sp_item;

    protected override void Start()
    {
        base.Start();
        facingDir = -1;
        facingRight = false;
        currentHp = MaxHP;
        damage = BaseDamage;
        id = 1;
    }

    protected override void Update()
    {
        base.Update();
        if (isDead)
        {
            anim.SetBool("isAttacking", false);
            Dead();
        }
        else
        {
            if (isPlayerDetected)
            {
                if (isPlayerDetected.distance > 1)
                {
                    rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y);
                }
                else
                {
                    isAttacking = true;
                }
            }
            else
            {
                rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
            }

            if (!isGrounded || isWallDetected)
            {
                Flip();
            }

            isMoving = rb.velocity.x != 0;

            AnimationController();
        }
    }

    public void SetGameObject(GameObject gameObject)
    {
        item = gameObject;
        sp_item = item.GetComponentInChildren<SpriteRenderer>();
    }

    public void PerformAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(attackCheck.position, Vector2.right, attackCheckDistance * facingDir, whatIsPlayer);
        if (hit)
        {
            Entity entity = hit.collider.gameObject.GetComponent<Entity>();
            //entity.TakeDamage(damage);
        }
    }

    private void AnimationController()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking);
    }

    protected override void CollisionTest()
    {
        base.CollisionTest();

        RaycastHit2D raycast1 = Physics2D.Raycast(playerCheck.position, Vector2.right, playerCheckDistance * facingDir, whatIsPlayer);
        RaycastHit2D raycast2 = Physics2D.Raycast(new Vector3(playerCheck.position.x, playerCheck.position.y + offset), Vector2.right, playerCheckDistance * facingDir, whatIsPlayer);
        isPlayerDetected = raycast1 == true ? raycast1 : raycast2;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + playerCheckDistance * facingDir, playerCheck.position.y));
        Gizmos.DrawLine(new Vector3(playerCheck.position.x, playerCheck.position.y + offset), new Vector3(playerCheck.position.x + playerCheckDistance * facingDir, playerCheck.position.y + offset));
    }

    public void AttackOver()
    {
        isAttacking = false;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!isAttacking && !isPlayerDetected)
        {
            Flip();
        }
    }

    private void Dead()
    {
        sp_item.enabled = true;
        anim.SetTrigger("isDead");
    }
}
