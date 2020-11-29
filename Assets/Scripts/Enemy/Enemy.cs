using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    public Vector3 StartLocalPosition { get; set; }
    public event Action<int, Vector3> EnemyDestroyed;
    private int hitsToKill;
    private int hitsRemaining;

    public void SetHitsToKill(int hits) 
    {
        hitsToKill = hits;
        hitsRemaining = hitsToKill;
    }

    private void OnParticleCollision(GameObject other)
    {
        HitByBullet(other);
    }

    protected virtual void HitByBullet(GameObject other) 
    {
        Debug.Log("particle collision");

        hitsRemaining--;

        if (hitsRemaining < 1)
        {            
            EnemyDestroyed?.Invoke(hitsToKill, transform.position);
            Destroy(gameObject);
        }

        //var part = other.GetComponent<ParticleSystem>();
        //int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        //int i = 0;
        //while (i < numCollisionEvents)
        //{
        //    Debug.Log("particle collision");
        //    i++;
        //}
    }
}




