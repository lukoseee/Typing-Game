using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Typer : MonoBehaviour
{  
    private Vector2 originalPosition;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 5f;

    public Text wordOutput = null;
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

        wordOutput.text = $"<color=#{typedHex}>{typed}</color><color=#{remainingHex}>{remaining}</color>";
    }

    private void checkInput()
    {       
        if (gameController.getFailed())
            return;

        if (Input.anyKeyDown)
        {   
            timerBar.resumeTimer();
            stopwatch.StartTimer();
            string keyPressed = Input.inputString;
            if (keyPressed.Length == 1)
            {
                enterLetter(keyPressed);
            }
        }
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
                
            }   
            
            if (isSentenceComplete()){
                gameController.loadNextSentence();
            }
            else{
                updateDisplay();
            }

        } else 
        {
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
}
