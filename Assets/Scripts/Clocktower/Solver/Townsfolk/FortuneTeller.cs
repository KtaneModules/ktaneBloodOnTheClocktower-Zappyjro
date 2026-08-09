using System;
using System.Collections.Generic;
using System.Linq;
namespace ClocktowerSolverTB.Townsfolk
{
    public class FortuneTeller : Character
    {
        public readonly List<int[]> playersChosen;
        public readonly List<bool> results;

        public FortuneTeller(List<int[]> playersChosen, List<bool> results, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
            : base("fortuneTeller", deathDay, deathNight, deathMethod, isYou)
        {
            this.playersChosen = playersChosen;
            this.results = results;
        }

        public override List<bool> GetMisinfoDays(List<Character> grim, int dayCount)
        {
            var misinfoDays = new List<bool>();
            
            for (int i = 0; i < dayCount; i++)
            {
                var starpassed = false;
                for (int j = 0; j < grim.Count; j++)
                {
                    if (grim[j].CharacterId == "imp" && grim[j].DeathNight != -1 && grim[j].DeathNight <= i)
                    {
                        starpassed = true;
                    }
                }

                var dayDied = false;
                for (int j = 0; j < grim.Count; j++)
                {
                    if (grim[j].CharacterId == "imp" && grim[j].DeathDay != -1 && grim[j].DeathDay < i)
                    {
                        dayDied = true;
                    }
                }

                try
                {
                    if (
                        grim[playersChosen[i][0]].CharacterId == "imp" 
                        || grim[playersChosen[i][1]].CharacterId == "imp"
                        || grim[playersChosen[i][0]].CharacterId == "redHerring"
                        || grim[playersChosen[i][1]].CharacterId == "redHerring")
                    {
                        misinfoDays.Add(!results[i]);
                    }
                    else if (starpassed && 
                        (grim[playersChosen[i][0]].CharacterId == "baron" || 
                        grim[playersChosen[i][1]].CharacterId == "baron" ||
                        grim[playersChosen[i][0]].CharacterId == "spy" || 
                        grim[playersChosen[i][1]].CharacterId == "spy" ||
                        grim[playersChosen[i][0]].CharacterId == "scarletWoman" || 
                        grim[playersChosen[i][1]].CharacterId == "scarletWoman" ||
                        grim[playersChosen[i][0]].CharacterId == "poisoner" || 
                        grim[playersChosen[i][1]].CharacterId == "poisoner"))
                    {
                        misinfoDays.Add(!results[i]);
                    }
                    else if (dayDied && 
                        (grim[playersChosen[i][0]].CharacterId == "scarletWoman" || 
						grim[playersChosen[i][1]].CharacterId == "scarletWoman"))
                    {
                        misinfoDays.Add(!results[i]);
                    }
                    else
                    {
                        // Check if in red herring calc
                        var herring = -1;
                        for (int j = 0; j < grim.Count; j++)
                        {
                            if (grim[j].CharacterId == "redHerring")
                            {
                                herring = j;
                                break;
                            }
                        }

                        if (herring == -1)
                        {
                            misinfoDays.Add(false);
                        }
                        else
                        {
                            misinfoDays.Add(results[i]);
                        }
                    }
                }
                catch
                {
                    misinfoDays.Add(false); // They dead
                }
            }

            return misinfoDays;
        }
    }
}