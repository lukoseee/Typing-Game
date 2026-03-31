using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour
{   
    private List<string> workingSentences = new List<string>();

    public void setWords(string[] newSentences)
    {   
        workingSentences.Clear();
        workingSentences.AddRange(newSentences);
    }

    public string getWord(){

        string newWord = string.Empty;

        if(workingSentences.Count != 0){

            newWord = workingSentences.First();
            workingSentences.Remove(newWord);
        }
        
        return newWord;

    }
    
    public bool isComplete()
    {
        return workingSentences.Count == 0;
    }   

    public int wordCount(){
        
        int counter = 0;

        foreach(string s in workingSentences){
            string[] words = s.Split(' ', '\t', '\n');
            counter += words.Length;
        }

        return counter;
    }

}
