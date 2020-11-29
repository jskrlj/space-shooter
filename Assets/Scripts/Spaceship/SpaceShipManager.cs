using System;
using UnityEngine;

public class SpaceShipManager : MonoSingleton<SpaceShipManager>
{
    public event Action HitByEnemy;
    private int currentWeapon;
    private const int DefaultBulletSpeed = 6;
    private const int DefaultBulletRateOverTime = 8;
    private Transform ship;
    private Transform weapons;
    [SerializeField]
    private const float PowerUpDuration = 15f;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        ship = transform.Find("ship");
        weapons = transform.Find("weapons");
    }

    private void ResetAllWeapons() 
    {
        foreach (Transform weapon in weapons)
        {
            foreach (Transform particleObject in weapon)
            {
                var ps = particleObject.GetComponent<ParticleSystem>();
                var emission = ps.emission;
                var main = ps.main;
                emission.rateOverTime = DefaultBulletRateOverTime;
                emission.enabled = false;
                main.startSpeed = DefaultBulletSpeed;
            }
        }
    }

    public void EnableWeapon(int type) 
    {
        ResetAllWeapons();
        
        currentWeapon = type;
        var weapon = weapons.GetChild(type);
        foreach (Transform ps in weapon)
        {
            var emission = ps.GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
        }
    }

    private void UpgradeShootSpeed() 
    {
        ResetAllWeapons();
        var weapon = weapons.GetChild(currentWeapon);
        foreach (Transform particlesObject in weapon)
        {
            var ps = particlesObject.GetComponent<ParticleSystem>();
            var emission = ps.emission;
            var main = ps.main;
            emission.rateOverTime = 12;
            main.startSpeed = 10;
            emission.enabled = true;
        }
    }

    private void PowerUpCollected(PowerUpGainType type) 
    {
        switch(type)
        {
            case PowerUpGainType.Canons:
                int newWeapon = Mathf.Clamp(currentWeapon + 1, 0, transform.childCount);
                EnableWeapon(newWeapon);
                break;
            case PowerUpGainType.Speed:
                UpgradeShootSpeed();
                break;
        }
        UiManager.Instance.RestartPowerUp(PowerUpDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.ToLower().StartsWith("powerup"))
        {
            PowerUpCollected(collision.gameObject.GetComponent<PowerUp>().PowerUpType);
            Destroy(collision.gameObject);
        }
        else 
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            HitByEnemy?.Invoke();
        }
    }
}
