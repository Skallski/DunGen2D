using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Room : MonoBehaviour
{
    #region ROOM POSITIONS STUFF AND CONTENT
    protected Vector2Int roomCenterPosition;
    private HashSet<Vector2Int> roomFloorsPositions;
    protected List<Vector2Int> availableFloorsPositions;
    
    [SerializeField] protected RoomData roomData;
    #endregion

    /// <summary>
    /// Initialization method that is overriden by specific room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public virtual void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions) {}

    /// <summary>
    /// Room general initialization method
    /// Sets room center and its floor positions
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    protected void Init(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions)
    {
        transform.position = (Vector2) centerPosition;
        
        roomCenterPosition = centerPosition;
        roomFloorsPositions = floorsPositions;

        availableFloorsPositions = roomFloorsPositions.ToList();
        availableFloorsPositions.Remove(centerPosition);
        
        // create walkable paths from room's center in four directions
        var walkablePaths = new List<Vector2Int>();
        walkablePaths.AddRange(Utils.NStepWalk(roomCenterPosition, 10, Utils.Direction2D.FourDirectionsList[0]));
        walkablePaths.AddRange(Utils.NStepWalk(roomCenterPosition, 10, Utils.Direction2D.FourDirectionsList[1]));
        walkablePaths.AddRange(Utils.NStepWalk(roomCenterPosition, 10, Utils.Direction2D.FourDirectionsList[2]));
        walkablePaths.AddRange(Utils.NStepWalk(roomCenterPosition, 10, Utils.Direction2D.FourDirectionsList[3]));

        foreach (var position in walkablePaths)
        {
            availableFloorsPositions.Remove(position);
        }
    }

    #region SPAWNING INTERIOR OBJECTS
    protected void SpawnInteriorObjectNearWall(InteriorObject interiorObject) => 
        InteriorGenerator.SpawnInteriorObjectNearWall(interiorObject, availableFloorsPositions, gameObject.transform);

    protected void SpawnInteriorObjectRandomly(InteriorObject interiorObject) =>
        InteriorGenerator.SpawnInteriorObjectRandomly(interiorObject, availableFloorsPositions, gameObject.transform);
    #endregion
    
}