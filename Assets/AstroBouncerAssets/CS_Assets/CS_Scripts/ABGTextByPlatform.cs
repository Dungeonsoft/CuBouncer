using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script allows the user to customize the a text string based on the platform type.
/// </summary>
public class ABGTextByPlatform : MonoBehaviour 
{
	//The text that will be displayed on PC/Mac/Webplayer
	public string computerText = "CLICK TO START";
	
	//The text that will be displayed on Android/iOS/WinPhone
	public string mobileText = "TAP TO START";
	
	//The text that will be displayed on Playstation, Xbox, Wii
	public string consoleText = "PRESS 'A' TO START";

	// Use this for initialization
	void Start() 
	{
		if ( Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer )
		{
			GetComponent<Text>().text = computerText;
		}
		else if ( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android )
		{
			GetComponent<Text>().text = mobileText;
		}
		else if ( Application.platform == RuntimePlatform.PS4 || Application.platform == RuntimePlatform.XboxOne )
		{
			GetComponent<Text>().text = consoleText;
		}
	}
}