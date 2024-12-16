using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCooldown_Potion : Potion
{
    protected override void Start()
    {
        base.Start();
        isGet = false;
        id = 3;
    }

    protected override void Update()
    {
        base.Update();
    }
}
