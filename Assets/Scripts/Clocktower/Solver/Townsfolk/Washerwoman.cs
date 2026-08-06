using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Washerwoman : Character
    {
        public readonly int info1;
        public readonly int info2;
        public readonly string infoCharacter;

        public Washerwoman(int info1, int info2, string infoCharacter, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("washerwoman", deathDay, deathNight, deathMethod, isYou)
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
                || grim[info1].CharacterId == "spy"
                || grim[info2].CharacterId == "spy")
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