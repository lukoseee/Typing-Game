using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Upgrades : MonoBehaviour
{   
    public static Upgrades Instance { get; private set; }

    [SerializeField] private GameObject root;
    [SerializeField] private Button exitButton;
    [SerializeField] private EndLevelPopup endLevelPopup;
    [SerializeField] private Image[] equipmentSlots;
    [SerializeField] private int maxEquipped = 2;


    [SerializeField] private int forgivingFlameCost = 5;
    [SerializeField] private int sacredPauseCost = 10;
    [SerializeField] private int inkOfConvictionCost = 6;
    [SerializeField] private int guidingLightCost = 7;

    private HashSet<PowerupType> purchasedPowerups = new HashSet<PowerupType>();

    private PowerupType[] equippedPowerups = new PowerupType[2];
    private Dictionary<PowerupType, bool> isEquipped = new Dictionary<PowerupType, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (root != null)
            root.SetActive(false);

        exitButton.onClick.AddListener(OnExitClicked);

        // Initialize
        for (int i = 0; i < maxEquipped; i++)
        {
            equippedPowerups[i] = PowerupType.None;
        }
    }

    public bool IsPowerupPurchased(PowerupType powerupType)
    {
        return purchasedPowerups.Contains(powerupType);
    }

    public int GetPowerupCost(PowerupType powerupType)
    {
        switch (powerupType)
        {
            case PowerupType.ForgivingFlame:
                return forgivingFlameCost;
            case PowerupType.SacredPause:
                return sacredPauseCost;
            case PowerupType.InkOfConviction:
                return inkOfConvictionCost;
            case PowerupType.GuidingLight:
                return guidingLightCost;
            default:
                return 0;
        }
    }

    public bool BuyPowerup(PowerupType powerupType)
    {
        // Already purchased
        if (purchasedPowerups.Contains(powerupType))
        {
            Debug.Log($"{powerupType} already purchased!");
            return false;
        }

        int cost = GetPowerupCost(powerupType);

        // Check if player can afford
        if (!GraceManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough grace!");
            return false;
        }

        // Spend grace and add to purchased
        GraceManager.Instance.Spend(cost);
        purchasedPowerups.Add(powerupType);
        Debug.Log($"Purchased {powerupType}!");
        return true;
    }

    public void EquipPowerup(Sprite powerupIcon, PowerupType powerupType)
    {
        // If already equipped, unequip it
        if (isEquipped.ContainsKey(powerupType) && isEquipped[powerupType])
        {
            UnequipPowerup(powerupType);
            return;
        }

        // If slots full, replace slot 0
        if (equipmentSlots[0].sprite != null && equipmentSlots[1].sprite != null)
        {
            UnequipPowerup(equippedPowerups[0]);
        }

        // Find first empty slot
        if (equipmentSlots[0].sprite == null)
        {
            equipmentSlots[0].sprite = powerupIcon;
            equipmentSlots[0].color = Color.white;
            equippedPowerups[0] = powerupType;
            isEquipped[powerupType] = true;
            ApplyPowerupEffect(powerupType);
        }
        else if (equipmentSlots[1].sprite == null)
        {
            equipmentSlots[1].sprite = powerupIcon;
            equipmentSlots[1].color = Color.white;
            equippedPowerups[1] = powerupType;
            isEquipped[powerupType] = true;
            ApplyPowerupEffect(powerupType);
        }
    }

    public void UnequipPowerup(PowerupType powerupType)
    {
        for (int i = 0; i < maxEquipped; i++)
        {
            if (equippedPowerups[i] == powerupType)
            {
                equipmentSlots[i].sprite = null;
                equipmentSlots[i].color = new Color32(26, 20, 40, 255);
                
                Outline outline = equipmentSlots[i].GetComponent<Outline>();
                outline.effectColor = new Color32(147, 112, 219, 255); // or your purple
                
                equippedPowerups[i] = PowerupType.None;
                isEquipped[powerupType] = false;
                RemovePowerupEffect(powerupType);
                return;
            }
        }
    }

    private void ApplyPowerupEffect(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.ForgivingFlame:
                Debug.Log("Forgiving Flame applied!");
                break;

            case PowerupType.SacredPause:
                Debug.Log("Sacred Pause applied!");
                break;

            case PowerupType.InkOfConviction:
                Debug.Log("Ink of Conviction applied!");
                break;

            case PowerupType.GuidingLight:
                Debug.Log("Guiding Light applied!");
                break;

            default:
                break;
        }
    }

    private void RemovePowerupEffect(PowerupType type)
    {
        switch (type)
        {
            case PowerupType.ForgivingFlame:
                Debug.Log("Forgiving Flame removed!");
                break;

            case PowerupType.SacredPause:
                Debug.Log("Sacred Pause removed!");
                break;

            case PowerupType.InkOfConviction:
                Debug.Log("Ink of Conviction removed!");
                break;

            case PowerupType.GuidingLight:
                Debug.Log("Guiding Light removed!");
                break;

            default:
                break;
        }
    }
    

    public bool IsPowerupEquipped(PowerupType powerupType)
    {
        return isEquipped.ContainsKey(powerupType) && isEquipped[powerupType];
    }

    public void Show()
    {
        root.SetActive(true);

    }

    private void OnExitClicked()
    {
        root.SetActive(false);
    }

}


public enum PowerupType
{
    None,
    ForgivingFlame,
    SacredPause,
    InkOfConviction,
    GuidingLight
}