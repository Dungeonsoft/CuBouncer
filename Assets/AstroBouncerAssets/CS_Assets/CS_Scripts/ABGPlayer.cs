using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script controls the player, allowing it to move and jump. The player has a limited move area, and a health value. If the health reacehs0, the player dies.
/// </summary>
public class ABGPlayer : MonoBehaviour 
{
	
	internal Transform thisTransform;
	internal GameObject gameController;
	
	//The number of lives the player has
	public float lives = 3;
	internal float livesMax = 3;
    public Transform livesText;
	
	//Is the player alive now?
	internal bool isAlive = true;
	
	//How long the hurt effect last when the player gets hit. While hit, the player cannot be harmed but he also can't jump
	public float hurtTime = 1;
	internal float hurtTimeCount = 0;
	
	//Should the player bounce when it is hurt?
	public bool bounceWhenHurt = true;
	
	//The effect that is created after the player is destroyed
	public Transform deadEffect;
	
	//A list of gibs that will fall off the player when it is destroyed
	public Transform[] gibs;
	
	//The player's speed
	public Vector2 speed;
	
	//The player's downward gravity
	public float fallSpeed = -10;
	
	//The upward speed that is given to the player when it jumps
	public float jumpPower = 5;
	
	//How many seconds to wait before allowing the player to jump again
	public float jumpDelay = 0.2f;
	internal float jumpDelayCount = 0;
	
	//The move direction of the player
	public int direction = 1;
	
	//Is the player falling now?
	public bool isFalling = false;
	
	//The flame jet that comes out of the player's jetpack when it jumps
	public ParticleSystem jet;
	
	//Various animations for the player
	public AnimationClip jumpAnimation;
	public AnimationClip fallAnimation;
	public AnimationClip hurtAnimation;
	public AnimationClip deadAnimation;
	
	//Various sounds for the player
	public AudioClip soundJump;
	public AudioClip soundHurt;
	public AudioClip soundDie;
	
	//The tag of the sound source
	public string soundSourceTag = "GameController";

	// Use this for initialization
	void Start() 
	{
		thisTransform = transform;
		
		//Register the game controller object
		gameController = GameObject.FindGameObjectWithTag("GameController");
		
		//Set the max number of lives
		livesMax = lives;
	}
	
	// Update is called once per frame
	void Update() 
	{
		//Count down the jump delay
		if ( jumpDelayCount > 0 )    jumpDelayCount -= Time.deltaTime;
		
		//Move the player vertically based on its vertical speed
		thisTransform.position += Vector3.up * speed.y * Time.deltaTime;
		
		//Increase the vertical speed of the player based on fallSpeed
		speed += Vector2.up * fallSpeed * Time.deltaTime;
		
		//A counter for the hurt time of the player
		if ( hurtTimeCount > 0 )
		{
			//Countdown
			hurtTimeCount -= Time.deltaTime;
			
			//Keep moving the player vertically based on its speed and direction
			thisTransform.position += Vector3.right * speed.x * direction * Time.deltaTime;
		}
		else
		{
			//Move the player vertically based on its speed and direction
			thisTransform.position += Vector3.right * speed.x * direction * Time.deltaTime;
			
			//If the player is alive, it can jump
			if ( isAlive == true )
			{
				//If the player's vertical speed is negative, then it is falling
				if ( speed.y < 0 && isFalling == false && fallAnimation && hurtTimeCount <= 0 )
				{
					isFalling = true;
					
					//Play the falling animation
					GetComponent<Animation>().Play(fallAnimation.name);
				}
			}
		}

		//If there are no more lives, the player dies
		if ( lives <= 0 )    StartCoroutine(Die());
	}

	//This function bounces the player off, and switches its direction
	public void BounceOffWall()
	{
		//If we are looking right, switch to left
		if ( direction == 1 )
		{
			//Switch direction to left
			direction = -1;
			thisTransform.localScale = new Vector3( direction, thisTransform.localScale.y, thisTransform.localScale.z);
			
			//Run a HitWall function in the gamecontroller which will show/hide some of the enemies in that wall
			gameController.SendMessage("HitWall", "right");
		}
		else if ( direction == -1 )
		{
			//Switch direction to right
			direction = 1;
			thisTransform.localScale = new Vector3( direction, thisTransform.localScale.y, thisTransform.localScale.z);
			
			//Run a HitWall function in the gamecontroller which will show/hide some of the enemies in that wall
			gameController.SendMessage("HitWall", "left");
		}
	}
	
