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
    [SerializeField] private Text[] levelsUnlockedText;


    private Dictionary<PowerupType, int[]> powerupCosts = new Dictionary<PowerupType, int[]>
    {
        { PowerupType.ForgivingFlame, new int[] { 4, 5, 6 } },
        { PowerupType.SacredPause, new int[] { 4, 5, 6 } },
        { PowerupType.InkOfConviction, new int[] { 3, 0, 0 } }, // Non-upgradable
        { PowerupType.GuidingLight, new int[] { 3, 0, 0 } } // Non-upgradable
    };
    private Dictionary<PowerupType, int> powerupLevels = new Dictionary<PowerupType, int>();
    private HashSet<PowerupType> purchasedPowerups = new HashSet<PowerupType>();
    private int maxEquipped = 2;
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

        // Initialize levels
        powerupLevels[PowerupType.ForgivingFlame] = 0;
        powerupLevels[PowerupType.SacredPause] = 0;
        powerupLevels[PowerupType.InkOfConviction] = 0;
        powerupLevels[PowerupType.GuidingLight] = 0;
    }

    public int GetPowerupLevel(PowerupType powerupType)
    {
        return powerupLevels.ContainsKey(powerupType) ? powerupLevels[powerupType] : 0;
    }


    public int GetPowerupCost(PowerupType powerupType, int level)
    {
        if (powerupCosts.ContainsKey(powerupType) && level > 0 && level <= 3)
        {
            return powerupCosts[powerupType][level - 1];
        }
        return 0;
    }

    public bool IsUpgradable(PowerupType powerupType)
    {
        return powerupType == PowerupType.ForgivingFlame || powerupType == PowerupType.SacredPause;
    }

    public bool IsPowerupPurchased(PowerupType powerupType)
    {
        return GetPowerupLevel(powerupType) > 0;
    }

    public bool IsMaxLevel(PowerupType powerupType)
    {   
        if (!IsUpgradable(powerupType) && IsPowerupPurchased(powerupType))
        {
            return true; // Non-upgradable are maxed if purchased
        }
        return GetPowerupLevel(powerupType) >= 3;
    }

    public bool BuyPowerup(PowerupType powerupType)
    {
        int currentLevel = GetPowerupLevel(powerupType);

        // Already maxed out
        if (currentLevel >= 3)
        {
            Debug.Log($"{powerupType} is already maxed out!");
            return false;
        }

        int nextLevel = currentLevel + 1;
        int cost = GetPowerupCost(powerupType, nextLevel);

        // Check if player can afford
        if (!GraceManager.Instance.CanAfford(cost))
        {
            Debug.Log("Not enough grace!");
            return false;
        }

        // Spend grace and increase level
        GraceManager.Instance.Spend(cost);
        powerupLevels[powerupType] = nextLevel;
        Debug.Log($"{powerupType} upgraded to level {nextLevel}!");
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
        }
        else if (equipmentSlots[1].sprite == null)
        {
            equipmentSlots[1].sprite = powerupIcon;
            equipmentSlots[1].color = Color.white;
            equippedPowerups[1] = powerupType;
            isEquipped[powerupType] = true;
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
                return;
            }
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