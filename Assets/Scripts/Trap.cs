using UnityEngine;

public class Trap : MonoBehaviour
{

    protected int id;

    protected virtual void Start()
    {

    }

    public Vector3 GetPlatformPos()
    {
        return transform.position;
    }

    public int GetPlatformId()
    {
        return id;
    }
}
