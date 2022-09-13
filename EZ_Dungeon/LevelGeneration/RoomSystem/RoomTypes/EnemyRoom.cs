using System;
using System.Collections.Generic;
using SkalluUtils.Extensions.ListExtensions;
using UnityEngine;

public class EnemyRoom : Room
{
    private List<EnemyToSpawn> blacklistedEnemies;
    
    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)
        
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[0]); // grave 1x2
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[1]); // grave 2x1
        
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[2]); // lantern
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[3]); // spikes

        SpawnEnemies();
    }

    #region SPAWNING ENEMIES
    /// <summary>
    /// Spawns enemies
    /// </summary>
    protected void SpawnEnemies()
    { 
        if (!roomData.CanSpawnEnemies) return;
        
        if (roomData.SpawnableEnemies.Count == 0)
            throw new Exception(gameObject.name + " Room has no 'spawnable enemies' attached, but trying to spawn them!");

        // spawn enemies
        blacklistedEnemies = new List<EnemyToSpawn>();

        for (int i = 1; i < 3; i++)
        {
            InteriorGenerator.SpawnEnemy(GetEnemyToSpawn(), availableFloorsPositions, gameObject.transform);
        }
    }
    
    /// <summary>
    /// Gets enemy to spawn, according to enemy's appearance level
    /// </summary>
    /// <returns> Enemy type that will be spawned </returns>
    private EnemyToSpawn GetEnemyToSpawn()
    {
        var limiter = 10;
        while (limiter > 0)
        {
            var randomEnemy = roomData.SpawnableEnemies.RandomItem();

            if (randomEnemy.AppearanceLevel <= LevelLoader.levelCounter && !blacklistedEnemies.Contains(randomEnemy))
            {
                blacklistedEnemies.Add(randomEnemy);
                return randomEnemy;
            }
    
            blacklistedEnemies.Add(randomEnemy);
    
            limiter--;
        }
    
        return null;
    }
    #endregion
    
}