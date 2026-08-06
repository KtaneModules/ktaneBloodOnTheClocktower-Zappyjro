using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Investigator : Character
    {
        private readonly int info1;
        private readonly int info2;
        private readonly string infoCharacter;

        public Investigator(int info1, int info2, string infoCharacter, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("investigator", deathDay, deathNight, deathMethod, isYou)
        {
            this.info1 = info1;
            this.info2 = info2;
            this.infoCharacter = infoCharacter;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            if (
                grim[info1].CharacterId == infoCharacter
                || grim[info2].CharacterId == infoCharacter
                || grim[info1].CharacterId == "recluse"
                || grim[info2].CharacterId == "recluse")
            {
                misinfoDays.Add(false);
            }
            else
            {
                misinfoDays.Add(true);
            }

            while (misinfoDays.Count < dayCount)
            {
                misinfoDays.Add(false);
            }

            return misinfoDays;
        }
    }
}