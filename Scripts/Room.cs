using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] public Vector2Int GridSize, GridTileSize;

    [SerializeField] public List<GridTile> Tiles = new List<GridTile>();
    [SerializeField] public List<GridTile> Gates = new List<GridTile>();
    [SerializeField] public GameObject NonNavigatableTilePrefab = null, NavigatableTilePrefab = null, GateTilePrefab = null;

    public Vector3 WorldSize => new Vector3(GridSize.x * GridTileSize.x, GridSize.y * GridTileSize.y, 0);
}
