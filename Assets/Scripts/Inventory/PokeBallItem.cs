using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new PokeBall")]
public class PokeBallItem : ItemBase
{
    [SerializeField] float catchrateModifier = 1;
    private ICatchStrategy catchStrategy;
    public override bool Use(Fakemon fakemon)
    {
        return true;
    }

    public void SetCatchStrategy(ICatchStrategy value)
    {
        catchStrategy = value;
    }

    public double GetCatchBaseRate(Fakemon fakemon)
    {
        return catchStrategy.BaseRateCalculation(fakemon);
    }
    public override bool CanUseOutSide => false;
    public float CatchRateModifier => catchrateModifier;
}
