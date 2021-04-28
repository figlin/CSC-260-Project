using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer instance;

    public GameObject gameOverPanel;
    public float timeRemaining = 60f;
    public bool timerIsRunning = false;
    public Text timeText;

    private void Start()
    {
        // Starts the timer automatically
        timerIsRunning = true;
        instance = GetComponent<CountdownTimer>();
        
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                //Debug.Log("Time's Up!");
                timeRemaining = 0;
                timerIsRunning = false;
                GameManager.instance.gameOver = true;
		        gameOverPanel.SetActive(true);
                GUIManager.instance.GameOver();
            }
        }
    }
    
    void DisplayTime(float timeToDisplay)
    {
        
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
    }
    
}
