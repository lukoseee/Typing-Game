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
    public AudioPlayer audioPlayer = null;
    public EndLevelPopup endLevelPopup = null;

    private bool isBlowingOut = false;

    private bool hasFailed = false;
    private int currentLevelIndex = 0;
    private int currentCandleIndex = 0;
    private bool hasFailedAlready = false;

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

    public void NextLevel(){
        currentLevelIndex++;

        Debug.Log($"Loading level {currentLevelIndex}...");
        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("Game Complete!");
            return;
        }

        audioPlayer.PlayMusic(levels[currentLevelIndex]);
        audioPlayer.resetSFX();
        StartLevel(currentLevelIndex, false);

    }

    private void Start()
    {   
        currentLevelIndex = 0;
        StartLevel(currentLevelIndex, false);
        audioPlayer.PlayMusic(levels[currentLevelIndex]);
        loadNextSentence();
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
            endLevelPopup.ShowWin();
        }
        string sentence = wordBank.getWord();
        typer.setCurrentSentence(sentence);
    }


    public void setFailed()
    {
        hasFailed = true;
        audioPlayer.failSFX();
        timerBar.pauseTimer();
        endLevelPopup.ShowFail();

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
            restartMessage.Show();
        }

        Debug.Log("failed - press R to restart");
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {   
            Debug.Log("Restarting game...");
            restartMessage.Hide();
            RestartLevel();
        }
    }

    private void checkTimer()
    {
        if (timerBar.isTimerExpired() && !hasFailedAlready)
        {   
            hasFailedAlready = true;
            setFailed();
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting current level...");

        audioPlayer.resetSFX();
        hasFailed = false;
        hasFailedAlready = false;
        isBlowingOut = false;
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
