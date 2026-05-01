using UnityEngine;

public enum VisualChallenge
{
    None,
    MissingLetters,
    FadingWords,
    FadingSentences
}

//scriptable object to hold level data and track player progress on each level for grace rewards and rank achievements
[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string[] sentences;
    public float timeLimit;
    public string message;
    public AudioClip backgroundMusic;
    public float[] ranksThresholds;
    public int graceEarned;

    [SerializeField] private bool baseGraceAwarded = false; //to track if player has already earned the base grace for completing the level, prevents farming grace by replaying levels
    [SerializeField] private Rank highestRankAwarded = Rank.None; //to track highest rank player has achieved on this level

    public VisualChallenge visualChallenge = VisualChallenge.None;

    public char maskMarker = '*'; //for missing letters challenge

    public char fadeWordMarker = '~'; //for fading words challenge

    public bool HasBaseGraceBeenAwarded()
    {
        return baseGraceAwarded;
    }

    public void SetBaseGraceAwarded()
    {
        baseGraceAwarded = true;
    }

    public Rank GetHighestRankAwarded()
    {
        return highestRankAwarded;
    }

    public void SetHighestRankAwarded(Rank rank)
    {
        highestRankAwarded = rank;
    }

    public void ResetForNewGame()
    {
        baseGraceAwarded = false;
        highestRankAwarded = Rank.None;
    }

}
