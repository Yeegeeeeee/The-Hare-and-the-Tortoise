using UnityEngine;

public class MainCamera : MonoBehaviour
{


    public static MainCamera Instance;

    public GameObject Target;
    public int Smoothvalue = 2;
    public float PosY = 5;

    public Transform sea, tree;

    private float lastXPos;

    // Use this for initialization
    public Coroutine my_co;

    private bool start = false;


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
        sea.position += new Vector3(amount, 0, 0);
        tree.position += new Vector3(amount * .7f, 0, 0);
        lastXPos = transform.position.x;
    }

    private void FollowTarget()
    {
        if (Target == null) return;
        Vector3 Targetpos = new Vector3(Target.transform.position.x, Target.transform.position.y + PosY, -100);
        transform.position = Vector3.Lerp(transform.position, Targetpos, Time.deltaTime * Smoothvalue);
    }

    public void StartMovingCamera()
    {
        start = true;
    }

    public void StopMovingCamera()
    {
        start = false;
    }
}
