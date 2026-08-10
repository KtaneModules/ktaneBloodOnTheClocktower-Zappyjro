using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Empath : Character
    {
        private readonly List<int> info;

        public Empath(List<int> info, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("empath", deathDay, deathNight, deathMethod, isYou)
        {
            this.info = info;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();

            for (int i = 0; i < dayCount; i++)
            {
                if (i > info.Count -1)
                {
                    misinfoDays.Add(false);
                }
                else
                {
                    misinfoDays.Add(IsInfoWrong(grim, i));
                }
            }

            return misinfoDays;
        }

        private bool IsInfoWrong(List<Character> grim, int currentDay)
        {
            var myIndex = grim.IndexOf(this);
            string nextLiving = "";
            string previousLiving = "";

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

            for (int i = 0; i < grim.Count; i++) // MAKE THIS CHECK IF THEY ARE DEAD RN
            {
                var next = grim[(myIndex + i + 1) % grim.Count];
                if (next.DeathDay == -1 && next.DeathNight == -1 ||
                    next.DeathDay >= currentDay ||
                    next.DeathNight > currentDay)
                {
                    nextLiving = next.CharacterId;
                    break;
                }
            }

            for (int i = 0; i < grim.Count; i++)
            {
                var index = (myIndex - i - 1) % grim.Count;
                var prev = grim[index < 0 ? index + grim.Count : index];
                if (prev.DeathDay == -1 && prev.DeathNight == -1 ||
                    prev.DeathDay >= currentDay ||
                    prev.DeathNight > currentDay)
                {
                    previousLiving = prev.CharacterId;
                    break;
                }
            }

            if (info[currentDay] == 0)
            {
                return evils.Contains(nextLiving) || evils.Contains(previousLiving);
            }
            else if (info[currentDay] == 1)
            {
                if (maybeEvils.Contains(nextLiving) || maybeEvils.Contains(previousLiving))
                {
                    return false;
                }
                else if (evils.Contains(nextLiving) ^ evils.Contains(previousLiving)) 
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return !((evils.Contains(nextLiving) || maybeEvils.Contains(nextLiving)) && (evils.Contains(previousLiving) || maybeEvils.Contains(previousLiving)));
            }
        }
    }
}