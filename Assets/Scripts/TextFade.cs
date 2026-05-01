using UnityEngine;
using System.Collections;
using TMPro;

//manages various text fade in/out effects 
public class TextFade : MonoBehaviour
{
    [SerializeField] private TMP_Text wordOutput = null;
    [SerializeField] private WordBank wordBank = null;

    public float currentFadeAlpha = 1f;

    public IEnumerator FadeSentencesRoutine()
    {
        while (true)
        {            
            yield return StartCoroutine(FadeText(0f, 1f, 0.1f));
            
            yield return new WaitForSeconds(4f);
            
            yield return StartCoroutine(FadeText(1f, 0f, 0.1f));
            
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color originalColor = wordOutput.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            wordOutput.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        wordOutput.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    public IEnumerator FadeWordsRoutine()
    {
        float fadeDuration = 0.4f;
        float fadeWordsCycleDuration = 3f; // seconds for full fade in/out cycle
        float visibleHold = fadeWordsCycleDuration * 0.4f;
        float invisibleHold = fadeWordsCycleDuration * 0.4f;

        while (true)
        {
            yield return StartCoroutine(LerpFadeAlpha(1f, 0f, fadeDuration));
            yield return new WaitForSeconds(invisibleHold);
            yield return StartCoroutine(LerpFadeAlpha(0f, 1f, fadeDuration));
            yield return new WaitForSeconds(visibleHold);
        }
    }

    private IEnumerator LerpFadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentFadeAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            ApplyFadeAlphaToMesh();
            yield return null;
        }
        currentFadeAlpha = endAlpha;
        ApplyFadeAlphaToMesh();
    }

    public void ApplyFadeAlphaToMesh()
    {
        if (wordOutput == null || wordBank == null) return;

        wordOutput.ForceMeshUpdate();
        TMPro.TMP_TextInfo textInfo = wordOutput.textInfo;

        if (textInfo.characterCount == 0) return;

        byte alphaByte = (byte)Mathf.RoundToInt(currentFadeAlpha * 255f);

        int wordIndex = 0;
        bool inWord = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                //spaces and other invisible chars, use them as word separators
                if (inWord)
                {
                    wordIndex++;
                    inWord = false;
                }
                continue;
            }

            inWord = true;

            if (!wordBank.DoesWordFade(wordIndex)) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;

            //each character has 4 vertices 
            vertexColors[vertIndex + 0].a = alphaByte;
            vertexColors[vertIndex + 1].a = alphaByte;
            vertexColors[vertIndex + 2].a = alphaByte;
            vertexColors[vertIndex + 3].a = alphaByte;
        }

        wordOutput.UpdateVertexData(TMPro.TMP_VertexDataUpdateFlags.Colors32);
    }

    
}
