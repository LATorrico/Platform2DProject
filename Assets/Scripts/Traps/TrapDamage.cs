using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    GameObject player;
    PlayerLife playerLife;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerLife = player.GetComponent<PlayerLife>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player)
        {
            playerLife.Die();
        }
    }
}
