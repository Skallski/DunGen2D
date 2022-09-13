using System.Collections.Generic;
using SkalluUtils.Extensions.ListExtensions;
using UnityEngine;

/// <summary>
/// This class generates interior objects inside rooms
/// Add this component to your level generator object (the one with main dungeon generator and room system scripts attached).
/// </summary>

[RequireComponent(typeof(RoomSystem))]
public class InteriorGenerator : MonoBehaviour
{
    public static readonly Vector2 placementOffset = new Vector2(0.5f, 0.5f);

    #region PUBLIC METHODS
    /// <summary>
    /// Spawns interior object in random place
    /// </summary>
    /// <param name="objectToSpawn"> interior object to spawn </param>
    /// <param name="availablePositions"> room's available floor positions </param>
    /// <param name="parent"> room's transform </param>
    public static void SpawnInteriorObjectRandomly(InteriorObject objectToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(objectToSpawn.MinQuantity, objectToSpawn.MaxQuantity + 1);
        
        for (int i = 0; i < quantity; i++)
        {
            var limiter = 10;

            while (limiter >= 0)
            {
                var spawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                var neighbours = new List<Vector2Int>();
                
                if (objectToSpawn.X > 1 && objectToSpawn.Y > 1) // x & y bigger than 1
                {
                    var current = spawnPosition;

                    for (int y = 1; y <= objectToSpawn.Y; y++)
                    {
                        for (int x = 0; x < objectToSpawn.X; x++)
                            neighbours.Add(new Vector2Int(current.x + x, current.y));

                        current = new Vector2Int(spawnPosition.x , current.y - 1);
                    }
                }
                else if (objectToSpawn.X > 1 && objectToSpawn.Y <= 1) // x bigger than 1
                {
                    for (int x = 0; x < objectToSpawn.X; x++)
                        neighbours.Add(spawnPosition + Utils.Direction2D.FourDirectionsList[1] * x);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y > 1) // y bigger than 1
                {
                    for (int y = 0; y < objectToSpawn.Y; y++)
                        neighbours.Add(spawnPosition + Utils.Direction2D.FourDirectionsList[2] * y);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y <= 1) // x & y equals 1
                {
                    SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                    break;
                }

                if (availablePositions.ContainsAll(neighbours))
                {
                    SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                    break;
                }

                limiter -= 1;
            }
        }
    }
    
    /// <summary>
    /// Spawns interior object in near wall
    /// </summary>
    /// <param name="objectToSpawn"> interior object to spawn </param>
    /// <param name="availablePositions"> room's available floor positions </param>
    /// <param name="parent"> room's transform </param>
    public static void SpawnInteriorObjectNearWall(InteriorObject objectToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(objectToSpawn.MinQuantity, objectToSpawn.MaxQuantity + 1);
        
        for (int i = 0; i < quantity; i++)
        {
            var limiter = 50;

            while (limiter >= 0)
            {
                var spawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                var neighbours = new List<Vector2Int>();

                if (objectToSpawn.X > 1 && objectToSpawn.Y > 1) // x & y bigger than 1
                {
                    var current = spawnPosition;

                    for (int y = 1; y <= objectToSpawn.Y; y++)
                    {
                        for (int x = 0; x < objectToSpawn.X; x++)
                            neighbours.Add(new Vector2Int(current.x + x, current.y));

                        current = new Vector2Int(spawnPosition.x , current.y - 1);
                    }
                }
                else if (objectToSpawn.X > 1 && objectToSpawn.Y <= 1) // x bigger than 1
                {
                    for (int x = 0; x < objectToSpawn.X; x++)
                        neighbours.Add(spawnPosition + Utils.Direction2D.FourDirectionsList[1] * x);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y > 1) // y bigger than 1
                {
                    for (int y = 0; y < objectToSpawn.Y; y++)
                        neighbours.Add(spawnPosition + Utils.Direction2D.FourDirectionsList[2] * y);
                }
                else if (objectToSpawn.X <= 1 && objectToSpawn.Y <= 1) // x & y equals 1
                {
                    if (CheckIfNearWall(spawnPosition, availablePositions) is true)
                    {
                        SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                        break;
                    }
                }

                if (availablePositions.ContainsAll(neighbours))
                {
                    if (CheckIfNearWall(spawnPosition, availablePositions) is true || CheckIfNearWall(neighbours[neighbours.Count - 1], availablePositions) is true)
                    {
                        SpawnInteriorObjectInPlace(objectToSpawn, spawnPosition, availablePositions, neighbours, parent);
                        break;
                    }
                }

                limiter -= 1;
            }
        }
    }

    public static void SpawnEnemy(EnemyToSpawn enemyToSpawn, List<Vector2Int> availablePositions, Transform parent)
    {
        var quantity = Random.Range(enemyToSpawn.MinQuantity, enemyToSpawn.MaxQuantity);
        
        for (int i = 0; i < quantity; i++)
        {
            var limiter = 10;

            while (limiter >= 0)
            {
                var randomSpawnPosition = availablePositions[Random.Range(0, availablePositions.Count)];

                // spawns enemy and removes it's position from list of available positions
                if (availablePositions.Contains(randomSpawnPosition))
                {
                    Instantiate(enemyToSpawn.ObjectPrefab, randomSpawnPosition + enemyToSpawn.positionOffset , Utils.Direction2D.GetRandomRotation(), parent);
                    availablePositions.Remove(randomSpawnPosition);

                    break;
                }

                limiter -= 1;
            }
        }
    }
    #endregion

    #region HELPER METHODS
    /// <summary>
    /// Spawn interior object in selected place according to offset
    /// </summary>
    /// <param name="objectToSpawn"> interior object to spawn </param>
    /// <param name="spawnPosition"> spawn position </param>
    /// <param name="availablePositions"> room's available floor positions </param>
    /// <param name="neighbouringPositions"> neighbouring positions if any </param>
    /// <param name="parent"> room's transform </param>
    private static void SpawnInteriorObjectInPlace(InteriorObject objectToSpawn, Vector2Int spawnPosition, ICollection<Vector2Int> availablePositions, IReadOnlyCollection<Vector2Int> neighbouringPositions, Transform parent)
    {
        Instantiate(objectToSpawn.ObjectPrefab, spawnPosition + objectToSpawn.PositionOffset, Quaternion.identity, parent);
    
        // remove occupied positions form list of available positions
        availablePositions.Remove(spawnPosition);

        if (neighbouringPositions.Count > 0)
        {
            foreach (var pos in neighbouringPositions)
            {
                availablePositions.Remove(pos);
            }
        }
    }
    
    /// <summary>
    /// Check if provided position is near wall
    /// </summary>
    /// <param name="position"> position to check </param>
    /// <param name="availablePositions"> room's available floor positions </param>
    /// <returns> true or false, depending on whether the position is near wall </returns>
    private static bool CheckIfNearWall(Vector2Int position, ICollection<Vector2Int> availablePositions)
    {
        var counter = 0;

        foreach (var dir in Utils.Direction2D.EightDirectionsList)
        {
            if (!availablePositions.Contains(position + dir))
            {
                counter += 1;
            }
            else
            {
                counter -= 1;

                if (counter <= 0)
                    counter = 0;
            }
        }

        return counter >= 3;
    }
    #endregion
    
} 