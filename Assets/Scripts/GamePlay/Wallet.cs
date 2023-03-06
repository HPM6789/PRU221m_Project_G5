using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour, ISavable
{
    [SerializeField] float money;
    public static Wallet Instance { get; private set; }
    public float Money => money;
    public event Action OnMoneyChange;
    
    private void Awake()
    {
        Instance = this;
    }
    public void AddMoney(float amount)
    {
        money += amount;
        OnMoneyChange?.Invoke();
    }
    public void TakeMoney(float amount)
    {
        money -= amount;
        OnMoneyChange?.Invoke();
    }
    public bool HasMoney(float amount)
    {
        return amount <= money;
    }

    public object CaptureState()
    {
        return money;
    }

    public void RestoreState(object state)
    {
       money = (float)state;
    }
}
