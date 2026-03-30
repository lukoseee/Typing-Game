using UnityEngine;

public class GameController : MonoBehaviour
{
    public Typer typer = null;
    public WordBank wordBank = null;

    private bool hasFailed = false;

    private void Start()
    {   
        Debug.Log("Starting game...");
        hasFailed = false;
        wordBank.resetSentences();
        loadNextSentence();
        Debug.Log("hasFailed: " + hasFailed);
    }

    private void Update()
    {
        if (hasFailed == true)
        {
            checkRestart();
        }
    }

    public void loadNextSentence()
    {
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
            Start();
        }
    }
}
