using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    END_GROUND,
    VINE_GROUND,
    HIDE_GROUND
}

public abstract class AbstractGround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public abstract Type getType();
    public abstract void doAction(GameObject actioner);
}
