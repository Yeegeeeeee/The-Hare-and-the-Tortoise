using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{

    [SerializeField] private MainPlayer mainPlayer;
    private Slider dashBar;

    void Start()
    {
        dashBar = GetComponent<Slider>();
    }

    void Update()
    {
        ChangeValue();
    }

    private void ChangeValue()
    {
        dashBar.value = 1 - mainPlayer.GetDashCooldownTimer() / mainPlayer.getDashCooldownTime();
    }
}