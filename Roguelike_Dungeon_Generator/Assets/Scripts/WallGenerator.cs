using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    /// <summary>
    /// Generates wall tiles
    /// </summary>
    /// <param name="floorPositions"> set of floor positions </param>
    /// <param name="tilemapGenerator"> tilemap generator that will used for generating wall tiles </param>
    public static void GenerateWalls(HashSet<Vector2Int> floorPositions, TilemapGenerator tilemapGenerator)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.simpleDirectionsList);

        foreach (var position in basicWallPositions)
        {
            tilemapGenerator.GenerateSingleWallTile(position);
        }
    }

    /// <summary>
    /// Finds walls in provided directions list
    /// </summary>
    /// <param name="floorPositions"> set of floor positions </param>
    /// <param name="directionsList"> list of directions </param>
    /// <returns> set of wall positions </returns>
    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionsList)
    {
        var wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                // if floor position list does not contain checked position, it means that this position is a wall
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition)) 
                    wallPositions.Add(neighbourPosition);
            }
        }

        return wallPositions;
    }
    
}