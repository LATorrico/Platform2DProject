using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGuardController : MonoBehaviour
{
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float rayDownDistance;
    [SerializeField] private float rayForwardDistance;
    [SerializeField] private Transform checkWallPoint;
    [SerializeField] private Vector2 checkWallSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float waitTime;

    Rigidbody2D rb2D;
    bool checkDown;
    bool checkForward;
    bool checkPlayer;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGround();
        if (checkPlayer)
        {
            rb2D.velocity = Vector2.zero;
        }
        else
        {
            rb2D.velocity = new Vector2(moveSpeed, rb2D.velocity.y);
            if (!checkDown && !checkForward || checkDown && checkForward)
            {
                StartCoroutine("Patrolling");
            }
        } 
    }

    IEnumerator Patrolling()
    {
        rb2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(waitTime);
        Flip();
        StopCoroutine("Patrolling");
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
        moveSpeed *= -1f;
    }

    void CheckGround()
    {
        checkDown = Physics2D.Raycast(rayPoint.position, Vector2.down, rayDownDistance, groundLayer);
        checkForward = Physics2D.Raycast(rayPoint.position, new Vector2(transform.localScale.x, 0), rayForwardDistance, groundLayer);
        checkPlayer = Physics2D.Raycast(rayPoint.position, new Vector2(transform.localScale.x, 0), rayForwardDistance, playerLayer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayPoint.transform.position, rayPoint.transform.position + Vector3.down * rayDownDistance);
        Gizmos.DrawLine(rayPoint.transform.position, rayPoint.transform.position + new Vector3(transform.localScale.x * rayForwardDistance, 0));
    }
}