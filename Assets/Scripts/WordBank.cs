using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour
{   
    public List<string> sentences = new List<string>();

    private List<string> workingSentences = new List<string>();

    private void Awake(){

        workingSentences.AddRange(sentences);
    }

    public void resetSentences(){

        workingSentences.Clear();
        workingSentences.AddRange(sentences);
    }

    public string getWord(){

        string newWord = string.Empty;

        if(workingSentences.Count != 0){

            newWord = workingSentences.First();
            workingSentences.Remove(newWord);
        }
        
        return newWord;

    }

}
