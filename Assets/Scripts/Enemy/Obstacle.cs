using UnityEngine;
using System.Collections;

public class Obstacle : Enemy
{
    public float Delay { get { return delay; } set { delay = value; } }
    private float delay;
    private static float obstacleSpeed = -1.5f;
    [SerializeField]
    private GameObject obstacleParticlesPrefab;
    private float startScale;
    private const float OutSpeed = 2f;

    private void OnEnable()
    {
        base.EnemyDestroyed += (int dmg, Vector3 pos) =>
        {
            GameObject particles = Instantiate(obstacleParticlesPrefab);
            particles.transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, -1);
        };
    }

    private void Awake()
    {
        var randomRotation = Random.Range(0f, 360f);
        var randomScale = Random.Range(1f, 3f);

        transform.localScale = new Vector3(randomScale, randomScale, 1);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, randomRotation));
        GetComponent<Rigidbody2D>().angularDrag = 0;
        var angluarvelocity = Random.Range(-25f, 25f);
        GetComponent<Rigidbody2D>().angularVelocity = angluarvelocity;
    }


    protected override void HitByBullet(GameObject other)
    {
        base.HitByBullet(other);
        startScale = (startScale == 0) ? transform.localScale.x : startScale;
        transform.localScale = new Vector3(0.95f*startScale, 0.95f*startScale, 1);
    }

    private void Update()
    {
        if (delay < 0)
        {
            transform.position += new Vector3(0, obstacleSpeed * Time.deltaTime, 0);
        }
        else
        {
            delay -= Time.deltaTime;
        }

        if (transform.localScale.x < startScale) 
        {
            transform.localScale += new Vector3(Time.deltaTime * OutSpeed, Time.deltaTime * OutSpeed, 0);
        }
    }
}