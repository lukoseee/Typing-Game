using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private Typer typer = null;
    [SerializeField] private WordBank wordBank = null;
    [SerializeField] private TimerBar timerBar = null;
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] private Text levelDisplay = null;
    [SerializeField] private CandleController candleController = null;
    [SerializeField] private Desk desk = null;
    [SerializeField] private Boy boy = null;
    [SerializeField] private CameraFollow mainCamera = null;
    [SerializeField] private RestartMessage restartMessage = null; //title of each level
    [SerializeField] private AudioPlayer audioPlayer = null;
    [SerializeField] private EndLevelPopup endLevelPopup = null;
    [SerializeField] private TMP_Text wordOutput = null; //where sentences come up
    [SerializeField] private TextFade textFade = null;
    public Stopwatch stopwatch = null;

    private bool isBlowingOut = false;
    private bool hasFailed = false;
    private int currentLevelIndex = 0;
    private int currentCandleIndex = 0;
    private bool hasFailedAlready = false;
    private int mistakesRemaining = 0;
    private int sacredPauses = 0;

    private Coroutine fadeCoroutine;
    private RectTransform textRectTransform;
    private Coroutine fadeWordsCoroutine;

    private void StartLevel(int index ){
        
        LevelData level = getCurrentLevelData();

        //stop fading words
        StopAllVisualChallenges();
        
        //stop blow out 
        candleController.StopBlowOut();
        
        //stop restart message fade 
        restartMessage.StopFade();

        //start visual challenge based on level
        switch (level.visualChallenge)
        {
            case VisualChallenge.FadingSentences:
                fadeCoroutine = StartCoroutine(textFade.FadeSentencesRoutine());
                break;
            case VisualChallenge.FadingWords:
                fadeWordsCoroutine = StartCoroutine(textFade.FadeWordsRoutine());
                break;
            default:
                break;
        }

        //apply upgrades 
        ApplyForgivingFlame();
        ApplySacredPause();

        //resets
        typer.resetCharCount();
        boy.ResetPosition();

        wordBank.setWords(level.sentences);
        currentCandleIndex = 0;

        candleController.SpawnCandles(wordBank.wordCount());

        //all candles unlit
        candleController.ResetCandles();
        
        //set WPM time limit based on level
        timerBar.SetDuration(level.timeLimit);

        //timer for recording WPM score
        stopwatch.ResetTimer();

        //reset WPM timer and puse until first key press
        timerBar.ResetTimer();
        timerBar.pauseTimer();

        //level HUD at the top
        levelDisplay.text = $"{index + 1}";

        //level message fades in
        StartMessageFadeIn(level.message);

        desk.ResizeDesk(wordBank.wordCount(), candleController.GetSpacing());
        mainCamera.ResetPosition();
        typer.ResetFirstKeyPress();
    }

    private void StartMessageFadeIn(string message)
    {
        restartMessage.SetMessage(message);
        restartMessage.StartFadeInAndOut();
    }

    public void NextLevel(){
        currentLevelIndex++;

        Debug.Log($"Loading level {currentLevelIndex}...");
        if (currentLevelIndex >= levels.Count)
        {   
            //game finished
            restartMessage.SetMessage("Thanks for playing!");
            restartMessage.Show();
            Debug.Log("Game Complete!");
            return;
        }
        
        audioPlayer.PlayMusic(levels[currentLevelIndex]);
        audioPlayer.resetSFX();
        StartLevel(currentLevelIndex);
        loadNextSentence();

    }

    private void Start()
    {   
        foreach (LevelData level in levels)
        {   
            //reset grace and rank earned for each level at start of game
            level.ResetForNewGame();
        }

        textRectTransform = wordOutput.GetComponent<RectTransform>();
        currentLevelIndex = 9;
        
        StartLevel(currentLevelIndex);
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

    
    private void ApplyForgivingFlame()
    {   
        if (Upgrades.Instance.IsPowerupEquipped(PowerupType.ForgivingFlame))
        {
            mistakesRemaining = Upgrades.Instance.GetPowerupLevel(PowerupType.ForgivingFlame); 
        }
    }

    private void ApplySacredPause(){
        if (Upgrades.Instance.IsPowerupEquipped(PowerupType.SacredPause))
        {
            sacredPauses = Upgrades.Instance.GetPowerupLevel(PowerupType.SacredPause);
        }
    }

    public bool SacredPause(){
        if ( sacredPauses <= 0) return false;

        return true;
    }

    public void UseSacredPause(){
        if (sacredPauses > 0)
        {
            sacredPauses--;
        }
    }

    public bool ShouldIgnoreMistake()
    {   
        if (mistakesRemaining > 0)
        {
            mistakesRemaining--;
            return true;
        }

        return false;
    }

    public void loadNextSentence()
    {   
        if (wordBank.isComplete())
        {   
            timerBar.pauseTimer();
            Debug.Log("Level Complete!");
            endLevelPopup.ShowWin();
            return;
        }
        string sentence = wordBank.getWord();

        LevelData level = getCurrentLevelData();
        //apply visual challenge parsing based on level
        if (level.visualChallenge == VisualChallenge.MissingLetters)
        {
            sentence = wordBank.ParseMaskedSentence(sentence, level.maskMarker);
            wordBank.ClearFadeMask();
        }
        else if (level.visualChallenge == VisualChallenge.FadingWords)
        {
            sentence = wordBank.ParseFadeSentence(sentence, level.fadeWordMarker);
            wordBank.ClearMask();
        }
        else
        {
            wordBank.ClearMask();
            wordBank.ClearFadeMask();
        }

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
        //start blow out and rewind
        if (!isBlowingOut)
        {
            isBlowingOut = true;
            mainCamera.StartRewind();
            candleController.StartBlowOutAll();
        }

    }
    
    //check if timer ran out 
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
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);  
        }

        Debug.Log("Restarting current level...");

        audioPlayer.resetSFX();
        hasFailed = false;
        hasFailedAlready = false;
        isBlowingOut = false;
        StartLevel(currentLevelIndex);   
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

    public int getCurrentLevel()
    {
        return currentLevelIndex + 1;
    }

    public LevelData getCurrentLevelData()
    {
        return levels[currentLevelIndex];
    }


    private void StopAllVisualChallenges()
    {   //stop active visual challenges
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        if (fadeWordsCoroutine != null)
        {
            StopCoroutine(fadeWordsCoroutine);
            fadeWordsCoroutine = null;
        }
        textFade.currentFadeAlpha = 1f;
        
        //reset text alpha to fully visible
        if (wordOutput != null)
        {
            Color c = wordOutput.color;
            wordOutput.color = new Color(c.r, c.g, c.b, 1f);

            textFade.ApplyFadeAlphaToMesh();
        }
    }

    //called by text fade to reapply alpha to words that may have changed due to player input
    public void ReapplyFadeIfActive()
    {
        if (fadeWordsCoroutine != null)
        {
            textFade.ApplyFadeAlphaToMesh();
        }
    }

}
