using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatBallCatchStrategy : ICatchStrategy
{
    public double BaseRateCalculation(Fakemon fakemon)
    {
        return (3 * fakemon.MaxHp - 2 * fakemon.HP) * fakemon.Base.CatchRate * (1/2) * ConditionDB.GetStatusBonus(fakemon.Status) / (3 * fakemon.MaxHp);
    }
}
