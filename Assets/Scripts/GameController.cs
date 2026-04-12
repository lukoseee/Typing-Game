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
    public Stopwatch stopwatch = null;

    private bool isBlowingOut = false;
    private bool hasFailed = false;
    private int currentLevelIndex = 0;
    private int currentCandleIndex = 0;
    private bool hasFailedAlready = false;
    private int mistakesRemaining = 0;
    private int sacredPauses = 0;

    private Coroutine fadeCoroutine;
    private Coroutine swayCoroutine;
    private RectTransform textRectTransform;

    private void StartLevel(int index ){
        
        if(IsLastLevel()){
            fadeCoroutine = StartCoroutine(FadeSentencesRoutine());
        }

        if(index == 7){
            swayCoroutine = StartCoroutine(SwayTextRoutine());
        }

        ApplyForgivingFlame();
        ApplySacredPause();
        typer.resetCharCount();
        boy.ResetPosition();
        LevelData level = levels[index];

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

        StopSway();
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

    private void StopSway()
    {
        if (swayCoroutine != null)
        {
            StopCoroutine(swayCoroutine);
        }
    }

    public void RestartLevel()
    {   
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);  
        }

        StopSway();

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

    private bool IsLastLevel()
    {
        return currentLevelIndex >= levels.Count - 1;
    }

    private IEnumerator FadeSentencesRoutine()
    {
        while (true)
        {            
            yield return StartCoroutine(FadeText(0f, 1f, 0.1f));
            
            yield return new WaitForSeconds(4f);
            
            yield return StartCoroutine(FadeText(1f, 0f, 0.1f));
            
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color originalColor = wordOutput.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wordOutput.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        wordOutput.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    private IEnumerator SwayTextRoutine()
    {
        Vector3 originalPosition = textRectTransform.anchoredPosition;
        float swayAmount = 20f; 
        float swaySpeed = 4f; 

        while (true)
        {
            float xOffset = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            textRectTransform.anchoredPosition = originalPosition + new Vector3(xOffset, 0, 0);
            yield return null;
        }
    }
}
