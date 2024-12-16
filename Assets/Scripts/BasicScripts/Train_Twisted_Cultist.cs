using UnityEngine;

public class Train_Twisted_Cultist : Entity
{
    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    [Header("Enemy Info")]
    [SerializeField] public float MaxHP = 100;

    private bool isMoving;

    private bool isTransformed;
    private bool justTransform;
    private Collider2D coll;

    public Player player;

    protected override void Start()
    {
        base.Start();
        facingDir = -1;
        facingRight = false;
        currentHp = MaxHP;
        id = 3;
        isTransformed = false;
        justTransform = false;
        coll = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        if (isDead)
        {
            DisableAll();
            anim.SetBool("isHurt", false);
            rb.velocity = Vector2.zero;
            Dead();
        }
        else
        {
            if (isTransformed && justTransform)
            {
                justTransform = false;
                anim.SetBool("isDead", isDead);
                anim.SetBool("isTransformed", isTransformed);
                rb.velocity = new Vector2(facingDir * moveSpeed, rb.velocity.y);
            }
            if (isHurt)
            {
                DisableAll();
                rb.velocity = Vector2.zero;
                GetHurt();
            }
            else
            {
                isMoving = rb.velocity.x != 0;

                AnimationController();
            }
        }
    }

    private void AnimationController()
    {
        anim.SetBool("isMoving", isMoving);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!isDead)
        {
            isHurt = true;
        }
        //Debug.Log("Training robot's health: " + currentHp);
    }

    private void Dead()
    {
        anim.SetBool("isDead", isDead);
        anim.SetBool("isTransformed", isTransformed);

        if (!isTransformed && !justTransform)
        {
            currentHp = MaxHP;
            justTransform = true;
        }

        isDead = false;
        if (!facingRight)
        {
            Flip();
            FlipCollider();
        }
    }

    private void GetHurt()
    {
        if (!facingRight && player.getFacingRight() || facingRight && !player.getFacingRight())
        {
            rb.velocity = new Vector2(-facingDir * 5f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(facingDir * 5f, rb.velocity.y);

        }
        anim.SetBool("isHurt", isHurt);
        anim.SetBool("isTransformed", isTransformed);
    }

    public void HurtAnimationOver()
    {
        isHurt = false;
        anim.SetBool("isHurt", isHurt);
    }

    private void FlipCollider()
    {
        coll.offset = new Vector2(-1.3f, coll.offset.y);
    }

    public void UpdateTransform()
    {
        isTransformed = true;
    }
}
