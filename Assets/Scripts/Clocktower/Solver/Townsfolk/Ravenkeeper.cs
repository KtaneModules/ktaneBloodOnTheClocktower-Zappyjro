using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Ravenkeeper : Character
    {
        public readonly string characterSaw;
        public readonly int playerSaw;

        public int seenDrunk = -1;

        public Ravenkeeper(string characterSaw = "", int playerSaw = -1, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("ravenkeeper", deathDay, deathNight, deathMethod, isYou)
        {
            this.characterSaw = characterSaw;
            this.playerSaw = playerSaw;

            if (characterSaw == "drunk")
            {
                seenDrunk = playerSaw;
            }
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            
            for (int i = 0; i < dayCount; i++)
            {
                if (i != DeathNight || characterSaw == "drunk")
                {
                    misinfoDays.Add(false);
                }
                else
                {
                    if (grim[playerSaw].CharacterId == "spy")
                    {
                        misinfoDays.Add(!IsGood(grim[playerSaw]));
                    } 
                    else if (grim[playerSaw].CharacterId == "recluse")
                    {
                        misinfoDays.Add(!IsEvil(grim[playerSaw]));
                    }
                    else
                    {
                        misinfoDays.Add(grim[playerSaw].CharacterId != characterSaw);
                    }
                }
            }

            return misinfoDays;
        }

        private bool IsGood(Character character)
        {
            var good = new List<string>
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
                "drunk",
                "butler",
                "saint",
                "recluse",
                "spy",
            };

            return good.Contains(character.CharacterId);
        }

        private bool IsEvil(Character character)
        {
            var evil = new List<string>
            {
                "scarletwoman",
                "poisoner",
                "baron",
                "spy",
                "imp",
                "recluse",
            };

            return evil.Contains(character.CharacterId);
        }
    }
}