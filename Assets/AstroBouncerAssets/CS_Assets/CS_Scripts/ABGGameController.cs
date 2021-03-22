#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This script controls the game, starting it, following game progress, and finishing it with game over. It also spawn items and enemies and shows/hides enemies
/// from the wall. When the game levels up, it increases speed and chance of enemy appearnace from the wall.
/// </summary>
public class ABGGameController : MonoBehaviour 
{
	//A list of player objects, and the number of the current player
	public Transform[] playerObjects;
	public int currentPlayer = 0;
	
	//The jump button of the player. This is defined in Input Manager
	public string jumpButton = "Jump";
	
	//Should the jump button also set the direction of the player?
	public bool jumpWithDirection = false;
	
	//The area within which the player can jump
	public Rect moveArea = new Rect(-8.5f,-4,8.5f,4);
	
	//Should the player wrap around the move area instead of bouncing off walls
	public bool wrapAroundMoveArea = false;
	
	//A list of the objects that can be dropped
	public ObjectDrop[] objectDrops;
	internal Transform[] objectDropList;

	[System.Serializable]
	public class ObjectDrop
	{
		//The object that can be dropped
		public Transform droppedObject;
		
		//The drop chance of the object
		public int dropChance = 1;
	}
	
	//The area in which objects can be dropped
	public Rect objectDropArea = new Rect(-5,-6,5,7);
	
	//How many seconds to wait before dropping another object
	public float objectDropRate = 1;
	internal float objectDropRateCount = 0;
	
	//The walls which hold enemies within them
	public Transform rightWall;
	public Transform leftWall;
	
	//The score and score text of the player
	static int score = 0;
	public Transform scoreText;
	internal int highScore = 0;
	
	//The overall game speed, and how much it increases with each level up

    //게임의 기본 스피드 값
	public float gameSpeed = 1;
    //레벨업시 증가하는 스피드 값
	public float gameSpeedIncrease = 0.2f;
	
	//The chance of a spike enemy showing from the wall, and how much it increases with each level up
    //장애물 등장 확률
	public float spikeChance = 0.1f;
    //레벨업시 상승하는 장애물 등장 확률
	public float spikeChanceIncrease = 0.05f;
	
	//How many points the player needs to collect before leveling up
    //레벨업에 요구되는 스코어 값
	public int levelUpEvery = 15;
    //레벨 상승에 따라 요구되는 스코어 가중치
	internal int increaseCount = 0;
	
	//Various canvases for the UI
	public Transform gameCanvas;
	public Transform pauseCanvas;
	public Transform gameOverCanvas;
	
	//Is the game over?
	internal bool isGameOver = false;
	
	//The level of the main menu that can be loaded after the game ends
	public string mainMenuLevelName = "StartMenu";
	
	//Various sounds
	public AudioClip soundLevelUp;
	public AudioClip soundGameOver;
	
	//The tag of the sound source
	public string soundSourceTag = "GameController";
	
	//The button that pauses the game. Clicking on the pause button in the UI also pauses the game
	public string pauseButton = "Cancel";
	internal bool isPaused = false;
	
	//Did we start the game?
	internal bool gameStart = true;
	
	internal int index = 0;

    //time over 
    public float limitTime;
    public Text timeText;
    public float overTime = 0;

