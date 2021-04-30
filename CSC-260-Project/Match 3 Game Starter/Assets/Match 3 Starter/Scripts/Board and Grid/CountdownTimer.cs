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
    public Text addedTime;
    public Text comboTxt;
    public float fadeOutTime;
    public int combo = 1;
    public bool matchCleared = false;
    public string ComboNotif;




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
                //Debug.Log("Time's Up!");  // For testing the timer - JP
                timeRemaining = 0;
                timerIsRunning = false;
                GameManager.instance.gameOver = true;
		        gameOverPanel.SetActive(true);
                GUIManager.instance.GameOver();
            }

         
            // Stop the timer when the moves run out - JP
            if (GUIManager.instance.MoveCounter == 0)
            {
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

    void ComboNotication(int combo)
    {

     

        //comboTxt.text = string.Format("Combo\n150 x {d}", combo);

    }



    //We check every frame if the timer has expired and the text should disappear

    public IEnumerator ShowaddedTime(string message, float delay)
    {
        addedTime.text = message;
        addedTime.enabled = true;
        yield return new WaitForSeconds(delay);
        addedTime.enabled = false;
    }
    /*public IEnumerator ShowCombo(float delay)
    {
        comboTxt.text = string.Format("Combo!\n150x{0}", combo);
        comboTxt.enabled = true;
        yield return new WaitForSeconds(delay);
        comboTxt.enabled = false;
    } */
    public IEnumerator ShowCombo()
    {
        if (combo < 2) yield return comboTxt.enabled = false;
        comboTxt.text = string.Format("Combo!\n150x{0}", combo);
        if (combo >= 2) yield return comboTxt.enabled = true;
        //yield return new WaitUntil(() => match);
        //comboTxt.enabled = false; 

       
       
    }


}


