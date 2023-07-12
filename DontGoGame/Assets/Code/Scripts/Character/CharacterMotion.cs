/*
Basic movement of a character on the XY plane

Gilberto Echeverria
2023-07-06
*/

using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class CharacterMotion : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform lamp;
    [SerializeField] Vector3 lampOffset;
    [SerializeField] Vector3 lampMovementMultiplier;

    Rigidbody2D rb2d;
    Animator animator;
    Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();        
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        // Normalize the vector to avoid faster diagonal movement
        move.Normalize();

        animator.SetFloat("move_x", move.x);
        animator.SetFloat("move_y", move.y);

        // Place the lamp in front of the player
        lamp.position = transform.position + lampOffset
                        + Vector3.Scale(move, lampMovementMultiplier);

        rb2d.velocity = move * speed;
    }
}
