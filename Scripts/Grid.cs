using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TTile> where TTile : Pathfinding<TTile>.IGridTile {

    public Vector2Int GridSize { get; private set; }
    public Vector2Int TileSize { get; private set; }
    public int GridWidth => GridSize.x;
    public int GridHeight => GridSize.y;
    public int TileWidth => TileSize.x;
    public int TileHeight => TileSize.y;

    public List<TTile> Tiles = new List<TTile>();



    private TTile[,] nodesOnGrid = null;
    public TTile[,] NodesOnGrid { get => nodesOnGrid; private set => nodesOnGrid = value; }

    public Grid(int gridWidth, int gridHeight, int tileWidth, int tileHeight) {
        this.GridSize = new Vector2Int(gridWidth, gridHeight); this.TileSize = new Vector2Int(tileWidth, tileHeight);
    }

    public Grid(Vector2Int gridSize, Vector2Int tileSize, Transform tilesParent = null, Sprite backgroundSprite = null, Sprite borderSprite = null) {
        this.GridSize = gridSize; this.TileSize = tileSize;
    }


    public void ClearNodes() {
        Tiles.Clear();
        NodesOnGrid = null;
    }
    public void CreateGrid(Vector2Int gridSize, Vector2Int tileSize, Func<int, int, TTile> tileConstructor) {
        ClearNodes();
        GridSize = gridSize;
        TileSize = tileSize;
        NodesOnGrid = new TTile[GridWidth, GridHeight];
        for (int w = 0; w < GridWidth; w++) {
            for (int h = 0; h < GridHeight; h++) {
                TTile newTile = tileConstructor.Invoke(w, h);
                Tiles.Add(newTile);
                NodesOnGrid[w, h] = newTile;
            }
        }
    }

    public TTile[,] GetGridTiles() {
        if (Tiles == null || Tiles.Count == 0)
            return null;
        if (NodesOnGrid == null && Tiles != null && Tiles.Count > 0) {
            foreach (TTile cube in Tiles)
                NodesOnGrid[cube.W, cube.H] = cube;
        }
        return NodesOnGrid;
    }



}
