using UnityEngine;
using System.Collections;

/// <summary>
/// This script controls an item which is spawned by the game controller, and can be picked up by a target object ( usually the Player ). 
/// An item can change the health of the player and the score in the game
/// </summary>
public class ABGItem : MonoBehaviour 
{
	//The target tag of the object that can pick up items
	public string targetTag = "Player";
	
	private Transform thisTransform;
	internal GameObject GameController;
	
	//The speed of the object
	public Vector2 speed = new Vector2(0,-5);
	
	//The score added when this item is picked up
	public int scoreValue = 1;
	
	//The damage caused to the target when this item is picked up
	public int damage = 0;
	
	//Should this object be removed when picked up
	public bool removeOnPickup = true;
	internal bool isPickedup = false;
	
	//A list of sounds that play when this item is picked up, and the source from which the sound plays
	public AudioClip[] soundPickup;
	public string soundSourceTag = "GameController";

	// Use this for initialization
	void Start() 
	{
		thisTransform = transform;

		GameController = GameObject.FindGameObjectWithTag("GameController");
	}
	
	// Update is called once per frame
	void Update() 
	{
		//Move the object based on its speed
		if ( speed.x != 0 )    thisTransform.position += Vector3.right * speed.x * Time.deltaTime;
		if ( speed.y != 0 )    thisTransform.position += Vector3.up *  speed.y * Time.deltaTime;
	}

	//This function runs when this object touches a target object ( usually the Player )
	void OnTriggerStay2D( Collider2D other ) 
	{
		//If the other object we hit is the target object, pick it up
		if ( other.tag == targetTag )
		{
			//If there is a score value, add it to the game score
			if ( scoreValue != 0 )    GameController.SendMessage("ChangeScore", scoreValue);
			
			//If there is a damage value, change the health of the target (usually the Player)
			if ( damage != 0 && other.gameObject.GetComponent<ABGPlayer>() )    other.gameObject.SendMessage("ChangeLives", -damage);
			
			//If there is a sound source and more than one sound assigned, play one of them from the source
			if ( soundSourceTag != string.Empty && soundPickup.Length > 0 )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundPickup[Mathf.FloorToInt(Random.value * soundPickup.Length)]);
			
			//Remove the picked up object
			if ( removeOnPickup == true )    Destroy(gameObject);
		}
	}
}
