using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] private Image timerImage;

    public float duration = 5f;
    private float timeLeft;
    private bool isPaused = false;

    void Start()
    {
        timeLeft = duration;
    }

    void Update()
    {   
        if (isPaused) return;

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            float fill = timeLeft / duration;
            timerImage.fillAmount = fill;
        }
    }
    
    public void ResetTimer()
    {
        timeLeft = duration;
        timerImage.fillAmount = 1f;
    }

    public bool isTimerExpired()
    {
        return timeLeft <= 0;
    }

    public void stopTimer()
    {
        timeLeft = 0;
        timerImage.fillAmount = 0f;
    }

    public void pauseTimer()
    {
        isPaused = true;
    }

    public void resumeTimer()
    {
        isPaused = false;
    }
}
