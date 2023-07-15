using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterStates
{
    Idle, Search, Chase, Attack, NotActive
}

public class Monster_FSM : MonoBehaviour
{
    [SerializeField] float rayLength = 1.0f;
    [SerializeField] Transform targetPosition;
    [SerializeField] Transform originPosition;
    [SerializeField] public MonsterStates currentState = MonsterStates.Idle;
    [SerializeField] float waitTillNextChase = 2f, waitTillNextSearch = 3f, closeDistance = 7f;
    [SerializeField] Vector3 currentTarget;
    [SerializeField] float randomRadius = 3f;

    [SerializeField] AudioSource chaseSound;
    [SerializeField] AudioSource attackSound;

    Vector3 rayDirection = Vector3.right;

    NavMeshAgent agent;

    Animator animator;

    GameManager gameManager;

    SpriteRenderer spriteRenderer;

    public bool playerLos = false;
    public LampState lampState;

    IEnumerator waitSearchCoroutine = null, updateTargetCoroutine = null;

    // Start is called before the first frame update
    private void OnEnable() 
    {
        Debug.Log("Monster enabled");
        animator = GetComponent<Animator>();
        animator.SetInteger("State", (int)currentState);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        waitSearchCoroutine = null;
	   spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        updateTargetCoroutine = UpdateTarget();

        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("State", (int)currentState);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        waitSearchCoroutine = null;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        updateTargetCoroutine = UpdateTarget();

        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = targetPosition.position - originPosition.position;
        rayDirection = targetDirection.normalized;

        LookAtPlayer();

        switch(currentState)
        {
            case MonsterStates.Idle:                
                if(waitSearchCoroutine == null)
                {
                    waitSearchCoroutine = WaitTillNextChase();
                    StartCoroutine(waitSearchCoroutine);
                }
                break;

            case MonsterStates.Search:
                UpdateSearch();
                break;

            case MonsterStates.Chase:
                UpdateChase();
                break;
                
            case MonsterStates.Attack:
                UpdateAttack();
                break;

            case MonsterStates.NotActive:
                animator.SetInteger("State", (int)currentState);
                break;
        }

        animator.SetInteger("State", (int)currentState);
    }

    IEnumerator WaitTillNextChase()
    {
        yield return new WaitForSeconds(waitTillNextChase);
        currentState = MonsterStates.Search;
        agent.isStopped = false;
        StartCoroutine(updateTargetCoroutine);
    }

    IEnumerator UpdateTarget()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTillNextSearch);

            NavMeshHit hit = new NavMeshHit();
            Vector3 randomPoint = targetPosition.position + Random.insideUnitSphere * randomRadius;
            
            if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.GetAreaFromName("Walkable")))
                currentTarget = hit.position;
            else
                currentTarget = targetPosition.position + new Vector3(Random.Range(-randomRadius, randomRadius), Random.Range(-randomRadius, randomRadius), 0);

            agent.SetDestination(currentTarget);
        }
    }

    IEnumerator AttackPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        CharacterMotion characterMotion = player.GetComponent<CharacterMotion>();

        characterMotion.canMove = false;

        gameManager.playerStatus.TakeDamage(10);

        Vector2 newPosition = player.transform.position + rayDirection * 2f;

        while (Vector2.Distance(player.transform.position, newPosition) > 0.1f)
        {
            player.transform.position = Vector2.Lerp(player.transform.position, newPosition, 0.1f);

            yield return null;
        }
        
        characterMotion.canMove = true;
    }

    void UpdateSearch()
    {
        float distance = Vector3.Distance(transform.position, targetPosition.position);
        
        if(playerLos && distance <= closeDistance)
        {
            currentState = MonsterStates.Chase;
            chaseSound.Play();
            StopCoroutine(updateTargetCoroutine);
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(targetPosition.position);

        if(!playerLos)
        {
            currentState = MonsterStates.Search;
            StartCoroutine(updateTargetCoroutine);
        }
        else if(Vector3.Distance(transform.position, targetPosition.position) < 2.0f)
        {
            attackSound.Play();
            currentState = MonsterStates.Attack;
        }
    }

    void UpdateAttack()
    {
        waitSearchCoroutine = null;
        currentState = MonsterStates.Idle;

        StartCoroutine(AttackPlayer());
    }

    private void FixedUpdate() 
    {
        Physics2D.queriesHitTriggers = true;
        RaycastHit2D[] hit = Physics2D.RaycastAll(originPosition.position, rayDirection, rayLength, LayerMask.GetMask("Player", "Obstacles"));

        if (hit.Length > 0)
        {
            if(hit[0].collider.CompareTag("Player"))
            {
                playerLos = true;
            }
            else
            {
                playerLos = false;
            }
        }
        else
        {
            playerLos = false;
        }

        Debug.DrawRay(originPosition.position, rayDirection * rayLength, playerLos ? Color.green : Color.red);
    }

    void LookAtPlayer()
    {
        if (rayDirection.x >= 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
