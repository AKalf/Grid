using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TileType { Undefined, Gate, NonNavigatable, Navigatable }
[System.Serializable]
public struct GridTile : Pathfinding.IGridTile {

    private Vector3Int position, size;
    public GameObject thisGameObject, objectOnTile;
    private GridSpace context;
    private TileType tileType;
    private int w, h;

    // Properties
    #region Properties and IGridTile implementation
    public TileType TypeOfTile => tileType;

    public int W { get => w; set => w = value; }
    public int H { get => h; set => h = value; }

    public Vector3 GetPosition => position;
    public Vector3 GetSize => size;

    public int WalkingCost { get; set; }
    public int HeuristicCost { get; set; }
    public int TotalCost => WalkingCost + HeuristicCost;
    public bool CanBeNavigated { get; set; }
    public Pathfinding.IGridTile CameFrom { get; set; }
    public GameObject GameObject { get => thisGameObject; set => thisGameObject = value; }
    #endregion
    /// <summary>Constructor for new instance</summary>
    /// <param name="w">W position on grid</param> <param name="h">H position on grid</param>
    /// <param name="context">The grid space that it belongs</param>
    /// <param name="newType">Tile type</param>
    /// <param name="position">Position on world</param>
    /// <param name="tileSize">Tile size</param>
    public GridTile(int w, int h, GridSpace context, TileType newType, Vector3 position, Vector3Int tileSize) {
        this.context = context;
        this.w = w; this.h = h;
        this.position = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        this.size = tileSize;
        this.thisGameObject = null;
        this.objectOnTile = null;
        WalkingCost = 0;
        HeuristicCost = 0;
        CanBeNavigated = true;
        CameFrom = null;
        GameObject objectToSpawn = null;

        if (newType == TileType.Navigatable) {
            CanBeNavigated = true;
            objectToSpawn = context.NavigatableTilePrefab;

        }
        else if (newType == TileType.NonNavigatable) {
            CanBeNavigated = false;
            objectToSpawn = context.NonNavigatableTilePrefab;
        }
        else if (newType == TileType.Gate) {
            CanBeNavigated = true;

            objectToSpawn = context.GateTilePrefab;
        }
        if (Application.isPlaying == false) thisGameObject = UnityEditor.PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
        else thisGameObject = MonoBehaviour.Instantiate(objectToSpawn);
        thisGameObject.name = "X: " + w + " H: " + h;
        thisGameObject.transform.position = position;
        thisGameObject.transform.parent = context.transform;
        thisGameObject.transform.localScale = size;

        this.tileType = newType;

        context.Tiles.Add(this);
        if (tileType == TileType.Gate) context.Gates.Add(this);
        TileWrapper wrapper = thisGameObject.GetComponent<TileWrapper>();
        if (wrapper != null) wrapper.SetTile(this);
        else thisGameObject.AddComponent<TileWrapper>().SetTile(this);
    }

    public void SetTileType(TileType newType) {
        if (newType == tileType) return;

        if (tileType == TileType.Gate && context.Gates.Contains(this)) context.Gates.Remove(this);
        if (Application.isPlaying == true && thisGameObject != null) MonoBehaviour.Destroy(thisGameObject);
        else if (thisGameObject != null) MonoBehaviour.DestroyImmediate(thisGameObject);
        GameObject objectToSpawn = null;

        if (newType == TileType.Navigatable) {
            CanBeNavigated = true;
            objectToSpawn = context.NavigatableTilePrefab;

        }
        else if (newType == TileType.NonNavigatable) {
            CanBeNavigated = false;
            objectToSpawn = context.NonNavigatableTilePrefab;
        }
        else if (newType == TileType.Gate) {
            CanBeNavigated = true;
            if (tileType == TileType.Gate && context.Gates.Contains(this) == false) context.Gates.Add(this);
            objectToSpawn = context.GateTilePrefab;
        }
        if (Application.isPlaying == false) thisGameObject = UnityEditor.PrefabUtility.InstantiatePrefab(objectToSpawn) as GameObject;
        else thisGameObject = MonoBehaviour.Instantiate(objectToSpawn);
        thisGameObject.name = "X: " + w + " H: " + h;
        thisGameObject.transform.position = GetPosition;
        thisGameObject.transform.parent = context.transform;
        thisGameObject.transform.localScale = GetSize;

        this.tileType = newType;

        context.Tiles.Add(this);
        TileWrapper wrapper = thisGameObject.GetComponent<TileWrapper>();
        if (wrapper != null) wrapper.SetTile(this);
        else thisGameObject.AddComponent<TileWrapper>().SetTile(this);
    }
}
