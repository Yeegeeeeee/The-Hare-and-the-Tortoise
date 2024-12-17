using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;
    [Header("Move Info")]
    [SerializeField] private float speed;
    private bool allowMoving = false;
    private Vector3 startPoint;
    private bool isAngry = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPoint = rb.transform.position;
        SetInitialState();
    }

    // Update is called once per frame
    void Update()
    {
        if (allowMoving)
            Move();
        AnimController();
    }

    private void AnimController()
    {
        anim.SetBool("isMoving", allowMoving);
    }

    private void DistanceCount()
    {
        float distance = rb.transform.position.x - startPoint.x;
        switch (distance)
        {


        }
    }

    private void DropPotion()
    {

    }

    private void DropTrap()
    {

    }

    private void DropBlock()
    {

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
