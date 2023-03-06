using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fakemons.TakeDameStrategy
{
    public interface ITakeDamageStrategy
    {
        int DamageCalculation(Move move, Fakemon attacker, Fakemon defender);
    }
}
