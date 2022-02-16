using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMNeedyModule module;
	public KMSelectable[] buttons;
	public int input = 0;

	public bool _isSolved = false;
	public bool needyActivated = false;

	public TextMesh[] screenTexts;

	public int textMessage1;
	public string textFinder1;

	public int textMessage2;
	public string textFinder2;

	static int ModuleIdCounter;
	int ModuleId;

	void Awake()
	{
		ModuleId = ModuleIdCounter++;

		GetComponent<KMNeedyModule> ().OnNeedyActivation += OnNeedyActivation;
		GetComponent<KMNeedyModule> ().OnNeedyDeactivation += OnNeedyDeactivation;
		GetComponent<KMNeedyModule> ().OnTimerExpired += OnTimerExpired;

		foreach (KMSelectable button in buttons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
		}
	}

	public void OnNeedyActivation()
	{
		needyActivated = true;

		textMessage1 = Random.Range (1, 4);
		textFinder1 = textMessage1.ToString ();
		screenTexts [0].text = textFinder1;

		textMessage2 = Random.Range (1, 31);
		textFinder2 = textMessage2.ToString ();
		screenTexts [1].text = textFinder2;
	}

	public void OnTimerExpired()
	{
		needyActivated = false;

		if (input == textMessage2) 
		{
			textFinder1 = "?";
			screenTexts [0].text = textFinder1;
			textFinder2 = "??";
			screenTexts [1].text = textFinder2;
		}
		else 
		{
			module.HandleStrike ();
			Log ("Wrong amount of button presses");
			textFinder1 = "?";
			screenTexts [0].text = textFinder1;
			textFinder2 = "??";
			screenTexts [1].text = textFinder2;
		}
	}

	public void OnNeedyDeactivation()
	{
		textFinder1 = "A";
		screenTexts [0].text = textFinder1;
		textFinder2 = "GG";
		screenTexts [1].text = textFinder2;

		needyActivated = false;
	}

	public void buttonPress(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < buttons.Length; i++)
		{
			if (pressedButton == buttons[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (_isSolved == false) 
		{
			if (needyActivated == true) 
			{
				switch (buttonPosition) 
				{
				case 0:
					if (textMessage1 == 1) 
					{
						input++;	
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
					}
					break;
				case 1:
					if (textMessage1 == 2) 
					{
						input++;	
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
					}
					break;
				case 2:
					if (textMessage1 == 3) 
					{
						input++;	
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
					}
					break;
				}
			}
		}

	}



	void Log(string message)
	{
		Debug.LogFormat("[zzz #{0}] {1}", ModuleId, message);
	}
}
