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
        var basicWallPositions = FindWallsInDirections(floorPositions, Utils.Direction2D.FourDirectionsList);

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
    private static IEnumerable<Vector2Int> FindWallsInDirections(ICollection<Vector2Int> floorPositions, IReadOnlyCollection<Vector2Int> directionsList)
    {
        var wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionsList)
            {
                var neighbourPosition = position + direction;
                
                if (!floorPositions.Contains(neighbourPosition)) // if floor position list does not contain checked position, it means that this position is a wall
                    wallPositions.Add(neighbourPosition);
            }
        }

        return wallPositions;
    }
    
}