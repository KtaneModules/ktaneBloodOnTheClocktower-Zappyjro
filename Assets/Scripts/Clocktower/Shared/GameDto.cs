using System.Collections.Generic;

public class GameDto
{
    public GameDto()
    {
        Characters = new List<CharacterDto>();
    }
    public int Days { get; set; }
    public List<CharacterDto> Characters { get; set; }

    // World to test if running reason mode
    public int? DemonPlayer { get; set;}
    public string MinonType { get; set;}
    public int? MinionPlayer { get; set;}
}

public class CharacterDto
{
    public CharacterDto()
    {
        Type = "";
        DeathDay = -1;
        DeathNight = -1;
        DeathMethod = DeathMethod.Alive;
        IsYou = false;
    }
    public string Type { get; set; }

    // Common
    public int DeathDay { get; set; }
    public int DeathNight { get; set; }
    public DeathMethod DeathMethod { get; set; }
    public bool IsYou { get; set; }

    // Washerwoman
    public int? WasherwomanOne { get; set; }
    public int? WasherwomanTwo { get; set; }
    public string WasherwomanCharacter { get; set; }

    // Librarian
    public int? LibrarianOne { get; set; }
    public int? LibrarianTwo { get; set; }
    public string LibrarianCharacter { get; set; }
    
    // Investigator
    public int? InvestigatorOne { get; set; }
    public int? InvestigatorTwo { get; set; }
    public string InvestigatorCharacter { get; set; }
    
    // Chef
    public int? ChefPairs { get; set; }
    
    // Empath
    public List<int> EmpathInfo { get; set; }
    
    // FortuneTeller
    public List<int[]> FortuneTellerPicks { get; set; }
    public List<bool> FortuneTellerResults { get; set; }
    
    // Undertaker
    public List<string> UndertakerInfo { get; set; }
    
    // Monk
    public List<int> MonkPicks { get; set; }
    
    // Ravenkeeper
    public string RavenkeeperCharacter { get; set; }
    public int? RavenkeeperPlayer { get; set; }
    
    // Virgin
    public int? VirginDayNommed { get; set; }
    public int? VirginPlayerNommed { get; set; }
    
    // Slayer
    public int? SlayerPlayerShot { get; set; }
    public int? SlayerDayShot { get; set; }
    
    // Soldier, Mayor, Butler, Saint and Recluse take no addition parameters.
}
