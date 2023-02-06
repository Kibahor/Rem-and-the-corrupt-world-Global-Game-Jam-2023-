using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelGroundScript : AbstractGround
{
    [SerializeField]
    public string scene;
    
    public override void doAction(GameObject actioner)
    {
        GameEngine.Instance.WinLevel(scene);
    }

    public override Type getType()
    {
        return Type.END_GROUND;
    }
}
