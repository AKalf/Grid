using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridTile : Pathfinding.INode<GridTile> {

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
    public Pathfinding.INode<GridTile> CameFrom { get; set; }
    public GameObject gameObject { get => thisGameObject; set => thisGameObject = value; }
    public GridTile GetNewNode(params object[] parameters) {
        return new GridTile((Vector3Int)parameters[0], (Vector3Int)parameters[1], (Sprite)parameters[2], (TileType)parameters[4], (GameObject)parameters[5]);
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
