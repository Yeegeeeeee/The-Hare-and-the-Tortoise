using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_HealthBar_UI : MonoBehaviour

{

    private RectTransform rectTransform;
    private Enemy_Big_Cultist enemy_Big_Cultist;
    private Slider slider;






    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        enemy_Big_Cultist = GetComponentInParent<Enemy_Big_Cultist>();
        slider = GetComponentInChildren<Slider>();
        enemy_Big_Cultist.onFilpped += FlipUI;
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
        slider.maxValue = enemy_Big_Cultist.MaxHP;
        slider.value = enemy_Big_Cultist.getCurrentHp();
    }

    private void OnDisalbe()
    {
        enemy_Big_Cultist.onFilpped -= FlipUI;
    }

}
