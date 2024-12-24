using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{

    private Enemy_Big_Cultist enemy_Big_Cultist;
    void Start()
    {
        enemy_Big_Cultist = GetComponentInParent<Enemy_Big_Cultist>();
    }

    private void OnTriggerDieEvent()
    {
        enemy_Big_Cultist.Die();
    }

    private void AttackPlayer()
    {
        enemy_Big_Cultist.PerformAttack();
    }

    private void TriggerOnAttackOver()
    {
        enemy_Big_Cultist.AttackOver();
    }

    private void TriggerOnGetHurtOver()
    {
        enemy_Big_Cultist.HurtAnimationOver();
    }
}
