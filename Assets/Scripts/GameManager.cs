using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameObject SpaceShipPrefab;
    [SerializeField]
    private GameObject PowerUpPrefab;
    private GameObject spaceShip;
    private int currentScore;
    private int roundsWithoutObstacles;
    private Transform powerups;

    public int CurrentScore { get { return currentScore; } }

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        currentScore = 0;
        Application.targetFrameRate = 60;
        powerups = transform.Find("powerups");
    }

    public void StartGame() 
    {
        roundsWithoutObstacles = Random.Range(2,6);
        currentScore = 0;
        UiManager.Instance.SetScore(currentScore);

        spaceShip = Instantiate(SpaceShipPrefab);
        spaceShip.GetComponent<SpaceShipManager>().HitByEnemy += GameOver;
        EnemiesManager.Instance.ClearEnemies();
        BackgroundManager.Instance.EnableMovement(true);
        ClearPowerUps();
        SpaceShipToStartPosition();
    }

    public void GameOver() 
    {
        int maxScore = PlayerPrefs.GetInt("highscore", 0);
        if (currentScore > maxScore) PlayerPrefs.SetInt("highscore", currentScore);
        UiManager.Instance.ShowMenu(true);
    }
 
    private void SpaceShipToStartPosition() 
    {
        Vector3 bottomCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, 10));
        spaceShip.transform.position = bottomCenter + new Vector3(0, -5f, 0);
        LeanTween.move(spaceShip, bottomCenter, 2f).setEaseInOutSine()
            .setOnComplete(()=> 
            {
                spaceShip.GetComponent<SpaceShipManager>().EnableWeapon(0);
                spaceShip.GetComponent<SpaceShipTouch>().enabled = true;
                SpawnNewWave();
                //SpawnNewObstacleWave();
            });
    }

    public void SpawnNewObstacleWave()
    {
        EnemiesManager.Instance.ClearEnemies();
        EnemiesManager.Instance.SetChoreographySpeed(0);
        EnemiesManager.Instance.SpawnObstacles();
    }

    public void SpawnNewWave() 
    {
        EnemiesManager.Instance.ClearEnemies();
        EnemiesManager.Instance.SpawnRandomEnemyShape();
        EnemiesManager.Instance.TransitionEnemiesToScene2();
    }

    public void SpawnPowerUp(Vector3 position) 
    {
        int randomPowerupType = Random.Range(0,2);
        var powerUp = Instantiate(PowerUpPrefab,powerups);
        PowerUpGainType randomType = (randomPowerupType == 0) ? PowerUpGainType.Canons : PowerUpGainType.Speed;
        powerUp.GetComponent<PowerUp>().SetType(randomType);
        powerUp.transform.position = position;
    }

    public void WaveCompleted() 
    {
        Debug.Log("Wave completed");
        roundsWithoutObstacles--;

        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            if (roundsWithoutObstacles < 1)
            {
                roundsWithoutObstacles = Random.Range(2, 6);
                SpawnNewObstacleWave();
            }
            else
            {
                SpawnNewWave();
            }
        }
    }

    public void PowerUpExpired() 
    {
        SpaceShipManager.Instance.EnableWeapon(0);
    }

    public void AddToCurrentScore(int points) 
    {
        currentScore += points;
        UiManager.Instance.SetScore(currentScore);
    }

    private void ClearPowerUps() 
    {
        int remainingPowerups = powerups.transform.childCount;
        for (int i = 0; i < remainingPowerups; i++)
        {
            Destroy(powerups.transform.GetChild(i).gameObject);
        }
    }

}
