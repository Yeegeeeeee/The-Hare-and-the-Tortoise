using UnityEngine;

public class Potion : MonoBehaviour
{

    public Enemy_Big_Cultist Target;
    protected bool isGet;
    private Animator anim;
    protected int id;
    private SpriteRenderer sp;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Target.SetGameObject(gameObject);
        sp = GetComponentInChildren<SpriteRenderer>();
        sp.enabled = false;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isGet)
        {
            Destroy(gameObject);
            return;
        }
        AnimatorController();
        StickToTarget();
    }

    private void AnimatorController()
    {
        anim.SetBool("isGet", isGet);
    }

    public void UpdateStatus(bool isGet)
    {
        this.isGet = isGet;
    }

    private void StickToTarget()
    {
        if (Target != null)
        {
            transform.position = Target.transform.position;
        }
    }

    public int GetId()
    {
        return id;
    }
}
