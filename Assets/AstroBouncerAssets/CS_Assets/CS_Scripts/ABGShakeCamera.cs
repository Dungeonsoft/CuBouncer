using UnityEngine;
using System.Collections;

/// <summary>
/// This script shakes an object when it runs, with values for strength and time. You can set which object to shake, and if you keep the object value empty it 
/// will shake the object it's attached to.
/// </summary>
public class ABGShakeCamera : MonoBehaviour 
{
	//The original position of the camera
	public Vector3 cameraOrigin;
	
	//How violently to shake the camera
	public Vector3 strength;
	private Vector3 strengthDefault;
	
	//How quickly to settle down from shaking
	public float decay = 0.8f;
	
	//How many seconds to shake
	public float shakeTime = 1;
	private float shakeTimeDefault;
	
	public bool playOnAwake = true;
	public float delay = 0;
	
	//Is this effect playing now?
	public bool isShaking = false;

	// Use this for initialization
	void Start() 
	{
		strengthDefault = strength;
		
		shakeTimeDefault = shakeTime;
		
		if ( playOnAwake )    
		{
			Invoke( "StartShake", delay);
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if ( isShaking == true )
		{
			if ( shakeTime > 0 )
			{		
				shakeTime -= Time.deltaTime;
				
				//Move the camera in all directions based on strength
				Camera.main.transform.position = new Vector3( cameraOrigin.x + Random.Range(-strength.x, strength.x), cameraOrigin.y + Random.Range(-strength.y, strength.y), cameraOrigin.z + Random.Range(-strength.z, strength.z));
				
				//Gradually reduce the strength value
				strength *= decay;
			}
			else if ( Camera.main.transform.position != cameraOrigin )
			{
				shakeTime = 0;
				
				//Reset the camera position
				Camera.main.transform.position = cameraOrigin;
				
				isShaking = false;
			}
		}
	}

	public void StartShake()
	{
		isShaking = true;
		
		strength = strengthDefault;
		
		shakeTime = shakeTimeDefault;
	}
}







