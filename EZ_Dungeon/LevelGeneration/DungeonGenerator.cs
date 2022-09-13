using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using SkalluUtils.Editor.Utils;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorEditor : Editor
{
    private DungeonGenerator dungeonGenerator;
    private SerializedProperty useRandomWalk;

    private void OnEnable()
    {
        dungeonGenerator = (DungeonGenerator) target;
        useRandomWalk = serializedObject.FindProperty("useRandomWalk");
    }

    public override void OnInspectorGUI()
    {
        if (dungeonGenerator == null) return;
        
        // default "script" object field
        EditorGUIUtils.DefaultScriptField(MonoScript.FromMonoBehaviour((MonoBehaviour)target));
        
        serializedObject.Update();
        EditorGUILayout.BeginVertical();

        if (!Application.isPlaying)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tilemapGenerator"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("roomSystem"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dungeon Size", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dungeonHeight"));
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Room parameters", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRoomWidth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("minRoomHeight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomOffset"));
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(useRandomWalk);
        if (EditorGUILayout.BeginFadeGroup(useRandomWalk.boolValue ? 1 : 0))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("randomWalkData"));
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
            dungeonGenerator.GenerateDungeon();

        if (GUILayout.Button("Clear"))
            dungeonGenerator.ClearDungeon();
        
        // apply modified properties and repaint
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
        
        if (GUI.changed)
            InternalEditorUtility.RepaintAllViews();
    }
}
#endif

/// <summary>
/// This is the main script responsible for generating dungeon.
/// Add this component to your level generator object.
/// </summary>

[SelectionBase]
public class DungeonGenerator : MonoBehaviour
{
    [Serializable]
    public struct RandomWalkData
    {
        [Tooltip("Number of algorithm iterations")]
        [SerializeField, Range(1, 100)] internal int iterations;
    
        [Tooltip("Number of steps algorithm performs during each iteration")]
        [SerializeField, Range(1, 50)] internal int steps;
    
        [Tooltip("Should the position be random during each iteration? \n YES : more tight passes \n NO : more island shape")]
        [SerializeField] internal bool startRandomlyEachIteration;
    }

    #region INSPECTOR FIELDS
    [Tooltip("Tilemap Generator script reference")]
    [SerializeField] private TilemapGenerator tilemapGenerator; // make sure to set the reference to tilemap generator script
    
    [Tooltip("Room System script reference")]
    [SerializeField] private RoomSystem roomSystem; // make sure to set the reference to room system script
    
    [SerializeField] private int dungeonWidth, dungeonHeight = 20;
    [SerializeField] private int minRoomWidth, minRoomHeight = 4;
    [SerializeField, Range(1, 5)] private int roomOffset = 1;
    
    [Tooltip("Use random walk algorithm? YES / NO")]
    [SerializeField] private bool useRandomWalk = false;
    [SerializeField] private RandomWalkData randomWalkData;
    #endregion
    
    private readonly Vector2Int startPosition = Vector2Int.zero;
    public Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    /// <summary>
    /// Generates new dungeon
    /// </summary>
    public void GenerateDungeon()
    {
        ClearDungeon();
        CreateRooms();
    }

    /// <summary>
    /// Clears dungeon
    /// </summary>
    public void ClearDungeon()
    {
        tilemapGenerator.ClearTilemap();
        roomSystem.ClearContent();
        roomsDictionary.Clear();
    }

    /// <summary>
    /// Create rooms
    /// </summary>
    private void CreateRooms()
    {
        var roomsList = Utils.BinarySpacePartitioning(new BoundsInt((Vector3Int) startPosition, 
            new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        // decides whether to create random shaped or rectangular rooms
        var floor = useRandomWalk
            ? CreateRandomRooms(roomsList)
            : CreateRectangularRooms(roomsList); 

        // create
        if (floor.Count > 0)
        {
            var roomCenterPositionsList = roomsList.Select(room => (Vector2Int) Vector3Int.RoundToInt(room.center)).ToList();

            var corridors = ConnectRooms(roomCenterPositionsList);
            floor.UnionWith(corridors);
        
            // generates floor and walls by placing tiles
            tilemapGenerator.GenerateFloorTiles(floor);
            WallGenerator.GenerateWalls(floor, tilemapGenerator);
            tilemapGenerator.GenerateFloorDecorationTiles(floor);
            
        }
        else
        {
            throw new Exception("Room generation error!");
        }
        
        roomSystem.CreateContent(roomsDictionary);
    }

    /// <summary>
    /// Creates rooms of rectangular shape
    /// </summary>
    /// <param name="roomsList"> list of rooms </param>
    /// <returns> floor hash set </returns>
    private HashSet<Vector2Int> CreateRectangularRooms(IEnumerable<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>();

        foreach (var roomBounds in roomsList)
        {
            for (int column = roomOffset; column < roomBounds.size.x - roomOffset; column++)
            {
                for (int row = roomOffset; row < roomBounds.size.y - roomOffset; row++)
                {
                    var position = (Vector2Int) roomBounds.min + new Vector2Int(column, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    /// <summary>
    /// Creates rooms of more random shape
    /// </summary>
    /// <param name="roomsList"> list of rooms </param>
    /// <returns> floor hash set </returns>
    private HashSet<Vector2Int> CreateRandomRooms(IEnumerable<BoundsInt> roomsList)
    {
        var floor = new HashSet<Vector2Int>(); // whole floor

        foreach (var roomBounds in roomsList)
        {
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = PerformRandomWalk(randomWalkData, roomCenter);
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
    private static IEnumerable<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
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
    private static IEnumerable<Vector2Int> ConnectRooms(IList<Vector2Int> roomCenterPositionsList)
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
    private static Vector2Int FindClosestPoint(Vector2Int currentRoomCenter, IEnumerable<Vector2Int> roomCenterPositionsList)
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
    /// <param name="simpleRandomWalkInputData"> random walk data preset </param>
    /// <param name="position"> starting position </param>
    /// <returns> set of points indicating positions of floor tiles </returns>
    private static IEnumerable<Vector2Int> PerformRandomWalk(RandomWalkData simpleRandomWalkInputData, Vector2Int position)
    {
        var currentPosition = position;
        var floorPositions = new HashSet<Vector2Int>();

        for (int i = 0; i < simpleRandomWalkInputData.iterations; i++)
        {
            var path = Utils.RandomWalk(currentPosition, simpleRandomWalkInputData.steps);
            floorPositions.UnionWith(path);

            if (simpleRandomWalkInputData.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count)); // set current position as random floor position
        }

        return floorPositions;
    }
    
}