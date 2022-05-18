using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridTile : Pathfinding.IGridTile {
    public static GridTile CreateNew(int w, int h, Transform tilesParent, Vector2Int GridSize, Vector2Int TileSize, GridSpace context, Sprite backgroundSprite, Sprite boardSprite) {
        bool isOnBoarder = w == 0 || h == 0 || w == GridSize.x - 1 || h == GridSize.y - 1;
        GridTile newTile = new GridTile(w, h,
            tilesParent.position + new Vector3(w * TileSize.x, h * TileSize.y, 0),
            (Vector3Int)TileSize,
            isOnBoarder ? boardSprite : backgroundSprite,
            isOnBoarder ? GridTile.TileType.Boarder : GridTile.TileType.Normal,
            tilesParent);
        context.Tiles.Add(newTile);
        newTile.thisGameObject.AddComponent<TileWrapper>().tile = newTile;
        SpriteRenderer renderer = newTile.thisGameObject.AddComponent<SpriteRenderer>();
        if (newTile.tileType == TileType.Boarder) {
            renderer.sortingOrder = 1;
            newTile.CanBeNavigated = false;
            renderer.color = Color.black;
            BoxCollider2D collider = newTile.thisGameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }
        else
            newTile.CanBeNavigated = true;
        renderer.sprite = newTile.Sprite;
        return newTile;
    }

    public enum TileType { Entrance, Exit, Boarder, Normal }
    private Vector3Int position;
    private Vector3Int size;
    public Sprite Sprite;
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
    public Pathfinding.IGridTile CameFrom { get; set; }
    public GameObject GameObject { get => thisGameObject; set => thisGameObject = value; }

    public GridTile(int w, int h, Vector3 position, Vector3Int tileSize, Sprite sprite, TileType type, Transform parent) {
        this.position = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        this.size = tileSize;
        this.Sprite = sprite;
        this.tileType = type;
        this.thisGameObject = new GameObject();
        this.thisGameObject.name = "X: " + position.x + " H: " + position.y;
        this.thisGameObject.transform.position = position;
        this.thisGameObject.transform.parent = parent;
        this.thisGameObject.transform.localScale = tileSize;

        this.objectOnTile = null;

        WalkingCost = 0;
        HeuristicCost = 0;
        CanBeNavigated = true;
        CameFrom = null;
    }
}
