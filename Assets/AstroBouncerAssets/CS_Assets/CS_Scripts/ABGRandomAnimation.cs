using UnityEngine;
using System.Collections;

/// <summary>
/// This script randomizes an object's animation.
/// </summary>
public class ABGRandomAnimation : MonoBehaviour 
{
	// Use this for initialization
	void Start() 
	{
		//Choose a random time from the animation
		GetComponent<Animation>()[GetComponent<Animation>().clip.name].time = Random.Range(0, GetComponent<Animation>().clip.length);
		
		//Enable the animation
		GetComponent<Animation>()[GetComponent<Animation>().clip.name].enabled = true;
		
		//Sample the animation at the current time
		GetComponent<Animation>().Sample();
		
		//Disable the animation
		GetComponent<Animation>()[GetComponent<Animation>().clip.name].enabled = false;
		
		//Play the animation from the new time we sampled
		GetComponent<Animation>().Play();
	}
}
