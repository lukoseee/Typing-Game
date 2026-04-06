using UnityEngine;

public class Stopwatch : MonoBehaviour
{   
    private bool timerActive;
    private float currentTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        currentTime = 0f;
    }

    // Update is called once per frame
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
