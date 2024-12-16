using UnityEngine;

public class BossAreaLine : DetectLine
{
    protected override void Start()
    {
        base.Start();
        id = 9;
        isDetected = false;
    }

    protected override void Update()
    {
        base.Update();
        if (isDetected)
        {
            Destroy(gameObject);
        }
    }
}