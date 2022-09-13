using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is responsible for spawning room types. 
/// Add this component to your level generator object (the one with main dungeon generator script attached).
/// </summary>

[RequireComponent(typeof(DungeonGenerator))]
public class RoomSystem : MonoBehaviour
{
    #region ROOM PREFABS
    [Header("Room prefabs")]
    [SerializeField] private SpawnRoom spawnRoomPrefab;
    [SerializeField] private ExitRoom exitRoomPrefab;
    [SerializeField] private EnemyRoom enemyRoomPrefab;
    [SerializeField] private TreasureRoom treasureRoomPrefab;
    [SerializeField] private ShopRoom shopRoomPrefab;
    #endregion

    [HideInInspector] public List<GameObject> createdRooms = new List<GameObject>();

    [Header("Special rooms spawn chance parameters")]
    [SerializeField, Range(0f, 1f)] private float treasureRoomChance = 0.15f;
    [SerializeField, Range(0f, 1f)] private float shopRoomChance = 0.3f;
    
    private bool treasureRoomSpawned = false;
    private bool shopRoomSpawned = false;

    public void CreateContent(Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary)
    {
        if (roomsDictionary.Count > 0)
        {
            for (int i = 0; i < roomsDictionary.Keys.Count; i++)
            {
                if (i == 0) // first room is always a spawn room
                {
                    var spawnRoom = Instantiate(spawnRoomPrefab);
                    spawnRoom.Create(roomsDictionary.ElementAt(0).Key, roomsDictionary.ElementAt(0).Value);
                    createdRooms.Add(spawnRoom.gameObject);
                }
                else if (i == roomsDictionary.Keys.Count - 1) // last room is always an exit room
                {
                    var exitRoom = Instantiate(exitRoomPrefab);
                    exitRoom.Create(roomsDictionary.ElementAt(roomsDictionary.Keys.Count - 1).Key, roomsDictionary.ElementAt(roomsDictionary.Keys.Count - 1).Value);
                    createdRooms.Add(exitRoom.gameObject);
                }
                else // other rooms
                {
                    Room roomToSpawn;
                    
                    if (!treasureRoomSpawned && Random.value <= treasureRoomChance) // chance to spawn treasure room
                    {
                        roomToSpawn = Instantiate(treasureRoomPrefab);
                        
                        treasureRoomSpawned = true;
                    }
                    else if (!shopRoomSpawned && Random.value <= shopRoomChance) // chance to spawn shop room
                    {
                        roomToSpawn = Instantiate(shopRoomPrefab);
                        
                        shopRoomSpawned = true;
                    }
                    else // otherwise spawn enemy room
                    {
                        roomToSpawn = Instantiate(enemyRoomPrefab);
                    }
                    
                    roomToSpawn.Create(roomsDictionary.ElementAt(i).Key, roomsDictionary.ElementAt(i).Value);
                    createdRooms.Add(roomToSpawn.gameObject);
                }
            }
        }
    }

    public void ClearContent()
    {
        foreach (var room in createdRooms)
        {
            DestroyImmediate(room);
        }
        
        createdRooms.Clear();

        // reset special rooms flags
        treasureRoomSpawned = false;
        shopRoomSpawned = false;
    }

}