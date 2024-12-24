using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash_Potion : Potion
{
    protected override void Start()
    {
        base.Start();
        isGet = false;
        id = 2;
    }

    protected override void Update()
    {
        base.Update();
    }
}


