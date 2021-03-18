using UnityEngine;
using System.Collections;

/// <summary>
/// This script plays a random sound from a list of sounds, through an audio source
/// </summary>
public class ABGPlaySound:MonoBehaviour 
{
	//A list of possible sounds
	public AudioClip[] audioList;
	
	//The tag of the sound source
	public string audioSourceTag = "GameController";
	
	public bool playOnStart = true;

	// Use this for initialization
	void Start() 
	{
		if ( playOnStart == true )    PlaySound();
	}
	
	public void PlaySound()
	{
		//If there is a sound source tag and audio to play, play the sound from the audio source based on its tag
		if ( audioSourceTag != string.Empty && audioList.Length > 0 )    GameObject.FindGameObjectWithTag(audioSourceTag).GetComponent<AudioSource>().PlayOneShot(audioList[Mathf.FloorToInt(Random.value * audioList.Length)]);
	}
	
	public void PlaySound( int soundIndex )
	{
		//If there is a sound source tag and audio to play, play the sound from the audio source based on its tag
		if ( audioSourceTag != string.Empty && audioList.Length > 0 )    GameObject.FindGameObjectWithTag(audioSourceTag).GetComponent<AudioSource>().PlayOneShot(audioList[soundIndex]);
	}
}
