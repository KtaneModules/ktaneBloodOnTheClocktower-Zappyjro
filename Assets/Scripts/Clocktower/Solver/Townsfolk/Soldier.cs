using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Soldier : Character
    {
        public Soldier(int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("soldier", deathDay, deathNight, deathMethod, isYou)
        {
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            
            for (int i = 0; i < dayCount; i++)
            {
                misinfoDays.Add(DeathNight == i);
            }

            return misinfoDays;
        }
    }
}