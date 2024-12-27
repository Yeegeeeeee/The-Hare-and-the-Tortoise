using UnityEngine;

public class Item : MonoBehaviour
{

    protected int id;

    protected virtual void Start()
    {

    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public Vector3 GetItemPos()
    {
        return transform.position;
    }

    public int GetItemId()
    {
        return id;
    }
}
