using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimEvent : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void AttackAnimationTrigger()
    {
        player.AttackOver();
    }

    private void OnTriggerDieEvent()
    {
        SceneManager.LoadScene("DiedScene");
        player.Die();
    }

    private void AttackEnemy()
    {
        player.PerformAttack();
    }
    private void TriggerOnGetHurt()
    {
        player.HurtAnimationOver();
    }
}
