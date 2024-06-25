using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TextMeshProUGUI killCounter;
    public TextMeshProUGUI healthCounter;
    public TextMeshProUGUI timerText;
    public GameObject overScreen;
    public GameObject completeScreen;
    public GameObject player;
    public GameObject scrope;

    public int numKillLevel = 1;


    public bool isGameActive = false;
    public float timerValue = 60f;
    public Button restartButton;

    private bool isGameComplete = false;

    [HideInInspector]
    public int killCount;
    public int healthCount;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // N?u ?ã t?n t?i m?t UIManager khác, h?y b? UIManager hi?n t?i
            Destroy(gameObject);
            return;
        }

        // Ki?m tra và gán giá tr? cho killCounter và healthCounter
        if (killCounter == null)
        {
            killCounter = GameObject.Find("KillCounter").GetComponent<TextMeshProUGUI>();
            if (killCounter == null)
            {
                Debug.LogError("KillCounter not found or assigned in UIManager!");
            }
        }

        if (healthCounter == null)
        {
            healthCounter = GameObject.Find("HealthCounter").GetComponent<TextMeshProUGUI>();
            if (healthCounter == null)
            {
                Debug.LogError("HealthCounter not found or assigned in UIManager!");
            }
        }

        // Kh?i t?o giá tr? ban ??u cho killCount và healthCount
        killCount = 0;
        healthCount = 0;
    }

     void Update()
    {
        if (!isGameComplete)
        {
            if (timerValue > 0)
            {
                timerValue -= Time.deltaTime;
                DisplayTime();
            }
            else
            {
                player.gameObject.SetActive(false);
                timerValue = 0;
                timerText.text = "00 : 00";
                GameOver();
            }
        }
            


         if(killCount >= numKillLevel)
        {
            GameComplete();
            isGameComplete = true; 
        }
        if (player == null)
        {
            isGameComplete = true;
        }
       
    }

    public void DisplayTime()
    {
        float minutes = Mathf.FloorToInt(timerValue / 60);
        float seconds = Mathf.FloorToInt(timerValue % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateKillCounter()
    {
        if (killCounter != null)
        {
            killCounter.text = " " + killCount.ToString();
        }
    }

    public void UpdateHealthCounter()
    {
        if (healthCounter != null)
        {
            healthCounter.text = healthCount.ToString();
        }
    }

    public void GameOver()
    {
        overScreen.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GameComplete()
    {
        completeScreen.gameObject.SetActive(true);
        player.gameObject.SetActive(false);
        scrope.gameObject.SetActive(false); 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Restart game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void BackGame()
    {
        SceneManager.LoadScene("Menu");
    }


 
}