	//This function makes the player jump up
	public void Jump( int jumpDirection )
	{
        Debug.Log("Jump Method IN :: "+ jumpDirection);
		//If the player is alive, it can jump
		if ( isAlive == true && hurtTimeCount <= 0 && jumpDelayCount <= 0 )
		{
			//Reset the jump delay count
			jumpDelayCount = jumpDelay;
			
			//Set the jump direction
			if ( jumpDirection != 0 )    
			{
				direction = jumpDirection;
				thisTransform.localScale = new Vector3( direction, thisTransform.localScale.y, thisTransform.localScale.z);
			}

			//The player can only jump if it is alive
			if ( isAlive == true )
			{
				//Set the vertical speed of the player to jumpPower
				speed.y = jumpPower;
				
				//The player is no longer falling
				isFalling = false;
				
				//Play the jump animation
				if ( jumpAnimation )    GetComponent<Animation>().Play(jumpAnimation.name);
				
				//Play the jet animation
				if ( jet )    jet.Play();
				
				//If there is a source and a sound, play the jump sound from the source
				if ( soundSourceTag != "" && soundJump )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundJump);
			}
		}
	}
	
	//This function changes the number of lives the player has
	public void ChangeLives( int changeValue )
	{
		//If the change value is positive, then add to the player's lives
		if ( changeValue > 0 )
		{
			//Change the player's lives
			lives += changeValue;
		}
		else if ( hurtTimeCount <= 0 ) //Otherwise, hurt the player
		{
			//Change the player's lives
			lives += changeValue;
			
			//Set teh hurt time.
			hurtTimeCount = hurtTime;
			
			//Play the hurt animation
			if ( hurtAnimation )    GetComponent<Animation>().Play(hurtAnimation.name);
			
			//Set the vertical speed of the player to jumpPower
			speed.y = jumpPower;
			
			//Bounce off the wall
			if ( bounceWhenHurt == true )    BounceOffWall();
			
			//If there is a sound source and a sound, play it
			if ( soundSourceTag != "" )
			{
				//Play hurt sound
				if ( soundHurt )    
				{
					//If this is the player's last life, play a muted hurt sound
					if ( lives <= 0 )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundHurt, 0.4f);
					else    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundHurt);
				}
				
				//If the player is dying, play the die sound
				if ( lives <= 0 && soundDie )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundDie);
			}
		}
		
		//Assign the player object
		if ( livesText == null )    livesText = gameController.transform.Find("Lives/Text");
		
		//Update the lives text
		if ( livesText )    
		{
			if ( livesText.GetComponent<Text>() )    livesText.GetComponent<Text>().text = lives.ToString();
			if ( livesText.GetComponent<Image>() )    
			{
				livesText.GetComponent<Image>().rectTransform.sizeDelta = new Vector2( lives/livesMax, livesText.GetComponent<Image>().rectTransform.sizeDelta.y);
			}	
		}
	}
	
	//This function handles the player death, animating it and then creating a death effect and gibs
	IEnumerator Die()
	{
		isAlive = false;
		
		//Reset the game speed back to 1
		Time.timeScale = 1;
		
		//Diable the player collider so we can's touch any other objects
		GetComponent<Collider2D>().enabled = false;
		
		//Play the dead animation
		if ( deadAnimation ) 
		{
			GetComponent<Animation>().Play(deadAnimation.name);
			
			yield return new WaitForSeconds(deadAnimation.length);
		}
		
		//If there are gib objects ( objects that fall off a body when it dies ) assigned from the player, launch them off the player
		if ( gibs.Length > 0 )
		{
			//Go through all available gibs
			foreach ( Transform gib in gibs )
			{
				//Detach the gib from the parent
				gib.parent = null;
				
				//Add a rigidbody2D to it
				gib.gameObject.AddComponent<Rigidbody2D>();
				
				//Set the scale to very low to make it float slowly
				gib.GetComponent<Rigidbody2D>().gravityScale = 0.01f;
				
				//Set the scale to 1
				gib.localScale = new Vector3( 1, 1, gib.localScale.z);
				
				//Give it a velocity to make it float away from the player				
				gib.GetComponent<Rigidbody2D>().angularVelocity = (gib.position.x - thisTransform.position.x) * 50;
				gib.GetComponent<Rigidbody2D>().velocity = new Vector2( gib.position.x - thisTransform.position.x, gib.position.y - thisTransform.position.y);
			}
		}
		
		//Create a dead effect after the player is destroyed
		if ( deadEffect ) Instantiate( deadEffect, thisTransform.position, Quaternion.identity);
		
		//Run the game over function in the game controller
		if ( gameController )    gameController.SendMessage("GameOver");
		
		//Deactivate the player game object
		gameObject.SetActive(false);
	}

}




