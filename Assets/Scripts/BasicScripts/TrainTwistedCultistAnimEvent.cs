using UnityEngine;

public class TrainTwistedCultistAnimEvent : MonoBehaviour
{
    private Train_Twisted_Cultist trainRobot;

    void Start()
    {
        trainRobot = GetComponentInParent<Train_Twisted_Cultist>();
    }

    private void TriggerOnHurtOver()
    {
        trainRobot.HurtAnimationOver();
    }

    private void OnTriggerDieEvent()
    {
        trainRobot.Die();
    }

    private void TriggerOnTransformOver()
    {
        trainRobot.UpdateTransform();
    }
}
