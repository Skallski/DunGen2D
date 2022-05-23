using System.Collections.Generic;
using UnityEngine;

public class EnemyRoom : Room
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    
    /// <summary>
    /// Specific room initialization method
    /// Sets room center and its floor positions then do stuff related to that room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public override void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        Init(centerPosition, floorsPositions); // initialize (set room center position and its floors positions)
        
        // TODO : fill with enemies
        // TODO : add chance to spawn traps
        // TODO : fill with interior objects
    }
    
}