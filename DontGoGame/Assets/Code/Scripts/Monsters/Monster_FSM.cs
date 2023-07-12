using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_FSM : MonoBehaviour
{
    enum States
    {
        Idle, Search, Chase, Attack
    }

    [SerializeField] float rayLength = 1.0f;
    [SerializeField] Transform targetPosition;
    [SerializeField] Transform originPosition;
    [SerializeField] States currentState = States.Idle;
    [SerializeField] float waitTillNextChase = 2f, waitTillNextSearch = 3f, closeDistance = 7f;
    [SerializeField] Vector3 currentTarget;
    Vector3 rayDirection = Vector3.right;

    NavMeshAgent agent;

    Animator animator;

    bool playerLos = false, close = false;

    IEnumerator waitSearchCoroutine = null, updateTargetCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("State", (int)currentState);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        updateTargetCoroutine = UpdateTarget();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = targetPosition.position - originPosition.position;
        rayDirection = targetDirection.normalized;

        switch(currentState)
        {
            case States.Idle:
                if(waitSearchCoroutine == null)
                {
                    waitSearchCoroutine = WaitTillNextChase();
                    StartCoroutine(waitSearchCoroutine);
                }
                break;

            case States.Search:
                UpdateSearch();
                break;

            case States.Chase:
                UpdateChase();
                break;
                
            case States.Attack:
                UpdateAttack();
                break;
        }

        animator.SetInteger("State", (int)currentState);
    }

    IEnumerator WaitTillNextChase()
    {
        yield return new WaitForSeconds(waitTillNextChase);
        currentState = States.Search;
        StartCoroutine(updateTargetCoroutine);
    }

    IEnumerator UpdateTarget()
    {
        while(true)
        {
            yield return new WaitForSeconds(waitTillNextSearch);
            currentTarget = targetPosition.position;
            agent.SetDestination(currentTarget);
        }
    }

    void UpdateSearch()
    {
        float distance = Vector3.Distance(transform.position, targetPosition.position);
        
        if(playerLos && distance <= closeDistance)
        {
            currentState = States.Chase;
            StopCoroutine(updateTargetCoroutine);
        }
    }

    void UpdateChase()
    {
        agent.SetDestination(targetPosition.position);

        if(!playerLos)
        {
            currentState = States.Search;
            StartCoroutine(updateTargetCoroutine);
        }
        else if(Vector3.Distance(transform.position, targetPosition.position) < 1.0f)
            currentState = States.Attack;
    }

    void UpdateAttack()
    {
        Debug.Log("Attack");
        waitSearchCoroutine = null;
        currentState = States.Idle;
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
}
