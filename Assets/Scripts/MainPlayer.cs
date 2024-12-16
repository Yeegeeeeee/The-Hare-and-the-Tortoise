using System;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    [SerializeField] private Transform headCheck;
    [SerializeField] private float headCheckDistance;
    [SerializeField] private float headCheck_xPos = 0.5f;
    [SerializeField] private float trapCheck_xPos = 0.5f;
    [SerializeField] private float itemCheck_yPos = 0.5f;
    [Header("Player Info")]
    [SerializeField] private float stamina = 1f;
    [SerializeField] private float focus = 1f;
    private float flip_offset = 0.3f;
    private float xInput;
    private float yInput;
    [Header("Move Info")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;
    private float dashDuration;
    private float dashTimer;
    [Header("Cooldown Info")]
    [SerializeField] private float dashCooldownTime = 3f;
    [Header("Audio Info")]


    private float dashCooldownTimer;
    [Header("Detection Info")]
    [SerializeField] protected Transform groundCheck;
    private bool isGrounded;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform itemCheck;
    [SerializeField] protected float itemCheckDistance;
    [SerializeField] private LayerMask whatIsItem;
    [SerializeField] private LayerMask whatIsTrap;



    private bool isMoving = false;
    private float currentDashCooldownTime;
    private DashCoolDownTimer dashCoolDownTimer;
    private bool isHeadHit;
    private bool isHurt;
    private bool isDead;
    private bool isTrapped;
    private bool isItemDetected;
    protected int facingDir = 1;
    protected bool facingRight = true;
    private bool allowMoving = false;
    private int jumpCount;

    protected void Start()
    {
        SetInitialState();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentDashCooldownTime = dashCooldownTime;
        dashDuration = stamina;
        //GameObject timer = transform.Find("Timer").gameObject;
        //dashCoolDownTimer = timer.GetComponentInChildren<DashCoolDownTimer>();
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

    protected void DisableAll()
    {
        anim.SetBool("isMoving", false);
    }

    // Update is called once per frame
    protected void Update()
    {
        CollisionTest();


        if (isHurt)
        {
            SetVelocity(0, 0);
        }
        else
        {
            DecreaseTimer();
            Move();
            CheckInput();
            GetPotion();
            FlipController();
            AnimatorController();
        }

    }

    private void UpdateIcon()
    {
        float dashCoolDownProgress = (currentDashCooldownTime - dashCooldownTimer) / currentDashCooldownTime;
        dashCoolDownTimer.SetTimer(dashCoolDownProgress);
    }

    private void Dead()
    {
        anim.SetTrigger("isDead");
    }

    private void MusicPlayer()
    {

    }

    private void MuteAll()
    {

    }

    private void GetPotion()
    {
        RaycastHit2D get = Physics2D.Raycast(itemCheck.position, Vector2.right, itemCheckDistance * facingDir, whatIsItem);
        if (get)
        {
            Potion potion = get.collider.gameObject.GetComponent<Potion>();
            int id = potion.GetId();

            switch (id)
            {
                case 0:
                    break;
            }
        }
    }

    public void SetAllowMoving(bool allowMoving)
    {
        Debug.Log("allow moving: " + allowMoving);
        this.allowMoving = allowMoving;
    }

    private void Move()
    {

        if ((xInput != 0 || xInput == 0 && isGrounded) && dashTimer < 0)
        {
            SetVelocity(xInput * speed, rb.velocity.y);
        }
    }

    public void PlayVictory()
    {

    }

    private void PlayDeath()
    {

    }

    private void PlayGetPotions()
    {

    }

    private void Jump()
    {
        jumpCount++;
        SetVelocity(rb.velocity.x, jumpHeight);
        if (jumpCount == 2)
            jumpCount = 0;
    }

    private void CheckInput()
    {
        if (allowMoving)
            xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && !isHeadHit)
                Jump();

            if (!isGrounded && jumpCount == 1)
            {
                Jump();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTimer < 0 && dashCooldownTimer < 0)
        {
            //dashCoolDownTimer.ResetTimer();
            dashTimer = dashDuration;
            dashCooldownTimer = currentDashCooldownTime;
            SetVelocity(facingDir * dashSpeed, rb.velocity.y);
        }
    }

    protected void CollisionTest()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        RaycastHit2D raycast1 = GetVerticalRaycastHit(groundCheck, 0, Vector2.down, groundCheckDistance, whatIsTrap);
        RaycastHit2D raycast2 = GetVerticalRaycastHit(groundCheck, trapCheck_xPos, Vector2.down, groundCheckDistance, whatIsTrap);
        RaycastHit2D raycast3 = GetVerticalRaycastHit(groundCheck, -trapCheck_xPos, Vector2.down, groundCheckDistance, whatIsTrap);

        isTrapped = GetRaycastResult(raycast1, raycast2, raycast3);

        raycast1 = GetVerticalRaycastHit(headCheck, 0, Vector2.up, headCheckDistance, whatIsGround);
        raycast2 = GetVerticalRaycastHit(headCheck, headCheck_xPos, Vector2.up, headCheckDistance, whatIsGround);
        raycast3 = GetVerticalRaycastHit(headCheck, -headCheck_xPos, Vector2.up, headCheckDistance, whatIsGround);

        isHeadHit = GetRaycastResult(raycast1, raycast2, raycast3);

        raycast1 = GetHorizontalRaycastHit(itemCheck, 0, facingDir * Vector2.right, itemCheckDistance, whatIsItem);
        raycast2 = GetHorizontalRaycastHit(itemCheck, itemCheck_yPos, facingDir * Vector2.right, itemCheckDistance, whatIsItem);
        raycast3 = GetHorizontalRaycastHit(itemCheck, -itemCheck_yPos, facingDir * Vector2.right, itemCheckDistance, whatIsItem);

        isItemDetected = GetRaycastResult(raycast1, raycast2, raycast3);
    }

    private bool GetRaycastResult(RaycastHit2D raycast1, RaycastHit2D raycast2, RaycastHit2D raycast3)
    {
        if (!raycast1)
            return raycast2 == true ? raycast2 : raycast3;
        else
            return raycast1 == true;
    }

    private RaycastHit2D GetVerticalRaycastHit(Transform check, float xPos, Vector2 direct, float distance, LayerMask layer)
    {
        return Physics2D.Raycast(new Vector3(check.position.x + xPos, check.position.y), direct, distance, layer);
    }

    private RaycastHit2D GetHorizontalRaycastHit(Transform check, float yPos, Vector2 direct, float distance, LayerMask layer)
    {
        return Physics2D.Raycast(new Vector3(check.position.x, check.position.y + yPos), direct, distance, layer);
    }

    protected void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        DrawVerticalLines(groundCheck, -groundCheckDistance, trapCheck_xPos, 3, Color.white);
        DrawHorizontalLines(itemCheck, itemCheckDistance, itemCheck_yPos, 3, Color.red);
        DrawVerticalLines(headCheck, headCheckDistance, headCheck_xPos, 3, Color.yellow);
    }

    private void DrawHorizontalLines(Transform check, float distance, float yPos, int numberOfIterations, Color color)
    {
        Gizmos.color = color;
        float[] positions = { 0, yPos, -yPos };
        for (int i = 0; i < numberOfIterations; i++)
        {
            Gizmos.DrawLine(new Vector3(check.position.x, check.position.y + positions[i]), new Vector3(check.position.x + facingDir * distance, check.position.y + positions[i]));
        }
    }

    private void DrawVerticalLines(Transform check, float distance, float xPos, int numberOfIterations, Color color)
    {
        Gizmos.color = color;
        float[] positions = { 0, xPos, -xPos };
        for (int i = 0; i < numberOfIterations; i++)
        {
            Gizmos.DrawLine(new Vector3(check.position.x + positions[i], check.position.y), new Vector3(check.position.x + positions[i], check.position.y + distance));
        }
    }

    private void DecreaseTimer()
    {
        dashTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
    }

    private void AnimatorController()
    {
        if (!isDead && !isHurt)
        {
            IsMovingAnim();
            IsGroundedAnim();
            IsDashingAnim();
            IsJumpAnim();
        }
    }

    private void IsJumpAnim()
    {
        anim.SetFloat("y_velocity", rb.velocity.y);
    }

    private void IsMovingAnim()
    {
        isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
    }

    private void IsGroundedAnim()
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    private void IsDashingAnim()
    {
        anim.SetBool("isDashing", dashTimer > 0);
    }

    private void FlipController()
    {
        if (rb.velocity.x > flip_offset && !facingRight)
            Flip();
        if (rb.velocity.x < -flip_offset && facingRight)
            Flip();
    }

    protected virtual void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void SetVelocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }

    private void SetInitialState()
    {
        float _speed = PlayerPrefs.GetFloat("speed", 0);
        speed += _speed;
        float _stamina = PlayerPrefs.GetFloat("stamina", 0);
        stamina += _stamina;
        float _focus = PlayerPrefs.GetFloat("focus", 0);
        focus += _focus;
    }
}
