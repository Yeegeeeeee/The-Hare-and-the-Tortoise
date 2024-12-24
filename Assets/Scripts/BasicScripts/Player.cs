using UnityEngine;

public class Player : Entity
{
    [SerializeField] private Transform headCheck;
    [SerializeField] private float headCheckDistance;
    private float xPos = 0.5f;
    private float yPos = 0.5f;
    [SerializeField] private GameObject damageNumPrefab;
    [Header("Player Info")]
    [SerializeField] public float MaxHP = 100;
    [SerializeField] private float BaseDamage = 10;
    [SerializeField] private float offset;
    private float xInput;
    private float yInput;
    [Header("Move Info")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;

    [Header("Dash Info")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    [Header("Cooldown Info")]
    [SerializeField] private float dashCooldownTime = 3f;
    [SerializeField] private float wallJumpCooldownTime = 0.4f;
    [SerializeField] private float xInputAfterWallJumpTime = 0.4f;
    [SerializeField] private float comboCooldownTime = 2f;
    [Header("Audio Info")]
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource Death;
    [SerializeField] private AudioSource Victory;
    [SerializeField] private AudioSource BossFight;
    [SerializeField] private AudioSource GetPotions;

    private float dashCooldownTimer;
    private float wallJumpCooldownTimer;
    private float comboCooldownTimer;
    private float xInputAfterWallJumpTimer = 0;
    [Header("Attack Info")]
    private float comboTime = 1;
    private float comboTimeWindow;
    private bool isAttacking;
    private int comboCounter;
    [Header("Detection Info")]
    [SerializeField] private LayerMask enemy;
    [SerializeField] private LayerMask item;
    [SerializeField] private LayerMask detectLine;
    [SerializeField] private LayerMask door;

    private bool isWallSlide;
    private bool isMoving = false;
    private float currentDashCooldownTime;
    private float currentComboCooldownTime;
    private DashCoolDownTimer dashCoolDownTimer;
    private ComboCoolDownTimer comboCoolDownTimer;
    private float IconTime = 1.5f;
    private float IconTimer;
    [Header("Upgrade Info")]
    [SerializeField] private GameObject HealIcon;
    [SerializeField] private GameObject DashIcon;
    [SerializeField] private GameObject ComboIcon;
    [SerializeField] private GameObject DamageIcon;
    private GameObject CurrentIcon;
    private bool isHeadHit;

    protected override void Start()
    {
        base.Start();
        currentHp = MaxHP;
        damage = BaseDamage;
        id = 0;
        currentDashCooldownTime = dashCooldownTime;
        currentComboCooldownTime = comboCooldownTime;
        GameObject timer = transform.Find("Timer").gameObject;
        dashCoolDownTimer = timer.GetComponentInChildren<DashCoolDownTimer>();
        comboCoolDownTimer = timer.GetComponentInChildren<ComboCoolDownTimer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isDead)
        {
            SetVelocity(0, 0);
            anim.SetBool("isHurt", false);
            Dead();
        }
        else
        {
            if (isHurt)
            {
                DisableAll();
                SetVelocity(0, 0);
                GetHurt();
            }
            else
            {
                DecreaseTimer();
                UpdateIcon();
                Move();
                CheckInput();
                WallSlideDCheck();
                GetPotion();
                GetDetected();
                FlipController();
                AnimatorController();
            }
        }
    }

    private void UpdateIcon()
    {
        float dashCoolDownProgress = (currentDashCooldownTime - dashCooldownTimer) / currentDashCooldownTime;
        dashCoolDownTimer.SetTimer(dashCoolDownProgress);
        float comboCoolDownProgress = (currentComboCooldownTime - comboCooldownTimer) / currentComboCooldownTime;
        comboCoolDownTimer.SetTimer(comboCoolDownProgress);
        if (IconTimer < 0 && CurrentIcon != null)
        {
            CurrentIcon.SetActive(false);
        }
    }

    private void Dead()
    {
        DisableAll();
        anim.SetTrigger("isDead");
    }

    public void PerformAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(attackCheck.position, Vector2.right, attackCheckDistance * facingDir, enemy);
        if (hit)
        {
            Entity entity = hit.collider.gameObject.GetComponent<Entity>();
            float x = transform.position.x - entity.transform.position.x;
            bool faceRight = entity.getFacingRight();
            float effectiveDamage = damage;
            if (comboCounter == 2)
            {
                effectiveDamage *= 2f;
            }
            if (x > 0.5f && !faceRight || x < 0.5f && faceRight)
            {
                entity.TakeDamage(effectiveDamage * 2f);
                GameObject damageTextInstance = Instantiate(damageNumPrefab, entity.transform.position, Quaternion.identity);
                DamageNum damageNum = damageTextInstance.GetComponent<DamageNum>();
                if (damageNum != null)
                {
                    damageNum.Initialize(entity);
                    damageNum.SetDamage(effectiveDamage * 2f);
                    damageNum.Play();
                }
            }
            else
            {
                entity.TakeDamage(effectiveDamage);
                GameObject damageTextInstance = Instantiate(damageNumPrefab, entity.transform.position, Quaternion.identity);
                DamageNum damageNum = damageTextInstance.GetComponent<DamageNum>();
                if (damageNum != null)
                {
                    damageNum.Initialize(entity);
                    damageNum.SetDamage(effectiveDamage);
                    damageNum.Play();
                }
            }
        }
    }

