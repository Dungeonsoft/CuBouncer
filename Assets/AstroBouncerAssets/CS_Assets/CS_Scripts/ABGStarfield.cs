using UnityEngine;
using System.Collections;

/// <summary>
/// This script creates a starfield background by dispersing little star graphics across an area
/// </summary>
public class ABGStarfield:MonoBehaviour 
{
	private Transform thisTransform;
	
	//The area of the starfield
	public float fieldWidth = 10;
	public float fieldHeight = 10;
	
	//The range of scale for an individual star
	public Vector2 starSizeRange = new Vector2(0.1f, 1);
	
	//The number of the stars to be created
	public int starCount = 100;
	
	//The star graphic that will be used
	public Transform starObject;
	
	//The Z offset of the starfield
	public float offsetZ = 0;

	// Use this for initialization
	void Start() 
	{
		thisTransform = transform;
		
		//Create a number of stars
		while ( starCount > 0 )
		{
			starCount--;
			
			//Create a new star object and place it at a random position within the starfield area
			Transform newStar = Instantiate( starObject, new Vector3( Random.Range( fieldWidth * -0.5f, fieldWidth * 0.5f), Random.Range( fieldHeight * -0.5f, fieldHeight * 0.5f), thisTransform.position.z + offsetZ), Quaternion.identity) as Transform;
			
			//give the star object a random size within the starSizeRange
			newStar.transform.localScale *= Random.Range( starSizeRange.x, starSizeRange.y);
			
			//Place the star in the parent starfield object
			newStar.parent = transform;
		}
	}
}




