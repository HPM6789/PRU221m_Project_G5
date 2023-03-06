using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB 
{
    public static void Init()
    {
        foreach(var item in Conditions)
        {
            var conditionId = item.Key;
            var condition = item.Value;

            condition.Id = conditionId;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name = "Poison",
                StartMassage = "has been poisoned",
                OnAfterTurn =(Fakemon fakemon) =>
                {
                    fakemon.DecreaseHP(fakemon.MaxHp/8);
                    fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} hurt itself due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMassage = "has been burned",
                OnAfterTurn =(Fakemon fakemon) =>
                {
                    fakemon.DecreaseHP(fakemon.MaxHp/16);
                    fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} hurt itself due to ");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyzed",
                StartMassage = "has been paralyzed",
                OnBeforeMove = (Fakemon fakemon) =>
                {
                    if(Random.Range(1,5) == 1)
                    {
                        fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name}'s paralyzed and can't move");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Freeze",
                StartMassage = "has been frozen",
                OnBeforeMove = (Fakemon fakemon) =>
                {
                    if(Random.Range(1,5) == 1)
                    {
                        fakemon.CureStatus();
                        fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} is not frozen anymore");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name = "Sleep",
                StartMassage = "has fallen asleep",
                OnStart = (Fakemon fakemon) =>
                {
                    fakemon.StatusTime = Random.Range(1,4);
                    Debug.Log($"Will be asleep for {fakemon.StatusTime} moves");
                },
                OnBeforeMove = (Fakemon fakemon) =>
                {
                    if(fakemon.StatusTime <= 0)
                    {
                        fakemon.CureStatus();
                        fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} woke up!");
                        return true;
                    }
                    fakemon.StatusTime--;
                    fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} is sleeping");
                    return false;
                }
            }
        },
        {
            ConditionID.confusion,
            new Condition()
            {
                Name = "Confusion",
                StartMassage = "has been confused",
                OnStart = (Fakemon fakemon) =>
                {
                    fakemon.VolatileStatusTime = Random.Range(1,5);
                    Debug.Log($"Will be confused for {fakemon.VolatileStatusTime} moves");
                },
                OnBeforeMove = (Fakemon fakemon) =>
                {
                    if(fakemon.VolatileStatusTime <= 0)
                    {
                        fakemon.CureVolatileStatus();
                        fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} kicked out of confusion!");
                        return true;
                    }
                    fakemon.VolatileStatusTime--;
                    //50% chance to do a move
                    if(Random.Range(1,4) == 1)
                        return true;

                    //Hurt by itself
                    fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} is confused");
                    fakemon.DecreaseHP(fakemon.MaxHp / 8);
                    fakemon.StatusChanges.Enqueue($"{fakemon.Base.Name} hurt itself fue to confusion");
                    return false;
                }
            }
        },
    };
    
    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if(condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
        {
            return 2f;
        }
        else
        {
            return 1.5f;
        } 
    }
}
public enum ConditionID
{
    none, psn, brn, slp, par, frz, confusion
}