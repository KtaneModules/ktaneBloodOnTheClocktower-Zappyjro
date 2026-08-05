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
	public KMSelectable submitButton;
	public TextMesh[] nameFields;
	public TextMesh displayField;
	public Material[] characterIconMaterials;
	public KMColorblindMode ColourBlindMode;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
