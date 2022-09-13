using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This script is responsible for placing tiles on tilemap.
/// Add this component to Your Grid object.
/// </summary>

[RequireComponent(typeof(Grid))]
public class TilemapGenerator : MonoBehaviour
{
    #region INSPECTOR FIELDS
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap floorDecorationsTilemap;
    
    [Header("Tiles")]
    [SerializeField] private List<TileBase> floorTiles = new List<TileBase>();
    [SerializeField] private List<TileBase> wallTiles = new List<TileBase>();
    [SerializeField] private List<TileBase> floorDecorationsTiles = new List<TileBase>();
    #endregion

    /// <summary>
    /// Generates floor tiles
    /// </summary>
    /// <param name="floorPositions"> positions to spawn floor tiles </param>
    public void GenerateFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        ClearTilemap(); // clear tilemap to prevent overwriting

        foreach (var position in floorPositions)
        {
            GenerateSingleTile(floorTilemap, floorTiles[Random.Range(0, floorTiles.Count)], position);
        }
    }
    
    /// <summary>
    /// Generates floor decoration tiles (bones and vines on floor)
    /// </summary>
    /// <param name="floorPositions"> positions considered for spawning floor decoration tiles on them </param>
    public void GenerateFloorDecorationTiles(IEnumerable<Vector2Int> floorPositions)
    {
        foreach (var position in floorPositions)
        {
            if (Random.value >= 0.95f)
                GenerateSingleTile(floorDecorationsTilemap, floorDecorationsTiles[Random.Range(0, floorDecorationsTiles.Count)], position);
        }
    }

    /// <summary>
    /// Generates single tile
    /// </summary>
    /// <param name="tilemap"> tilemap, on which tile will be spawned </param>
    /// <param name="tile"> tile to generate </param>
    /// <param name="position"> position to spawn tile </param>
    private static void GenerateSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int) position);
        tilemap.SetTile(tilePosition, tile);
    }

    /// <summary>
    /// Generates single wall tile
    /// </summary>
    /// <param name="position"> positions to spawn wall tile </param>
    public void GenerateSingleWallTile(Vector2Int position)
    {
        var randomWallTile = wallTiles[Random.Range(0, wallTiles.Count)];
        GenerateSingleTile(wallTilemap, randomWallTile, position);
    }
    
    /// <summary>
    /// Clears all tilemaps
    /// </summary>
    public void ClearTilemap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorDecorationsTilemap.ClearAllTiles();
    }
    
}