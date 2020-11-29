using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private static float fallSpeed = -1.1f;
    public PowerUpGainType PowerUpType;
    [SerializeField]
    private Sprite[] Skins;

    public void SetType(PowerUpGainType type) 
    {
        PowerUpType = type;
        switch (PowerUpType) 
        {
            case PowerUpGainType.Canons:
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case PowerUpGainType.Speed:
                transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
        }
        //transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Skins[(int)PowerUpType];
    }

    void Update()
    {
        transform.position += new Vector3(0, fallSpeed * Time.deltaTime, 0);
    }
}

public enum PowerUpGainType 
{
    Canons,
    Speed
}
