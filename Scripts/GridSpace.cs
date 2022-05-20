using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSpace : MonoBehaviour {
    [SerializeField] public Vector2Int GridSize, GridTileSize;


    [SerializeField] public List<GridTile> Tiles = new List<GridTile>();
    [SerializeField] public List<GridTile> Gates = new List<GridTile>();
    [SerializeField] public GameObject NonNavigatableTilePrefab = null, NavigatableTilePrefab = null, GateTilePrefab = null;

}
