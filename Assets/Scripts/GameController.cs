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
    [SerializeField] private RestartMessage restartMessage = null;
    [SerializeField] private AudioPlayer audioPlayer = null;
    [SerializeField] private EndLevelPopup endLevelPopup = null;
    [SerializeField] private TMP_Text wordOutput = null;
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

        StopAllVisualChallenges();

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

        ApplyForgivingFlame();
        ApplySacredPause();
        typer.resetCharCount();
        boy.ResetPosition();

        wordBank.setWords(level.sentences);
        currentCandleIndex = 0;

        candleController.SpawnCandles(wordBank.wordCount());

        candleController.ResetCandles();
        
        timerBar.SetDuration(level.timeLimit);
        stopwatch.ResetTimer();
        timerBar.ResetTimer();
        timerBar.pauseTimer();

        levelDisplay.text = $"{index + 1}";

        restartMessage.SetMessage(level.message);
        StartCoroutine(restartMessage.FadeInAndOut());


        desk.ResizeDesk(wordBank.wordCount(), candleController.GetSpacing());
        mainCamera.ResetPosition();
        typer.ResetFirstKeyPress();
    }

    public void NextLevel(){
        currentLevelIndex++;

        Debug.Log($"Loading level {currentLevelIndex}...");
        if (currentLevelIndex >= levels.Count)
        {
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
            level.ResetForNewGame();
        }

        textRectTransform = wordOutput.GetComponent<RectTransform>();
        currentLevelIndex = 0;
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

        if (!isBlowingOut)
        {
            isBlowingOut = true;
            mainCamera.StartRewind();
            StartCoroutine(candleController.BlowOutAll());
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
    {
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

        if (wordOutput != null)
        {
            Color c = wordOutput.color;
            wordOutput.color = new Color(c.r, c.g, c.b, 1f);

            textFade.ApplyFadeAlphaToMesh();
        }
    }

    public void ReapplyFadeIfActive()
    {
        if (fadeWordsCoroutine != null)
        {
            textFade.ApplyFadeAlphaToMesh();
        }
    }

}
