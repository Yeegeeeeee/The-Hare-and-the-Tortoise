using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject Target;
    public int Smoothvalue = 2;
    public float PosY = 5;

    public Transform sea, tree;

    private float lastXPos;
    private bool start = false;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        lastXPos = transform.position.x;
    }

    void Update()
    {
        if (start)
        {
            FollowTarget();
        }

        float amount = transform.position.x - lastXPos;
        UpdateParallax(sea, amount, 1.0f);
        UpdateParallax(tree, amount, 0.7f);
        lastXPos = transform.position.x;
    }

    private void FollowTarget()
    {
        if (Target == null) return;
        float clampedY = Mathf.Max(Target.transform.position.y + PosY, -15);

        Vector3 targetPos = new Vector3(Target.transform.position.x, clampedY, -100);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / Smoothvalue);
    }

    private void UpdateParallax(Transform layer, float amount, float parallaxEffect)
    {
        layer.position += new Vector3(amount * parallaxEffect, 0, 0);
    }

    public void StartMovingCamera()
    {
        if (!start)
        {
            start = true;
            lastXPos = transform.position.x;
        }
    }

    public void StopMovingCamera()
    {
        start = false;
    }
}
