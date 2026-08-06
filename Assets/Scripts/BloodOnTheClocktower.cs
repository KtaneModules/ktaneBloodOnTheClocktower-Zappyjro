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

	private static int _moduleIdCounter = 1;
	private int _moduleId = 0;

	private string[] names;
	private List<string> playerNames;
	private GameDto generatedGame;
	private List<int> characterIds;
	private List<int> characterSelected;
	private bool _isSolved = false;
	private Coroutine displayScroller;

	void Start () 
	{
		_moduleId = _moduleIdCounter++;

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

		displayField.text = "";

		playerNames = new List<string>();
		characterIds = new List<int>();
		characterSelected = new List<int>();
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
			int playerIndex = i;
			characterIcons[i].material = characterIconMaterials[CharacterToID(generatedGame.Characters[i].Type)];
			nameFields[i].text = playerNames[i];
			characterIds.Add(CharacterToID(generatedGame.Characters[i].Type));
			characterButtons[playerIndex].OnInteract += delegate {
				HandleTokenPressed(playerIndex);
				return false;
			};
			nameButtons[playerIndex].OnInteract += delegate {
				HandleNamePressed(playerIndex);
				return false;
			};
			characterSelected.Add(0);

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
					shroudFields[i].text = (generatedGame.Characters[i].DeathNight+1).ToString();
					shroudFields[i].color = Color.white;
					break;
				case DeathMethod.Execution:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = (generatedGame.Characters[i].DeathDay+1).ToString();
					shroudFields[i].color = Color.white;
					break;
				case DeathMethod.Slayer:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = (generatedGame.Characters[i].DeathDay+1).ToString();
					shroudFields[i].color = Color.red;
					break;
				case DeathMethod.Virgin:
					shroudsFull[i].SetActive(true);
					shroudPartsLeft[i].material = shroudMaterials[1];
					shroudPartsRight[i].material = shroudMaterials[1];
					shroudPartsTop[i].material = shroudMaterials[1];
					shroudFields[i].text = (generatedGame.Characters[i].DeathDay+1).ToString();
					shroudFields[i].color = Color.blue;
					break;
				default:
					continue;
			}
		}
		submitButton.OnInteract += delegate {
			HandleSubmit();
			return false;
		};
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public GameDto GetGeneratedGame()
	{
		return generatedGame;
	}

	private void HandleTokenPressed(int playerNumber) 
	{
		UnityEngine.Debug.Log("Cycled token "+playerNumber.ToString());
		characterButtons[playerNumber].AddInteractionPunch(0.3f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, characterButtons[playerNumber].transform);
		switch (characterSelected[playerNumber]) 
		{
			case 0:
				characterSelected[playerNumber]++;
				characterIcons[playerNumber].material = characterIconMaterials[20]; // Imp
				break;
			case 1:
				characterSelected[playerNumber]++;
				characterIcons[playerNumber].material = characterIconMaterials[16]; // Poisoner
				break;
			case 2:
				characterSelected[playerNumber]++;
				characterIcons[playerNumber].material = characterIconMaterials[17]; // Spy
				break;
			case 3:
				characterSelected[playerNumber]++;
				characterIcons[playerNumber].material = characterIconMaterials[18]; // Baron
				break;
			case 4:
				characterSelected[playerNumber]++;
				characterIcons[playerNumber].material = characterIconMaterials[19]; // Scarlet Woman
				break;
			default:
				characterSelected[playerNumber] = 0;
				characterIcons[playerNumber].material = characterIconMaterials[characterIds[playerNumber]]; // Good character again
				break;
		}
	}

	private void HandleNamePressed(int playerNumber) 
	{
		UnityEngine.Debug.Log("Clicked name "+playerNumber.ToString());
		nameButtons[playerNumber].AddInteractionPunch(0.3f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, nameButtons[playerNumber].transform);
		SetScrollingText(GetClaimedInfo(playerNumber));
	}

	private void HandleSubmit() 
	{
		UnityEngine.Debug.Log("Submit pressed");
		submitButton.AddInteractionPunch(0.3f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitButton.transform);
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

	private string FormatCharacterName(string characterName) 
	{
		if (string.IsNullOrEmpty (characterName)) 
		{
			return characterName;
		}

		string result = char.ToUpper(characterName[0]).ToString();

		for (int i = 1; i < characterName.Length; i++) 
		{
			if (char.IsUpper(characterName [i])) 
			{
				result = result + " ";
			}

			result = result + characterName[i];
		}

		return result;
	}

	private string GetClaimedInfo(int playerNumber) 
	{
		switch (characterIds[playerNumber]) 
		{
			case 0: // Washerwoman
			return playerNames[generatedGame.Characters[playerNumber].WasherwomanOne.Value]+" or "+playerNames[generatedGame.Characters[playerNumber].WasherwomanTwo.Value]+" is the "+FormatCharacterName(generatedGame.Characters[playerNumber].WasherwomanCharacter);
			case 1: // Librarian
			if (generatedGame.Characters[playerNumber].LibrarianOne.Value != -1) 
				{
				return playerNames[generatedGame.Characters[playerNumber].LibrarianOne.Value]+" or "+playerNames[generatedGame.Characters [playerNumber].LibrarianTwo.Value]+" is the "+FormatCharacterName(generatedGame.Characters[playerNumber].LibrarianCharacter);
				}
				else 
				{
					return "0 Outsiders";
				}
			case 2: // Investigator
			return playerNames[generatedGame.Characters[playerNumber].InvestigatorOne.Value]+" or "+playerNames[generatedGame.Characters[playerNumber].InvestigatorTwo.Value]+" is the "+FormatCharacterName(generatedGame.Characters[playerNumber].InvestigatorCharacter);
			case 3: // Chef
				return generatedGame.Characters[playerNumber].ChefPairs.ToString()+" Pairs";
			case 4: // Empath
				string empathInfo = "";
				foreach (var empathNumber in generatedGame.Characters[playerNumber].EmpathInfo) 
				{
					empathInfo = empathInfo + empathNumber.ToString() + " ";
				}
				return empathInfo;
			case 5: // Fortune Teller
				string ftInfo = "";
				for (int i = 0; i < generatedGame.Characters[playerNumber].FortuneTellerResults.Count; i++) 
				{
				ftInfo = ftInfo + playerNames[generatedGame.Characters[playerNumber].FortuneTellerPicks[i][0]] + " " + playerNames[generatedGame.Characters[playerNumber].FortuneTellerPicks[i][1]] + " " + (generatedGame.Characters[playerNumber].FortuneTellerResults[i] ? "Yes" : "No");
					if (i != generatedGame.Characters [playerNumber].FortuneTellerResults.Count - 1) 
					{
						ftInfo = ftInfo + " / ";
					}
				}
				return ftInfo;
			case 6: // Undertaker
				string undertakerInfo = "";
				foreach (var undertakerNight in generatedGame.Characters[playerNumber].UndertakerInfo) {
					var info = undertakerNight;
					if (undertakerNight == "") 
					{
						info = "N/A";
					}
					undertakerInfo = undertakerInfo + FormatCharacterName(info) + ", ";
				}
				undertakerInfo = undertakerInfo.Substring (0, undertakerInfo.Length - 2);
				return undertakerInfo;
			case 7: // Monk
				string monkInfo = "";
				foreach (var monkPick in generatedGame.Characters[playerNumber].MonkPicks) 
				{
					monkInfo = monkInfo + playerNames[monkPick] + ", ";
				}
				monkInfo = monkInfo.Substring (0, monkInfo.Length - 2);
				return monkInfo;
			case 8: // Ravenkeeper
				if (generatedGame.Characters[playerNumber].RavenkeeperPlayer == -1)
				{
					return "I never died at night";
				}
				else 
				{
					return playerNames[generatedGame.Characters[playerNumber].RavenkeeperPlayer.Value] + " " + FormatCharacterName(generatedGame.Characters[playerNumber].RavenkeeperCharacter);
				}
			case 9: // Virgin
			return playerNames[generatedGame.Characters[playerNumber].VirginPlayerNommed.Value]+" nominated me on day "+(generatedGame.Characters[playerNumber].VirginDayNommed.Value+1).ToString();
			case 10: // Slayer
			return "I shot at "+playerNames[generatedGame.Characters[playerNumber].SlayerPlayerShot.Value]+" on day "+(generatedGame.Characters[playerNumber].SlayerDayShot.Value+1).ToString();
			case 11: // Soldier
				return "I am the soldier, defender of the truth.";
			case 12: // Mayor
				return "Should we go for a mayor win?";
			case 13: // Butler
				return "I'm just a butler :(";
			case 14: // Saint
				return "Don't execute me, we'll lose!";
			case 15: // Recluse
				return "I'm just the recluse, leave me be.";
			default:
				return "Something has gone terribly wrong";
		}
	}

	private void SetScrollingText(string text) 
	{
		const float maxWidth = 0.097f;
		if (displayScroller != null) 
		{
			StopCoroutine (displayScroller);
			displayScroller = null;
		}

		if (GetTextWidth(text) <= maxWidth) 
		{
			displayField.text = text;
			return;
		}

		displayScroller = StartCoroutine (
			ScrollText(text, maxWidth)
		);
	}

	private float GetTextWidth(string text) 
	{
		string originalText = displayField.text;
		displayField.text = text;

		float width = displayField.GetComponent<Renderer> ().bounds.size.x;

		displayField.text = originalText;
		return width;
	}

	private IEnumerator ScrollText(string text, float maxWidth) 
	{
		string scrollingText = text + "                ";

		while (true) 
		{
			for (int i = 0; i < scrollingText.Length; i++) 
			{
				string repeatedText = scrollingText + scrollingText;
				string visibleText = "";

				for (int j = 0; i + j <= repeatedText.Length; j++) 
				{
					string candidate = repeatedText.Substring (i, j);

					if (GetTextWidth (candidate) > maxWidth) 
					{
						break;
					}

					visibleText = candidate;
				}

				displayField.text = visibleText;

				yield return new WaitForSeconds(0.2f);
			}
		}
	}
}
