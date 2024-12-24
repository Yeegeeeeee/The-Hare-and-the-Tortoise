using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Cultist_Assassin : Entity
{
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [Header("Player Detection Info")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Player player;
    [Header("Enemy Info")]
    [SerializeField] public float MaxHP = 400;
    [SerializeField] private float BaseDamage = 20;
    [SerializeField] protected Transform playerCheck;
    [SerializeField] protected float playerCheckDistance;
    [SerializeField] protected float offset;
    private RaycastHit2D isPlayerDetected;

    private Vector3 originalPosition;

    private bool isAttacking;
    private int attackCounter;
    private float acceptable = 0.2f;

    private bool isMoving;

    private SpriteRenderer sr;
    private Collider2D col;
    private bool isVanished;
    private GameObject healthBar;
    protected override void Start()
    {
        base.Start();
        facingDir = -1;
        facingRight = false;
        currentHp = MaxHP;
        damage = BaseDamage;
        originalPosition = transform.position;
        id = 2;
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        isVanished = false;
        healthBar = transform.Find("Entity_Status_UI").gameObject;
    }

    protected override void Update()
    {
        if (isVanished) return;
        base.Update();
        if (isDead)
        {
            DisableAll();
            anim.SetBool("isHurt", false);
            rb.velocity = Vector2.zero;
            Dead();
        }
        else
        {
            if (isHurt)
            {
                DisableAll();
                rb.velocity = Vector2.zero;
                GetHurt();
            }
            else
            {
                if (isPlayerDetected)
                {
                    if (isPlayerDetected.distance > 2)
                    {
                        rb.velocity = new Vector2(moveSpeed * 1.5f * facingDir, rb.velocity.y);
                    }
                    else
                    {
                        UpdateAttackStatus();
                    }
                }
                else
                {
                    isAttacking = false;
                    GoBack();

                    if (!isGrounded || isWallDetected)
                    {
                        Flip();

                    }
                }
                isMoving = rb.velocity.x != 0;

                AnimationController();
            }
        }
    }

    private void UpdateAttackStatus()
    {
        isAttacking = true;
    }

    public Player GetPlayer()
    {
        return player;
    }

    private void GoBack()
    {
        float x = transform.position.x - originalPosition.x;

        FlipController(x);

        if (Mathf.Abs(x) > acceptable)
        {
            rb.velocity = new Vector2(moveSpeed * facingDir, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void PerformAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(attackCheck.position, Vector2.right, attackCheckDistance * facingDir, whatIsPlayer);
        if (hit)
        {
            if (attackCounter == 3)
            {
                damage = 60;
            }
            player.TakeDamage(damage);
            damage = BaseDamage;
        }
    }

    private void AnimationController()
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("AttackCounter", attackCounter);
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

    private void Dead()
    {
        anim.SetTrigger("isDead");
    }

    public void Vanish()
    {

        if (sr != null)
            sr.enabled = false;

        if (col != null)
            col.enabled = false;

        if (healthBar != null)
            healthBar.SetActive(false);

        isVanished = true;
    }

    public void Reappear()
    {
        float dir = player.getFacingDir();
        Vector2 newPosition = new Vector2(player.transform.position.x - 1 * dir, player.transform.position.y);
        transform.position = newPosition;
        if (sr != null)
            sr.enabled = true;

        if (col != null)
            col.enabled = true;

        if (healthBar != null)
            healthBar.SetActive(true);

        if (facingDir != dir)
            Flip();

    }

    private void FlipController(float x)
    {
        if ((x < -acceptable && !facingRight) || (x > acceptable && facingRight) || (x > -acceptable && x < acceptable && facingRight))
        {
            Flip();
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!isAttacking && !isPlayerDetected)
        {
            Flip();
        }
        if (!isDead)
        {
            isHurt = true;
        }
    }

    public void AttackOver()
    {
        isVanished = false;
        isAttacking = false;
        attackCounter++;
        if (attackCounter > 3)
        {
            attackCounter = 0;
        }
    }

    private void GetHurt()
    {
        rb.velocity = new Vector2(-facingDir * 2f, rb.velocity.y);
        anim.SetBool("isHurt", isHurt);
    }

    public void HurtAnimationOver()
    {
        isHurt = false;
        anim.SetBool("isHurt", isHurt);
    }
}
