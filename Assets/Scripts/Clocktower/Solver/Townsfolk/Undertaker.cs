using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ClocktowerSolverTB.Townsfolk
{
    public class Undertaker : Character
    {
        private readonly List<string> info;

        public List<int> notDrunk;

        public bool pingedDrunk = false;

        public List<int[]> drunkPlayerAndNight;

        public Undertaker(List<string> info, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("undertaker", deathDay, deathNight, deathMethod, isYou)
        {
            this.info = info;
            notDrunk = new List<int>();
            drunkPlayerAndNight = new List<int[]>();
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            notDrunk = new List<int>();
            drunkPlayerAndNight = new List<int[]>();

            var evils = new List<string>
            {
                "baron",
                "imp",
                "scarletWoman",
                "poisoner",
                "spy",
            };

            misinfoDays.Add(false); // No info day 1

            for (int i = 0; i < dayCount -1; i++)
            {
                try
                {
                    if (info[i] == string.Empty)
                    {
                        misinfoDays.Add(false);
                    }
                    else
                    {
                        var executed = grim.First(x => x.DeathDay == i);
                        if (info[i] == "drunk")
                        {
                            drunkPlayerAndNight.Add(new int[] { grim.IndexOf(executed), i + 1 });
                            if (executed.CharacterId != "spy")
                            {
                                pingedDrunk = true;
                            }

                            misinfoDays.Add(false);
                        }
                        else
                        {
                            if (executed.CharacterId == info[i])
                            {
                                misinfoDays.Add(false);
                            }
                            else if (evils.Contains(info[i]))
                            {
                                misinfoDays.Add(executed.CharacterId != "recluse");
                            }
                            else
                            {
                                misinfoDays.Add(executed.CharacterId != "spy");
                            }
                        }
                    }
                }
                catch
                {
                    misinfoDays.Add(false);
                }

                if (misinfoDays.Last() == false && i < info.Count && info[i] != "drunk" && info[i] != "")
                {
                    notDrunk.Add(grim.IndexOf(grim.First(x => x.DeathDay == i)));
                }
            }

            return misinfoDays;
        }
    }
}