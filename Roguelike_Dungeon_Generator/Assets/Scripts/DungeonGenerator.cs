using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    private static DungeonGenerator _self;
    public static DungeonGenerator self => _self;
    
    #region INSPECTOR FIELDS
    [Header("Related scripts")]
    [SerializeField] private TilemapGenerator tilemapGenerator;
    [SerializeField] private RoomSystem roomSystem;
    
    [Header("Dungeon size")]
    [SerializeField] private int dungeonWidth = 50;
    [SerializeField] private int dungeonHeight = 50;
    
    [Header("Room parameters")]
    [SerializeField] private int minRoomWidth = 10;
    [SerializeField] private int minRoomHeight = 10;
    [SerializeField] [Range(1, 5)] private int roomOffset = 2;
    #endregion

    #region RANDOM WALK PARAMETERS
    [Header("Random walk data")]
    public bool useRandomWalk = true;
    [HideInInspector] public int iterations;
    [HideInInspector] public int steps;
    [HideInInspector] public bool startRandomlyEachIteration;
    #endregion

    private readonly Vector2Int startPosition = Vector2Int.zero; // generation starting position
    
    public Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>(); // stores center position and floors positions set for each room

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
    /// Clears old dungeon and generates new one
    /// </summary>
    public void GenerateDungeon()
    {
        ClearDungeon();
        CreateRooms();
    }

    /// <summary>
    /// Clears generated dungeon and its whole content
    /// </summary>
    public void ClearDungeon()
    {
        tilemapGenerator.ClearTilemap();

        roomSystem.DeleteContent();
        roomsDictionary.Clear();
    }
    
    /// <summary>
    /// Creates dungeon rooms
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void CreateRooms()
    {
        var roomsList = ProceduralGeneration.BinarySpacePartitioning(new BoundsInt((Vector3Int) startPosition, 
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = useRandomWalk ? CreateRandomRooms(roomsList) : CreateRectangularRooms(roomsList); // decides whether to create random shaped or rectangular rooms

        if (floor.Count > 0)
        {
            var roomCenterPositionsList = new List<Vector2Int>();
            foreach (var room in roomsList)
            {
                roomCenterPositionsList.Add((Vector2Int) Vector3Int.RoundToInt(room.center));
            }

            var corridors = ConnectRooms(roomCenterPositionsList);
            floor.UnionWith(corridors);
        
            // generates floor and walls by placing tiles
            tilemapGenerator.GenerateFloorTiles(floor);
            WallGenerator.GenerateWalls(floor, tilemapGenerator);
            tilemapGenerator.GenerateFloorDecorationTiles(floor);
            
            roomSystem.CreateContent(roomsDictionary);
        }
        else
        {
            throw new Exception("Room generation error!");
        }
    }
    
    /// <summary>
    /// Creates rooms of rectangular shape
    /// </summary>
    /// <param name="roomsList"> list of rooms </param>
    /// <returns> floor hash set </returns>
    private HashSet<Vector2Int> CreateRectangularRooms(List<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>();
        var roomFloorWithOffset = new HashSet<Vector2Int>(); // room's floor positions with offset to apply

        foreach (var room in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y));
            
            for (int column = roomOffset; column < room.size.x - roomOffset; column++)
            {
                for (int row = roomOffset; row < room.size.y - roomOffset; row++)
                {
                    var position = (Vector2Int) room.min + new Vector2Int(column, row);
                    floor.Add(position);
                    roomFloorWithOffset.Add(position); // room's floor positions with offset applied
                }
            }
            
            roomsDictionary.Add(roomCenter, roomFloorWithOffset); // save created room inside dictionary
        }

        return floor;
    }

    /// <summary>
    /// Creates rooms of more random shape
    /// </summary>
    /// <param name="roomsList"> list of rooms </param>
    /// <returns> floor hash set </returns>
    private HashSet<Vector2Int> CreateRandomRooms(List<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>(); // whole floor

        foreach (var roomBounds in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = PerformRandomWalk(roomCenter);
            var roomFloorWithOffset = new HashSet<Vector2Int>(); // room's floor positions with offset to apply
            
            // apply offset
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + roomOffset) && position.x <= (roomBounds.xMax - roomOffset) &&
                    position.y >= (roomBounds.yMin - roomOffset) && position.y <= (roomBounds.yMax - roomOffset))
                {
                    floor.Add(position);
                    roomFloorWithOffset.Add(position); // room's floor positions with offset applied
                }
            }

            roomsDictionary.Add(roomCenter, roomFloorWithOffset); // save created room inside dictionary
        }

        return floor;
    }
    
    /// <summary>
    /// Creates corridor between two points
    /// </summary>
    /// <param name="currentRoomCenter"> corridor start position </param>
    /// <param name="destination"> corridor end position </param>
    /// <returns> set of points, which make up the corridor </returns>
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        var position = currentRoomCenter;
        var corridor = new HashSet<Vector2Int> {position};

        // go up or down
        while (position.y != destination.y)
        {
            if (destination.y > position.y) // go up
                position += Vector2Int.up;
            else if (destination.y < position.y) // go down
                position += Vector2Int.down;

            corridor.Add(position);
        }

        // go right or left
        while (position.x != destination.x)
        {
            if (destination.x > position.x) // go right
                position += Vector2Int.right;
            else if (destination.x < position.x) // go left
                position += Vector2Int.left;

            corridor.Add(position);
        }

        return corridor;
    }

    /// <summary>
    /// Connects rooms by creating corridors
    /// </summary>
    /// <param name="roomCenterPositionsList"> list of room's center positions </param>
    /// <returns> corridors hash set </returns>
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenterPositionsList)
    {
        var corridors = new HashSet<Vector2Int>();
        
        var currentRoomCenter = roomCenterPositionsList[Random.Range(0, roomCenterPositionsList.Count)]; // set random room center as start point

        roomCenterPositionsList.Remove(currentRoomCenter);

        while (roomCenterPositionsList.Count > 0)
        {
            var closestPoint = FindClosestPoint(currentRoomCenter, roomCenterPositionsList);
            roomCenterPositionsList.Remove(closestPoint);
            var newCorridor = CreateCorridor(currentRoomCenter, closestPoint);
            currentRoomCenter = closestPoint;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }
    
    /// <summary>
    /// Finds closest point from provided point
    /// </summary>
    /// <param name="currentRoomCenter"> starting point </param>
    /// <param name="roomCenterPositionsList"> list of room's center positions  </param>
    /// <returns> Vector2Int value that is considered as the closest point </returns>
    private Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, List<Vector2Int> roomCenterPositionsList)
    {
        var closestPoint = Vector2Int.zero;
        var distance = float.MaxValue;

        foreach (var position in roomCenterPositionsList)
        {
            var currentDistance = Vector2.Distance(position, currentRoomCenter);

            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestPoint = position;
            }
        }

        return closestPoint;
    }
    
    /// <summary>
    /// Performs random walk algorithm
    /// </summary>
    /// <param name="position"> starting position </param>
    /// <returns> set of points indicating positions of floor tiles </returns>
    private HashSet<Vector2Int> PerformRandomWalk(Vector2Int position)
    {
        var currentPosition = position;
        var floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralGeneration.RandomWalk(currentPosition, steps);
            floorPositions.UnionWith(path);

            if (startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // set current position as random floor position
        }

        return floorPositions;
    }
    
}