    private void MusicPlayer()
    {
        if (GetPotions != null && GetPotions.isPlaying)
            return;

        if (Victory != null && Victory.isPlaying)
            return;

        if (Death != null && Death.isPlaying)
            return;

        if (BossFight != null && BossFight.isPlaying)
            return;

        if (BGM != null && !BGM.isPlaying)
            BGM.Play();
    }

    private void MuteAll()
    {
        if (GetPotions != null && GetPotions.isPlaying)
        {
            GetPotions.Stop();
            return;
        }
        if (Victory != null && Victory.isPlaying)
        {
            Victory.Stop();
            return;
        }

        if (Death != null && Death.isPlaying)
        {
            Death.Stop();
            return;
        }

        if (BossFight != null && BossFight.isPlaying)
        {
            BossFight.Stop();
            return;
        }

        if (BGM != null && BGM.isPlaying)
        {
            BGM.Stop();
            return;
        }
    }

    private void GetPotion()
    {
        RaycastHit2D get = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, item);
        if (get)
        {
            Potion potion = get.collider.gameObject.GetComponent<Potion>();
            int id = potion.GetId();
            if (CurrentIcon != null)
                CurrentIcon.SetActive(false);
            switch (id)
            {
                case 0:
                    currentHp = 100;
                    CurrentIcon = HealIcon;
                    Debug.Log("Player has healed: " + currentHp);
                    break;
                case 1:
                    damage *= 1.5f;
                    CurrentIcon = DamageIcon;
                    Debug.Log("Player has strengthened: " + damage);
                    break;
                case 2:
                    currentDashCooldownTime -= 1;
                    CurrentIcon = DashIcon;
                    Debug.Log("Player's dash cool down time: " + currentDashCooldownTime);
                    break;
                case 3:
                    currentComboCooldownTime *= 0.5f;
                    CurrentIcon = ComboIcon;
                    Debug.Log("Player's combo cool down time: " + currentComboCooldownTime);
                    break;
            }
            potion.UpdateStatus(true);
            CurrentIcon.SetActive(true);
            IconTimer = IconTime;
            PlayGetPotions();
        }
    }

    private void GetDetected()
    {
        RaycastHit2D detect = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance * facingDir, detectLine);
        if (detect)
        {
            DetectLine line = detect.collider.gameObject.GetComponent<DetectLine>();
            int id = line.GetId();
            switch (id)
            {
                case 9:
                    PlayBossBGM();
                    break;
                case 10:
                    MuteAll();
                    break;
            }
            line.UpdateStatus(true);
        }
    }

    private void Move()
    {

        if (!isAttacking)
        {
            if ((xInput != 0 || xInput == 0 && isGrounded) && dashTimer < 0)
            {
                SetVelocity(xInput * speed, rb.velocity.y);
            }
        }
        else
        {
            SetVelocity(0, 0);
        }
    }

    public void PlayVictory()
    {
        if (GetPotions != null && GetPotions.isPlaying)
            GetPotions.Stop();
        if (BossFight != null && BossFight.isPlaying)
            BossFight.Stop();
        if (BGM != null && BGM.isPlaying)
            BGM.Stop();
        if (Death != null && Death.isPlaying)
            Death.Stop();
        Victory.Play();
    }

    private void PlayDeath()
    {
        if (GetPotions != null && GetPotions.isPlaying)
            GetPotions.Stop();
        if (BossFight != null && BossFight.isPlaying)
            BossFight.Stop();
        if (BGM != null && BGM.isPlaying)
            BGM.Stop();
        if (Victory != null && Victory.isPlaying)
            Victory.Stop();
        Death.Play();
    }

    private void PlayBossBGM()
    {
        if (GetPotions != null && GetPotions.isPlaying)
            GetPotions.Stop();
        if (BGM != null && BGM.isPlaying)
            BGM.Stop();
        if (Victory != null && Victory.isPlaying)
            Victory.Stop();
        if (Death != null && Death.isPlaying)
            Death.Stop();
        BossFight.Play();
    }

    private void PlayGetPotions()
    {
        if (GetPotions != null && GetPotions.isPlaying)
            GetPotions.Stop();

        if (Victory != null && Victory.isPlaying)
            Victory.Stop();

        if (Death != null && Death.isPlaying)
            Death.Stop();

        if (BossFight != null && BossFight.isPlaying)
            BossFight.Stop();
        GetPotions.Play();
    }

    private void Jump()
    {
        SetVelocity(rb.velocity.x, jumpHeight);
    }

    public void AttackOver()
    {
        isAttacking = false;
        comboCounter++;
        if (comboCounter > 2)
        {
            comboCounter = 0;
        }
    }

    private void CheckInput()
    {
        if (xInputAfterWallJumpTimer < 0)
            xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (isGrounded && !isHurt)
            {
                if (comboTimeWindow < 0)
                {
                    comboCounter = 0;
                }
                if (comboCooldownTimer < 0)
                {
                    isAttacking = true;
                    comboTimeWindow = comboTime;
                }
                if (comboCounter == 2)
                {
                    comboCooldownTimer = currentComboCooldownTime;
                    comboCoolDownTimer.ResetTimer();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (isGrounded && !isHeadHit)
                Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isAttacking && dashTimer < 0 && dashCooldownTimer < 0)
        {
            dashCoolDownTimer.ResetTimer();
            dashTimer = dashDuration;
            dashCooldownTimer = currentDashCooldownTime;
            SetVelocity(facingDir * dashSpeed, rb.velocity.y);
        }
    }

    protected override void CollisionTest()
    {
        base.CollisionTest();
        RaycastHit2D raycast1 = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckDistance, whatIsGround);
        RaycastHit2D raycast2 = Physics2D.Raycast(new Vector3(headCheck.position.x + xPos, headCheck.position.y), Vector2.up, headCheckDistance, whatIsGround);
        RaycastHit2D raycast3 = Physics2D.Raycast(new Vector3(headCheck.position.x - xPos, headCheck.position.y), Vector2.up, headCheckDistance, whatIsGround);
        if (!raycast1)
            isHeadHit = raycast2 == true ? raycast2 : raycast3;
        else
            isHeadHit = raycast1 == true;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(headCheck.position, new Vector3(headCheck.position.x, headCheck.position.y + headCheckDistance));
        Gizmos.DrawLine(new Vector3(headCheck.position.x + xPos, headCheck.position.y), new Vector3(headCheck.position.x + xPos, headCheck.position.y + headCheckDistance));
        Gizmos.DrawLine(new Vector3(headCheck.position.x - xPos, headCheck.position.y), new Vector3(headCheck.position.x - xPos, headCheck.position.y + headCheckDistance));
    }

    private void DecreaseTimer()
    {
        dashTimer -= Time.deltaTime;
        dashCooldownTimer -= Time.deltaTime;
        comboTimeWindow -= Time.deltaTime;
        wallJumpCooldownTimer -= Time.deltaTime;
        xInputAfterWallJumpTimer -= Time.deltaTime;
        comboCooldownTimer -= Time.deltaTime;
        IconTimer -= Time.deltaTime;
    }

    private void AnimatorController()
    {
        if (!isDead && !isHurt)
        {
            JumpFallAnim();
            IsMovingAnim();
            IsGroundedAnim();
            IsDashingAnim();
            IsAttackingAnim();
            ComboCounterAnim();
            IsWallSlideAnim();
        }
    }

    private void JumpFallAnim()
    {
        if (isAttacking && (rb.velocity.y < -yPos || rb.velocity.y > yPos))
            isAttacking = false;
        anim.SetFloat("yVelocity", rb.velocity.y);
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

    private void IsAttackingAnim()
    {
        anim.SetBool("isAttacking", isAttacking);
    }

    private void ComboCounterAnim()
    {
        anim.SetInteger("comboCounter", comboCounter);
    }

    private void IsWallSlideAnim()
    {
        anim.SetBool("isWallSlide", isWallSlide);
    }

    private void FlipController()
    {
        if (rb.velocity.x > offset && !facingRight)
            Flip();
        if (rb.velocity.x < -offset && facingRight)
            Flip();
    }

    private void WallSlideDCheck()
    {
        if (!isGrounded && isWallDetected)
        {
            if (xInput == 0 || xInput > 0 && facingRight || xInput < 0 && !facingRight)
            {
                isWallSlide = true;
                if (yInput < 0)
                {
                    SetVelocity(0, rb.velocity.y);
                }
                else
                {
                    SetVelocity(0, rb.velocity.y * 0.7f);
                }
                if (Input.GetKeyDown(KeyCode.K) && wallJumpCooldownTimer < 0 && xInput == 0)
                {
                    wallJumpCooldownTimer = wallJumpCooldownTime;
                    SetVelocity(5f * -facingDir, jumpHeight);
                    isWallSlide = false;
                    xInputAfterWallJumpTimer = xInputAfterWallJumpTime;
                }
            }
            else
            {
                isWallSlide = false;
            }
        }
        else
        {
            isWallSlide = false;
        }
    }

    private void SetVelocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (!isDead)
        {
            isHurt = true;
        }
        else
        {
            PlayDeath();
        }
        Debug.Log("Player's HP: " + currentHp);
        if (isAttacking)
        {
            isAttacking = false;
            anim.SetBool("isAttacking", isAttacking);
        }
    }

    protected override void Flip()
    {
        base.Flip();
    }

    private void GetHurt()
    {
        SetVelocity(-facingDir * 5f, rb.velocity.y);
        anim.SetBool("isHurt", isHurt);
    }

    public void HurtAnimationOver()
    {
        isHurt = false;
        anim.SetBool("isHurt", isHurt);
    }

    protected override void DisableAll()
    {
        anim.SetFloat("yVelocity", 0);
        anim.SetBool("isGrounded", true);
        anim.SetBool("isDashing", false);
        anim.SetBool("isWallSlide", false);
    }
}
