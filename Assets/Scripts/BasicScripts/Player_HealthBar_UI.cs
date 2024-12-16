using UnityEngine;
using UnityEngine.UI;

public class Player_HealthBar_UI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Player player;
    private Slider slider;






    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        player = GetComponentInParent<Player>();
        slider = GetComponentInChildren<Slider>();
        player.onFilpped += FlipUI;
    }

    private void Update()
    {
        UpdateHealthUI();
    }



    private void FlipUI()
    {
        rectTransform.Rotate(0, 180, 0);
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = player.MaxHP;
        slider.value = player.getCurrentHp();
    }

    private void OnDisalbe()
    {
        player.onFilpped -= FlipUI;
    }
}
