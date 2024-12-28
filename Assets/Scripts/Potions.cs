using UnityEngine;

public class Potions : MonoBehaviour
{
    [SerializeField, ReadOnly] protected int id;
    private bool isGet;

    protected virtual void Awake()
    {
        isGet = false;
    }

    protected virtual void Update()
    {
        if (isGet)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void UpdateStatus(bool isGet)
    {
        this.isGet = isGet;
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int _id)
    {
        id = _id;
    }
}