using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class Slayer : Character
    {
        private readonly int playerShot;
        
        private readonly int dayShot;

        public Slayer(int playerShot = -1, int dayShot = -1, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("slayer", deathDay, deathNight, deathMethod, isYou)
        {
            this.playerShot = playerShot;
            this.dayShot = dayShot;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            var evils = new List<string>
            {
                "baron",
                "imp",
                "scarletWoman",
                "poisoner",
                "spy",
            };
            
            for (int i = 0; i < dayCount; i++)
            {
                if (i != dayShot)
                {
                    misinfoDays.Add(false);
                }
                else
                {
                    var passed = false;
                    for (int j = 0; j < grim.Count; j++)
                    {
                        if (grim[j].CharacterId == "imp" 
                            && ((grim[j].DeathNight != -1 && grim[j].DeathNight <= i)
                            || (grim[j].DeathDay != -1 && grim[j].DeathDay < i)))
                        {
                            passed = true;
                        }
                    }

                    if (!passed)
                    {
                        misinfoDays.Add(grim[playerShot].DeathMethod != DeathMethod.Slayer && grim[playerShot].CharacterId == "imp");
                    }
                    else
                    {
                        misinfoDays.Add(grim[playerShot].DeathMethod != DeathMethod.Slayer && evils.Contains(grim[playerShot].CharacterId));
                    }
                }
            }

            return misinfoDays;
        }
    }
}