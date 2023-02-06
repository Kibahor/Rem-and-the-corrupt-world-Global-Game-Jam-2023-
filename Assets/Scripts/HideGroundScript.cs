using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGroundScript : AbstractGround
{
    public override void doAction(GameObject actioner)
    {
       
    }

    public override Type getType()
    {
        return Type.HIDE_GROUND;
    }
}
