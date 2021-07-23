using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kapliczka : Unit
{
    public TurnManager turnManager;
    public override void Kill()
    {
        base.Kill();
        turnManager.WIN();
    }
}
