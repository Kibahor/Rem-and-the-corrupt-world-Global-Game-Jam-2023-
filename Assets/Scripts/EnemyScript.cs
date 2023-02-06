using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 public enum EnemyState
    {
        NORMAL,
        PLAYER_DETECTED,
        DEAD
    }
public class EnemyScript : MonoBehaviour, IPausable
{
    [SerializeField]
    public GameObject leftPositionX;
    [SerializeField]
    public GameObject rightPositionX;
    public AudioSource audioInterro;
    public AudioSource audioAttack;
    public float EnemySpeed = 1;
    public float viewdistance = 3;
    private float viewdistanceExpension = .5f;
    private float offset = 0.05f;
    private EnemyState enemyState = EnemyState.NORMAL;
    private float timeToWaitForPlayer = 2f;
    private Animator anim;
    public float offseth = 0;

	public bool onPause = false;
    // Direction de base du monstre : le monstre commence � se d�placer � droite.
    private Vector2 direction = Vector2.right;
    IEnumerator WaitAndCheckPlayer()
    {
        // suspend execution for 2 seconds
        yield return new WaitForSeconds(timeToWaitForPlayer);
        RaycastHit2D[] rAll;
        rAll = Physics2D.RaycastAll(new Vector2(this.transform.position.x + offset, this.transform.position.y-offseth), direction, viewdistance);
        Debug.DrawRay(new Vector2(this.transform.position.x + offset, this.transform.position.y-offseth), new Vector2(direction.x * viewdistance, 0), Color.green, 1f);
        bool hasPlayerBeenFoundAgain = false;
        foreach (RaycastHit2D r in rAll)
        {
            PlayerControler p = null;
            Collider2D hit = ((Collider2D)r.collider);
            if (hit != null && hit.GetComponent(typeof(PlayerControler)) != null)
                p = (PlayerControler)hit.GetComponent(typeof(PlayerControler));
            if (p != null && p.detectable())
            {
				anim.SetBool("isAction", true); // Animation "!"
                if (audioAttack != null)
                {
                    audioAttack.Play();
                }
                p.restartLevel();
                enemyState = EnemyState.DEAD;
                hasPlayerBeenFoundAgain = true;
            }
		}
        if (!hasPlayerBeenFoundAgain)
        {

            Debug.Log("Enemy routine resumed.");
            enemyState = EnemyState.NORMAL;
			anim.SetBool("isSurprised", false);
		}
    }
    public void InitialisePause(IPauser pauser)
    {
        pauser.AddPausable(this);
    }

    public void OnPause()
    {
        onPause = true;
    }

    public void OnUnPause()
    {
        onPause = false;
    }

    void Start()
    {
        InitialisePause(GameEngine.Instance);
        offset += GetComponent<Collider2D>().bounds.size.x / 2;
		anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (onPause || enemyState == EnemyState.PLAYER_DETECTED || enemyState == EnemyState.DEAD)
        {
            return;
        }

        float moveDistance = EnemySpeed * Time.deltaTime;
        // Le monstre se d�place sur la droite
        if (direction.Equals(Vector2.right))
        {
           
            // Le monstre peut encore bouge � droite
            if (transform.position.x < rightPositionX.transform.position.x)
            {
                if (transform.position.x + moveDistance < rightPositionX.transform.position.x)
                {
                    transform.position = new Vector2(transform.position.x + moveDistance,transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(rightPositionX.transform.position.x,transform.position.y);
                }
            }
            else
            // Le monstre � d�j� atteint sa position max sur la droite
            {
                // TODO : CHANGER SENS ANIMATION ICI
                direction = Vector2.left;
                // j'inverse le c�t�
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                offset = -offset;
                viewdistanceExpension = -viewdistanceExpension;
            }
            
        }
        else 
        // le monstre se d�place sur la gauche
        {
            // Le monstre peut encore se d�placer sur la gauche
            if (transform.position.x > leftPositionX.transform.position.x)
            {
                if (transform.position.x - moveDistance > leftPositionX.transform.position.x)
                {
                    transform.position = new Vector2(transform.position.x - moveDistance, transform.position.y);
                }
                else
                {
                    transform.position = new Vector2(leftPositionX.transform.position.x, transform.position.y);
                }
            }
            // Le monstre ne peut plus se d�placer sur la gauche
            else 
            {
                // TODO : CHANGER SENS ANIMATION ICI
                direction = Vector2.right;
                // j'inverse le c�t�
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                offset = -offset;
                viewdistanceExpension = -viewdistanceExpension;
            }
        }
        RaycastHit2D[] rAll;
        rAll = Physics2D.RaycastAll(new Vector2(this.transform.position.x + offset, this.transform.position.y- offseth), direction, viewdistance);
        Debug.DrawRay(new Vector2(this.transform.position.x + offset, this.transform.position.y- offseth), new Vector2(direction.x * viewdistance, 0), Color.red, 0.001f);
        foreach (RaycastHit2D r in rAll)
        {
            Collider2D hit = ((Collider2D)r.collider);
            PlayerControler p = null;
            if (hit != null && hit.GetComponent(typeof(PlayerControler)) != null)
                p = (PlayerControler)hit.GetComponent(typeof(PlayerControler));
            if (p != null && p.detectable())
            {
                // Joueur trouv� !!!!!
                enemyState = EnemyState.PLAYER_DETECTED;
				Debug.Log("Player detected!");
				anim.SetBool("isSurprised", true); //Animation "?"
                if (audioInterro != null)
                {
                    audioInterro.Play();
                }
				StartCoroutine(WaitAndCheckPlayer());
            }
		}
    }
}

