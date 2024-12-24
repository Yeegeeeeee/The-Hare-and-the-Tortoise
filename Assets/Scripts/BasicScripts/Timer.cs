using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    protected Slider slider;

    protected virtual void Start()
    {
        slider = GetComponent<Slider>();
    }

    protected virtual void Update()
    {

    }

    public virtual void ResetTimer()
    {
        slider.value = 0;
    }

    public virtual void SetTimer(float timeLeft)
    {
        slider.value = timeLeft;
    }
}
