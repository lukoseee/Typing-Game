using UnityEngine;
using System.Collections;
using System;
using TMPro;

public static class FadeUtil
{
    public static IEnumerator Lerp(float startAlpha, float endAlpha, float duration, Action<float> apply)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            apply(a);
            yield return null;
        }
        apply(endAlpha); // snap to final
    }
}

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
            yield return FadeUtil.Lerp(0f, 1f, 0.1f, SetSentenceAlpha);
            yield return new WaitForSeconds(4f);
            yield return FadeUtil.Lerp(1f, 0f, 0.1f, SetSentenceAlpha);
            yield return new WaitForSeconds(2f);
        }
    }

    public IEnumerator FadeWordsRoutine()
    {
        const float fadeDuration = 0.4f;
        const float cycle = 5f; // total time for one fade in + fade out cycle
        float hold = cycle * 0.4f;

        while (true)
        {
            yield return FadeUtil.Lerp(1f, 0f, fadeDuration, SetWordsAlpha);
            yield return new WaitForSeconds(hold);
            yield return FadeUtil.Lerp(0f, 1f, fadeDuration, SetWordsAlpha);
            yield return new WaitForSeconds(hold);
        }
    }

    private void SetSentenceAlpha(float a)
    {
        Color c = wordOutput.color;
        wordOutput.color = new Color(c.r, c.g, c.b, a);
    }

    private void SetWordsAlpha(float a)
    {
        currentFadeAlpha = a;
        ApplyFadeAlphaToMesh();
    }

    //applies currentFadeAlpha to characters that should fade according to markers
    public void ApplyFadeAlphaToMesh()
    {
        if (wordOutput == null || wordBank == null) return;

        wordOutput.ForceMeshUpdate();
        
        //actual characters of the text
        TMPro.TMP_TextInfo textInfo = wordOutput.textInfo;

        if (textInfo.characterCount == 0) return;

        byte alphaByte = (byte)Mathf.RoundToInt(currentFadeAlpha * 255f);

        int wordIndex = 0;
        bool inWord = false;


        //check for every character
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMPro.TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            //increment word index based on invisible chars
            if (!charInfo.isVisible)
            {
                if (inWord)
                {
                    wordIndex++;
                    inWord = false;
                }
                continue; //skip fading invisible characters
            }

            inWord = true; 

            if (!wordBank.DoesWordFade(wordIndex)) continue; //check if word is marked for fading
            
            //apply fading
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