    // Use this for initialization
    void Start () 
	{
		//Update the score without adding to it
		ChangeScore(0);
		
		//Get the currently selected player from PlayerPrefs
		currentPlayer = PlayerPrefs.GetInt("CurrentPlayer", currentPlayer);
		
		//Set the current player object
		SetPlayer(currentPlayer);
		
		//Hide the game over canvas
		if ( gameOverCanvas )    gameOverCanvas.gameObject.SetActive(false);
		
		//Get the highscore for the player on this level
		#if UNITY_5_3_OR_NEWER
		highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "_HighScore", 0);
		#else
		highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "_HighScore", 0);
		#endif

		//Calculate the chances for the objects to drop
		int totalDrops = 0;
		int totalDropsIndex = 0;
		
		//Calculate the total number of drops with their chances
		for ( index = 0 ; index < objectDrops.Length ; index++ )
		{
			totalDrops += objectDrops[index].dropChance;
		}
		
		//Create a new list of the objects that can be dropped
		objectDropList = new Transform[totalDrops];
		
		//Go through the list again and fill out each type of drop based on its drop chance
		for ( index = 0 ; index < objectDrops.Length ; index++ )
		{
			int dropChanceCount = 0;
			
			while ( dropChanceCount < objectDrops[index].dropChance )
			{
				objectDropList[totalDropsIndex] = objectDrops[index].droppedObject;
				
				dropChanceCount++;
				
				totalDropsIndex++;
			}
		}
		
		//Pause the game at the start
		Pause();
	}
	
	// Update is called once per frame
	void Update () 
	{
        //Assign the player object
        //if ( playerObjects[currentPlayer] == null )    playerObjects[currentPlayer] = GameObject.FindGameObjectWithTag(playerTag).transform;		

        
        //time Over
        limitTime -= Time.deltaTime;
        timeText.text = "Time: " + Mathf.Round(limitTime);
        if(limitTime<=0)
        {
            limitTime = 0;
            StartCoroutine(GameOver());
            
        }
        

		//If the game is over, listen for the Restart and MainMenu buttons
		if ( isGameOver == true )
		{
			//The jump button restarts the game
			if ( Input.GetButtonDown(jumpButton) )
			{
				Restart();
			}
			
			//The pause button goes to the main menu
			if ( Input.GetButtonDown(pauseButton) )
			{
				MainMenu();
			}
		}
		else
		{
			if ( objectDropList.Length > 0 )
			{
				//Count the drop rate of the objects
				objectDropRateCount += Time.deltaTime;
				
				//Drop an object	
				if ( objectDropRateCount >= objectDropRate )
				{
					DropObject();
					
					objectDropRateCount = 0;
				}
			}
			
			//If the player presses the jump button, jump!
			if ( Input.GetButtonDown(pauseButton) )
			{
				if ( isPaused == true )    Unpause();
				else    Pause();
			}
			
			if ( playerObjects[currentPlayer] )
			{
				//Jump only if we are within the move area
				if ( playerObjects[currentPlayer].position.y < moveArea.height )
				{
					//If the player jumps while the game is paused, unpause it
					if ( isPaused == true && Input.GetButtonDown(jumpButton) )
					{
						Unpause();			
					}
					else
					{
						if ( jumpWithDirection == true )
						{
							//If we press the positive jump axis, jump right
							if ( Input.GetAxisRaw(jumpButton) > 0 )
							{
								playerObjects[currentPlayer].SendMessage("Jump",1);
							}
							
							//If we press the negative jump axis, jump left
							if ( Input.GetAxisRaw(jumpButton) < 0 )
							{
								playerObjects[currentPlayer].SendMessage("Jump",-1);
							}
						}
						else
						{
							//If we press the jump button, jump
							if ( Input.GetButtonDown(jumpButton) )
							{
								playerObjects[currentPlayer].SendMessage("Jump", 0.0f);
							}
						}
					}
				}
				
				//If the player reaches the left edge of the move area, bounce off it to the right
				if ( playerObjects[currentPlayer].position.x < moveArea.x )
				{
					//Place the player at the edge of the move area, so it doesn't accidentally get stuck beyond it
					playerObjects[currentPlayer].position = new Vector3( moveArea.x, playerObjects[currentPlayer].position.y, playerObjects[currentPlayer].position.z);

					if ( wrapAroundMoveArea == true )    playerObjects[currentPlayer].position = new Vector3( moveArea.width, playerObjects[currentPlayer].position.y, playerObjects[currentPlayer].position.z);
					else    playerObjects[currentPlayer].SendMessage("BounceOffWall");
				}
				
				//If the player reaches the left edge of the move area, bounce off it to the left
				if ( playerObjects[currentPlayer].position.x > moveArea.width )
				{
					//Place the player at the edge of the move area, so it doesn't accidentally get stuck beyond it
					playerObjects[currentPlayer].position = new Vector3( moveArea.width, playerObjects[currentPlayer].position.y, playerObjects[currentPlayer].position.z);

					
					if ( wrapAroundMoveArea )    playerObjects[currentPlayer].position = new Vector3( moveArea.x, playerObjects[currentPlayer].position.y, playerObjects[currentPlayer].position.z);
					else    playerObjects[currentPlayer].SendMessage("BounceOffWall");
				}
			}
		}
	}

	//Drop a random object for the list of objects
	public void DropObject()
	{
		Instantiate( objectDropList[Mathf.FloorToInt(Random.Range(0, objectDropList.Length))], new Vector3( Random.Range(objectDropArea.x, objectDropArea.width), Random.Range(objectDropArea.y, objectDropArea.height),0), Quaternion.identity);
	}
	
	//This function changes the score of the player
	public void ChangeScore( int changeValue )
	{
		//Change the score
		score += changeValue;
		
		//Update the score text
		if ( scoreText )    scoreText.GetComponent<Text>().text = score.ToString();
		
		//Increase the counter to the next level
		increaseCount += changeValue;
		
		//If we reached the required number of points, level up!
		if ( increaseCount >= levelUpEvery )
		{
			increaseCount -= levelUpEvery;
			
			LevelUp();
		}
	}
	
	//This function levels up, and increases the difficulty of the game
	public void LevelUp()
	{
		//Increase the chance of spikes appearing from the wall
		spikeChance += spikeChanceIncrease;
		
		//Increase the speed of the game
		gameSpeed += gameSpeedIncrease;
		
		//Set time scale to game speed
		Time.timeScale = gameSpeed;
		
		//If there is a source and a sound, play it from the source
		if ( soundSourceTag != "" && soundLevelUp )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundLevelUp);
		
		//Shake the camera
		if ( GetComponent<ABGShakeCamera>() )    GetComponent<ABGShakeCamera>().StartShake();
	}
	
	//This function runs when the player hits the wall. It flips the player's direction and shows/hides some of the spikes in the walls
	public void HitWall( string wallSide )
	{
		//Hitting the left wall
		if ( wallSide == "left" )
		{
			if ( leftWall )
			{
				//Go through all the spikes in the wall, hide some of them, and show some 
				foreach ( Transform spike in leftWall )
				{
					if ( Random.value > spikeChance )    spike.gameObject.SendMessage("Hide");
					else    spike.gameObject.SendMessage("Show");
				}
				
				//Make sure at least one of the spikes is hidden, to give the player a chance to bounce off without being hit
				leftWall.GetChild(Mathf.FloorToInt(Random.Range(0, leftWall.childCount))).gameObject.SendMessage("Hide");
			}
		}
		else if ( wallSide == "right" )
		{
			if ( rightWall )
			{
				//Go through all the spikes in the wall, hide some of them, and show some 
				foreach ( Transform spike in rightWall )
				{
					if ( Random.value > spikeChance )    spike.gameObject.SendMessage("Hide");
					else    spike.gameObject.SendMessage("Show");
				}
				
				//Make sure at least one of the spikes is hidden, to give the player a chance to bounce off without being hit
				rightWall.GetChild(Mathf.FloorToInt(Random.Range(0, rightWall.childCount))).gameObject.SendMessage("Hide");
			}
		}
	}
	
	//This function pauses the game
	public void Pause()
	{
		isPaused = true;
		
		//Set timescale to 0, preventing anything from moving
		Time.timeScale = 0;
		
		//Show the pause screen and hide the game screen
		if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);
		if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);
	}
	
	public void Unpause()
	{
		isPaused = false;
		
		//Set timescale back to the current game speed
		Time.timeScale = gameSpeed;
		
		//Hide the pause screen and show the game screen
		if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
		if ( gameCanvas )    gameCanvas.gameObject.SetActive(true);
		
		//if we are at the start of the game, make the player jump
		if ( playerObjects[currentPlayer] && score == 0 && gameStart == true )    
		{
			playerObjects[currentPlayer].SendMessage("Jump", 1);
			
			gameStart = false;		
		}
	}
	
	//This function triggers a jump function on the player object associated with this gamecontroller
	//It is used to bypass having to reassign the player object on the jump buttons in the 4.6 UI
	public void PlayerJump( int jumpDirection )
	{
		if ( playerObjects[currentPlayer] )    
		{
			if ( playerObjects[currentPlayer].position.y < moveArea.height )    playerObjects[currentPlayer].SendMessage("Jump", jumpDirection);
		}
	}
	
	//This function handles the game over event
	IEnumerator GameOver()
	{
		if ( isGameOver == false )
		{
			yield return new WaitForSeconds(1);

			isGameOver = true;
			
			//If there is a source and a sound, play it from the source
			if ( soundSourceTag != "" && soundGameOver )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundGameOver);
			
			//Remove the pause and game screens
			if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
			if ( gameCanvas )    Destroy(gameCanvas.gameObject);
			
			//Show the game over screen
			if ( gameOverCanvas )    
			{
				//Show the game over screen
				gameOverCanvas.gameObject.SetActive(true);
				
				//Write the score text
				gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
				
				//Check if we got a high score
				if ( score > highScore )    
				{
					highScore = score;
					
					//Register the new high score
					#if UNITY_5_3_OR_NEWER
					PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "_HighScore", score);
					#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "_HighScore", score);
					#endif
				}
				
				//Write the high sscore text
				gameOverCanvas.Find("TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();
			}
			
			//Reset the global score
			ABGGameController.score = 0;
			
			Time.timeScale = 0;
		}
	}

   
    //This function handles the victory event
    IEnumerator Victory()
	{
		if ( isGameOver == false )
		{
			yield return new WaitForSeconds(1);

			isGameOver = true;
			
			//If there is a source and a sound, play it from the source
			if ( soundSourceTag != "" && soundGameOver )    GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundGameOver);
			
			//Remove the pause and game screens
			if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
			if ( gameCanvas )    Destroy(gameCanvas.gameObject);
			
			//Reset the global score
			ABGGameController.score = 0;
			
			Time.timeScale = 0;
		}
	}
	
	//This function restarts the current level
	public void Restart()
	{
		Time.timeScale = 1;

		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		#else
		Application.LoadLevel(Application.loadedLevelName);
		#endif
	}
	
	//This function returns to the Main Menu
	public void MainMenu()
	{
		Time.timeScale = 1;

		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(mainMenuLevelName);
		#else
		Application.LoadLevel(mainMenuLevelName);
		#endif
	}
	
	//This function activates the selected player, while deactivating all the others
	public void SetPlayer( int playerNumber )
	{
		//Go through all the players, and hide each one except the current player
		for( index = 0; index < playerObjects.Length; index++ )
		{
			if ( index != playerNumber )    playerObjects[index].gameObject.SetActive(false);
			else    playerObjects[index].gameObject.SetActive(true);
		}
	}
	
	//This function draws the object spawn area in the editor
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		
		Gizmos.DrawLine(new Vector3(objectDropArea.x,objectDropArea.y,0), new Vector3(objectDropArea.width,objectDropArea.y,0));
		Gizmos.DrawLine(new Vector3(objectDropArea.x,objectDropArea.height,0), new Vector3(objectDropArea.width,objectDropArea.height,0));
		Gizmos.DrawLine(new Vector3(objectDropArea.x,objectDropArea.y,0), new Vector3(objectDropArea.x,objectDropArea.height,0));
		Gizmos.DrawLine(new Vector3(objectDropArea.width,objectDropArea.y,0), new Vector3(objectDropArea.width,objectDropArea.height,0));
		
		//Draw the player's movements area
		Gizmos.color = Color.green;
		
		Gizmos.DrawLine(new Vector3(moveArea.x,moveArea.y,0), new Vector3(moveArea.width,moveArea.y,0));
		Gizmos.DrawLine(new Vector3(moveArea.x,moveArea.height,0), new Vector3(moveArea.width,moveArea.height,0));
		Gizmos.DrawLine(new Vector3(moveArea.x,moveArea.y,0), new Vector3(moveArea.x,moveArea.height,0));
		Gizmos.DrawLine(new Vector3(moveArea.width,moveArea.y,0), new Vector3(moveArea.width,moveArea.height,0));
	}
}





