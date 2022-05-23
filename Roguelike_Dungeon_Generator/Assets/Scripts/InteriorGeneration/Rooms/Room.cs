using System;
using System.Collections.Generic;
using System.Linq;
using SkalluUtils.PropertyAttributes.ReadOnlyInspectorPropertyAttribute;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    [Serializable]
    public struct InteriorObject
    {
        [Tooltip("Object's prefab to spawn")]
        [SerializeField] internal GameObject objectPrefab;
        
        [Tooltip("Maximum number of this object in the room")]
        [SerializeField] internal int quantity;
        
        [Tooltip("Object's width (counting from left to right)")]
        [SerializeField] internal int x;
        
        [Tooltip("Object's height (counting from top to bottom)")]
        [SerializeField] internal int y;
    }

    #region ROOM POSITIONS STUFF
    [SerializeField] [ReadOnlyInspector] protected Vector2Int roomCenterPosition;
    private HashSet<Vector2Int> roomFloorsPositions;
    private List<Vector2Int> availableFloorsPositions; // free room floors positions
    #endregion

    #region CONTENT
    [SerializeField] protected List<InteriorObject> interiorObjectsList;
    protected readonly Vector2 placementOffset = new Vector2(0.5f, 0.5f); // offset helps with placing object on certain tile
    #endregion

    /// <summary>
    /// Initialization method that is overriden by specific room
    /// </summary>
    /// <param name="centerPosition"> room center position </param>
    /// <param name="floorsPositions"> set of floors position for current room </param>
    public virtual void Create(Vector2Int centerPosition, HashSet<Vector2Int> floorsPositions) { }

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
    }

    /// <summary>
    /// Spawns interior object
    /// </summary>
    /// <param name="interiorObjectToSpawn"> interior object to spawn </param>
    protected void SpawnInteriorObject(InteriorObject interiorObjectToSpawn)
    {
        for (int i = 0; i < interiorObjectToSpawn.quantity; i++)
        {
            var limiter = 10; // iteration limit, to prevent infinite searching and performance issues

            while (limiter >= 0)
            {
                var randomSpawnPosition = availableFloorsPositions[Random.Range(0, availableFloorsPositions.Count)];
                var neighbourPosition = randomSpawnPosition; // as default the neighbouring position is a spawn position due to fact that it has to be available

                // sets neighbour position
                if (interiorObjectToSpawn.x > 1)
                {
                    neighbourPosition = randomSpawnPosition + Direction2D.simpleDirectionsList[1] * (interiorObjectToSpawn.x - 1); // right neighbour
                }
                else if (interiorObjectToSpawn.y > 1)
                {
                    neighbourPosition = randomSpawnPosition + Direction2D.simpleDirectionsList[2] * (interiorObjectToSpawn.y - 1); // bottom neighbour
                }

                // spawns object and removes it's position and neighbouring position from list of available positions
                if (availableFloorsPositions.Contains(neighbourPosition))
                {
                    Instantiate(interiorObjectToSpawn.objectPrefab, randomSpawnPosition + placementOffset, Quaternion.identity, gameObject.transform);
                    
                    availableFloorsPositions.Remove(randomSpawnPosition);
                    availableFloorsPositions.Remove(neighbourPosition);
                    
                    break;
                }

                limiter -= 1;
            }
        }
    }

}