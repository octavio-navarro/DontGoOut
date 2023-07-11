using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_FSM : MonoBehaviour
{
    [SerializeField] float rayLength = 1.0f;
    [SerializeField] Transform targetPosition;

    Vector3 rayDirection = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDirection = targetPosition.position - transform.position;
        rayDirection = targetDirection.normalized;    
    }

    private void FixedUpdate() 
    {
        Physics2D.queriesHitTriggers = true;
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, rayDirection, rayLength, LayerMask.GetMask("Player", "Obstacles"));
        bool playerHit = false;

        if (hit.Length > 0)
        {
            foreach(RaycastHit2D h in hit)
            {
                Debug.Log(h.collider.name);
            }

            if(hit[0].collider.CompareTag("Player"))
            {
                playerHit = true;
            }
            else
            {
                playerHit = false;
            }
        }

        Debug.DrawRay(transform.position, rayDirection * rayLength, playerHit ? Color.green : Color.red);
    }
}
