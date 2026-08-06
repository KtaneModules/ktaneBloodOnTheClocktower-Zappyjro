using System.Collections.Generic;

public abstract class Character
{
    public string CharacterId;
    public bool IsYou;
    public int DeathDay;
    public int DeathNight;
    public DeathMethod DeathMethod;

    protected Character(string characterId, int deathDay = -1, int deathNight = -1, DeathMethod deathMethod = DeathMethod.Alive, bool isYou = false)
    {
        CharacterId = characterId;
        IsYou = isYou;
        DeathDay = deathDay;
        DeathNight = deathNight;
        DeathMethod = deathMethod;
    }

    public abstract List<bool> GetMisinfoDays(List<Character> grim, int dayCount);
}
