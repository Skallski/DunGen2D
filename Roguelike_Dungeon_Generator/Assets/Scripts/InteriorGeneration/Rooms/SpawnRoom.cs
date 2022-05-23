﻿using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : Room
{
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

        // TODO: spawn player character in center
        
        SpawnInteriorObject(interiorObjectsList[1]);
        SpawnInteriorObject(interiorObjectsList[0]);
    }

}