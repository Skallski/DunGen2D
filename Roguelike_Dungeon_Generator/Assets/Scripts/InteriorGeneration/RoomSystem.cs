using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomSystem : MonoBehaviour
{
    private static RoomSystem _self;
    public static RoomSystem self => _self;
    
    #region ROOM PREFABS
    [Header("Room prefabs")]
    [SerializeField] private SpawnRoom spawnRoomPrefab;
    [SerializeField] private ExitRoom exitRoomPrefab;
    [SerializeField] private EnemyRoom enemyRoomPrefab;
    [SerializeField] private SpecialRoom specialRoomPrefab;
    #endregion

    [HideInInspector] public List<GameObject> createdRooms = new List<GameObject>();

    #region SPECIAL ROOM PARAMETERS
    [Header("Special rooms spawn chance parameters")]
    [SerializeField] private float specialRoomChance = 0.15f;
    private bool specialRoomSpawned = false;
    #endregion

    private void Awake()
    {
        if (_self != null && _self != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _self = this;
        }
    }

    /// <summary>
    /// Creates room prefabs
    /// </summary>
    /// <param name="roomsDictionary"></param>
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
                    
                    if (!specialRoomSpawned && Random.value <= specialRoomChance) // chance to spawn treasure room
                    {
                        roomToSpawn = Instantiate(specialRoomPrefab);
                        
                        specialRoomSpawned = true;
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

    /// <summary>
    /// Deletes created rooms and their interiors
    /// </summary>
    public void DeleteContent()
    {
        foreach (var room in createdRooms)
        {
            DestroyImmediate(room);
        }
        
        createdRooms.Clear();

        // reset special rooms flags
        specialRoomSpawned = false;
    }

}