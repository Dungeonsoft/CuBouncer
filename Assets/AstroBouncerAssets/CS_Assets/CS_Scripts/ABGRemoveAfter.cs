using UnityEngine;
using System.Collections;

/// <summary>
/// This script removes an object after some time. Used to remove the effects after they are done animating.
/// </summary>
public class ABGRemoveAfter : MonoBehaviour 
{
	internal Transform thisTransform;
	
	//How many seconds to wait before removing hte object
	public float removeAfter = 10;
	
	//How many second before the object disappears should it start to scale out?
	public float scaleOutTime = 1;

	// Use this for initialization
	void Start() 
	{
		thisTransform = transform;
	}
	
	// Update is called once per frame
	void Update() 
	{
		//Count down
		removeAfter -= Time.deltaTime;
		
		if ( removeAfter <= scaleOutTime )
		{
			thisTransform.localScale = Vector3.one * removeAfter/scaleOutTime;
		}
		
		//If the timer reaches 0, remove the object
		if ( removeAfter <= 0 )
		{
			Destroy( gameObject);
		}
	}
}