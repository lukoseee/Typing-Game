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
//manages end level popup UI, displays results, calculates rank and grace rewards, and handles navigation to next level or upgrades screen
public class EndLevelPopup : MonoBehaviour
{   
    [SerializeField] private GameObject root;

    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private Text level; //current level number
    [SerializeField] private Text completeText; //complete or failed text
    [SerializeField] private Text timer; 
    [SerializeField] private Text WPM;

    //rank image components
    [SerializeField] private Image bronzeImage; 
    [SerializeField] private Image silverImage;
    [SerializeField] private Image goldImage;

    //rank sprites 
    [SerializeField] private Sprite bronzeFull;
    [SerializeField] private Sprite silverFull;
    [SerializeField] private Sprite goldFull;
    [SerializeField] private Sprite outline; //outline sprite for unearned ranks

    [SerializeField] private GameController gameController;
    [SerializeField] private Typer typer;
    
    //threshold text component on popup
    [SerializeField] private Text bronzeThreshold;
    [SerializeField] private Text silverThreshold;
    [SerializeField] private Text goldThreshold;

    [SerializeField] private Button upgradesButton;
    [SerializeField] private Upgrades upgrades;
    [SerializeField] private Text graceEarned;

    private float wpmScore;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        retryButton.onClick.AddListener(OnRetryClicked);
        nextButton.onClick.AddListener(OnNextClicked);
        upgradesButton.onClick.AddListener(OnUpgradesClicked);
    }

    private void DisplayRank(Rank rank)
    {   
        //update rank images based on earned rank 
        //all ranks up to earned rank should be filled, others outlined
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

        //only show rank on win
        Rank rank = GetRank(wpmScore);
        DisplayRank(rank);

        LevelData levelData = gameController.getCurrentLevelData();

        //grant grace for completing level if not already awarded, then grant bonus grace for rank if it's a new highest rank
        if (!levelData.HasBaseGraceBeenAwarded())
        {
            int baseGrace = levelData.graceEarned;
            GraceManager.Instance.Add(baseGrace);
            graceEarned.text = $"+{baseGrace}";
            levelData.SetBaseGraceAwarded();
        }
        else
        {   // on retry after earning grace, base grace is not awarded again, so indicate that with +0.
            graceEarned.text = "+0";
        }
        
        // check if new rank is higher than previously awarded rank for this level, and if so, grant bonus grace for new rank
        if (rank > levelData.GetHighestRankAwarded())
        {
            int bonusGrace = GetBonusForRank(rank);
            GraceManager.Instance.Add(bonusGrace);
            graceEarned.text += $"+{bonusGrace}";
            levelData.SetHighestRankAwarded(rank);
        }
        
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

        //disable next button on fail
        nextButton.gameObject.SetActive(false);
        ResetRankDisplay();
    }

    //common setup for both win and fail states
    private void ShowCommon()
    {   
        root.SetActive(true);
        level.text = $"Level {gameController.getCurrentLevel()}";
        timer.text = gameController.stopwatch.GetTime().ToString("F2");

        //calculate wpm on fail and success
        wpmScore = CalculateWPM(typer.getCorrectCharCount(), gameController.stopwatch.GetTime());
        WPM.text = wpmScore.ToString("F2"); //2 decimal places

        //display thresholds for each rank for that level
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
        //hide itself so commit 
        Hide();
        gameController.NextLevel();
    }

    private float CalculateWPM(int characterCount, float timeInSeconds)
    {
        float minutes = timeInSeconds / 60f;
        float words = characterCount / 5f;
        return words / minutes;
    }

    //determine rank based on score and thresholds defined in level data
    private Rank GetRank(float score)
    {   
        float[] thresholds = gameController.getCurrentLevelData().ranksThresholds;

        if (score >= thresholds[2]){ 
            return Rank.Gold; 
        }
        if (score >= thresholds[1]) { 
            return Rank.Silver; 
        }
        if (score >= thresholds[0]) { 
            return Rank.Bronze; 
        }
        return Rank.None;
    }

    //determine bonus based on rank
    private int GetBonusForRank(Rank rank)
    {
        switch (rank)
        {
            case Rank.Bronze:
                return 1;
            case Rank.Silver:
                return 2;
            case Rank.Gold:
                return 3;
            default:
                return 0;
        }
    }

    private void OnUpgradesClicked()
    {
        upgrades.Show();
    }
    
    
}
