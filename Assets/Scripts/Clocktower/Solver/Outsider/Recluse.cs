using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Outsider
{
    public class Recluse : Character
    {
        public Recluse(int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("recluse", deathDay, deathNight, deathMethod, isYou)
        {
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