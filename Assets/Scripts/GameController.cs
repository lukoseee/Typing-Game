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
    public RestartMessage restartMessage = null;

    private bool isBlowingOut = false;

    private bool hasFailed = false;
    private int currentLevelIndex = 0;
    private int currentCandleIndex = 0;

    private void StartLevel(int index , bool isRestart){
        
        boy.ResetPosition();
        LevelData level = levels[index];

        wordBank.setWords(level.sentences);
        currentCandleIndex = 0;

        candleController.SpawnCandles(wordBank.wordCount());

        candleController.ResetCandles();
        
        timerBar.duration = level.timeLimit;
        timerBar.ResetTimer();
        timerBar.pauseTimer();

        levelDisplay.text = $"{index + 1}";

        if(!isRestart){

            restartMessage.SetMessage(level.message);
            StartCoroutine(restartMessage.FadeInAndOut());

        }

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

        StartLevel(currentLevelIndex, false);

    }

    private void Start()
    {   
        currentLevelIndex = 0;
        StartLevel(currentLevelIndex, false);
        loadNextSentence();
    }

    private void Update()
    {   
        checkTimer();

        if (hasFailed == true)
        {   timerBar.pauseTimer();
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

        if (!isBlowingOut)
        {
            isBlowingOut = true;
            mainCamera.StartRewind();
            StartCoroutine(candleController.BlowOutAll());
            restartMessage.SetMessage("Press [Shift] to Restart");
            StartCoroutine(restartMessage.FadeIn());
        }

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
        isBlowingOut = false;
        restartMessage.Hide();

        StartLevel(currentLevelIndex, true);   
        loadNextSentence();     
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
