using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeBallCatchStrategy : ICatchStrategy
{
    public double BaseRateCalculation(Fakemon fakemon)
    {
        return (3 * fakemon.MaxHp - 2 * fakemon.HP) * fakemon.Base.CatchRate * 1 * ConditionDB.GetStatusBonus(fakemon.Status) / (3 * fakemon.MaxHp);
    }
}
