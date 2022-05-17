using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridTile : Pathfinding.IGridTile<GridTile> {

    public enum TileType { Entrance, Exit, Boarder, Normal }

    public Vector3Int position;
    public Vector3Int size;
    public Sprite spirte;
    public GameObject thisGameObject, objectOnTile;
    public TileType tileType;

    public int W { get => position.x; set => position.x = value; }
    public int H { get => position.y; set => position.y = value; }

    public Vector3 GetPosition => position;

    public int WalkingCost { get; set; }
    public int HeuristicCost { get; set; }
    public int TotalCost => WalkingCost + HeuristicCost;
    public bool CanBeNavigated { get; set; }
    public Pathfinding.IGridTile<GridTile> CameFrom { get; set; }
    public GameObject gameObject { get => thisGameObject; set => thisGameObject = value; }
    public GridTile GetNewNode(Vector3 position, Vector3 size) {
        return new GridTile(
            position: new Vector3Int((int)position.x, (int)position.y, (int)position.z),
            size: new Vector3Int((int)size.x, (int)size.y, (int)size.z),
            sprite: null,
            type: TileType.Normal,
            representation: null);
    }
    public GridTile(Vector3Int position, Vector3Int size, Sprite sprite, TileType type, GameObject representation) {
        this.position = position;
        this.size = size;
        this.spirte = sprite;
        this.tileType = type;
        this.thisGameObject = representation;
        this.objectOnTile = null;

        WalkingCost = 0;
        HeuristicCost = 0;
        CanBeNavigated = true;
        CameFrom = null;
    }
}
