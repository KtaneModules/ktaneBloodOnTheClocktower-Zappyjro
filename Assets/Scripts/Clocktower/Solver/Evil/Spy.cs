using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Evil
{
    public class Spy : Character
    {
        public Spy(int deathDay = -1, int deathNight = -1,  DeathMethod deathMethod = DeathMethod.Alive) : base("spy", deathDay, deathNight, deathMethod, false)
        {
            return;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            for (int i = 0; i < dayCount; i++)
            {
                misinfoDays.Add(false);
            }

            return misinfoDays;
        }
    }
}