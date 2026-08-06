using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ClocktowerSolverTB.Townsfolk
{
    public class Chef : Character
    {
        private readonly int PairCount;

        public Chef(int pairCount, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("chef", deathDay, deathNight, deathMethod, isYou)
        {
            this.PairCount = pairCount;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            if (CheckPairCount(grim, PairCount))
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

        private bool CheckPairCount(List<Character> grim, int pairCount)
        {
            var currentCount = 0;
            var currentMaybes = 0;
            var evils = new List<string>
            {
                "baron",
                "imp",
                "scarletWoman",
                "poisoner"
            };
            var maybeEvils = new List<string>
            {
                "spy",
                "recluse"
            };

            for (int i = 0; i < grim.Count; i++)
            {
                var current = grim[i].CharacterId;
                var next = grim[(i+1) % grim.Count].CharacterId;

                if (evils.Contains(current) && evils.Contains(next))
                {
                    currentCount++;
                }
                else if ((evils.Contains(current) || maybeEvils.Contains(current)) && (evils.Contains(next) || maybeEvils.Contains(next)))
                {
                    currentMaybes++;
                }
            }

            return pairCount >= currentCount && pairCount <= currentCount + currentMaybes;
        }
    }
}