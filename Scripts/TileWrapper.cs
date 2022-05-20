using UnityEngine;
public class TileWrapper : MonoBehaviour, Pathfinding.IGridTile {
    private GridTile tile;

    public int W { get => tile.W; set => tile.W = value; }
    public int H { get => tile.H; set => tile.H = value; }

    public Vector3 GetPosition => tile.GetPosition;
    public Vector3 GetSize => tile.GetSize;

    public int WalkingCost { get => tile.WalkingCost; set => tile.WalkingCost = value; }
    public int HeuristicCost { get => tile.HeuristicCost; set => tile.HeuristicCost = value; }

    public int TotalCost => tile.TotalCost;

    public bool CanBeNavigated { get => tile.CanBeNavigated; set => tile.CanBeNavigated = value; }
    public Pathfinding.IGridTile CameFrom { get => tile.CameFrom; set => tile.CameFrom = value; }
    public GameObject GameObject { get => tile.GameObject; set => tile.GameObject = value; }

    public TileType TileType { get => tile.TypeOfTile; set => tile.SetTileType(value); }

    public void SetTile(GridTile tile) {
        this.tile = tile;
    }

}
