using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOnTheClocktower : MonoBehaviour {
	public KMAudio Audio;
	public KMBombModule Module;
	public KMBombInfo Info;
	public KMModSettings modSettings;
	public KMSelectable moduleSelectable;
	public KMSelectable[] characterButtons;
	public KMSelectable[] nameButtons;
	public MeshRenderer[] characterIcons;
	public MeshRenderer[] shroudPartsTop;
	public MeshRenderer[] shroudPartsLeft;
	public MeshRenderer[] shroudPartsRight;
	public GameObject[] shroudsFull;
	public KMSelectable submitButton;
	public TextMesh[] nameFields;
	public TextMesh[] shroudFields;
	public TextMesh displayField;
	public Material[] shroudMaterials;
	public Material[] characterIconMaterials;
	public KMColorblindMode ColourBlindMode;

	private string[] names;
	private List<string> playerNames;
	private GameDto generatedGame;

	void Start () 
	{
		names = new string[] {
			"Ben",
			"Caitlin",
			"Ciara",
			"Charlie",
			"Dan",
			"David",
			"Ed",
			"Emily",
			"Isha",
			"Kieran",
			"Matt",
			"Michael",
			"Millie",
			"Oliver",
			"Olivia",
			"Robert",
			"Ryan",
			"Sohan",
			"Tabby",
			"Zachary"
		};

		playerNames = new List<string> ();
		while (playerNames.Count < 8) 
		{
			var name = names[UnityEngine.Random.Range(0, names.Length)];
			if (!playerNames.Contains(name)) 
			{
				playerNames.Add(name);
			}
		}

		generatedGame = ClocktowerGenerator.Generate();
		while (generatedGame.Characters.Count != 8) 
		{
			generatedGame = ClocktowerGenerator.Generate();
		}

		UnityEngine.Debug.Log("Generated puzzle with " + generatedGame.Characters.Count + " players and " + generatedGame.Days + " days.");

		for (int i = 0; i < generatedGame.Characters.Count; i++) 
		{
			characterIcons[i].material = characterIconMaterials[CharacterToID(generatedGame.Characters[i].Type)];
			nameFields[i].text = playerNames[i];

			switch (generatedGame.Characters[i].DeathMethod) 
			{
				case DeathMethod.Alive:
					shroudsFull[i].SetActive(false);
					break;
				case DeathMethod.Night:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[0];
					shroudPartsRight[i].material = shroudMaterials[0];
					shroudPartsTop[i].material = shroudMaterials[0];
					shroudFields[i].text = generatedGame.Characters[i].DeathNight.ToString();
					shroudFields[i].color = Color.white;
					break;
				case DeathMethod.Execution:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = generatedGame.Characters[i].DeathDay.ToString();
					shroudFields[i].color = Color.white;
					break;
				case DeathMethod.Slayer:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = generatedGame.Characters[i].DeathDay.ToString();
					shroudFields[i].color = Color.red;
					break;
				case DeathMethod.Virgin:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = generatedGame.Characters[i].DeathDay.ToString();
					shroudFields[i].color = Color.blue;
					break;
				default:
					continue;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public GameDto GetGeneratedGame()
	{
		return generatedGame;
	}

	private int CharacterToID(string characterType)
	{
		switch (characterType) 
		{
			case "washerwoman":
				return 0;
			case "librarian":
				return 1;
			case "investigator":
				return 2;
			case "chef":
				return 3;
			case "empath":
				return 4;
			case "fortuneTeller":
				return 5;
			case "undertaker":
				return 6;
			case "monk":
				return 7;
			case "ravenkeeper":
				return 8;
			case "virgin":
				return 9;
			case "slayer":
				return 10;
			case "soldier":
				return 11;
			case "mayor":
				return 12;
			case "butler":
				return 13;
			case "saint":
				return 14;
			case "recluse":
				return 15;
			default:
				throw new KeyNotFoundException();
		}
	}
}
