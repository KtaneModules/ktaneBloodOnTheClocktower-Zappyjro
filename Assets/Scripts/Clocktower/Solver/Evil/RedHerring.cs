using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Evil
{
    public class RedHerring : Character
    {
        public RedHerring(int deathDay = -1, int deathNight = -1,  DeathMethod deathMethod = DeathMethod.Alive) : base("redHerring", deathDay, deathNight, deathMethod, false)
        {
            return;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            throw new InvalidOperationException();
        }
    }
}