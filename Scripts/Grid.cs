using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TTile> {
    [SerializeField] public List<Pathfinding.IGridTile<TTile>> NodeCubes = new List<Pathfinding.IGridTile<TTile>>();

    [SerializeField, HideInInspector] private List<GameObject> gridObjects = new List<GameObject>();

    [SerializeField] private Transform tilesParent = null;

    [SerializeField] private Sprite backgroundSprite, borderSprite;

    [SerializeField] private int width = 0, height = 0, tileLength = 0;

    private Pathfinding.IGridTile<TTile>[,] nodesOnGrid = null;
    public Pathfinding.IGridTile<TTile>[,] NodesOnGrid { get => nodesOnGrid; private set => nodesOnGrid = value; }

    public int Width => width;
    public int Height => height;
    public int Length => tileLength;

    public Grid(int width, int height, int tileLength, Transform tilesParent = null, Sprite backgroundSprite = null, Sprite borderSprite = null) {
        this.width = width; this.height = height; this.tileLength = tileLength;
        this.tilesParent = tilesParent;
        this.backgroundSprite = backgroundSprite; this.borderSprite = borderSprite;
    }


    public void ClearNodes() {

        NodeCubes.Clear();
        NodesOnGrid = null;
    }
    public void CreateGrid() {

        ClearNodes();
        foreach (var node in gridObjects)
            MonoBehaviour.DestroyImmediate(node);
        gridObjects.Clear();
        NodesOnGrid = new Pathfinding.IGridTile<TTile>[width, height];
    }
    public void CreateTileForGrid(int w, int h, Func<Pathfinding.IGridTile<TTile>> tileConstructor, Vector3 startingPosition, Vector3 tileSize) {
        if (NodesOnGrid == null)
            return;
        Pathfinding.IGridTile<TTile> newGizmo = tileConstructor.Invoke();
        newGizmo.CanBeNavigated = true;
        NodeCubes.Add(newGizmo);
        NodesOnGrid[w, h] = newGizmo;
        GameObject newGamboject = new GameObject();
        newGamboject.name = "X: " + w + " H: " + h;
        newGamboject.transform.position = startingPosition + new Vector3(w * tileSize.x, h * tileSize.y, 0);
        newGamboject.transform.parent = tilesParent;
        gridObjects.Add(newGamboject);
        SpriteRenderer renderer = newGamboject.AddComponent<SpriteRenderer>();
        if (w == 0 || h == 0 || w == NodesOnGrid.GetLength(0) - 1 || h == NodesOnGrid.GetLength(1) - 1 || (w == 5 && h != 2)) {
            newGizmo.CanBeNavigated = false;
            renderer.sprite = backgroundSprite;
            renderer.color = Color.black;
            newGamboject.AddComponent<PolygonCollider2D>();
        }
        else
            renderer.sprite = backgroundSprite;

    }
    public Pathfinding.IGridTile<TTile>[,] GetGridTiles() {
        if (NodeCubes == null || NodeCubes.Count == 0)
            return null;
        if (NodesOnGrid == null && NodeCubes != null && NodeCubes.Count > 0) {
            foreach (Pathfinding.IGridTile<TTile> cube in NodeCubes)
                NodesOnGrid[cube.W, cube.H] = cube;
        }
        return NodesOnGrid;
    }

    // Editor Functions
    #region Editor Functions
#if UNITY_EDITOR
    /// <summary> SHOULD ONLY BE CALLED FROM INSIDE UNITY_EDITOR DIRECTIVE</summary>
    public void SetWidthFromEditor(int width) {
        this.width = width;
    }
    /// <summary> SHOULD ONLY BE CALLED FROM INSIDE UNITY_EDITOR DIRECTIVE</summary>
    public void SetHeightFromEditor(int height) {
        this.height = height;
    }
    /// <summary> SHOULD ONLY BE CALLED FROM INSIDE UNITY_EDITOR DIRECTIVE</summary>
    public void SetLengthFromEditor(int length) {
        this.tileLength = length;
    }
#endif
    #endregion

}
