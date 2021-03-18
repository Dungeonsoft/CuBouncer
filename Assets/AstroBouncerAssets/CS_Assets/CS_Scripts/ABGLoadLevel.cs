#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// This script includes functions for loading levels and URLs. It's intended for use with UI Buttons
/// </summary>
public class ABGLoadLevel : MonoBehaviour 
{
	//Load a URL
	public void LoadURL( string urlName )
	{
		Application.OpenURL(urlName);
	}
	
	//Load a level from the scene hierarchy
	public void LoadLevel( string levelName )
	{
		Time.timeScale = 1;
		
		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(levelName);
		#else
		Application.LoadLevel(levelName);
		#endif
	}
	
	//This function restarts the currently played level
	public void RestartLevel()
	{
		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		#else
		Application.LoadLevel(Application.loadedLevelName);
		#endif
	}
}
