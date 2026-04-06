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
}
