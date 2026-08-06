using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Monk : Character
    {
        private readonly List<int> playersChosen;

        public Monk(List<int> playersChosen, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("monk", deathDay, deathNight, deathMethod, isYou)
        {
            this.playersChosen = playersChosen;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            misinfoDays.Add(false); // No day 1

            var currentNight = 0;
            foreach (int player in playersChosen)
            {
                currentNight++;
                misinfoDays.Add(grim[player].DeathNight == currentNight);
            }

            for (int i = misinfoDays.Count; i < dayCount; i++)
            {
                misinfoDays.Add(false);
            }

            return misinfoDays;
        }
    }
}