using UnityEngine;
using System.Collections;

/// <summary>
/// This script controls an enemy, showing and hiding it through the gamecontroller. If an enemy touches the target ( usually the Player ), it damages it.
/// </summary>
public class ABGEnemy : MonoBehaviour 
{
	private Transform thisTransform;

	//The target this enemy can touch
	public string targetTag = "Player";
	internal Transform targetObject;
	
	//This is the alternate target which can be touched by this enemy. It can't be followed like the default target
	public string alternateTargetTag = "AIPlayer";
	
	//How fast the enemy chases the target. If set to 0, the enemy will not chase the target and will instead use speedX/Y to move
	public float chaseSpeed = 0;
	
	//The movement speed of the enemy, if it's not chasing the target
	public Vector2 moveSpeed = new Vector2(0,0);
	
	//The damage that will be caused to the target
	public int damage = 1;
	
	//Should this object be destroyed when it's touched by the target?
	public bool dieOnTouch = false;
	
	//The effect that will be created at the position of this object when it dies
	public Transform dieEffect;
	
	//Is an object hidden now?
	public bool isHidden = false;

	// Use this for initialization
	void Start() 
	{
		thisTransform = transform;

		//If the object is set as hidden at the start of the game, hide it
		if ( isHidden == true )   thisTransform.localScale = new Vector2(0,0);
	}
	
	// Update is called once per frame
	void Update() 
	{
		//If there is a target tag, assign it to the target object so that the enemy can chase it
		if ( targetObject == null && GameObject.FindGameObjectWithTag(targetTag) )    targetObject = GameObject.FindGameObjectWithTag(targetTag).transform;
		
		//If the enemy has a chase speed, and a target, chase it!
		if ( targetObject )
		{
			if ( chaseSpeed > 0 )
			{
				thisTransform.position = Vector3.MoveTowards(thisTransform.position, targetObject.position, chaseSpeed * Time.deltaTime);
			}
			else
			{
				thisTransform.position = new Vector2( thisTransform.position.x + moveSpeed.x * Time.deltaTime, thisTransform.position.y + moveSpeed.y * Time.deltaTime);
			}
		}
	}

	
	//This function runs when an object touches a target object ( usually the Player )
	void OnTriggerEnter2D( Collider2D other ) 
	{
		//If the other object has the correct target, damage it
		if ( other.tag == targetTag || other.tag == alternateTargetTag )
		{
			//Damage the other object
			if ( damage > 0 )    other.gameObject.SendMessage("ChangeLives", -damage);
			
			if ( dieOnTouch == true )    
			{
                if (other.GetComponent<ABGPlayer>()) other.GetComponent<ABGPlayer>().Jump(0);

                //other.gameObject.SendMessage("Jump", 10);
				
				Instantiate(dieEffect, thisTransform.position, thisTransform.rotation);
				
				Destroy(gameObject);
			}
		}
	}
	
	//This function hides an object
	IEnumerator Hide()
	{
		if ( isHidden == false )   
		{
			isHidden = true;
			
			//Animate the object shrinking
			while ( thisTransform.localScale.x > 0 )
			{
				//Scale the object down
				thisTransform.localScale = new Vector2( thisTransform.localScale.x - Time.deltaTime, thisTransform.localScale.y - Time.deltaTime);

				yield return new WaitForFixedUpdate();
			}
			
			//Set the scale to 0
			thisTransform.localScale = new Vector2( 0, 0);
		}
	}
	
	//This function shows an object
	IEnumerator Show()
	{
		//Check if the object is not already shown, show it
		if ( isHidden == true )   
		{
			isHidden = false;

			//Animate the object expanding
			while ( thisTransform.localScale.x < 1 )
			{
				//Scale the object up
				thisTransform.localScale = new Vector2( thisTransform.localScale.x + Time.deltaTime, thisTransform.localScale.y + Time.deltaTime);

				yield return new WaitForFixedUpdate();
			}
			
			//Set the scale to 1, full size
			thisTransform.localScale = new Vector2( 1, 1);

		}
	}
}





