using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Typer : MonoBehaviour
{  
    private Vector2 originalPosition;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 5f;

    public TMP_Text wordOutput = null;
    public Color typedColor = Color.green;
    public GameController gameController = null;
    public Color remainingColor = Color.black;
    public Color failedColor = Color.red;
    public WordBank wordBank = null;
    public TimerBar timerBar = null;
    public CandleController candleController = null;
    public Boy boy = null;
    public AudioPlayer audioPlayer = null;
    public Stopwatch stopwatch = null;
    
    private string currentSentence = null;
    private int typedCount = 0;
    private int correctCharCount = 0;
    private bool isPaused = false;
    private bool firstKeyPress = false;

    private void Start()
    {
        originalPosition = wordOutput.transform.localPosition;
    }

    private void Update()
    {   
        checkInput();
    }

    public void setCurrentSentence(string sentence)
    {
        currentSentence = sentence;
        typedCount = 0;
        updateDisplay();
    }

    private void updateDisplay()
    {
        string typed = currentSentence.Substring(0, typedCount);
        string remaining = currentSentence.Substring(typedCount);

        Color displayTypedColor = gameController.getFailed() ? failedColor : typedColor;

        string typedHex = ColorUtility.ToHtmlStringRGB(displayTypedColor);
        string remainingHex = ColorUtility.ToHtmlStringRGB(remainingColor);

        // Check if powerups are equipped
        bool hasInkOfConviction = Upgrades.Instance.IsPowerupEquipped(PowerupType.InkOfConviction);
        bool hasGuidingLight = Upgrades.Instance.IsPowerupEquipped(PowerupType.GuidingLight);

        if (hasInkOfConviction)
        {  
            string styledTyped = "";

            foreach (char c in typed)
            {
                if (c == ' ')
                {
                    styledTyped += " ";
                }
                else
                {
                    styledTyped += $"<size=120%>{c}</size>";
                }
            }

            typed = styledTyped;
        }
        

        // Build the display text
        string displayText = $"<color=#{typedHex}>{typed}</color>";

        if (remaining.Length > 0)
        {
            string nextLetter = remaining[0].ToString();
            string restRemaining = remaining.Substring(1);

            if (hasGuidingLight)
            {
                if (nextLetter == " ")
                {
                    nextLetter = $"<color=#FFFF00>_</color>"; 
                }
                else
                {
                    nextLetter = $"<color=#FFFF00><u>{nextLetter}</u></color>";
                }
            }

            displayText += $"<color=#{remainingHex}>{nextLetter}{restRemaining}</color>";
        }

        wordOutput.text = displayText;
    }

    private void checkInput()
    {       
        if (gameController.getFailed())
            return;

        // Check for Sacred Pause activation (press 1)
        if (Input.GetKeyDown(KeyCode.Alpha1) && gameController.SacredPause())
        {   
            ActivateSacredPause();
            return;
        }

        if (Input.anyKeyDown && !firstKeyPress)
        {   
            timerBar.resumeTimer();
            firstKeyPress = true;

        }

        string keyPressed = Input.inputString;
        stopwatch.StartTimer();
        if (keyPressed.Length == 1)
        {
            enterLetter(keyPressed);
        }

    }

    private void ActivateSacredPause(){

        if (isPaused)
        {
            Debug.Log("Already paused!");
            return;
        }

        isPaused = true;
        gameController.UseSacredPause();
        timerBar.pauseTimer();
        Debug.Log($"Sacred Pause activated!");
    }

    private void enterLetter(string letter)
    {
        if (isCorrectLetter(letter))
        {
            typedCount++;
            correctCharCount++;
            if(isWordComplete()){
                timerBar.ResetTimer();
                boy.MoveToCandle(gameController.getCurrentCandleIndex());
                audioPlayer.litSFX();
                Debug.Log("boy moving to candle index: " + gameController.getCurrentCandleIndex());

                gameController.incCurrentCandleIndex();
                
                if (isPaused)
                {   
                    Debug.Log("Resuming from Sacred Pause.");
                    isPaused = false;
                    timerBar.resumeTimer();
                }
                
            }   
            
            if (isSentenceComplete()){
                gameController.loadNextSentence();
            }
            else{
                updateDisplay();
            }

        } else 
        {   
            
            if (gameController.ShouldIgnoreMistake())
            {
                Debug.Log("Mistake ignored!");
                audioPlayer.failSFX();
                updateDisplay();
                return;
            }
            ResetFirstKeyPress();  
            gameController.setFailed();
            StartCoroutine(ShakeText());
            updateDisplay();
        }
    }


    private bool isCorrectLetter(string letter){
        return currentSentence[typedCount].ToString() == letter;
    }

    private bool isSentenceComplete(){
        return (typedCount) >= currentSentence.Length;
    }

    private IEnumerator ShakeText()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            wordOutput.transform.localPosition = originalPosition + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        wordOutput.transform.localPosition = originalPosition;
    }

    private bool isWordComplete()
    {   
        if (typedCount >= currentSentence.Length)
            return true;

        return currentSentence[typedCount] == ' ';
    }

    public void resetCharCount()
    {
        correctCharCount = 0;
    }

    public int getCorrectCharCount()
    {
        return correctCharCount;
    }

    public void ResetFirstKeyPress()
    {
        firstKeyPress = false;
    }
}
