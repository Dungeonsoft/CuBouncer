using UnityEngine;
using System.Collections;

/// <summary>
/// This script runs a function on a target object when clicked on. It needs to be attached ti a UI Button.
/// </summary>
public class ABGButtonFunction : MonoBehaviour 
{
	private Transform thisTransform;
	
	//The tag of the object in which the function needs to be executed
	public string targetTag = "GameController";
	
	//The target object in which the function needs to be executed
	public GameObject targetObject;
	
	//The name of the function that will be executed
	public string functionName;
	
	//The numerical parameter passed along with this function
	public float functionParameter;

	// Use this for initialization
	void Start() 
	{
		//Get the target object by tag
		if ( targetObject == null )    targetObject = GameObject.FindGameObjectWithTag(targetTag);
	}
	
	public void ExecuteFunction()
	{
		//Run the function at the target object
		if ( functionName != string.Empty )
		{  
			if ( targetObject )    
			{
				//Send the message to the target object
				targetObject.SendMessage(functionName, functionParameter);
			}
		}
	}
}
