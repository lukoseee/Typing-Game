using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Rank
{
    None,
    Bronze,
    Silver,
    Gold
}

public class EndLevelPopup : MonoBehaviour
{   
    [SerializeField] private GameObject root;  
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Text level;
    [SerializeField] private Text completeText;
    [SerializeField] private Text timer;
    [SerializeField] private Text WPM;
    [SerializeField] private Image bronzeImage;
    [SerializeField] private Image silverImage;
    [SerializeField] private Image goldImage;
    [SerializeField] private Sprite bronzeFull;
    [SerializeField] private Sprite silverFull;
    [SerializeField] private Sprite goldFull;

    [SerializeField] private GameController gameController;
    [SerializeField] private Typer typer;
    [SerializeField] private Sprite outline;
    [SerializeField] private Text bronzeThreshold;
    [SerializeField] private Text silverThreshold;
    [SerializeField] private Text goldThreshold;
    [SerializeField] private Button upgradesButton;
    [SerializeField] private Upgrades upgrades;
    [SerializeField] private Text graceEarned;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        retryButton.onClick.AddListener(OnRetryClicked);
        nextButton.onClick.AddListener(OnNextClicked);
        upgradesButton.onClick.AddListener(OnUpgradesClicked);
    }

    public void DisplayRank(Rank rank)
    {
        // Fill based on achieved rank
        if (rank >= Rank.Bronze)
            bronzeImage.sprite = bronzeFull;

        if (rank >= Rank.Silver)
            silverImage.sprite = silverFull;

        if (rank >= Rank.Gold)
            goldImage.sprite = goldFull;
    }

    public void ShowWin()
    {
        ShowCommon();
        completeText.color = Color.green;
        completeText.text = "Complete!";

        int graceEarnedAmount = gameController.getCurrentLevelData().graceEarned;
        GraceManager.Instance.Add(graceEarnedAmount);
        graceEarned.text = $"+{graceEarnedAmount}";

        float wpmScore = CalculateWPM(typer.getCorrectCharCount(), gameController.stopwatch.GetTime());
        WPM.text = wpmScore.ToString("F2");
        Rank rank = GetRank(wpmScore);
        DisplayRank(rank);

        nextButton.gameObject.SetActive(true);
    }

    private void ResetRankDisplay()
    {
        bronzeImage.sprite = outline;
        silverImage.sprite = outline;
        goldImage.sprite = outline;
    }

    public void ShowFail()
    {   
        ShowCommon();
        completeText.text = "Failed!";
        completeText.color = Color.red;
        nextButton.gameObject.SetActive(false);
        ResetRankDisplay();
    }

    public void ShowCommon()
    {
        root.SetActive(true);
        level.text = $"Level {gameController.getCurrentLevel()}";
        timer.text = gameController.stopwatch.GetTime().ToString("F2");
        bronzeThreshold.text = $"{gameController.getCurrentLevelData().ranksThresholds[0]} WPM";
        silverThreshold.text = $"{gameController.getCurrentLevelData().ranksThresholds[1]} WPM";
        goldThreshold.text = $"{gameController.getCurrentLevelData().ranksThresholds[2]} WPM";

    }

    public void Hide()
    {
        root.SetActive(false);
    }

    private void OnRetryClicked()
    {   
        Debug.Log("Retrying level...");
        Hide();
        gameController.RestartLevel();
    }

    private void OnNextClicked()
    {   
        Debug.Log("Loading next level...");
        Hide();
        gameController.NextLevel();
    }

    private float CalculateWPM(int characterCount, float timeInSeconds)
    {
        float minutes = timeInSeconds / 60f;
        float words = characterCount / 5f;
        return words / minutes;
    }

    private Rank GetRank(float score)
    {   
        float[] thresholds = gameController.getCurrentLevelData().ranksThresholds;

        if (score >= thresholds[2]) return Rank.Gold;
        if (score >= thresholds[1]) return Rank.Silver;
        if (score >= thresholds[0]) return Rank.Bronze;
        return Rank.None;
    }

    private void OnUpgradesClicked()
    {
        upgrades.Show();
    }
}
