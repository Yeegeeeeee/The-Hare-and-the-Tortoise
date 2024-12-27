using UnityEngine;

public class HurtAnimation : MonoBehaviour
{
    public MainPlayer _player;
    public void OnHurtAnimationEnd()
    {
        bool _isHurt = false;
        _player.SetIsHurt(_isHurt);
        Debug.Log("Hurt animation ended, resetting isHurt: " + _isHurt);
    }
}