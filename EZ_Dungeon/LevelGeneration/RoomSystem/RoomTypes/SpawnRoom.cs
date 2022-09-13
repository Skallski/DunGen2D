using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : Room
{
    [Space]
    [SerializeField] private GameObject playerPrefab;

    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)
        
        SpawnPlayer();
        
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[0]); // grave 1x2
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[1]); // grave 2x1
        SpawnInteriorObjectRandomly(roomData.SpawnableInteriorObjects[2]); // lantern
    }

    // Spawns player character or moves existing player to spawn point
    private void SpawnPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerSpawnPosition = roomCenterPosition + InteriorGenerator.placementOffset;

        if (player != null)
            player.transform.position = playerSpawnPosition;
        // else
        //     Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
    }

}