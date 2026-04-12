using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string[] sentences;
    public float timeLimit;
    public string message;
    public AudioClip backgroundMusic;
    public float[] ranksThresholds;
    public int graceEarned;

    [SerializeField] private bool baseGraceAwarded = false;
    [SerializeField] private Rank highestRankAwarded = Rank.None;

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
