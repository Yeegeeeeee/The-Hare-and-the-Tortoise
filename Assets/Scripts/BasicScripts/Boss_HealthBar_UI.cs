using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private RectTransform rectTransform;
    private Enemy_Cultist_Assassin enemy_Cultist_Assassin;
    private Slider slider;






    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        rectTransform = GetComponent<RectTransform>();
        enemy_Cultist_Assassin = GetComponentInParent<Enemy_Cultist_Assassin>();
        slider = GetComponentInChildren<Slider>();

        entity.onFilpped += FlipUI;

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
        slider.maxValue = enemy_Cultist_Assassin.MaxHP;
        slider.value = entity.getCurrentHp();
    }

    private void OnDisalbe()
    {
        entity.onFilpped -= FlipUI;
    }
}
