using UnityEngine;
using System.Collections;
using System;

public class PowerUpTimer : MonoBehaviour
{
    public event Action PowerUpExpired;
    private Transform status;
    private float duration;
    private float secondsRemaining;

    private void Awake()
    {
        status = transform.GetChild(0);
    }

    public void Restart(float duration) 
    {
        status.localScale = Vector3.one;
        this.duration = duration;
        secondsRemaining = this.duration;
    }

    void Update()
    {
        if (secondsRemaining > 0)
        {
            float newScale = secondsRemaining / duration;
            status.localScale = new Vector3(newScale, 1, 1);
            secondsRemaining -= Time.deltaTime;
        }
        else 
        {
            if (duration != 0) 
            {
                PowerUpExpired?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
