using UnityEngine;

//stopwatch timer for tracking time in levels
public class Stopwatch : MonoBehaviour
{   
    private bool timerActive;
    private float currentTime;
    
    void Start()
    {   
        currentTime = 0f;
    }

    void Update()
    {
        if(timerActive)
        {
            currentTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public float GetTime()
    {
        return currentTime;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        timerActive = false;
    }
}
