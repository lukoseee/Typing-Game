using UnityEngine;
using UnityEngine.UI;

public class Powerup : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button buyButton;
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
        // Only allow equip if purchased
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
        bool isPurchased = Upgrades.Instance.IsPowerupPurchased(powerupType);
        int cost = Upgrades.Instance.GetPowerupCost(powerupType);

        // Show buy button if not purchased, enable if can afford
        buyButton.gameObject.SetActive(!isPurchased);
        buyButton.interactable = graceAmount >= cost;

        // Show equip button if purchased
        equipButton.gameObject.SetActive(isPurchased);
    }
}
