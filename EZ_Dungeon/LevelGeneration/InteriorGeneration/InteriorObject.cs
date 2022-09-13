using System;
using UnityEngine;

[Serializable]
public sealed class InteriorObject
{
    [SerializeField] private GameObject objectPrefab;
    public GameObject ObjectPrefab => objectPrefab;
    
    [SerializeField] private int maxQuantity, minQuantity;
    public int MaxQuantity => maxQuantity;
    public int MinQuantity => minQuantity;
    
    [SerializeField, Tooltip("Width (left to right)")] private int x;
    [SerializeField, Tooltip("Height (top to bottom)")] private int y;
    internal int X => x;
    internal int Y => y;

    [Space]
    [SerializeField] private Vector2 positionOffset = new Vector2(0.5f, 0.5f);
    public Vector2 PositionOffset => positionOffset;
}