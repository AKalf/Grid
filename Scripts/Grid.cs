using System;
using System.Collections.Generic;
using UnityEngine;
using IGridTile = Pathfinding.IGridTile;
public class Grid {

    public IGridTile[,] NodesOnGrid { get => nodesOnGrid; private set => nodesOnGrid = value; }
    private IGridTile[,] nodesOnGrid = null;

    public Vector2Int GridSize { get; private set; }
    public Vector2Int TileSize { get; private set; }
    public int GridWidth => GridSize.x;
    public int GridHeight => GridSize.y;
    public int TileWidth => TileSize.x;
    public int TileHeight => TileSize.y;

    public Grid(int gridWidth, int gridHeight, int tileWidth, int tileHeight) {
        this.GridSize = new Vector2Int(gridWidth, gridHeight); this.TileSize = new Vector2Int(tileWidth, tileHeight);
    }
    public Grid(Vector2Int gridSize, Vector2Int tileSize) {
        this.GridSize = gridSize; this.TileSize = tileSize;
    }

    public IGridTile[,] BuildGrid(Vector2Int gridSize, Vector2Int tileSize, Func<int, int, IGridTile> tileConstructor) {
        NodesOnGrid = null;
        GridSize = gridSize;
        TileSize = tileSize;
        NodesOnGrid = new IGridTile[GridWidth, GridHeight];
        for (int w = 0; w < GridWidth; w++) {
            for (int h = 0; h < GridHeight; h++) {
                IGridTile newTile = tileConstructor.Invoke(w, h);
                NodesOnGrid[w, h] = newTile;
            }
        }
        return NodesOnGrid;
    }




}
