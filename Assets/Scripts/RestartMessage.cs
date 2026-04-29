using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestartMessage : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private string message;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;

    private Coroutine fadeCoroutine;

    void Start()
    {
        Hide();
        messageText.text = message;
    }

    private void SetAlpha(float a)
    {
        Color c = messageText.color;
        c.a = a;
        messageText.color = c;
    }

    public void Show()
    {
        SetAlpha(1f);
    }

    private void Hide()
    {
        SetAlpha(0f);
    }

    public void SetMessage(string newMessage)
    {
        messageText.text = newMessage;
    }

    private IEnumerator FadeIn(){
        yield return FadeText(messageText, 0f, 1f, fadeInDuration);
    }

    private IEnumerator FadeOut(){
        yield return FadeText(messageText, 1f, 0f, fadeOutDuration);
    }

    private IEnumerator FadeInAndOut()
    {
        yield return FadeIn();
        
        yield return new WaitForSeconds(displayDuration);
        
        yield return FadeOut();
    }

    private IEnumerator FadeText(Text textComponent, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color color = textComponent.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            textComponent.color = color;
            yield return null;
        }
        Color finalColor = textComponent.color;
        finalColor.a = endAlpha;
        textComponent.color = finalColor;
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