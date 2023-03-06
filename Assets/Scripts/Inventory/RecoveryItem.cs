using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new Recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;

    public override bool Use(Fakemon fakemon)
    {
        //Revive
        if(revive || maxRevive)
        {
            if (fakemon.HP > 0) return false;
            if (revive)
            {
                fakemon.IncreaseHP(fakemon.MaxHp / 2);
            }
            else if (maxRevive)
            {
                fakemon.IncreaseHP(fakemon.MaxHp);
            }

            fakemon.CureStatus();

            return true;
        }

        //No Item can use in fainteeed fakemon
        if (fakemon.HP == 0) return false;

        //Restore HP
       if( restoreMaxHP || hpAmount > 0)
        {
            if (fakemon.HP == fakemon.MaxHp)
                return false;

            if (restoreMaxHP)
                fakemon.IncreaseHP(fakemon.MaxHp);
            else
                fakemon.IncreaseHP(hpAmount);
        }

       //Recover Status
       if(recoverAllStatus || status != ConditionID.none)
        {
            if(fakemon.Status == null && fakemon.VolatileStatus == null)
            {
                return false;
            }

            if (recoverAllStatus)
            {
                fakemon.CureStatus();
                fakemon.CureVolatileStatus();
            }
            else
            {
                if (fakemon.Status == null && fakemon.VolatileStatus == null) return false;
                    if (fakemon.Status != null)
                {
                    if (fakemon.Status.Id == status)
                    {
                        fakemon.CureStatus();
                    }
                }
                
                if(fakemon.VolatileStatus != null)
                {
                    if (fakemon.VolatileStatus.Id == status)
                        fakemon.CureVolatileStatus();
                }
                
                
            }
        }

        //Restore PP
        if (restoreMaxPP)
        {
            fakemon.Moves.ForEach(m => m.IncreasePP(m.Base.Pp));
        }else if(ppAmount > 0)
        {
            fakemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }
        return true;
    }
}
