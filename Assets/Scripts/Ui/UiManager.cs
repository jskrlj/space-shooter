using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoSingleton<UiManager>
{
    private GameObject HUD;
    private Text ScoreHUD;
    private GameObject powerUp;

    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        HUD = transform.Find("HUD").gameObject;
        ScoreHUD = HUD.transform.Find("score").GetComponent<Text>();
        powerUp = HUD.transform.Find("powerup").gameObject;
    }

    public void ShowMenu(bool b) 
    {
        string trigger = (b) ? "show_menu" : "hide_menu";
        GetComponent<Animator>().SetTrigger(trigger);

        if(b)ShowPowerUp(false);
    }

    public void SetScore(int score) 
    {
        ScoreHUD.text = "Score " + score.ToString();
    }

    public void ShowPowerUp(bool b) 
    {
        powerUp.SetActive(b);
    }

    public void RestartPowerUp(float duration) 
    {
        ShowPowerUp(true);
        powerUp.GetComponent<PowerUpTimer>().Restart(duration);
        powerUp.GetComponent<PowerUpTimer>().PowerUpExpired += GameManager.Instance.PowerUpExpired;
    }
}
