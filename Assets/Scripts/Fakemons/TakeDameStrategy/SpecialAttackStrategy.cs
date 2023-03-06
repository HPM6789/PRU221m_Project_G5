using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Assets.Scripts.Fakemons.TakeDameStrategy
{
    public class SpecialAttackStrategy : ITakeDamageStrategy
    {
        public int DamageCalculation(Move move, Fakemon attacker, Fakemon defender)
        {
            float attack = attacker.SpAttack;
            float defense = defender.SpDefense;

            float modifier = UnityEngine.Random.Range(0.85f, 1f);
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * ((float)attack / defense) + 2;


            int damage = Mathf.FloorToInt(d * modifier);

            return damage;
        }
    }
}
