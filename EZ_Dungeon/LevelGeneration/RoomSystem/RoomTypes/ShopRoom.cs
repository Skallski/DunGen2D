using System.Collections.Generic;
using UnityEngine;

public class ShopRoom : Room
{
    [Space]
    [SerializeField] private GameObject merchantPrefab;
    
    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)

        Instantiate(merchantPrefab, roomCenterPosition + InteriorGenerator.placementOffset, Quaternion.identity, gameObject.transform); // spawns merchant prefab
        
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[0]); // lantern
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[1]); // merchant's cart
        SpawnInteriorObjectNearWall(roomData.SpawnableInteriorObjects[2]); // box
    }
    
}