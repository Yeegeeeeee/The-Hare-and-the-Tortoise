using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimEvent : MonoBehaviour
{
    private Enemy_Cultist_Assassin boss;
    private Player player;

    void Start()
    {
        boss = GetComponentInParent<Enemy_Cultist_Assassin>();
        player = boss.GetPlayer();
    }

    private void OnTriggerDieEvent()
    {
        player.PlayVictory();
        boss.Die();
    }

    private void AttackPlayer()
    {
        boss.PerformAttack();
    }

    private void AttackAnimationTrigger()
    {
        boss.AttackOver();
    }

    private void TriggerVanish()
    {
        boss.Vanish();
    }

    private void TriggerReappear()
    {
        boss.Reappear();
    }

    private void TriggerOnHurtAnimationOver()
    {
        boss.HurtAnimationOver();
    }
}
