using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineGroundScript : AbstractGround, IPausable
{
    [SerializeField]
    public GameObject vine;
    [SerializeField]
    public float toTransform;
    private bool actionTaken = false;
    private bool pause = false;
    // Start is called before the first frame update
    void Start()
    {   
        
    }


    // Update is called once per frame
    void Update()
    {
        if(actionTaken && !pause){
            if (toTransform>0){
                float transformValue = 0f;
                // Prevent the prefab to go too far up when game is freezing.
                if (Time.deltaTime > toTransform)
                {
                    transformValue = toTransform ;
                }
                else
                {
                    transformValue = Time.deltaTime;
                }
                vine.transform.position = new Vector3(vine.transform.position.x, vine.transform.position.y + transformValue, vine.transform.position.z);
                toTransform-=transformValue;
            }
        }
    }

    public override Type getType(){
        return Type.VINE_GROUND;
    }
    
    public override void doAction(GameObject actioner)
    {
        actionTaken = true;
    }

    public void InitialisePause(IPauser pauser)
    {
        pauser.AddPausable(this);
    }

    public void OnPause()
    {
        pause = true;
    }

    public void OnUnPause()
    {
        pause = false;
    }
}
