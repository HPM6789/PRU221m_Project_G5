using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Create new Evolution Item")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Fakemon fakemon)
    {
        return true;
    }
}
