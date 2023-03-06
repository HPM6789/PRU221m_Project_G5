using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new TM")]
public class TmItem : ItemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $": {move.Name}";
    public override bool Use(Fakemon fakemon)
    {
        return fakemon.HasMove(move);
    }

    public bool CanBeTaught(Fakemon fakemon)
    {
        return fakemon.Base.LearnableByItems.Contains(Move);
    }

    public override bool IsReusable => isHM;
    public override bool CanUseInBattle => false;
    public MoveBase Move => move;
    public bool IsHM => isHM;
}
