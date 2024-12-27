using UnityEngine;

public class BottomDetectLine : DetectLine
{
    protected override void Start()
    {
        base.Start();
        id = 1;
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