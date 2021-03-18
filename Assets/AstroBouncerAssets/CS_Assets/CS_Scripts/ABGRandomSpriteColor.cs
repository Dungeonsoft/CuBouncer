using UnityEngine;
using System.Collections;

/// <summary>
///This script gives a sprite a random color when it's spawned
/// </summary>
public class ABGRandomSpriteColor : MonoBehaviour 
{
	//The list of colors to choose from 
	public Color[] colors;
	
	void Start() 
	{
		//Choose a random color from the list and assign it to the sprite
		if ( colors.Length > 0 )    gameObject.GetComponent<SpriteRenderer>().color = colors[Mathf.FloorToInt(Random.value * colors.Length)];
	}

}
