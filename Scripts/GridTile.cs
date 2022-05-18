using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridTile : Pathfinding<GridTile>.IGridTile {

    public enum TileType { Entrance, Exit, Boarder, Normal }

    public Vector3Int position;
    public Vector3Int size;
    public Sprite spirte;
    public GameObject thisGameObject, objectOnTile;
    public TileType tileType;

    public int W { get => position.x; set => position.x = value; }
    public int H { get => position.y; set => position.y = value; }

    public Vector3 GetPosition => position;
    public Vector3 GetSize => size;

    public int WalkingCost { get; set; }
    public int HeuristicCost { get; set; }
    public int TotalCost => WalkingCost + HeuristicCost;
    public bool CanBeNavigated { get; set; }
    public Pathfinding<GridTile>.IGridTile CameFrom { get; set; }
    public GameObject gameObject { get => thisGameObject; set => thisGameObject = value; }


    public GridTile(int w, int h, Vector3 position, Vector3Int tileSize, Sprite sprite, TileType type, Transform parent) {
        this.position = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        this.size = tileSize;
        this.spirte = sprite;
        this.tileType = type;
        this.thisGameObject = new GameObject();
        this.thisGameObject.name = "X: " + position.x + " H: " + position.y;
        this.thisGameObject.transform.position = position;
        this.thisGameObject.transform.parent = parent;
        SpriteRenderer renderer = this.thisGameObject.AddComponent<SpriteRenderer>();
        this.objectOnTile = null;
        if (type == TileType.Boarder) {
            CanBeNavigated = false;
            renderer.color = Color.black;
            this.thisGameObject.AddComponent<PolygonCollider2D>();
        }
        else
            CanBeNavigated = true;
        renderer.sprite = sprite;
        WalkingCost = 0;
        HeuristicCost = 0;
        CanBeNavigated = true;
        CameFrom = null;
    }
}
