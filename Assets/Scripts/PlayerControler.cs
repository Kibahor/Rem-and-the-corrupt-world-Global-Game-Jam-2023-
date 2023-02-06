using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum PlayerState
{
	TOPLANT=0,
    MOVING=1,
    PLANTING=2,
    PLANTED=3,
    REVIVING=4,
    WON=5,
    PAUSED=6,
	IDLE =7
}

public class PlayerControler : MonoBehaviour, IPausable
{
    [SerializeField]
    public float playerSpeed = 0;

    private Vector3 cameraPos;
    private AbstractGround currentGroundObject = null;

	private bool actionCompletedOnPlant = false;

    private Animator anim;
    private Rigidbody2D rb2D;
    private bool isFacingRight = true;
    private bool isInVine = false;
    private float gravityScale = 1f;
    // i don't know why this works but with 1 it doesn't. don't touch it our code is perfect :) don't even think about looking into it.
    private float randomVariable = 1.4f;
    private float secondFuckingRandomValueToAvoidThePlayerFromBeingStuck = 0f;
    private float upSpeed = 5f;
    private SpriteRenderer exclamation;
    

    private PlayerState currentState;
    private PlayerState pastState;

    private Vector2 initialPos;

    private void Start()
    {
        initialPos = transform.position;
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        InitialisePause(GameEngine.Instance);
        currentState = PlayerState.IDLE;
        pastState = PlayerState.IDLE;
        gravityScale = rb2D.gravityScale;
        if (this.gameObject.transform.GetChild(0) != null)
            exclamation = this.gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        if (exclamation != null)
            exclamation.color = new Color(255, 255, 255, 0);
    }

    public void winLevel()
    {
        if (currentState != PlayerState.PAUSED)
            changeState(PlayerState.WON);
    }
    IEnumerator RestartLevelScene()
    {
        // suspend execution for 5 seconds
        yield return new WaitForSeconds(2);
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public void restartLevel()
    {
        if (exclamation != null)
            exclamation.color = new Color(255, 255, 255, 255);
        if (currentState != PlayerState.PAUSED)
        {
            StartCoroutine(RestartLevelScene());
        }
    }

    public bool detectable()
    {
        return currentState != PlayerState.PLANTED && currentState != PlayerState.REVIVING && currentState != PlayerState.WON;
    }


    public void InitialisePause(IPauser pauser)
    {
        pauser.AddPausable(this);
    }

    public void OnPause()
    {
        changeState(PlayerState.PAUSED);
    }

    public void OnUnPause()
    {
        changeState(pastState);
    }

    private void changeState(PlayerState state)
    {
        if(currentState != state)
        {
            pastState = currentState;
            currentState = state;
			anim.SetInteger("State", (int)state);
        }
    }

    private Vector2 MovePlayer(float xVelocityComponent, float yVelocityComponent)
    {

        // Movement
        Vector2 moveGood = new Vector2(xVelocityComponent * playerSpeed, yVelocityComponent);
		Vector2 moveBad = new Vector2(xVelocityComponent * playerSpeed, yVelocityComponent + secondFuckingRandomValueToAvoidThePlayerFromBeingStuck);

		rb2D.MovePosition(rb2D.position + moveBad * Time.fixedDeltaTime);
		if(xVelocityComponent != 0)
			isFacingRight = xVelocityComponent > 0;
		if (moveGood.Equals(Vector2.zero))
        {

		}
		else
		{
			if(!isFacingRight)
				transform.localRotation = Quaternion.Euler(0, 180, 0);
			else
				 transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        return moveGood;
    }

	IEnumerator WaitAndChangeStateToPlanted()
	{
		// suspend execution for 5 seconds
		yield return new WaitForSeconds(2);
		currentGroundObject.doAction(gameObject);
		changeState(PlayerState.PLANTED);
	}

	IEnumerator WaitAndChangeStateToIdle()
	{
		// suspend execution for 5 seconds
		yield return new WaitForSeconds(1);
		changeState(PlayerState.IDLE);
	}

	//private bool IsGrounded()
	//{
	//    Debug.DrawRay(new Vector2(transform.position.x, -(GetComponent<BoxCollider2D>().bounds.size.y / 2) + transform.position.y), Vector2.down, Color.red);
	//    RaycastHit2D groundCheck = Physics2D.Raycast(new Vector2(transform.position.x, -(GetComponent<BoxCollider2D>().bounds.size.y/2) + transform.position.y), Vector2.down);

	//    return groundCheck != null && groundCheck.collider.CompareTag("Ground");
	//}

	void Update()
    {
        float GroundX;
        Debug.Log("inVine = " + isInVine);
        // sera remis en néga si on va sur la vine donc aucun soucis
        //rb2D.gravityScale = gravityScale;
        switch (currentState)
        {
            case PlayerState.IDLE:
            case PlayerState.MOVING:
                if(Input.GetButtonDown("Vertical") && isInVine)
                {
                    rb2D.gravityScale = -upSpeed;
                }else if (Input.GetButtonDown("Jump") && currentGroundObject != null)
                {
                    changeState(PlayerState.TOPLANT);
                }
                else
                {
                    if (MovePlayer(Input.GetAxisRaw("Horizontal"), 0).Equals(Vector2.zero))
                        changeState(PlayerState.IDLE);
                    else
                        changeState(PlayerState.MOVING);
                }
                break;
			case PlayerState.TOPLANT:
				// Move to center of ground
				GroundX = currentGroundObject.transform.position.x;
				if (System.Math.Abs(GroundX - transform.position.x) < 0.1)
				{
					changeState(PlayerState.PLANTING);
				}
				else
				{
					if (GroundX < transform.position.x)
						MovePlayer(Vector2.left.x / 2 * randomVariable, 0);
					else
						MovePlayer(Vector2.right.x / 2 * randomVariable, 0);
				}
				break;
            case PlayerState.PLANTING:
				if (pastState == PlayerState.PLANTED)
				{
					if (actionCompletedOnPlant)
					{
						StartCoroutine(WaitAndChangeStateToIdle());
						actionCompletedOnPlant = false;
					}
				}
				else
				{
					if (!actionCompletedOnPlant)
					{
						StartCoroutine(WaitAndChangeStateToPlanted());
						actionCompletedOnPlant = true;
					}
				}
                break;
            case PlayerState.PLANTED:
                if (Input.GetButtonDown("Jump") && currentGroundObject != null)
                {
                    changeState(PlayerState.PLANTING);
                }
                break;
            case PlayerState.REVIVING:
                transform.position = initialPos;
                changeState(PlayerState.IDLE);
                break;
            case PlayerState.WON:
                break;
            case PlayerState.PAUSED:
                return;
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.GetComponent(typeof(EnemyScript)) != null && detectable())
        {
            restartLevel();
        }

        var ground = collider2D.gameObject.GetComponent(typeof(AbstractGround));
        
        if (ground != null)
            currentGroundObject = (AbstractGround)ground;

        if (collider2D.CompareTag("Vine"))
        {
             isInVine = true;
        }

    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        var ground = collider2D.gameObject.GetComponents(typeof(AbstractGround));
        if (ground.Length != 0)
        {
            if (ground.Length > 1)
                Debug.LogError("There are multiple grounds colliding with the player during exit!");
            else
                currentGroundObject = null;
        }

        if (collider2D.CompareTag("Vine"))
        {
            isInVine = false;
            rb2D.gravityScale = gravityScale;
        }
    }
}
