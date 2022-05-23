using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main algorithms used for dungeon generation
/// </summary>
public static class ProceduralGeneration
{
    /// <summary>
    /// Creates path by randomly walking n-steps from starting position
    /// </summary>
    /// <param name="startPosition"> starting position </param>
    /// <param name="steps"> number of steps algorithm take, to calculate path, during single iteration </param>
    /// <returns> set of points that make up the path </returns>
    public static HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int steps)
    {
        var path = new HashSet<Vector2Int> {startPosition};
        var previousPosition = startPosition;

        for (int i = 0; i < steps; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomSimpleDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    /// <summary>
    /// Splits provided space (dungeon area) into smaller rooms
    /// </summary>
    /// <param name="spaceToSplit"> dungeon bounds </param>
    /// <param name="minWidth"> minimal room width (x) </param>
    /// <param name="minHeight"> minimal room height (y) </param>
    /// <returns> list of rooms created as the result of splitting </returns>
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        var roomsQueue = new Queue<BoundsInt>();
        var roomsList = new List<BoundsInt>();
        
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            
            // discard rooms that are too small for room generation
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (Random.value < 0.5f) // horizontal split first
                {
                    if (room.size.y >= minHeight * 2)
                        SplitHorizontally(roomsQueue, room);
                    else if (room.size.x >= minWidth * 2)
                        SplitVertically(roomsQueue, room);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                        roomsList.Add(room);
                }
                else // vertical split first
                {
                    if (room.size.x >= minWidth * 2)
                        SplitVertically(roomsQueue, room);
                    else if (room.size.y >= minHeight * 2)
                        SplitHorizontally(roomsQueue, room);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight)
                        roomsList.Add(room);
                }
            }
        }

        return roomsList;
    }

    /// <summary>
    /// Splits room vertically into two smaller rooms
    /// </summary>
    /// <param name="roomsQueue"> queue of rooms used for partitioning algorithm </param>
    /// <param name="room"> room to split </param>
    private static void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        
        var room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// Splits room horizontally into two smaller rooms
    /// </summary>
    /// <param name="roomsQueue"> queue of rooms used for partitioning algorithm </param>
    /// <param name="room"> room to split </param>
    private static void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        
        var room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        var room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
    
}

/// <summary>
/// Helper class that stores possible directions for random walk algorithm
/// </summary>
public static class Direction2D
{
    // list of simple perpendicular directions
    public static readonly List<Vector2Int> simpleDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // top
        new Vector2Int(1,0), // right
        new Vector2Int(0,-1), // bottom
        new Vector2Int(-1,0) // left
    };
    
    // list of all directions including diagonals
    public static readonly List<Vector2Int> allDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // top
        new Vector2Int(1,1), // top-right
        new Vector2Int(1,0), // right
        new Vector2Int(1,-1), // bottom-right
        new Vector2Int(0,-1), // bottom
        new Vector2Int(-1,-1), // bottom-left
        new Vector2Int(-1,0), // left
        new Vector2Int(-1,1), // top-left
    };

    /// <summary>
    /// Gets random direction (top / right / bottom / left)
    /// </summary>
    /// <returns> normalized vector that indicates the direction </returns>
    public static Vector2Int GetRandomSimpleDirection()
    {
        return simpleDirectionsList[Random.Range(0, simpleDirectionsList.Count)];
    }

    /// <summary>
    /// Gets random direction (top / top-right / right / bottom-right / bottom / bottom-left / left / top-left)
    /// </summary>
    /// <returns> normalized vector that indicates the direction </returns>
    public static Vector2Int GetRandomDirection()
    {
        return allDirectionsList[Random.Range(0, allDirectionsList.Count)];
    }

}