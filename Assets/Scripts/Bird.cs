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
    [SerializeField] private Tilemap itemTileMap;
    [SerializeField] private float eatingPeriod = 2f;
    private bool allowMoving = false;
    private Vector3 startPoint;
    private bool isAngry = false;
    private bool isEating = false;
    private bool isRest = false;
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
        if (allowMoving && !isEating && !isRest)
            Move();
        AnimController();
        CheckForItems();
    }

    private void AnimController()
    {
        anim.SetBool("isMoving", allowMoving);
        anim.SetBool("isEating", isEating);
    }

    private void DistanceCount()
    {
        float distance = rb.transform.position.x - startPoint.x;

    }

    private void DropTrap()
    {

    }

    private void CheckForItems()
    {
        if (isEating) return;

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
