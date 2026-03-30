using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Typer typer = null;
    public WordBank wordBank = null;
    public TimerBar timerBar = null;
    public List<LevelData> levels = new List<LevelData>();

    private bool hasFailed = false;
    private int currentLevelIndex = 0;

    private void StartLevel(int index){
        LevelData level = levels[index];

        wordBank.setWords(level.sentences);
        timerBar.duration = level.timeLimit;
    }

    private void NextLevel(){
        currentLevelIndex++;

        Debug.Log($"Loading level {currentLevelIndex}...");
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("Game Complete!");
            return;
        }

        StartLevel(currentLevelIndex);

    }

    private void Start()
    {   
        currentLevelIndex = 0;
        RestartLevel();
    }

    private void Update()
    {   
        checkTimer();
        if (hasFailed == true)
        {
            checkRestart();
        }
    }

    public void loadNextSentence()
    {   
        if (wordBank.isComplete())
        {   
            Debug.Log("Level Complete!");
            NextLevel();
        }
        string sentence = wordBank.getWord();
        typer.setCurrentSentence(sentence);
    }


    public void setFailed()
    {
        hasFailed = true;
    }

    public bool getFailed()
    {
        return hasFailed;
    }

    private void checkRestart()
    {   
        Debug.Log("failed - press R to restart");
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {   
            Debug.Log("Restarting game...");
            RestartLevel();
        }
    }

    private void checkTimer()
    {
        if (timerBar.isTimerExpired())
        {
            setFailed();
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting current level...");

        hasFailed = false;

        StartLevel(currentLevelIndex);   
        loadNextSentence();              
        timerBar.ResetTimer();
    }
}
