using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ABGPlayerSelection : MonoBehaviour 
{
	//A list of the player icons that can be selected
	public Transform[] playerIcons;
	
	//The currently selected player
	public int currentPlayer = 0;
	
	//The player prefs record name. It's where we save the current player so we can load it in the level
	public string playerPrefsName = "CurrentPlayer";
	
	internal int index = 0;

	// Use this for initialization
	void Start() 
	{
		//Get the current player and change the icon accordingly
		currentPlayer = PlayerPrefs.GetInt(playerPrefsName, currentPlayer);
		
		SetPlayer(currentPlayer);
	}
	
	//This function changes the current player
	public void ChangePlayer( int changeValue )
	{
		currentPlayer += changeValue;
		
		if ( currentPlayer > playerIcons.Length - 1 )    currentPlayer = 0;
		if ( currentPlayer < 0 )    currentPlayer = playerIcons.Length - 1;
		
		SetPlayer(currentPlayer);
	}
	
	//This function activates the selected player, while deactivating all the others
	public void SetPlayer( int playerNumber )
	{
		//Go through all the players, and hide each one except the current player
		for( index = 0; index < playerIcons.Length; index++ )
		{
			if ( index != playerNumber )    playerIcons[index].gameObject.SetActive(false);
			else    playerIcons[index].gameObject.SetActive(true);
		}
		
		PlayerPrefs.SetInt(playerPrefsName,playerNumber);
	}
}
