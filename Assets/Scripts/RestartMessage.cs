using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//manages the message at the start of level with fade in/out animation
public class RestartMessage : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private string message;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;    

    private Coroutine fadeCoroutine;

    void Awake()
    {
        SetAlpha(0f);
        messageText.text = message;
    }

    public void Show() => SetAlpha(1f);
    public void SetMessage(string newMessage) => messageText.text = newMessage;

    private void SetAlpha(float a)
    {
        Color c = messageText.color;
        c.a = a;
        messageText.color = c;
    }

    private IEnumerator FadeInAndOut()
    {
        yield return FadeUtil.Lerp(0f, 1f, fadeInDuration, SetAlpha);
        yield return new WaitForSeconds(displayDuration);
        yield return FadeUtil.Lerp(1f, 0f, fadeOutDuration, SetAlpha);
    }

    public void StartFadeInAndOut()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInAndOut());
    }

    public void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

}