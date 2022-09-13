using System;
using UnityEngine;

[Serializable]
public sealed class EnemyToSpawn
{
    [SerializeField] private GameObject objectPrefab;
    public GameObject ObjectPrefab => objectPrefab;
    
    [SerializeField] private int maxQuantity, minQuantity;
    public int MaxQuantity => maxQuantity;
    public int MinQuantity => minQuantity;
    
    [Tooltip("From which dungeon level this enemy can appear")]
    [SerializeField] private int appearanceLevel;
    public int AppearanceLevel => appearanceLevel;
    
    public readonly Vector2 positionOffset = new Vector2(0.5f, 0.5f);
}