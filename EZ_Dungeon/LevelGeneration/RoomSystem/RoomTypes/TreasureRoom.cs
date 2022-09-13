using System.Collections.Generic;
using UnityEngine;

public class TreasureRoom : EnemyRoom
{
    [Space]
    [SerializeField] private GameObject treasureChestPrefab;

    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)
        
        Instantiate(treasureChestPrefab, roomCenterPosition + InteriorGenerator.placementOffset, Quaternion.identity, gameObject.transform); // spawns exit prefab
        
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[0]); // lantern
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]); // spikes

        SpawnEnemies();
    }

}