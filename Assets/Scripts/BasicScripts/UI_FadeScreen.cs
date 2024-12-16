using UnityEngine;

public class UI_FadeScreen : MonoBehaviour
{

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    public void FadeOut() => anim.SetTrigger("FadeOut");
    public void FadeIn() => anim.SetTrigger("FadeIn");
}
