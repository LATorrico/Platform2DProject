using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDamage : MonoBehaviour
{
    [SerializeField] private int timeOnLightToDeath;

    float timer;
    int seconds;
    bool playerOnLight;


    private void Update()
    {
        if(playerOnLight)
        {
            // seconds in float
            timer += Time.deltaTime;
            // turn seconds in float to int
            seconds = (int)(timer % 60);
        }
        else
        {
            timer = 0;
        }

        if(seconds == timeOnLightToDeath)
        {
            Debug.Log("Muerte");
            //Player death
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnLight = true;
            
            //Player damage effect
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnLight = false;

            //Stop player damage effect
        }
    }
}