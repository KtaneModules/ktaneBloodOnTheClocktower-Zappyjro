using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Librarian : Character
    {
        public readonly int info1;
        public readonly int info2;
        public readonly string infoCharacter;

        public Librarian(int info1, int info2, string infoCharacter, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("librarian", deathDay, deathNight, deathMethod, isYou)
        {
            this.info1 = info1;
            this.info2 = info2;
            this.infoCharacter = infoCharacter;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            if (info1 == -1 && info2 == -1) //Librarian 0
            {
                if (CountOutsiders(grim) != 0)
                {
                    misinfoDays.Add(true);
                }
                else
                {
                    misinfoDays.Add(false);
                }
            }
            else
            {
                if (
                    grim[info1].CharacterId == infoCharacter
                    || grim[info2].CharacterId == infoCharacter
                    || grim[info1].CharacterId == "spy"
                    || grim[info2].CharacterId == "spy"
                    || infoCharacter == "drunk")
                {
                    misinfoDays.Add(false);
                }
                else
                {
                    misinfoDays.Add(true);
                }
            }

            while (misinfoDays.Count < dayCount)
            {
                misinfoDays.Add(false);
            }

            return misinfoDays;
        }

        private static int CountOutsiders(List<Character> world)
        {
            var count = 0;

            foreach (Character character in world)
            {
                if (IsOutsider(character))
                {
                    count += 1;
                }
            }

            return count;
        }

        private static bool IsOutsider(Character character)
        {
            return character.CharacterId == "saint" || character.CharacterId == "recluse" || character.CharacterId == "butler";
        }
    }
}