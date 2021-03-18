using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script toggles a sound source when clicked on. It also records the sound state (on/off) in a PlayerPrefs. In order to detect clicks you need to attach a collider to this object.
/// </summary>
public class ABGToggleSound:MonoBehaviour 
{
	//The source of the sound
	public Transform soundObject;
	
	//The PlayerPrefs name of the sound
	public string playerPref = "SoundVolume";
	
	//The index of the current value of the sound
	internal float currentState = 1;

	// Use this for initialization
	void Awake() 
	{
		//Get the current state of the sound from PlayerPrefs
		if ( soundObject )    currentState = PlayerPrefs.GetFloat( playerPref, soundObject.GetComponent<AudioSource>().volume);
		else    currentState = PlayerPrefs.GetFloat( playerPref, currentState);
		
		//Set the sound in the sound source
		SetSound();
	}
	
	public void SetSound()
	{
		//Set the sound in the PlayerPrefs
		PlayerPrefs.SetFloat( playerPref, currentState);
		
		//Update the graphics of the button image to fit the sound state
		if ( currentState == 1 )    GetComponent<Image>().color = new Color( GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 1);
		else    GetComponent<Image>().color = new Color( GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0.5f);
		
		//Set the value of the sound state to the source object
		if ( soundObject )    soundObject.GetComponent<AudioSource>().volume = currentState;
	}
	
	//Toggle the sound. Cycle through all sound modes and set the volume and icon accordingly
	public void ToggleSound()
	{
		currentState = 1 - currentState;
		
		SetSound();
	}
	
	//Start playing the sound source
	public void StartSound()
	{	
		if ( soundObject )    soundObject.GetComponent<AudioSource>().Play();
	}
}





