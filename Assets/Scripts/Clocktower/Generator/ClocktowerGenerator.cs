using System;
using System.Collections.Generic;
using System.Linq;

public static class ClocktowerGenerator
{
    public static GameDto Generate()
    {
        var random = new Random();
        var playerCount = random.Next(7,10);
        
        var demon = random.Next(0,playerCount);
        var minion = random.Next(0,playerCount);
        var you = random.Next(0,playerCount);
        while (true)
        {
            if (demon == minion)
            {
                minion = random.Next(0,playerCount);
            }

            if (you == minion || you == demon)
            {
                you = random.Next(0,playerCount);
            }

            if (you != minion && you != demon && demon != minion)
            {
                break;
            }
        }

        var minionType = random.Next(0,4);
        var minionEnum = (Minion)minionType;
        while (true)
        {
            var claims = new List<Good>();

            for (int i = 0; i < playerCount; i++)
            {
                if (i != demon && i != minion)
                {
                    var validGoodCharFound = false;
                    while (!validGoodCharFound)
                    {
                        var testClaim = (Good)random.Next(0,16);
                        if (!claims.Contains(testClaim))
                        {
                            claims.Add(testClaim);
                            validGoodCharFound = true;
                        }
                    }
                }
                else
                {
                    claims.Add(i == minion ? Good.Minion : Good.Demon);
                }
            }

            List<int> deathOrder = Enumerable.Range(0, playerCount).ToList();

            for (int i = deathOrder.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);

				int temp = deathOrder[i];
				deathOrder[i] = deathOrder[j];
				deathOrder[j] = temp;
            }

            var deathDays = new List<int>();
            var deathNights = new List<int>();
            var deathMethods = new List<DeathMethod>();
            for (int i = 0; i < playerCount; i++)
            {
                deathDays.Add(-1);
                deathNights.Add(-1);
                deathMethods.Add(DeathMethod.Alive);
            }

            var currentDay = 0;
            var currentNight = 1;
            for (int i = 0; i < playerCount-3; i++)
            {
                if (i % 2 == 0 && (i != 4 || playerCount != 8))
                {
                    deathDays[deathOrder[i]] = currentDay;
                    deathMethods[deathOrder[i]] = DeathMethod.Execution;
                    currentDay++;
                }
                else if (i % 2 == 1)
                {
                    deathNights[deathOrder[i]] = currentNight;
                    deathMethods[deathOrder[i]] = DeathMethod.Night;
                    currentNight++;
                }
            }

            var days = currentNight;
            var characters = new List<CharacterDto>();
            for (int i = 0; i < playerCount; i++)
            {
                var character = new CharacterDto
                {
                    DeathDay = deathDays[i],
                    DeathNight = deathNights[i],
                    DeathMethod = deathMethods[i],
                    IsYou = i == you,
                    Type = GetID(claims[i])
                };

                characters.Add(character);
            }

            for (int i = 0; i < playerCount; i++)
            {
                if (claims[i] == Good.Demon || claims[i] == Good.Minion)
                {
                    continue;
                }
                else
                {
                    MakeDtoFitCharacter(characters[i], claims[i], claims, characters, minionEnum, days, random);
                }
            }

            for (int i = 0; i < playerCount; i++)
            {
                if (claims[i] == Good.Demon || claims[i] == Good.Minion)
                {
                    var claim = random.Next(0,16);
                    characters[i].Type = GetID((Good)claim);
                    for (int j = 0; j < 2; j++)
                    {
                        if (claims.Any(c => c == (Good)claim))
                        {
                            claim = random.Next(0,16);
                            characters[i].Type = GetID((Good)claim);
                        }
                    }

                    if (characters[i].DeathMethod == DeathMethod.Slayer)
                    {
                        claim = 15;
                        characters[i].Type = GetID((Good)claim);
                    }

                    MakeDtoFitCharacter(characters[i], (Good)claim, claims, characters, (Minion)random.Next(0,4), days, random);
                }
            }

            var gameDto = new GameDto()
            {
                Characters = characters,
                Days = days,
            };

            SolverResult solverResult = ClocktowerSolver.Solve(gameDto);
            if (solverResult.HasExactlyOneWorld)
            {
                return gameDto;
            }
        }

    }

    private static void MakeDtoFitCharacter(CharacterDto character, Good claim, List<Good> grim, List<CharacterDto> characters, Minion minionType, int days, Random random)
    {
        var infoGood = random.Next(0,10) < 8;
        switch (claim)
        {
            case Good.Washerwoman:
                if (infoGood)
                {
                    var infoFound = false;
                    while (!infoFound)
                    {
                        var correctPing = random.Next(0,grim.Count);
                        if (grim[correctPing] == Good.Demon || grim[correctPing] == Good.Minion || grim[correctPing] == Good.Recluse || grim[correctPing] == Good.Saint || grim[correctPing] == Good.Butler || grim[correctPing] == Good.Washerwoman)
                        {
                            continue;
                        }
                        else
                        {
                            character.WasherwomanCharacter = GetID(grim[correctPing]);
                            var wrongPing = random.Next(0,grim.Count);
                            while (wrongPing == correctPing)
                            {
                                wrongPing = random.Next(0,grim.Count);
                            }

                            var firstPingRight = random.Next(0,2) == 0;
                            if (firstPingRight)
                            {
                                character.WasherwomanOne = correctPing;
                                character.WasherwomanTwo = wrongPing;
                            }
                            else
                            {
                                character.WasherwomanOne = wrongPing;
                                character.WasherwomanTwo = correctPing;
                            }

                            infoFound = true;
                        }
                    }
                }
                else
                {
                    int ping1;
                    int ping2;
                    while (true)
                    {
                        ping1 = random.Next(0,grim.Count);
                        ping2 = random.Next(0,grim.Count);

                        if (ping1 != ping2 && grim[ping1] != Good.Washerwoman && grim[ping2] != Good.Washerwoman)
                        {
                            break;
                        }
                    }
                    character.WasherwomanCharacter = GetID((Good)random.Next(1,13));
                    character.WasherwomanOne = ping1;
                    character.WasherwomanTwo = ping2;
                }
                break;
            case Good.Librarian:
                if (infoGood)
                {
                    var infoFound = false;
                    while (!infoFound)
                    {
                        var whatSeen = random.Next(0,5);
                        switch (whatSeen)
                        {
                            case 0: // Zero
                                if (grim.Contains(Good.Recluse) || grim.Contains(Good.Saint) || grim.Contains(Good.Butler))
                                {
                                    continue;
                                }
                                else
                                {
                                    character.LibrarianCharacter = "";
                                    character.LibrarianOne = -1;
                                    character.LibrarianTwo = -1;
                                    infoFound = true;
                                }
                                break;
                            case 1: // Recluse
                                if (grim.Contains(Good.Recluse))
                                {
                                    var correctReclusePing = grim.IndexOf(Good.Recluse);
                                    var wrongReclusePing = random.Next(0,grim.Count);
                                    while (wrongReclusePing == correctReclusePing)
                                    {
                                        wrongReclusePing = random.Next(0,grim.Count);
                                    }

                                    var firstPingRight = random.Next(0,2) == 0;
                                    if (firstPingRight)
                                    {
                                        character.LibrarianOne = correctReclusePing;
                                        character.LibrarianTwo = wrongReclusePing;
                                    }
                                    else
                                    {
                                        character.LibrarianOne = wrongReclusePing;
                                        character.LibrarianTwo = correctReclusePing;
                                    }

                                    character.LibrarianCharacter = "recluse";
                                    infoFound = true;
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            case 2: // Saint
                                if (grim.Contains(Good.Saint))
                                {
                                    var correctSaintPing = grim.IndexOf(Good.Saint);
                                    var wrongSaintPing = random.Next(0,grim.Count);
                                    while (wrongSaintPing == correctSaintPing)
                                    {
                                        wrongSaintPing = random.Next(0,grim.Count);
                                    }

                                    var firstPingRight = random.Next(0,2) == 0;
                                    if (firstPingRight)
                                    {
                                        character.LibrarianOne = correctSaintPing;
                                        character.LibrarianTwo = wrongSaintPing;
                                    }
                                    else
                                    {
                                        character.LibrarianOne = wrongSaintPing;
                                        character.LibrarianTwo = correctSaintPing;
                                    }

                                    character.LibrarianCharacter = "saint";
                                    infoFound = true;
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            case 3: // Butler
                                if (grim.Contains(Good.Butler))
                                {
                                    var correctButlerPing = grim.IndexOf(Good.Butler);
                                    var wrongButlerPing = random.Next(0,grim.Count);
                                    while (wrongButlerPing == correctButlerPing)
                                    {
                                        wrongButlerPing = random.Next(0,grim.Count);
                                    }

                                    var firstPingRight = random.Next(0,2) == 0;
                                    if (firstPingRight)
                                    {
                                        character.LibrarianOne = correctButlerPing;
                                        character.LibrarianTwo = wrongButlerPing;
                                    }
                                    else
                                    {
                                        character.LibrarianOne = wrongButlerPing;
                                        character.LibrarianTwo = correctButlerPing;
                                    }

                                    character.LibrarianCharacter = "butler";
                                    infoFound = true;
                                }
                                else
                                {
                                    continue;
                                }
                                break;
                            case 4: // Drunk
                                var correctDrunkPing = random.Next(0,grim.Count);
                                var wrongDrunkPing = random.Next(0,grim.Count);
                                while (wrongDrunkPing == correctDrunkPing)
                                {
                                    wrongDrunkPing = random.Next(0,grim.Count);
                                }
                                character.LibrarianCharacter = "drunk";
                                character.LibrarianOne = correctDrunkPing;
                                character.LibrarianTwo = wrongDrunkPing;
                                break;
                        }
                    }
                }
                else
                {
                    int ping1;
                    int ping2;
                    while (true)
                    {
                        ping1 = random.Next(0,grim.Count);
                        ping2 = random.Next(0,grim.Count);

                        if (ping1 != ping2 && grim[ping1] != Good.Librarian && grim[ping2] != Good.Librarian)
                        {
                            break;
                        }
                    }
                    character.LibrarianCharacter = GetID((Good)random.Next(13,16));
                    character.LibrarianOne = ping1;
                    character.LibrarianTwo = ping2;
                }
                break;
            case Good.Investigator:
                var investPingGood = random.Next(0,10) < 8;
                if (investPingGood)
                {
                    var correctInvestPing = -1;
                    var investSeat = -1;
                    for (int i = 0; i < grim.Count; i++)
                    {
                        if (grim[i] == Good.Minion)
                        {
                            correctInvestPing = i;
                        }
                        else if (grim[i] == Good.Investigator)
                        {
                            investSeat = i;
                        }
                    }

                    var incorrectInvest = random.Next(0,grim.Count);
                    while (incorrectInvest == investSeat || incorrectInvest == correctInvestPing)
                    {
                        incorrectInvest = random.Next(0,grim.Count);
                    }

                    var pingFirst = random.Next(0,2) == 0;
                    if (pingFirst)
                    {
                        character.InvestigatorOne = correctInvestPing;
                        character.InvestigatorTwo = incorrectInvest;
                    }
                    else
                    {
                        character.InvestigatorOne = incorrectInvest;
                        character.InvestigatorTwo = correctInvestPing;
                    }

                    switch (minionType)
                    {
                        case Minion.Poisoner:
                            character.InvestigatorCharacter = "poisoner";
                            break;
                        case Minion.Baron:
                            character.InvestigatorCharacter = "baron";
                            break;
                        case Minion.ScarletWoman:
                            character.InvestigatorCharacter = "scarletWoman";
                            break;
                        case Minion.Spy:
                            character.InvestigatorCharacter = "spy";
                            break;
                    }
                }
                else
                {
                    var invest1 = random.Next(0,grim.Count);
                    var invest2 = random.Next(0,grim.Count);
                    while (grim[invest1] == Good.Investigator || invest1 == invest2 || grim[invest2] == Good.Investigator)
                    {
                        invest1 = random.Next(0,grim.Count);
                        invest2 = random.Next(0,grim.Count);
                    }

                    switch ((Minion)random.Next(0,4))
                    {
                        case Minion.Poisoner:
                            character.InvestigatorCharacter = "poisoner";
                            break;
                        case Minion.Baron:
                            character.InvestigatorCharacter = "baron";
                            break;
                        case Minion.ScarletWoman:
                            character.InvestigatorCharacter = "scarletWoman";
                            break;
                        case Minion.Spy:
                            character.InvestigatorCharacter = "spy";
                            break;
                    }

                    character.InvestigatorOne = invest1;
                    character.InvestigatorTwo = invest2;
                }
                break;
            case Good.Chef:
                var chance = random.Next(0,100);
                if (chance < 70)
                {
                    character.ChefPairs = 0;
                }
                else if (chance < 90)
                {
                    character.ChefPairs = 1;
                }
                else
                {
                    character.ChefPairs = 2;
                }
                break;
            case Good.Empath:
                var myIndex = grim.IndexOf(Good.Empath);
                var info = new List<int>();
                int nextLiving = -1;
                int previousLiving = -1;
                var empathCount = character.DeathDay;
                if (character.DeathNight != -1)
                {
                    empathCount = character.DeathNight;
                }

                if (empathCount == -1)
                {
                    empathCount = days;
                }
                for (int day = 0; day < empathCount; day++)
                {
                    for (int i = 0; i < grim.Count; i++)
                    {
                        var next = characters[(myIndex + i + 1) % grim.Count];
                        if (next.DeathDay == -1 && next.DeathNight == -1 ||
                            next.DeathDay >= day ||
                            next.DeathNight > day)
                        {
                            nextLiving = (myIndex + i + 1) % grim.Count;
                            break;
                        }
                    }
                    for (int i = 0; i < grim.Count; i++)
                    {
                        var index = (myIndex - i - 1) % grim.Count;
                        var prev = characters[index < 0 ? index + grim.Count : index];
                        if (prev.DeathDay == -1 && prev.DeathNight == -1 ||
                            prev.DeathDay >= day ||
                            prev.DeathNight > day)
                        {
                            previousLiving = (myIndex - i - 1) % grim.Count;
                            if (previousLiving < 0)
                            {
                                previousLiving += grim.Count;
                            }
                            break;
                        }
                    }

                    if ((grim[previousLiving] == Good.Minion || grim[previousLiving] == Good.Demon || grim[previousLiving] == Good.Recluse) && (grim[nextLiving] == Good.Minion || grim[nextLiving] == Good.Demon || grim[nextLiving] == Good.Recluse))
                    {
                        info.Add(2);
                    }
                    else if ((grim[previousLiving] == Good.Minion || grim[previousLiving] == Good.Demon || grim[previousLiving] == Good.Recluse) && (grim[nextLiving] == Good.Minion || grim[nextLiving] == Good.Demon || grim[nextLiving] == Good.Recluse))
                    {
                        info.Add(1);
                    }
                    else
                    {
                        info.Add(0);
                    }

                    for (int i = 0; i < info.Count; i++)
                    {
                        if (random.Next(0,10) < 9)
                        {
                            info[i] = random.Next(0,3);
                        }
                    }
                }

                character.EmpathInfo = info;
                break;
            case Good.FortuneTeller:
                var ftPicks = new List<int[]>();
                var ftResults = new List<bool>();
                var ftPicksNeeded = character.DeathDay;
                if (character.DeathNight != -1)
                {
                    ftPicksNeeded = character.DeathNight-1;
                }
                if (ftPicksNeeded == -1)
                {
                    ftPicksNeeded = days;
                }
                for (int i = 0; i <= ftPicksNeeded; i++)
                {
                    var pick1 = random.Next(0,grim.Count);
                    var pick2 = random.Next(0,grim.Count);
                    while (pick1 == pick2)
                    {
                        pick2 = random.Next(0,grim.Count);
                    }
                    
					ftPicks.Add(new int[] { pick1, pick2 });
                    ftResults.Add(random.Next(0,2) == 0);
                }
                character.FortuneTellerPicks = ftPicks;
                character.FortuneTellerResults = ftResults;
                break;
            case Good.Undertaker:
                var undertakerInfoNeeded = character.DeathDay;
                var undertakerInfo = new List<string>();
                if (character.DeathNight != -1)
                {
                    undertakerInfoNeeded = character.DeathNight;
                }
                if (undertakerInfoNeeded == -1)
                {
                    undertakerInfoNeeded = days-1;
                }
                for (int i = 0; i < undertakerInfoNeeded; i++)
                {
                    var correctInfo = random.Next(0,10) < 8;
                    if (correctInfo)
                    {
                        var undertakerToday = characters.First(c => c.DeathDay == i).Type;
                        if (characters.First(c => c.DeathDay == i).DeathMethod == DeathMethod.Slayer)
                        {
                            undertakerInfo.Add("");
                            continue;
                        }

                        if (undertakerToday == "spy")
                        {
                            if (random.Next(0,10) > 4)
                            {
                                undertakerToday = GetID((Good)random.Next(0,16));
                            }
                        }
                        else if (undertakerToday == "recluse")
                        {
                            switch (random.Next(0,5))
                            {
                                case 0:
                                    undertakerToday = "spy";
                                    break;
                                case 1:
                                    undertakerToday = "imp";
                                    break;
                                case 2:
                                    undertakerToday = "baron";
                                    break;
                                case 3:
                                    undertakerToday = "scarletWoman";
                                    break;
                                case 4:
                                    undertakerToday = "poisoner";
                                    break;
                            }
                        }

                        undertakerInfo.Add(undertakerToday);
                    }
                    else
                    {
                        var undertakerToday = characters.First(c => c.DeathDay == i).Type;
                        if (characters.First(c => c.DeathDay == i).DeathMethod == DeathMethod.Slayer)
                        {
                            undertakerInfo.Add("");
                            continue;
                        }

                        var randomChar = random.Next(0,21);
                        switch (randomChar)
                        {
                                case 0:
                                    undertakerToday = "spy";
                                    break;
                                case 1:
                                    undertakerToday = "imp";
                                    break;
                                case 2:
                                    undertakerToday = "baron";
                                    break;
                                case 3:
                                    undertakerToday = "scarletWoman";
                                    break;
                                case 4:
                                    undertakerToday = "poisoner";
                                    break;
                                default:
                                    undertakerToday = GetID((Good)(randomChar-5));
                                    break;
                        }

                        undertakerInfo.Add(undertakerToday);
                    }

                    if (undertakerInfo.Last() == "")
                    {
                        throw new InvalidOperationException();
                    }
                }
                character.UndertakerInfo = undertakerInfo;
                break;
            case Good.Monk:
                var picksNeeded = character.DeathDay;
                if (character.DeathNight != -1)
                {
                    picksNeeded = character.DeathNight;
                }
                if (picksNeeded == -1)
                {
                    picksNeeded = days-1;
                }
                var picks = new List<int>();
                for (int i = 0; i < picksNeeded; i++)
                {
                    var validPick = false;
                    while (!validPick)
                    {
                        var pick = random.Next(0,grim.Count);
                        if (grim[pick] != Good.Monk && (characters[pick].DeathDay > i || characters[pick].DeathNight > i || characters[pick].DeathMethod == DeathMethod.Alive))
                        {
                            validPick = true;
                            picks.Add(pick);
                        }
                    }
                }
                character.MonkPicks = picks;
                break;
            case Good.Ravenkeeper:
                var ravenInfoGood = random.Next(0,10) < 9;
                var ravenPlayer = random.Next(0,grim.Count);
                while (ravenPlayer == grim.IndexOf(Good.Ravenkeeper))
                {
                    ravenPlayer = random.Next(0,grim.Count);
                }

                if (character.DeathNight == -1)
                {
                    character.RavenkeeperCharacter = "";
                    character.RavenkeeperPlayer = -1;
                    break;
                }

                if (ravenInfoGood)
                {
                    var ravenCharacter = GetID(grim[ravenPlayer]);
                    if (grim[ravenPlayer] == Good.Minion)
                    {
                        switch (minionType)
                        {
                            case Minion.Poisoner:
                                ravenCharacter = "poisoner";
                                break;
                            case Minion.ScarletWoman:
                                ravenCharacter = "scarletWoman";
                                break;
                            case Minion.Spy:
                                ravenCharacter = GetID((Good)random.Next(0,16));
                                break;
                            case Minion.Baron:
                                ravenCharacter = "baron";
                                break;
                        }
                    }

                    character.RavenkeeperCharacter = ravenCharacter;
                    character.RavenkeeperPlayer = ravenPlayer;
                }
                else
                {
                    character.RavenkeeperPlayer = ravenPlayer;
                    var randomChar = random.Next(0,21);
                    switch (randomChar)
                    {
                        case 0:
                            character.RavenkeeperCharacter = "spy";
                            break;
                        case 1:
                            character.RavenkeeperCharacter = "imp";
                            break;
                        case 2:
                            character.RavenkeeperCharacter = "baron";
                            break;
                        case 3:
                            character.RavenkeeperCharacter = "scarletWoman";
                            break;
                        case 4:
                            character.RavenkeeperCharacter = "poisoner";
                            break;
                        default:
                            character.RavenkeeperCharacter = GetID((Good)(randomChar-5));
                            break;
                    }
                }
                break;
            case Good.Virgin:
                var mustBeBefore = character.DeathDay;
                if (character.DeathNight != -1)
                {
                    mustBeBefore = character.DeathNight;
                }

                if (mustBeBefore == -1)
                {
                    mustBeBefore = 3;
                }
                
                if (mustBeBefore == 0)
                {
                    character.VirginDayNommed = -1;
                    character.VirginPlayerNommed = -1;
                }
                else
                {
                    var dayNommed = random.Next(0,mustBeBefore);
                    character.VirginDayNommed = dayNommed;
                    var triggered = random.Next(0,4) != 0;
                    if (triggered)
                    {
                        if (!characters.Any(c => c.DeathDay == dayNommed))
                        {
                            triggered = false;
                        }
                        else
                        {
                            var nommedBy = characters.IndexOf(characters.First(c => c.DeathDay == dayNommed));
                            if (grim[nommedBy] != Good.Virgin && grim[nommedBy] != Good.Demon && grim[nommedBy] != Good.Minion && grim[nommedBy] != Good.Butler && grim[nommedBy] != Good.Saint && grim[nommedBy] != Good.Recluse)
                            {
                                characters[nommedBy].DeathMethod = DeathMethod.Virgin;
                                character.VirginPlayerNommed = nommedBy;
                            }
                            else
                            {
                                triggered = false;
                            }
                        }
                    }

                    if (!triggered)
                    {
                        var nommedBy = random.Next(0,grim.Count);
                        while (grim[nommedBy] == Good.Virgin)
                        {
                            nommedBy = random.Next(0,grim.Count);
                        }

                        character.VirginPlayerNommed = nommedBy;
                    }
                }
                break;
            case Good.Slayer:
                var mustBeShotBefore = character.DeathDay;
                if (character.DeathNight != -1)
                {
                    mustBeShotBefore = character.DeathNight;
                }

                if (mustBeShotBefore == -1)
                {
                    mustBeShotBefore = 3;
                }

                var demonDeathDay = -1;
                var demonSeat = -1;
                for (int i = 0; i < grim.Count; i++)
                {
                    if (grim[i] == Good.Demon)
                    {
                        demonSeat = i;
                        demonDeathDay = characters[i].DeathDay;
                        break;
                    }
                }
                if (demonDeathDay != -1)
                {
                    if (((character.DeathDay == -1 && character.DeathNight == -1) || character.DeathDay > demonDeathDay || character.DeathNight > demonDeathDay) && random.Next(0,2) == 0)
                    {
                        characters[demonSeat].DeathMethod = DeathMethod.Slayer;
                        character.SlayerPlayerShot = demonSeat;
                        character.SlayerDayShot = demonDeathDay;
                        break;
                    }
                }
                var recluseDeathDay = -1;
                var recluseSeat = -1;
                for (int i = 0; i < grim.Count; i++)
                {
                    if (grim[i] == Good.Recluse)
                    {
                        recluseSeat = i;
                        recluseDeathDay = characters[i].DeathDay;
                        break;
                    }
                }
                if (recluseDeathDay != -1)
                {
                    if (((character.DeathDay == -1 && character.DeathNight == -1) || character.DeathDay > recluseDeathDay || character.DeathNight > recluseDeathDay) && random.Next(0,2) == 0)
                    {
                        characters[recluseSeat].DeathMethod = DeathMethod.Slayer;
                        character.SlayerPlayerShot = recluseSeat;
                        character.SlayerDayShot = recluseDeathDay;
                        break;
                    }
                }

                var target = random.Next(0,grim.Count);
                while (grim[target] == Good.Slayer)
                {
                    target = random.Next(0,grim.Count);
                }

                character.SlayerPlayerShot = target;
                character.SlayerDayShot = random.Next(0,mustBeShotBefore);
                break;
            default:
                break;
        }
    }

    private static string GetID(Good claim)
    {
        switch (claim)
        {
            case Good.Washerwoman:
                return "washerwoman";
            case Good.Librarian:
                return "librarian";
            case Good.Investigator:
                return "investigator";
            case Good.Chef:
                return "chef";
            case Good.Empath:
                return "empath";
            case Good.FortuneTeller:
                return "fortuneTeller";
            case Good.Undertaker:
                return "undertaker";
            case Good.Monk:
                return "monk";
            case Good.Ravenkeeper:
                return "ravenkeeper";
            case Good.Virgin:
                return "virgin";
            case Good.Slayer:
                return "slayer";
            case Good.Soldier:
                return "soldier";
            case Good.Mayor:
                return "mayor";
            case Good.Butler:
                return "butler";
            case Good.Saint:
                return "saint";
            case Good.Recluse:
                return "recluse";
            case Good.Demon:
                return "imp";
            default:
                return "minion";
        }
    }
}