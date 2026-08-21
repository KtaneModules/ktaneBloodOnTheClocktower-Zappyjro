using System;
using System.Collections.Generic;
using System.Linq;
using ClocktowerSolverTB.Evil;
using ClocktowerSolverTB.Outsider;
using ClocktowerSolverTB.Townsfolk;

public static class ClocktowerSolver
{
    public static SolverResult Solve(GameDto game)
    {
        List<Character> grim = game.Characters
            .Select(CreateCharacter)
            .ToList();

        int days = game.Days;

        var worlds = new List<List<Character>>();
        var reasons = new List<string>();

        if (game.DemonPlayer == null)
        {
            worlds = GenerateAllWorlds(grim);
        }
        else
        {
            if (game.DemonPlayer == game.MinionPlayer)
            {
                reasons.Add("The minion and the demon cannot be the same player");
            }

            var world = new List<Character>();
            for (int i = 0; i < grim.Count; i++)
            {
                if (i == game.DemonPlayer)
                {
                    world.Add(new Imp(grim[i].DeathDay,grim[i].DeathNight,grim[i].DeathMethod));
                }
                else if (i == game.MinionPlayer)
                {
                    if (game.MinonType == "baron")
                    {
                        world.Add(new Baron(grim[i].DeathDay,grim[i].DeathNight,grim[i].DeathMethod));
                    }
                    else if (game.MinonType == "poisoner")
                    {
                        world.Add(new Poisoner(grim[i].DeathDay,grim[i].DeathNight,grim[i].DeathMethod));
                    }
                    else if (game.MinonType == "scarletWoman")
                    {
                        world.Add(new ScarletWoman(grim[i].DeathDay,grim[i].DeathNight,grim[i].DeathMethod));
                    }
                    else if (game.MinonType == "spy")
                    {
                        world.Add(new Spy(grim[i].DeathDay,grim[i].DeathNight,grim[i].DeathMethod));
                    }
                }
                else
                {
                    world.Add(grim[i]);
                }
            }

            reasons = BasicReasons(world, reasons);

            worlds.Add(world);
        }

        var validWorlds = new List<List<Character>>();
        foreach (List<Character> world in worlds)
        {
            if (!TestMisinfo(world, days, reasons))
            {
                continue;
            }

            var invalidShot = false;
            foreach (Character player in world)
            {
                if (player.CharacterId != "imp" && player.CharacterId != "recluse" && player.DeathMethod == DeathMethod.Slayer)
                {
                    invalidShot = true;
                    break;
                }
            }
            if (invalidShot)
            {
                reasons.Add("The slayer's shot can only kill the imp or the recluse.");
                continue;
            }

            // Test for bad red herring
            var foundValid = true;
            for (int i = 0; i < world.Count; i++)
            {
                if (world[i].CharacterId == "fortuneTeller")
                {
                    foundValid = false;
                    for (int j = 0; j < world.Count; j++) // Try each person (j) as red herring
                    {
                        if (world[j].CharacterId == "imp" || 
                            world[j].CharacterId == "poisoner" ||
                            world[j].CharacterId == "scarletWoman" ||
                            world[j].CharacterId == "baron")
                        {
                            continue;
                        }

                        var fakeGrim = new List<Character>(world);
                        fakeGrim[j] = new RedHerring();

                        var newMisinfo = world[i].GetMisinfoDays(fakeGrim, days);

                        if (TestMisinfo(world, days, new List<string>(), newMisinfo))
                        {
                            foundValid = true;
                            break;
                        }
                    }
                }
            }

            if (!foundValid)
            {
                reasons.Add("There is not a good player who can be the red herring for the fortune teller.");
                continue;
            }

            validWorlds.Add(world);
        }

        var output = new List<List<string>>();

        if (game.DemonPlayer == null)
        {
            foreach(var valid in validWorlds)
            {
                var validWorld = new List<string>();
                foreach(var character in valid)
                {
                    validWorld.Add(character.CharacterId);
                }

                output.Add(validWorld);
            }
        }
        else
        {
            if (reasons.Count == 0)
            {
                output.Add(new List<string> { "The world is valid" });
            }
            else
            {
                output.Add(reasons);
            }
        }

        return new SolverResult(output, reasons);
    }

