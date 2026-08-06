using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Virgin : Character
    {
        private readonly int dayNommed;
        private readonly int playerNommed;

        public Virgin(int dayNommed = -1, int playerNommed = -1, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("virgin", deathDay, deathNight, deathMethod, isYou)
        {
            this.dayNommed = dayNommed;
            this.playerNommed = playerNommed;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            
            for (int i = 0; i < dayCount; i++)
            {
                if (i != dayNommed)
                {
                    misinfoDays.Add(false);
                }
                else
                {
                    misinfoDays.Add(grim[playerNommed].DeathMethod != DeathMethod.Virgin && IsTownsfolk(grim[playerNommed]));
                }
            }

            return misinfoDays;
        }

        private bool IsTownsfolk(Character character)
        {
            var townsfolk = new List<string>
            {
                "washerwoman",
                "librarian",
                "investigator",
                "chef",
                "empath",
                "fortuneTeller",
                "undertaker",
                "monk",
                "ravenkeeper",
                "virgin",
                "slayer",
                "soldier",
                "mayor",
            };

            return townsfolk.Contains(character.CharacterId);
        }
    }
}