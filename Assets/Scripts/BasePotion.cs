using UnityEngine;

public class BasePotion : Potions
{
    protected override void Awake()
    {
        base.Awake();
        id = 1;
    }

    protected override void Update()
    {
        base.Update();
    }
}