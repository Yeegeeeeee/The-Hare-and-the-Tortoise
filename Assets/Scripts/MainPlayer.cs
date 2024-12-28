using System.Collections;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float flip_offset = 0.3f;
    private float xInput;
    private float yInput;
    [Header("Move Info")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private GameObject ghost;
    [SerializeField] private int ghostNum = 3;
    private float ghostTime;
    [SerializeField] private float dashDuration = .3f;
    private float dashTimer;
    [Header("Cooldown Info")]
    [SerializeField] private float stamina = 3f;
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
    [SerializeField] private LayerMask detectLine;



    private bool isMoving = false;
    private float currentDashCooldownTime;
    private DashCoolDownTimer dashCoolDownTimer;
    private bool isHeadHit;
    private bool isHurt;
    private bool isDead;
    private bool isTrapped;
    private bool isFall;
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
        currentDashCooldownTime = stamina;
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
        if (isHurt)
        {
            HandleHurt();
        }
        else if (isDead)
        {
            if (transform.position.y < -20f)
            {
                SceneChangeManager();
            }
        }
        else
        {
            CollisionTest();
            DecreaseTimer();
            CheckFall();
            Move();
            CheckInput();
            GetPotion();
            FlipController();
            GenerateGhost();
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

    public void SetIsHurt(bool _isHurt)
    {
        Debug.Log($"SetIsHurt called. isHurt: {_isHurt}, frame: {Time.frameCount}");
        isHurt = _isHurt;
        IsHurtAnim();
    }

    private void MusicPlayer()
    {

    }

    private void HandleHurt()
    {
        Debug.Log("Handle Hurt");
        if (isDead) return;
        isDead = true;
        IsHurtAnim();
        SetVelocity(0, jumpHeight);
        Destroy(GetComponent<Collider2D>());
        isHurt = false;
    }

    private void SceneChangeManager()
    {

        if (isTrapped)
        {
            JumpToTrapDeath();
            return;
        }
        if (isFall)
        {
            JumpToFallDeath();
            return;
        }
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

    private void CheckFall()
    {
        if (isHurt || isTrapped || isDead) return;
        if (transform.position.y < -10f && transform.position.y > -20f)
        {
            isFall = true;
            isHurt = true;
        }
    }

    private void JumpToFallDeath()
    {
        SceneManager.LoadScene("FallDeathConversation");
    }

    private void JumpToTrapDeath()
    {
        SceneManager.LoadScene("TrapDeathConversation");
    }

    public void SetAllowMoving(bool allowMoving)
    {
        Debug.Log("allow moving: " + allowMoving);
        this.allowMoving = allowMoving;
    }

    private void Move()
    {
        currentDashCooldownTime = stamina;
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
        if (jumpCount < 2)
        {
            jumpCount++;
            SetVelocity(rb.velocity.x, jumpHeight);
        }

    }

    private void CheckInput()
    {
        if (allowMoving)
            xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || jumpCount < 2)
            {
                Jump();
            }
            if (isGrounded && jumpCount == 2)
            {
                jumpCount = 0;
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

    private void GenerateGhost()
    {
        if (dashTimer > 0 && Time.time > ghostTime)
        {
            GameObject ghostObj = Instantiate(ghost, transform.position, Quaternion.identity);
            if (!facingRight)
            {
                ghostObj.transform.Rotate(0, 180, 0);
            }
            ghostTime = Time.time + dashDuration / ghostNum;
            if (dashTimer < 0.1f)
            {
                dashTimer = 0;
            }
        }
    }

    protected void CollisionTest()
    {
        if (isHurt) return;
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        RaycastHit2D raycast1 = GetVerticalRaycastHit(groundCheck, 0, Vector2.down, groundCheckDistance, whatIsTrap);
        RaycastHit2D raycast2 = GetVerticalRaycastHit(groundCheck, trapCheck_xPos, Vector2.down, groundCheckDistance, whatIsTrap);
        RaycastHit2D raycast3 = GetVerticalRaycastHit(groundCheck, -trapCheck_xPos, Vector2.down, groundCheckDistance, whatIsTrap);

        isTrapped = GetRaycastResult(raycast1, raycast2, raycast3);
        if (isTrapped)
            isHurt = true;

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

    private void IsHurtAnim()
    {
        anim.SetBool("isHurt", isHurt);
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
        float focus = PlayerPrefs.GetFloat("focus", 0);
        float courage = PlayerPrefs.GetFloat("courage", 0);
        float determination = PlayerPrefs.GetFloat("determination", 0);
        float inspection = PlayerPrefs.GetFloat("inspection", 0);
        float confidence = PlayerPrefs.GetFloat("confidence", 0);
        Debug.Log("focus: " + focus + ", courage: " + courage + ", determination: " + determination);
        float _duration = 0, _speed = 0, _dashSpeed = 0;
        if (focus != 0)
        {
            _duration = 1f;
        }

        if (courage != 0)
        {
            _speed = 2f;
        }
        if (determination != 0)
        {
            _dashSpeed = 2f;
        }

        stamina -= _duration;
        speed += _speed;
        dashSpeed += _dashSpeed;
    }
}
