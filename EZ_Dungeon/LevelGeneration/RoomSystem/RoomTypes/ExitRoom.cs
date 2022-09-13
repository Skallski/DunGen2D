using System.Collections.Generic;
using UnityEngine;

public class ExitRoom : Room
{
    [Space]
    [SerializeField] private GameObject exitPrefab;
    
    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)
        
        Instantiate(exitPrefab, roomCenterPosition + InteriorGenerator.placementOffset, Quaternion.identity, gameObject.transform); // spawns exit prefab
        
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[0]); // grave 1x2
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[1]); // grave 2x1
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[2]); // lantern
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[3]); // spikes
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[4]); // box
    }
    
}