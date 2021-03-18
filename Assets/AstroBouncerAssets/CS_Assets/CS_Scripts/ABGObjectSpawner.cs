using UnityEngine;
using System.Collections;

/// <summary>
/// This script contains a list of objects that can drop in an area. The chance of each object to drop is set individually.
/// </summary>
public class ABGObjectSpawner : MonoBehaviour 
{
	//A list of the objects that can be spawned
	public ObjectSpawn[] objectSpawns;
	internal Transform[] objectSpawnList;

	[System.Serializable]
	public class ObjectSpawn
	{
		//The object that can be spawned
		public Transform spawnedObject;
		
		//The drop chance of the object
		public int dropChance = 1;
	}
	
	//The area in which objects can be spawned
	public Rect objectSpawnArea = new Rect(-4,-4,4,4);
	
	//How many seconds to wait before starting to spawn objects
	public float spawnDelay = 0;
	
	//How many seconds to wait before dropping another object
	public float objectSpawnRate = 1;
	internal float objectSpawnRateCount = 0;
	
	//Limit the number of objects created by this spawner that are on-screen at the same time. 0 means no limit
	public int limitSpawnNumber = 0;
	
	//This holds the currently spawned objects. If it is full, no more objects are spawned until one of the on-screen objects dies.
	internal Transform[] spawnPool;
	
	//How far should this object be thrown. Objects that are thrown, automatically recieve a rigidbody component
	public Vector2 throwSideSpeed = new Vector2(-2,2);
	public Vector2 throwUpSpeed = new Vector2(8,12);
	
	internal int index = 0;

	// Use this for initialization
	void Start () 
	{
		//Calculate the chances for the objects to drop
		int totalSpawns = 0;
		int totalSpawnsIndex = 0;
		
		//Calculate the total number of drops with their chances
		for ( index = 0 ; index < objectSpawns.Length ; index++ )
		{
			totalSpawns += objectSpawns[index].dropChance;
		}
		
		//Create a new list of the objects that can be spawned
		objectSpawnList = new Transform[totalSpawns];
		
		//Go through the list again and fill out each type of drop based on its drop chance
		for ( index = 0 ; index < objectSpawns.Length ; index++ )
		{
			int dropChanceCount = 0;
			
			while ( dropChanceCount < objectSpawns[index].dropChance )
			{
				objectSpawnList[totalSpawnsIndex] = objectSpawns[index].spawnedObject;
				
				dropChanceCount++;
				
				totalSpawnsIndex++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( spawnDelay > 0 )
		{
			spawnDelay -= Time.deltaTime;
		}
		else
		{
			//Count the drop rate of the objects
			objectSpawnRateCount += Time.deltaTime;
			
			//Spawn an object	
			if ( objectSpawnRateCount >= objectSpawnRate )
			{
				SpawnObject();
				
				objectSpawnRateCount = 0;
			}
		}
	}

	//Spawn a random object for the list of objects
	public void SpawnObject()
	{
		Transform newObject = Instantiate( objectSpawnList[Mathf.FloorToInt(Random.Range(0, objectSpawnList.Length))], new Vector3( Random.Range(objectSpawnArea.x, objectSpawnArea.width), Random.Range(objectSpawnArea.y, objectSpawnArea.height),0), Quaternion.identity) as Transform;
		
		//If the throwing speed is not 0, add a rigidbody and throw the object
		if ( throwSideSpeed.x != 0 && throwSideSpeed.y != 0 )
		{
			if ( !newObject.GetComponent<Rigidbody2D>() )
			{
				newObject.gameObject.AddComponent<Rigidbody2D>();
				
				newObject.GetComponent<Rigidbody2D>().velocity = new Vector2( Random.Range(throwSideSpeed.x, throwSideSpeed.y), Random.Range(throwUpSpeed.x, throwUpSpeed.y));
				
				newObject.GetComponent<Rigidbody2D>().angularVelocity = newObject.GetComponent<Rigidbody2D>().velocity.x * -100;
			}
		}
	}
	
	//This function draws the object spawn area in the editor
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		
		Gizmos.DrawLine( new Vector3(objectSpawnArea.x,objectSpawnArea.y,0), new Vector3(objectSpawnArea.width,objectSpawnArea.y,0));
		Gizmos.DrawLine( new Vector3(objectSpawnArea.x,objectSpawnArea.height,0), new Vector3(objectSpawnArea.width,objectSpawnArea.height,0));
		Gizmos.DrawLine( new Vector3(objectSpawnArea.x,objectSpawnArea.y,0), new Vector3(objectSpawnArea.x,objectSpawnArea.height,0));
		Gizmos.DrawLine( new Vector3(objectSpawnArea.width,objectSpawnArea.y,0), new Vector3(objectSpawnArea.width,objectSpawnArea.height,0));
	}
}







