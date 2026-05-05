using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

//all logic related to player typing 
public class Typer : MonoBehaviour
{   
    //text shake settings
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 5f;

    [SerializeField] private TMP_Text wordOutput = null;
    [SerializeField] private Color typedColor = Color.green;
    [SerializeField] private GameController gameController = null;
    [SerializeField] private Color remainingColor = Color.black;
    [SerializeField] private Color failedColor = Color.red;
    [SerializeField] private TimerBar timerBar = null;
    [SerializeField] private Boy boy = null;
    [SerializeField] private AudioPlayer audioPlayer = null;
    [SerializeField] private Stopwatch stopwatch = null;
    [SerializeField] private WordBank wordBank = null;
    [SerializeField] private PowerupPopup popupPrefab;
    [SerializeField] private Transform popupParent;

    private string currentSentence = null;
    private int typedCount = 0; //no of chars typed so far
    private int correctCharCount = 0; //for calculating WPM
    private bool isPaused = false;
    private bool firstKeyPress = false;
    private Vector2 originalPosition;
    
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
        //split typed and remaining parts of the sentence
        string typed = currentSentence.Substring(0, typedCount);
        string remaining = currentSentence.Substring(typedCount);

        //red if failed, green otherwise
        Color displayTypedColor = gameController.getFailed() ? failedColor : typedColor;

        //
        string typedHex = ColorUtility.ToHtmlStringRGB(displayTypedColor);
        string remainingHex = ColorUtility.ToHtmlStringRGB(remainingColor);

        //check if player has visual power-ups
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
                    styledTyped += $"<size=120%>{c}</size>"; //typed letters are slightly bigger
                }
            }

            typed = styledTyped;
        }
        
        string displayText = $"<color=#{typedHex}>{typed}</color>";

        if (remaining.Length > 0)
        {
            string nextLetter = remaining[0].ToString();
            string restRemaining = remaining.Substring(1);

            //apply masking to the next letter (for level 8)
            if (wordBank != null && wordBank.IsMaskedAt(typedCount))
            {
                nextLetter = "_";
            }

            //apply masking to the rest, character by character
            System.Text.StringBuilder maskedRest = new System.Text.StringBuilder();
            for (int i = 0; i < restRemaining.Length; i++)
            {
                int globalIndex = typedCount + 1 + i;
                if (wordBank != null && wordBank.IsMaskedAt(globalIndex))
                {
                    maskedRest.Append('_');
                }
                else
                {
                    maskedRest.Append(restRemaining[i]);
                }
            }
            restRemaining = maskedRest.ToString();

            if (hasGuidingLight)
            {
                if (nextLetter == " ")
                {
                    nextLetter = $"<color=#FFFF00>_</color>"; 
                }
                else
                {
                    nextLetter = $"<color=#FFFF00><u>{nextLetter}</u></color>"; //next letter highlighted and underlined
                }
            }

            displayText += $"<color=#{remainingHex}>{nextLetter}{restRemaining}</color>";
        }

        wordOutput.text = displayText;
        
        gameController.ReapplyFadeIfActive();
    }

    private void checkInput()
    {       
        if (gameController.getFailed())
            return;
        
        //activate sacred pause powerup on 2 key press, if available
        if (Input.GetKeyDown(KeyCode.Alpha2) && gameController.SacredPause())
        {   
            ActivateSacredPause();
            PowerupPopup.Spawn(popupPrefab, popupParent, "SacredPause");
            return;
        }

        //only start timer on first press
        if (Input.anyKeyDown && !firstKeyPress)
        {   
            timerBar.resumeTimer();
            firstKeyPress = true;

        }

        string keyPressed = Input.inputString;
        stopwatch.StartTimer();

        //only consider single character inputs         
        if (keyPressed.Length == 1)
        {   
            //check letter input
            enterLetter(keyPressed);
        }

    }

    //decrement usage and pause timer
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
                //reset timer on word completion
                timerBar.ResetTimer();

                boy.MoveToCandle(gameController.getCurrentCandleIndex());
                audioPlayer.litSFX(); //success sfx
                Debug.Log("boy moving to candle index: " + gameController.getCurrentCandleIndex());

                //move to next candle 
                gameController.incCurrentCandleIndex();
                
                //resume timer if word is complete after using sacred pause
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
            //check if player has forgiving flame  
            if (gameController.ShouldIgnoreMistake())
            {
                Debug.Log("Mistake ignored!");
                audioPlayer.failSFX();

                //powerup icon popup
                PowerupPopup.Spawn(popupPrefab, popupParent, "ForgivingFlame");
                updateDisplay();
                return;
            }
            ResetFirstKeyPress();  

            //notify game controller of fail
            gameController.setFailed();
            StartCoroutine(ShakeText());
            updateDisplay();
        }
    }


    private bool isCorrectLetter(string letter){
        //check against the current letter in the sentence 
        return currentSentence[typedCount].ToString() == letter;
    }

    private bool isSentenceComplete(){
        return (typedCount) >= currentSentence.Length;
    }

    //slighty shake the text on mistake
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
        //if finished level
        if (typedCount >= currentSentence.Length)
            return true;
            
        //if next char is space, consider word complete 
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
