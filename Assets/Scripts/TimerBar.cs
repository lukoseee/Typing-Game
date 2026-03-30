using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    public Image timerImage;
    public float duration = 5f;

    private float timeLeft;

    void Start()
    {
        timeLeft = duration;
    }

    void Update()
    {
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
}
