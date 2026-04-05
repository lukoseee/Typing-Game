using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndLevelPopup : MonoBehaviour
{   
    [SerializeField] private GameObject root;  
    [SerializeField] private Button retryButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private GameController gameController; 

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);

        retryButton.onClick.AddListener(OnRetryClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    public void ShowWin()
    {
        root.SetActive(true);

        nextButton.gameObject.SetActive(true);
        // optionally disable retry on win, or keep it
    }

    public void ShowFail()
    {
        root.SetActive(true);

        nextButton.gameObject.SetActive(false);
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
}
