using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] private float shadowDelay;
    [SerializeField] private GameObject shadow;
    public bool isShadowActive = false;
    float shadowDelaySeconds;

    private void Start()
    {
        shadowDelaySeconds = shadowDelay;
    }

    private void Update()
    {
        if (isShadowActive)
        {
            if (shadowDelaySeconds > 0)
            {
                shadowDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentShadow = Instantiate(shadow, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentShadow.GetComponent<SpriteRenderer>().sprite = currentSprite;
                currentShadow.transform.localScale = this.transform.localScale;
                shadowDelaySeconds = shadowDelay;
                Destroy(currentShadow, 0.21f);
            }
        } 
    }
}
