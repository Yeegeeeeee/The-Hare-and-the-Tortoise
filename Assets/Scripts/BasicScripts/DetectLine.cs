using UnityEngine;

public class DetectLine : MonoBehaviour
{
    protected bool isDetected;

    protected int id;

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    public void UpdateStatus(bool status)
    {
        isDetected = status;
    }

    public int GetId()
    {
        return id;
    }
}