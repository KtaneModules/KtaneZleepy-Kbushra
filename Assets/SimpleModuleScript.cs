using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;
using UnityEngine.XR.WSA.WebCam;

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

	public AudioSource click;

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

	void Start()
	{
		Log ("This module is on the bomb."); 
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

		Log ("Triggered needy");
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
			input = 0;
		}
		else 
		{
			module.HandleStrike ();
			Log ("Wrong amount of button presses");
			textFinder1 = "?";
			screenTexts [0].text = textFinder1;
			textFinder2 = "??";
			screenTexts [1].text = textFinder2;
			input = 0;
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
						click.Play ();
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
						input = 0;
					}
					break;
				case 1:
					if (textMessage1 == 2) 
					{
						input++;
						click.Play ();
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
						input = 0;
					}
					break;
				case 2:
					if (textMessage1 == 3) 
					{
						input++;
						click.Play ();
					}
					else 
					{
						module.HandleStrike ();
						module.HandlePass();
						needyActivated = false;
						Log ("Wrong button.");
						input = 0;
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

	#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press 1(button) 22(amount) [Presses the stated button the stated amount of times]";
	#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command) 
	{
		if (!needyActivated) yield return "sendtochaterror Module not activated!";
		else
		{
			string[] parameters = command.Split(' ');
			if (parameters[0] == "press")
			{
				if (parameters.Length < 3) yield return "sendtochaterror Too little parameters!";
				else if (parameters.Length > 3) yield return "sendtochaterror Too many parameters!";
				else
				{
					int number;

					if (!int.TryParse(parameters[1], out number) || !int.TryParse(parameters[2], out number)) yield return "sendtochaterror 2nd or 3rd parameter not number!";
					else if (int.Parse(parameters[1]) < 1 || int.Parse(parameters[1]) > 3 || int.Parse(parameters[2]) < 1) yield return "sendtochaterror 2nd or 3rd parameter out of range!";
					else for (int i = 0; i < int.Parse(parameters[2]); i++) buttons[int.Parse(parameters[1]) - 1].OnInteract(); yield return null;
				}
			}
			else yield return "sendtochaterror Invalid parameter!";
		}
	}

	IEnumerator TwitchHandleForcedSolve() { for (int i = 0; i < textMessage2; i++) buttons[textMessage1-1].OnInteract(); yield return null; }
}
