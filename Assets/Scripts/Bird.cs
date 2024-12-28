using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bird : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    [Header("Move Info")]
    [SerializeField] private float speed;
    [Header("Item Info")]
    [SerializeField] private LayerMask whatIsItem;
    [SerializeField] private Transform itemCheck;
    [SerializeField] private float verticalCheckDistance;
    [SerializeField] private float horizontalCheckDistance;
    [SerializeField] private float eatingPeriod = 2f;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [Header("Platform")]
    [SerializeField] private LayerMask whatIsPlatform;
    [SerializeField] private float platformCheckDistance;
    [SerializeField] private float restPeriod = 5f;
    [SerializeField] private float p_xOffset;
    [SerializeField] private float p_yOffset;
    private bool allowMoving = false;
    private Vector3 startPoint;
    private bool isAngry = false;
    private bool isEating = false;
    private bool isRest = false;
    private bool isVerticalItems;
    private bool isHorizontalItems;
    private bool isPlatformDetected;
    private Item targetItem;
    private Platform targetPlatform;
    private float originLevel;
    private bool isFlyingToTarget = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPoint = rb.transform.position;
        SetInitialState();
        if (isAngry)
        {
            speed = 15;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CollisionTest();
        if (allowMoving && !isEating && !isRest)
            Move();
        AnimController();
        CheckForItems();
        CheckForPlatform();
    }

    private void AnimController()
    {
        anim.SetBool("isMoving", allowMoving);
        anim.SetBool("isEating", isEating);
        anim.SetBool("isRest", isRest);
    }

    private void DistanceCount()
    {
        float distance = rb.transform.position.x - startPoint.x;

    }

    private void DropTrap()
    {

    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(itemCheck.position, new Vector3(itemCheck.position.x, itemCheck.position.y - verticalCheckDistance));
        Gizmos.DrawLine(itemCheck.position, new Vector3(itemCheck.position.x + horizontalCheckDistance, itemCheck.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(itemCheck.position, new Vector3(itemCheck.position.x, itemCheck.position.y - platformCheckDistance));
    }

    protected void CollisionTest()
    {
        if (isEating || targetItem != null || isRest) return;

        isVerticalItems = Physics2D.Raycast(itemCheck.position, Vector3.down, verticalCheckDistance, whatIsItem);
        if (!isVerticalItems)
        {
            isHorizontalItems = Physics2D.Raycast(itemCheck.position, Vector3.right, horizontalCheckDistance, whatIsItem);
        }
        if (isVerticalItems || isHorizontalItems)
        {
            GetItem();
        }

        isPlatformDetected = Physics2D.Raycast(itemCheck.position, Vector3.down, platformCheckDistance, whatIsPlatform);
        if (isPlatformDetected)
        {
            GetPlatform();
        }
    }

    private void GetItem()
    {
        if (isVerticalItems)
        {
            RaycastHit2D get = Physics2D.Raycast(itemCheck.position, Vector3.down, verticalCheckDistance, whatIsItem);
            if (get)
            {
                Item item = get.collider.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    targetItem = item;
                }
            }
        }
        else if (isHorizontalItems)
        {
            RaycastHit2D get = Physics2D.Raycast(itemCheck.position, Vector3.right, horizontalCheckDistance, whatIsItem);
            if (get)
            {
                Item item = get.collider.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    targetItem = item;
                }
            }
        }
    }

    private void GetPlatform()
    {
        RaycastHit2D get = Physics2D.Raycast(itemCheck.position, Vector3.down, platformCheckDistance, whatIsPlatform);
        if (get)
        {
            Platform platform = get.collider.gameObject.GetComponent<Platform>();
            if (platform != null)
            {
                targetPlatform = platform;
            }
        }
    }

    private void CheckForItems()
    {
        if (isEating || isFlyingToTarget || isRest) return;
        originLevel = transform.position.y;
        if (isVerticalItems || isHorizontalItems)
        {
            allowMoving = false;
            StartCoroutine(StartEating());
        }
    }

    private void CheckForPlatform()
    {
        if (isEating || isFlyingToTarget || isRest) return;
        originLevel = transform.position.y;
        if (isPlatformDetected)
        {
            allowMoving = false;
            StartCoroutine(StartResting());
        }
    }

    private IEnumerator StartEating()
    {
        float instantSpeed = 5f;
        float targetTolerance = 0.5f;

        if (isEating || targetItem == null) yield break;

        Vector3 itemPos = targetItem.GetItemPos();
        Vector3 targetPos = new Vector3(itemPos.x - xOffset, itemPos.y + yOffset);
        Vector3 returnPos = new Vector3(transform.position.x, originLevel);

        Debug.Log($"TargetItemPos: {targetPos}");

        rb.velocity = Vector3.zero;

        isFlyingToTarget = true;
        Vector3 direction = (targetPos - transform.position).normalized;
        while (Vector3.Distance(transform.position, targetPos) > targetTolerance)
        {
            rb.velocity = direction * instantSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        transform.position = targetPos;

        isEating = true;
        AnimController();

        yield return new WaitForSeconds(eatingPeriod);

        if (targetItem != null)
        {
            Destroy(targetItem.gameObject);
            targetItem = null;
        }

        isEating = false;
        AnimController();

        direction = (returnPos - transform.position).normalized;
        while (Vector3.Distance(transform.position, returnPos) > targetTolerance)
        {
            rb.velocity = direction * instantSpeed;
            yield return null;
        }

        isFlyingToTarget = false;
        rb.velocity = Vector3.zero;
        transform.position = returnPos;

        allowMoving = true;

    }
    private IEnumerator StartResting()
    {
        float instantSpeed = 5f;
        float targetTolerance = 0.3f;
        if (isRest || targetPlatform == null) yield break;
        Vector3 platformPos = targetPlatform.GetPlatformPos();
        Vector3 targetPos = new Vector3(platformPos.x - p_xOffset, platformPos.y + p_yOffset);
        Vector3 returnPos = new Vector3(transform.position.x, originLevel);
        Debug.Log($"TargetPlatformPos: {targetPos}");

        rb.velocity = Vector3.zero;

        isFlyingToTarget = true;
        Vector3 direction = (targetPos - transform.position).normalized;
        while (Vector3.Distance(transform.position, targetPos) > targetTolerance)
        {
            rb.velocity = direction * instantSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero;
        transform.position = targetPos;

        isRest = true;
        AnimController();

        yield return new WaitForSeconds(restPeriod);

        if (targetPlatform != null)
        {
            Collider2D collider2D = targetPlatform.GetComponent<Collider2D>();
            if (collider2D != null)
            {
                Destroy(collider2D);
            }
            targetPlatform = null;
        }

        isRest = false;
        AnimController();

        direction = (returnPos - transform.position).normalized;
        while (Vector3.Distance(transform.position, returnPos) > targetTolerance)
        {
            rb.velocity = direction * instantSpeed;
            yield return null;
        }

        isFlyingToTarget = false;
        rb.velocity = Vector3.zero;
        transform.position = returnPos;

        allowMoving = true;
    }


    private void Move()
    {
        rb.velocity = new Vector3(speed, 0, 0);
    }

    public void SetAllowMoving(bool allowMoving)
    {
        this.allowMoving = allowMoving;
    }

    private void SetInitialState()
    {
        float _angry = PlayerPrefs.GetFloat("angry", 0);
        if (_angry != 0)
        {
            isAngry = true;
        }
    }
}
