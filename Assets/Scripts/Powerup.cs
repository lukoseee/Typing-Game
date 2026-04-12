using UnityEngine;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text costText;
    [SerializeField] private Text levelText;
    [SerializeField] private PowerupType powerupType;

    private void OnEnable()
    {
        equipButton.onClick.AddListener(OnEquipClicked);
        buyButton.onClick.AddListener(OnBuyClicked);
        

        if (GraceManager.Instance != null)
            {
                GraceManager.Instance.OnGraceChanged += UpdateButtonStates;
                UpdateButtonStates(GraceManager.Instance.getGraceAmount());
            }
    }

    private void OnDisable()
    {
        equipButton.onClick.RemoveListener(OnEquipClicked);
        buyButton.onClick.RemoveListener(OnBuyClicked);
        if (GraceManager.Instance != null)
            GraceManager.Instance.OnGraceChanged -= UpdateButtonStates;
    }

    private void OnEquipClicked()
    {   
        if (!Upgrades.Instance.IsPowerupPurchased(powerupType))
        {
            Debug.Log("You must purchase this powerup first!");
            return;
        }

        Upgrades.Instance.EquipPowerup(icon.sprite, powerupType);
        UpdateButtonStates(GraceManager.Instance.getGraceAmount());
    }

    private void OnBuyClicked()
    {
        if (Upgrades.Instance.BuyPowerup(powerupType))
        {
            UpdateButtonStates(GraceManager.Instance.getGraceAmount());
        }
    }

    private void UpdateButtonStates(int graceAmount)
    {
        int currentLevel = Upgrades.Instance.GetPowerupLevel(powerupType);
        bool isMaxed = Upgrades.Instance.IsMaxLevel(powerupType);
        bool isPurchased = Upgrades.Instance.IsPowerupPurchased(powerupType);
        bool isUpgradable = Upgrades.Instance.IsUpgradable(powerupType);

        if (currentLevel == 0 || (!isUpgradable && isPurchased))
        {
            levelText.text = "";
        }
        else
        {
            levelText.text = $"LVL: {currentLevel}/3";
        }

        if (isMaxed)
        {
            buyButton.gameObject.SetActive(true);
            costText.text = "MAX";
            buyButton.interactable = false;
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            int nextLevel = currentLevel + 1;
            int cost = Upgrades.Instance.GetPowerupCost(powerupType, nextLevel);
            costText.text = cost.ToString();
            buyButton.interactable = graceAmount >= cost;
        }

        equipButton.gameObject.SetActive(isPurchased);
    }
    
}