    private static bool TestMisinfo(List<Character> world, int days, List<string> reasons, List<bool> ftInfo = null)
    {
        var nightValues = new List<int>();
        for (int i = 0; i < days; i++)
        {
            nightValues.Add(0);
        }

        var poisoner = false;
        var librarianSawDrunk = false;
        var libInfo1 = -1;
        var libInfo2 = -1;
        var undertakerSawDrunk = false;
        var undertakeDrunkNights = new List<int[]>();
        var washerwomanCanBePoinsonedOne = false;
        var washInfo1 = -1;
        var washInfo2 = -1;
        var washSeenSide = -1;
        var ravenDrunkPlayer = -1;
        var ravenDeathNight = -1;
        var ravenCanBePoisoned = false;
        var drunkID = "";

        var drunk = false;
        for (int i = 0; i < world.Count; i++)
        {
            if (world[i].CharacterId == "drunk")
            {
                drunk = true;
            }
        }

        if (!drunk)
        {
            drunk = GetExpectedOutsiderCount(world) != CountOutsiders(world) && ftInfo == null;
        }

        var characterFails = new List<List<bool>>();
        foreach (Character character in world)
        {
            if (character.CharacterId == "poisoner")
            {
                poisoner = true;
            }

            Librarian librarian = character as Librarian;
            if (librarian != null
                && librarian.infoCharacter == "drunk"
                && world[librarian.info1].CharacterId != "spy"
                && world[librarian.info2].CharacterId != "spy")
            {
                librarianSawDrunk = true;
                libInfo1 = librarian.info1;
                libInfo2 = librarian.info2;
            }

            Undertaker undertaker = character as Undertaker;
            if (undertaker != null)
            {
                undertakerSawDrunk = undertaker.pingedDrunk;
                undertakeDrunkNights = undertaker.drunkPlayerAndNight;
            }

            Ravenkeeper ravenkeeper = character as Ravenkeeper;
            if (ravenkeeper != null)
            {
                ravenDeathNight = ravenkeeper.DeathNight;
                ravenDrunkPlayer = ravenkeeper.seenDrunk;
            }

            Washerwoman washerwoman = character as Washerwoman;
            if (washerwoman != null)
            {
                washInfo1 = washerwoman.info1;
                washInfo2 = washerwoman.info2;

                if (world[washInfo1].CharacterId == "spy")
                {
                    washSeenSide = 0;
                }
                else if (world[washInfo2].CharacterId == "spy")
                {
                    washSeenSide = 1;
                }
                else if (world[washInfo1].CharacterId == washerwoman.infoCharacter)
                {
                    washSeenSide = 0;
                }
                else if (world[washInfo2].CharacterId == washerwoman.infoCharacter)
                {
                    washSeenSide = 1;
                }
            }

            var characterDays = character.GetMisinfoDays(world, days);

            if (character.CharacterId == "fortuneTeller" && ftInfo != null)
            {
                characterDays = ftInfo;
            }

            characterFails.Add(characterDays);
            for (int i = 0; i < days; i++)
            {
                if (characterDays[i])
                {
                    nightValues[i]++;
                }
            }
        }

        if (!drunk && !poisoner)
        {
            if ((librarianSawDrunk && world[libInfo1].CharacterId != "spy" && world[libInfo2].CharacterId != "spy") || 
                undertakerSawDrunk ||
                (ravenDrunkPlayer != -1 && world[ravenDrunkPlayer].CharacterId != "spy"))
            {
                reasons.Add("A good player saw that there was a drunk in play, with no drunk (outsider count) or poisoner to provide misinformation this is impossible.");
                return false;
            }

            if (nightValues.Sum() != 0)
            {
                reasons.Add("A good player received misinformation on night "+(nightValues.IndexOf(nightValues.First(n => n > 0))+1)+". With no drunk (outsider count) or poisoner to provide misinformation this is impossible.");
                return false;
            }
        }
        else if (drunk && !poisoner)
        {
            var failedCharacters = 0;
            var undertakerConf = new List<int>();

            for (int i = 0; i < world.Count; i++)
            {
                Undertaker undertaker = world[i] as Undertaker;
                if (undertaker != null)
                {
                    undertakerConf = undertaker.notDrunk;
                }
            }

            for (int i = 0; i < characterFails.Count; i++)
            {
                for (int j = 0; j < characterFails[i].Count; j++)
                {
                    if (characterFails[i][j])
                    {
                        failedCharacters++;
                        drunkID = world[i].CharacterId;
                        
                        if (ftInfo == null)
                        {
                            world[i] = new Drunk(world[i].DeathDay, world[i].DeathNight, world[i].DeathMethod, world[i].IsYou);
                        }
                        else
                        {
                            break;
                        }

                        if (librarianSawDrunk && drunkID != "librarian" && i != libInfo1 && i != libInfo2 && world[libInfo1].CharacterId != "spy" && world[libInfo2].CharacterId != "spy")
                        {
                            failedCharacters++;
                        }

                        if (undertakerConf.Contains(i) && drunkID != "undertaker")
                        {
                            failedCharacters++;
                        }

                        for (int l = 0; l < undertakeDrunkNights.Count; l++)
                        {
                            if (undertakeDrunkNights[l][0] != i && world[undertakeDrunkNights[l][0]].CharacterId != "spy")
                            {
                                failedCharacters++;
                                break;
                            }
                        }

                        if (ravenDrunkPlayer != -1 && ravenDrunkPlayer != i && drunkID != "ravenkeeper")
                        {
                            failedCharacters++;
                        }

                        break;
                    }
                }
            }

			if (failedCharacters == 0) {
				var drunkCandidateFound = false;
				if (world.Any (s => s.CharacterId == "drunk")) {
					drunkCandidateFound = true;
				}

				for (int i = 0; i < world.Count; i++) {
					if (librarianSawDrunk && libInfo1 != i && libInfo2 != i && world [i].CharacterId != "librarian") {
						if (world [libInfo1].CharacterId != "spy" && world [libInfo2].CharacterId != "spy") {
							continue;
						}
					}

					if (undertakerConf.Contains (i) && world [i].CharacterId != "undertaker") {
						continue;
					}

					if (undertakerSawDrunk) {
						var underDrunk = -1;
						foreach (int[] pair in undertakeDrunkNights) {
							if (world [pair [0]].CharacterId != "spy") {
								if (underDrunk == -1) {
									underDrunk = pair [0];
								} else {
									underDrunk = -2;
								}
							}
						}

						if (underDrunk == -2) {
							if (world [i].CharacterId != "undertaker") {
								continue;
							}
						} else if (underDrunk != -1 && underDrunk != i && world [i].CharacterId != "undertaker") {
							continue;
						}
					}

					if (ravenDrunkPlayer != i && ravenDrunkPlayer != -1 && world [i].CharacterId != "ravenkeeper") {
						continue;
					}

					if (washSeenSide == 0 && world [i].CharacterId != "washerwoman" && i == washInfo1) {
						continue;
					}
					if (washSeenSide == 1 && world [i].CharacterId != "washerwoman" && i == washInfo2) {
						continue;
					}

					if (world [i].CharacterId != "imp" &&
					                   world [i].CharacterId != "spy" &&
					                   world [i].CharacterId != "baron" &&
					                   world [i].CharacterId != "poisoner" &&
					                   world [i].CharacterId != "scarletWoman" &&
					                   world [i].CharacterId != "recluse" &&
					                   world [i].CharacterId != "saint" &&
					                   world [i].CharacterId != "butler" &&
					                   world [i].CharacterId != "slayer" &&
					                   world [i].CharacterId != "virgin" &&
					                   world [i].DeathMethod != DeathMethod.Virgin &&
					                   ftInfo == null) {
						world [i] = new Drunk (world [i].DeathDay, world [i].DeathNight, world [i].DeathMethod, world [i].IsYou);
						drunkCandidateFound = true;
						break;
					}
				} 
				if (!drunkCandidateFound) {
					for (int i = 0; i < world.Count; i++) {
						if (((world [i].CharacterId == "slayer" && !world.Any (w => w.DeathMethod == DeathMethod.Slayer)) ||
						                      (world [i].CharacterId == "virgin" && !world.Any (w => w.DeathMethod == DeathMethod.Virgin))) &&
						                      world [i].DeathMethod != DeathMethod.Virgin) {
							if (librarianSawDrunk && libInfo1 != i && libInfo2 != i) {
								continue;
							}

							if (undertakerConf.Contains (i)) {
								continue;
							}

							if (ravenDrunkPlayer == i) {
								continue;
							}

							if (ftInfo != null) {
								continue;
							}

							world [i] = new Drunk (world [i].DeathDay, world [i].DeathNight, world [i].DeathMethod, world [i].IsYou);
							drunkCandidateFound = true;
							break;
						}
					} 
				}
				if (!drunkCandidateFound) {
					reasons.Add ("Due to outsider count there must be a drunk in, however there are no valid candidates for this to be.");
					return false;
				}
			} 
			else if (failedCharacters == 1 && ftInfo != null) 
			{
				return false;
			}
            else if (failedCharacters > 1)
            {
                reasons.Add("This has a drunk and no poisoner, yet there is more than 1 player with incorrect information");
                return false;
            }
        }
        else if (!drunk && poisoner)
        {
            if (librarianSawDrunk && world[libInfo1].CharacterId != "spy" && world[libInfo2].CharacterId != "spy")
            {
                nightValues[0]++;
            }
            if (ravenDrunkPlayer != -1 && world[ravenDrunkPlayer].CharacterId != "spy")
            {
                nightValues[ravenDeathNight]++;
            }

            if (undertakerSawDrunk)
            {
                for (int j = 0; j < undertakeDrunkNights.Count; j++)
                {
                    nightValues[undertakeDrunkNights[j][1]]++;
                }
            }

            if (nightValues[0] == 0)
            {
                washerwomanCanBePoinsonedOne = true;
            }

            if (ravenDeathNight != -1 && nightValues[ravenDeathNight] == 0)
            {
                ravenCanBePoisoned = true;
            }

            if (nightValues.Any(x => x > 1))
            {
                reasons.Add("This has "+nightValues.First(n => n > 1)+" players receiving misinformation on night "+(nightValues.IndexOf(nightValues.First(n => n > 1))+1)+". At most 1 of them can be poisoned");
                return false;
            }

            var poisonerInvalid = false;
            for (int i = 0; i < world.Count; i++)
            {
                if (world[i].CharacterId == "imp" || world[i].CharacterId == "poisoner")
                {
                    if (world[i].DeathMethod != DeathMethod.Alive)
                    {
                        if (world.Count <= 9)
                        {
                            for (int j = 0; j < nightValues.Count; j++)
                            {
                                if (world[i].DeathDay != -1 && j > world[i].DeathDay && nightValues[j] > 0)
                                {
                                    poisonerInvalid = true;
                                    continue;
                                }
                                if (world[i].DeathNight != -1 && j >= world[i].DeathNight && nightValues[j] > 0)
                                {
                                    poisonerInvalid = true;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }

            if (poisonerInvalid)
            {
                reasons.Add("This has players still getting incorrect information after the poisoner died or became the new imp");
                return false;
            }
        }
        else
        {
            var comboFound = false;

            for (int i = 0; i < world.Count; i++)
            {
                var newNightValues = new List<int>();
                for (int p = 0; p < days; p++)
                {
                    newNightValues.Add(0);
                }

                if (librarianSawDrunk && world[i].CharacterId != "librarian" && i != libInfo1 && i != libInfo2)
                {
                    if (world[libInfo1].CharacterId != "spy" && world[libInfo2].CharacterId != "spy")
                    {
                        newNightValues[0]++;
                    }
                }

                if (ravenDrunkPlayer != -1 && world[i].CharacterId != "ravenkeeper" && i != ravenDrunkPlayer)
                {
                    if (world[ravenDrunkPlayer].CharacterId != "spy")
                    {
                        newNightValues[ravenDeathNight]++;
                    }
                }

                var undertakerConf = new List<int>();
                var undertakerSeat = -1;

                for (int k = 0; k < world.Count; k++)
                {
                    Undertaker undertaker = world[k] as Undertaker;
                    if (undertaker != null)
                    {
                        undertakerSeat = k;
                        undertakerConf = undertaker.notDrunk;
                    }
                }

                if (undertakerSawDrunk && undertakerSeat != i)
                {
                    foreach (int[] playerThenNight in undertakeDrunkNights)
                    {
                        if (playerThenNight[0] != i)
                        {
                            newNightValues[playerThenNight[1]]++;
                        }
                    }
                }

                if (undertakerConf.Contains(i) && undertakerSeat != i)
                {
                    newNightValues[world[i].DeathDay+1]++;
                }

                for (int k = 0; k < world.Count; k++)
                {
                    if (world[k].CharacterId == "washerwoman")
                    {
                        if (characterFails[k][0])
                        {
                            washerwomanCanBePoinsonedOne = true;
                        }
                    }
                }

                var allOthers = characterFails.Where((x, index) => index != i).ToList();
                for (int k = 0; k < allOthers.Count; k++)
                {
                    for (int j = 0; j < allOthers[k].Count; j++)
                    {
                        if (allOthers[k][j])
                        {
                            newNightValues[j]++;
                        }
                    }
                }

                if (newNightValues.Any(x => x > 1))
                {
                    continue;
                }
                else
                {
                    if (newNightValues[0] == 0)
                    {
                        washerwomanCanBePoinsonedOne = true;
                    }
                    if (ravenDeathNight != -1 && newNightValues[ravenDeathNight] == 0)
                    {
                        ravenCanBePoisoned = true;
                    }

                    var poisonerInvalid = false;
                    for (int l = 0; l < world.Count; l++)
                    {
                        if (world[l].CharacterId == "imp" || world[l].CharacterId == "poisoner")
                        {
                            if (world[l].DeathMethod != DeathMethod.Alive)
                            {
                                if (world.Count <= 9)
                                {
                                    for (int j = 0; j < nightValues.Count; j++)
                                    {
                                        if (world[l].DeathDay != -1 && j > world[l].DeathDay && newNightValues[j] > 0)
                                        {
                                            poisonerInvalid = true;
                                            continue;
                                        }
                                        if (world[l].DeathNight != -1 && j >= world[l].DeathNight && newNightValues[j] > 0)
                                        {
                                            poisonerInvalid = true;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (poisonerInvalid)
                    {
                        continue;
                    }

                    if (world[i].CharacterId == "saint" ||
                        world[i].CharacterId == "butler" ||
                        world[i].CharacterId == "recluse" ||
                        world[i].CharacterId == "baron" ||
                        world[i].CharacterId == "imp" ||
                        world[i].CharacterId == "spy" ||
                        world[i].CharacterId == "poisoner" ||
                        world[i].CharacterId == "scarletWoman" ||
                        world[i].DeathMethod == DeathMethod.Virgin ||
                        ftInfo != null)
                    {
						comboFound = ftInfo != null;
                        continue;
                    }

                    comboFound = true;
                    drunkID = world[i].CharacterId;
                    world[i] = new Drunk(world[i].DeathDay, world[i].DeathNight, world[i].DeathMethod, world[i].IsYou);
                    break;
                }
            }

            if (!comboFound)
            {
                reasons.Add("This has too much incorrect information that if any given player is the drunk, there still exists a night where more than 1 player would need to have been poisoned (or a night after the poisoner died/became the demon where a player would need to be)");
                return false;
            }
        }

        if (world.Any(w =>
        {
            Washerwoman wash = w as Washerwoman;
            return wash != null
                && wash.infoCharacter == drunkID
                && !washerwomanCanBePoinsonedOne
                && world[wash.info1].CharacterId != "spy"
                && world[wash.info2].CharacterId != "spy";
        }))
        {
            reasons.Add("The player that the washerwoman learned is the drunk, so the washerwoman is wrong");
            return false;
        }

        if (world.Any(w =>
        {
            Ravenkeeper raven = w as Ravenkeeper;
            return raven != null
                && raven.playerSaw != -1
                && !ravenCanBePoisoned
                && raven.characterSaw == drunkID
                && world[raven.playerSaw].CharacterId != "spy";
        }))
        {
            reasons.Add("The player that the ravenkeeper learned is the drunk, so the ravenkeeper is wrong");
            return false;
        }

        var invalidShot = false;
        foreach (Character player in world)
        {
            if (player.CharacterId != "imp" && player.CharacterId != "recluse" && player.DeathMethod == DeathMethod.Slayer)
            {
                invalidShot = true;
                break;
            }
        }
        if (invalidShot)
        {
            reasons.Add("The slayer's shot can only kill the imp or the recluse.");
            return false;
        }

        var virginDeath = false;
        var virginPresent = false;
        foreach (Character player in world)
        {
            if (player.DeathMethod == DeathMethod.Virgin)
            {
                virginDeath = true;

                if (player.CharacterId == "drunk" || 
                    player.CharacterId == "imp" || 
                    player.CharacterId == "baron" || 
                    player.CharacterId == "poisoner" ||
                    player.CharacterId == "scarletWoman" ||
                    player.CharacterId == "recluse" ||
                    player.CharacterId == "butler" ||
                    player.CharacterId == "saint")
                {
                    reasons.Add("The "+player.CharacterId+" cannot die to the virgin ability.");
                    return false;
                }
            }

            if (player.CharacterId == "virgin")
            {
                virginPresent = true;
            }
        }
        if (virginDeath && !virginPresent)
        {
            reasons.Add("There must be a virgin present for a player to die to the virgin.");
            return false;
        }

        return true;
    }

    private static List<List<Character>> GenerateAllWorlds(List<Character> grim)
    {
        var worlds = new List<List<Character>>();

        for (int demon = 0; demon < grim.Count; demon++) // Cycle demon candidates
        {
            if (grim[demon].IsYou || grim[demon].DeathMethod == DeathMethod.Virgin)
            {
                continue;
            }

            for (int minion1 = 0; minion1 < grim.Count; minion1++)
            {
                if (grim[minion1].IsYou)
                {
                    continue;
                }

                if (demon == minion1)
                {
                    continue;
                }

                //if (grim.Count > 9)
                //{
                //    for (int minion2 = 0; minion2 < grim.Count; minion2++)
                //    {
                //        if (minion2 == demon || minion2 == minion1)
                //        {
                //            continue;
                //        }
                //        throw new NotImplementedException();
                //        //TODO 2 minion
                //        if (grim.Count > 12)
                //        {
                //            for (int minion3 = 0; minion3 < grim.Count; minion3++)
                //            {
                //                if (minion3 == demon || minion3 == minion1 || minion3 == minion2)
                //                {
                //                    continue;
                //                }
                //            }
                //            throw new NotImplementedException();
                //            //TODO 3 minion
                //        }
                //    }
                //}

                // Generating 1 minion worlds
                var baronWorld = new List<Character>();
                var swWorld = new List<Character>();
                var poisonWorld = new List<Character>();
                var spyWorld = new List<Character>();
                for (int i = 0; i < grim.Count; i++)
                {
                    if (i == demon)
                    {
                        var imp = new Imp(grim[i].DeathDay, grim[i].DeathNight, grim[i].DeathMethod);
                        baronWorld.Add(imp);
                        swWorld.Add(imp);
                        poisonWorld.Add(imp);
                        spyWorld.Add(imp);
                    }
                    else if (i == minion1)
                    {
                        var baron = new Baron(grim[i].DeathDay, grim[i].DeathNight, grim[i].DeathMethod);
                        baronWorld.Add(baron);
                        var sw = new ScarletWoman(grim[i].DeathDay, grim[i].DeathNight, grim[i].DeathMethod);
                        swWorld.Add(sw);
                        var poison = new Poisoner(grim[i].DeathDay, grim[i].DeathNight, grim[i].DeathMethod);
                        poisonWorld.Add(poison);
                        var spy = new Spy(grim[i].DeathDay, grim[i].DeathNight, grim[i].DeathMethod);
                        spyWorld.Add(spy);
                    }
                    else
                    {
                        baronWorld.Add(grim[i]);
                        swWorld.Add(grim[i]);
                        poisonWorld.Add(grim[i]);
                        spyWorld.Add(grim[i]);
                    }
                }

                // Both evils dead
                var minionDead = baronWorld[minion1].DeathDay > -1 || baronWorld[minion1].DeathNight > -1;
                var demonDead = baronWorld[demon].DeathDay > -1 || baronWorld[demon].DeathNight > -1;
                if (minionDead && demonDead)
                {
                    continue;
                }

                if (SeemsViable(swWorld))
                {
                    worlds.Add(swWorld);
                    if (swWorld[demon].DeathDay == -1)
                    {
                        worlds.Add(poisonWorld);
                        worlds.Add(spyWorld);
                    }
                }
                if (SeemsViable(baronWorld) && baronWorld[demon].DeathDay == -1)
                {
                    worlds.Add(baronWorld);
                }
            }
        }

        return worlds;
    }

    private static List<string> BasicReasons(List<Character> world, List<string> reasons)
    {
        var deadEvils = 0;
        var scarletWoman = false;
        var impDeathMethod = DeathMethod.Alive;
        for (int i = 0; i < world.Count; i++)
        {
            if (world[i].CharacterId == "imp" || world[i].CharacterId == "poisoner" || world[i].CharacterId == "spy" || world[i].CharacterId == "scarletWoman" || world[i].CharacterId == "baron")
            {
                if (world[i].IsYou)
                {
                    reasons.Add("You cannot be the "+world[i].CharacterId+" as you know that you are good.");
                }
                
                if (world[i].DeathMethod != DeathMethod.Alive)
                {
                    deadEvils += 1;
                }

                if (world[i].CharacterId == "imp")
                {
                    impDeathMethod = world[i].DeathMethod;
                }

                if (world[i].CharacterId == "scarletWoman")
                {
                    scarletWoman = true;
                }
            }
        }

        if (deadEvils == 2)
        {
            reasons.Add("There is no way the game can continue if all starting evil players have died.");
        }

        if (!scarletWoman && impDeathMethod != DeathMethod.Alive && impDeathMethod != DeathMethod.Night)
        {
            reasons.Add("The demon died during the day and the game did not end, so there must be a scarlet woman.");
        }

        if (CountOutsiders(world) > GetExpectedOutsiderCount(world))
        {
            reasons.Add("There are too many outsiders.");
        }

        if (CountOutsiders(world) < GetExpectedOutsiderCount(world)-1)
        {
            reasons.Add("There are too few outsiders.");
        }

        if (!HasNoDuplicates(world))
        {
            reasons.Add("There are duplicate good characters.");
        }

        return reasons;
    }

    private static int GetExpectedOutsiderCount(List<Character> world)
    {
        var count = world.Count;
        count -= 7;
        while (count > 2)
        {
            count -= 3;
        }

        foreach (Character character in world)
        {
            if (character.CharacterId == "baron")
            {
                count += 2;
            }
        }

        return count;
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

    private static bool IsOutsiderCountCorrect(List<Character> world)
    {
        return CountOutsiders(world) <= GetExpectedOutsiderCount(world) && CountOutsiders(world) >= GetExpectedOutsiderCount(world)-1;
    }

    private static bool HasNoDuplicates(List<Character> world)
    {
        return world.Count == world.Select(x => x.CharacterId).Distinct().Count();
    }

    private static bool SeemsViable(List<Character> world)
    {
        return IsOutsiderCountCorrect(world) && HasNoDuplicates(world);
    }

    private static bool IsOutsider(Character character)
    {
        return character.CharacterId == "saint" || character.CharacterId == "recluse" || character.CharacterId == "butler";
    }

    private static void Shuffle<T>(IList<T> list)
    {
        var rng = new Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Character CreateCharacter(CharacterDto dto)
    {
        switch (dto.Type.ToLower())
        {
            case "washerwoman":
                return new Washerwoman(dto.WasherwomanOne.Value, dto.WasherwomanTwo.Value, dto.WasherwomanCharacter, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "librarian":
                return new Librarian(dto.LibrarianOne.Value, dto.LibrarianTwo.Value, dto.LibrarianCharacter, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "investigator":
                return new Investigator(dto.InvestigatorOne.Value, dto.InvestigatorTwo.Value, dto.InvestigatorCharacter, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "chef":
                return new Chef(dto.ChefPairs.Value, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "empath":
                return new Empath(dto.EmpathInfo, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "fortuneteller":
                return new FortuneTeller(dto.FortuneTellerPicks, dto.FortuneTellerResults, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "undertaker":
                return new Undertaker(dto.UndertakerInfo, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "monk":
                return new Monk(dto.MonkPicks, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "ravenkeeper":
                return new Ravenkeeper(dto.RavenkeeperCharacter, dto.RavenkeeperPlayer.Value, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "virgin":
                return new Virgin(dto.VirginDayNommed.Value, dto.VirginPlayerNommed.Value, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "slayer":
                return new Slayer(dto.SlayerPlayerShot.Value, dto.SlayerDayShot.Value, dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "soldier":
                return new Soldier(dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "mayor":
                return new Mayor(dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "butler":
                return new Butler(dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "saint":
                return new Saint(dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            case "recluse":
                return new Recluse(dto.DeathDay, dto.DeathNight, dto.DeathMethod, dto.IsYou);
            default:
                throw new ArgumentException("Unknown character type '" + dto.Type + "'.");
        }
    }
}