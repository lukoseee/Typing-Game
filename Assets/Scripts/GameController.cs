using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Typer typer = null;
    public WordBank wordBank = null;
    public TimerBar timerBar = null;
    public List<LevelData> levels = new List<LevelData>();
    public Text levelDisplay = null;
    public CandleController candleController = null;
    public Desk desk = null;
    public Boy boy = null;
    public CameraFollow mainCamera = null;

    private bool hasFailed = false;
    private int currentLevelIndex = 0;
    private int currentCandleIndex = 0;

    private void StartLevel(int index){
        
        boy.ResetPosition();
        LevelData level = levels[index];

        wordBank.setWords(level.sentences);
        currentCandleIndex = 0;

        candleController.SpawnCandles(wordBank.wordCount());

        candleController.ResetCandles();
        
        timerBar.duration = level.timeLimit;
        levelDisplay.text = $"{index + 1}";

        desk.ResizeDesk(wordBank.wordCount(), candleController.spacing);
        mainCamera.ResetPosition();
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
        StartCoroutine(candleController.BlowOutAll());
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

    public void incCurrentCandleIndex()
    {
        
        currentCandleIndex++;
    }

    public int getCurrentCandleIndex()
    {
        return currentCandleIndex;
    }
}
