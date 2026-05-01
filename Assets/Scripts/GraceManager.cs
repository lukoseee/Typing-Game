using UnityEngine;
using System;

//manages player's grace currency, allows other scripts to check balance, spend, and earn grace, and notifies listeners of changes to grace amount
public class GraceManager : MonoBehaviour
{   
    public static GraceManager Instance { get; private set; }

    [SerializeField] private int graceAmount = 99;
    public event Action<int> OnGraceChanged;

    private void Awake()
    {
         if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int getGraceAmount()
    {
        return graceAmount;
    }
    
    public bool CanAfford(int amount)
    {
        return graceAmount >= amount;
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount))
            return false;

        graceAmount -= amount;
        OnGraceChanged?.Invoke(graceAmount); //notify listeners of change
        return true;
    }

    public void Add(int amount)
    {
        graceAmount += amount;
        OnGraceChanged?.Invoke(graceAmount);
    }
}
