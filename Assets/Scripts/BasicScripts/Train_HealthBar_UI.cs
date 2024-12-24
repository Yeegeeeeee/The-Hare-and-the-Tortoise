using UnityEngine;
using UnityEngine.UI;

public class Train_HealthBar_UI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Train_Twisted_Cultist train_Twisted_Cultist;
    private Slider slider;






    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        train_Twisted_Cultist = GetComponentInParent<Train_Twisted_Cultist>();
        slider = GetComponentInChildren<Slider>();
        train_Twisted_Cultist.onFilpped += FlipUI;
    }

    private void Update()
    {
        UpdateHealthUI();
    }



    private void FlipUI()
    {
        rectTransform.Rotate(0, 180, 0);
        rectTransform.position = new Vector2(rectTransform.position.x + 1.3f, rectTransform.position.y);
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = train_Twisted_Cultist.MaxHP;
        slider.value = train_Twisted_Cultist.getCurrentHp();
    }

    private void OnDisalbe()
    {
        train_Twisted_Cultist.onFilpped -= FlipUI;
    }
}
