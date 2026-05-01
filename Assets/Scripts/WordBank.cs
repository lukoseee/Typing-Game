using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


//manages the sentences and words for each level
public class WordBank : MonoBehaviour
{   
    private List<string> workingSentences = new List<string>();
    private bool[] currentMask;
    private bool[] currentFadeMask; 


    public void setWords(string[] newSentences)
    {   
        workingSentences.Clear();
        workingSentences.AddRange(newSentences);
        currentMask = null;
        currentFadeMask = null;
    }

    public string getWord(){

        string newWord = string.Empty;

        if(workingSentences.Count != 0){

            newWord = workingSentences.First();
            workingSentences.Remove(newWord);
        }
        
        return newWord;

    }

    //for missing letters
    public string ParseMaskedSentence(string raw, char marker)
    {
        StringBuilder clean = new StringBuilder();
        List<bool> mask = new List<bool>();

        for (int i = 0; i < raw.Length; i++)
        {
            if (raw[i] == marker)
            {
                if (i + 1 < raw.Length)
                {
                    clean.Append(raw[i + 1]);
                    mask.Add(true);
                    i++; 
                }
            }
            else
            {
                clean.Append(raw[i]);
                mask.Add(false);
            }
        }

        currentMask = mask.ToArray();
        return clean.ToString();
    }

    public void ClearMask()
    {
        currentMask = null;
    }

    public bool IsMaskedAt(int index)
    {
        if (currentMask == null || index < 0 || index >= currentMask.Length) return false;
        return currentMask[index];
    }

    //for fading words 
    public string ParseFadeSentence(string raw, char marker)
    {
        StringBuilder clean = new StringBuilder();
        List<bool> fadeMask = new List<bool>();

        string[] words = raw.Split(' ');

        for (int w = 0; w < words.Length; w++)
        {
            string word = words[w];
            bool fades = word.Length > 0 && word[0] == marker;

            if (fades)
            {
                word = word.Substring(1); // strip the marker
            }

            clean.Append(word);
            fadeMask.Add(fades);

            if (w < words.Length - 1)
            {
                clean.Append(' ');
            }
        }

        currentFadeMask = fadeMask.ToArray();
        return clean.ToString();
    }

    public void ClearFadeMask()
    {
        currentFadeMask = null;
    }

    public bool DoesWordFade(int wordIndex)
    {
        if (currentFadeMask == null || wordIndex < 0 || wordIndex >= currentFadeMask.Length)
            return false;
        return currentFadeMask[wordIndex];
    }
    
    public bool isComplete()
    {
        return workingSentences.Count == 0;
    }   

    public int wordCount(){
        
        int counter = 0;

        foreach(string s in workingSentences){
            string cleaned = s.Replace("*", "").Replace("~", ""); // remove any markers for counting words
            string[] words = cleaned.Split(' ', '\t', '\n');
            counter += words.Length;
        }

        return counter;
    }

}
