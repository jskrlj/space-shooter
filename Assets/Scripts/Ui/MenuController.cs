using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameObject score;

    private void Awake()
    {
        score = transform.Find("Score").gameObject;
    }

    private void OnEnable()
    {
        SetHighScore();
    }

    private void SetHighScore() 
    {
        int s = PlayerPrefs.GetInt("highscore", 0);
        score.GetComponent<Text>().text = s.ToString();
    }

    public void PlayClicked() 
    {
        UiManager.Instance.ShowMenu(false);
        GameManager.Instance.StartGame();
    }
}
