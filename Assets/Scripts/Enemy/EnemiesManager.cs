using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoSingleton<EnemiesManager>
{
    [SerializeField]
    private GameObject EnemyPrefab;
    [SerializeField]
    private GameObject ObstaclePrefab;    

    private int choreographyDirectionX;
    private int previousChoreographyDirectionX;
    private int choreographyDirectionY;
    private float yDelta;
    private float choreographySpeed;
    private float choreographySpeedDown;
    private int powerUpEnemyId;

    private void InitVariables() 
    {
        transform.position = Vector3.zero;
        transform.position += new Vector3(0, 1, 0);
        choreographyDirectionX = 1;
        choreographyDirectionY = -1;
        choreographySpeed = 0.2f;
        choreographySpeedDown = 0.5f;
        yDelta = 0;
        previousChoreographyDirectionX = choreographyDirectionX;
        powerUpEnemyId = -1;
    }

    public void ClearEnemies() 
    {
        InitVariables();
        int remainingEnemies = transform.childCount;
        for (int i = 0; i < remainingEnemies; i++) 
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void SetChoreographySpeed(float s) 
    {
        choreographySpeed = 0;
    }
    
    private void Choreography0() 
    {
        // simple left right movement

        transform.position += new Vector3(choreographyDirectionX * Time.deltaTime * choreographySpeed, 0, 0);

        if (-0.3f > transform.position.x)   choreographyDirectionX = 1;
        if (transform.position.x > 0.3f)    choreographyDirectionX = -1;
    }

    private void Choreography1() 
    {
        // simple left right up and down movement
        if (yDelta > 0)
        {
            float yDistance = choreographyDirectionY * Time.deltaTime * choreographySpeedDown;
            transform.position += new Vector3(0,yDistance, 0);
            yDelta -= Mathf.Abs(yDistance);
            return;
        }

        transform.position += new Vector3(choreographyDirectionX * Time.deltaTime * choreographySpeed,0,0);

        if (-0.3f > transform.position.x) choreographyDirectionX = 1;        
        if (transform.position.x > 0.3f) choreographyDirectionX  = -1;        
        if (-4f > transform.position.y) choreographyDirectionY   = 1;        
        if (transform.position.y > 1.6f) choreographyDirectionY  = -1;        

        if (previousChoreographyDirectionX != choreographyDirectionX) yDelta = 1;

        previousChoreographyDirectionX = choreographyDirectionX;
    }

    public void TransitionEnemiesToScene2()
    {
        int d = 0;
        int xRandom = Random.Range(-5, 5);
        int yRandom = Random.Range(5, 10);
        int randomDirection = Random.Range(0, 2);

        foreach (Transform enemy in transform)
        {
            int enemyCount = d;
            //Transform currentEnemy = enemy;
            enemy.position += new Vector3(xRandom, 5+yRandom, 0);

            Vector3 startPoint   = enemy.position;
            Vector3 endPoint     = enemy.GetComponent<Enemy>().StartLocalPosition;
            Vector3 startControl = startPoint + new Vector3(5, 0, 0); 
            Vector3 endControl   = endPoint + new Vector3(-5, 0, 0);

            Vector3[] bezierDirection1 = new Vector3[] { startPoint, endControl, startControl, endPoint };
            Vector3[] bezierDirection2 = new Vector3[] { startPoint, startControl, endControl, endPoint };
            Vector3[] randomCurve = (randomDirection == 0) ? bezierDirection1 : bezierDirection2;

            LTBezierPath ltPath = new LTBezierPath(randomCurve);


            LeanTween.moveLocal(enemy.gameObject, ltPath, 2f)
                .setEaseOutSine()
                .setDelay(enemyCount * 0.1f);
            d++;
        }
    }

    private void TransitionEnemiesToScene()
    {
        int d = 0;
        foreach (Transform enemy in transform)
        {
            int enemyDelay = d;
            //Transform currentEnemy = enemy;
            enemy.position += new Vector3(0, 10, 0);
            LeanTween.move(enemy.gameObject, enemy.GetComponent<Enemy>().StartLocalPosition, 1f)
                .setEaseInOutSine()
                .setDelay(enemyDelay * 0.1f);
            d++;
        }
    }

    public void SpawnObstacles()
    {
        Vector3 topLeftScreen = Camera.main.ViewportToWorldPoint(new Vector3(0, 1));

        for (int i = 0; i < 20; i++)
        {
            GameObject obstacle = Instantiate(ObstaclePrefab, transform);
            obstacle.transform.position = new Vector3(topLeftScreen.x, topLeftScreen.y + 2f, 0);
            obstacle.transform.position += new Vector3(Random.Range(0f, 5f), 0, 0);
            obstacle.GetComponent<Obstacle>().Delay = i + Random.Range(0f, 2f);
            obstacle.GetComponent<Enemy>().SetHitsToKill(10);
            obstacle.GetComponent<Enemy>().EnemyDestroyed += EnemyDestroyed;
        }
        powerUpEnemyId = Random.Range(2, 20);
    }

    public void SpawnRandomEnemyShape() 
    {
        int randomShape = Random.Range(0, EnemyShapes.Shapes.Length);
        string[][] shape = EnemyShapes.Shapes[randomShape];
        int c = 0;
        for (int i = 0; i < shape.Length; i++)
        {
            for (int j = 0; j < shape[i].Length; j++)
            {
                if (shape[i][j].Length > 0) 
                {
                    c++;
                    SpawnEnemy(j,i,shape[i][j]);
                }
            }
        }
        powerUpEnemyId = Random.Range(2,c);
    }

    private void SpawnEnemy(int x, int y, string shape) 
    {
        float xDistance = .7f;
        float yDistance = .7f;
        Vector3 xOffset = new Vector3(7 * 0.7f * 0.5f, 0, 0);

        int isAggressive    = shape[0];
        char enemySkin      = shape[1];
        string hits         = shape.Substring(2);
        int hitsToKill = 1;
        int.TryParse(hits, out hitsToKill);

        GameObject enemy = Instantiate(EnemyPrefab, transform);
        enemy.transform.localPosition = new Vector3(xDistance * x, yDistance * y, 0) - xOffset;

        enemy.GetComponent<Enemy>().SetHitsToKill(hitsToKill);
        enemy.GetComponent<Enemy>().StartLocalPosition = enemy.transform.localPosition;
        enemy.GetComponent<Enemy>().EnemyDestroyed += EnemyDestroyed ;
        if (x == 0 && y == 0) enemy.GetComponent<BasicEnemy>().RandomizeSkins();
        enemy.GetComponent<BasicEnemy>().SetEnemySkin(enemySkin);
        enemy.GetComponent<BasicEnemy>().IsAggressive = isAggressive != 0;
    }

    private void EnemyDestroyed(int points, Vector3 position) 
    {
        GameManager.Instance.AddToCurrentScore(points);
        if (transform.childCount < 2)
        {
            GameManager.Instance.WaveCompleted();
        }
        if (transform.childCount == powerUpEnemyId) 
        {
            GameManager.Instance.SpawnPowerUp(position);
        }
    }

    private void Update()
    {
        Choreography1();
    }
}
