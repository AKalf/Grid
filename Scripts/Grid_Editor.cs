using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using GizmoTile = Pathfinding<NodeRectangle_Gizmo>.IGridTile;
using GameTile = Pathfinding<GridTile>.IGridTile;

[CustomEditor(typeof(GridSpace))]
public class Grid_Editor : Editor {

    private static Grid<NodeRectangle_Gizmo> grid = null;
    private NodeRectangle_Gizmo[,] gizmoTiles => grid.NodesOnGrid;

    private Vector2Int GridSize => context.GridSize;
    private Vector2Int TileSize => context.GridTileSize;
    private Vector3 widthSlider = Vector3.zero, heightSlider = Vector3.zero;
    private int GridWidth => GridSize.x;
    private int GridHeight => GridSize.y;
    private int TileWidth => TileSize.x;
    private int TileHeight => TileSize.y;


    private Transform tilesParent = null;
    private NodeRectangle_Gizmo lastNode = null;
    private SphereController debugSphere = null;
    private EditorCoroutine debugCoroutine = null;
    private GridSpace context = null;
    private Sprite backgroundSprite = null, boarderSprite = null;
    private void OnEnable() {
        if (context == null)
            context = target as GridSpace;
        if (debugSphere == null)
            debugSphere = GameObject.FindObjectOfType<SphereController>();
        if (grid == null || context.Grid == null) {
            grid = new Grid<NodeRectangle_Gizmo>(GridSize, TileSize, tilesParent);
            context.Grid = new Grid<GridTile>(GridSize, TileSize, tilesParent, backgroundSprite, boarderSprite);
        }

    }
    private void BuildGrid() {
        if (GridSize.x > 0 && GridSize.y > 0 && TileSize.x > 0 && TileSize.y > 0) {
            grid.ClearNodes();
            Func<int, int, NodeRectangle_Gizmo> gizmoContructor = (w, h) => {
                return new NodeRectangle_Gizmo(w, h,
                    tilesParent.position + new Vector3(w * TileSize.x, h * TileSize.y, 0),
                    TileSize,
                    "X: " + w + "\nY: " + h,
                    Color.white, Color.black);
            };
            grid.CreateGrid(GridSize, TileSize, gizmoContructor);

            context.Grid.ClearNodes();
            Func<int, int, GridTile> gameTileConstructor = (w, h) => {
                bool isOnBoarder = w == 0 || h == 0 || w == GridWidth - 1 || h == GridHeight - 1;
                return new GridTile(w, h,
                    tilesParent.position + new Vector3(w * TileSize.x, h * TileSize.y, 0),
                    (Vector3Int)TileSize,
                    isOnBoarder ? boarderSprite : backgroundSprite,
                    isOnBoarder ? GridTile.TileType.Boarder : GridTile.TileType.Normal,
                    tilesParent);
            };
            context.Grid.CreateGrid(GridSize, TileSize, gameTileConstructor);
            widthSlider = tilesParent.position + Vector3.right * (GridWidth * TileWidth);
            heightSlider = tilesParent.position + Vector3.up * (GridHeight * TileHeight);
        }
    }
    protected virtual void OnSceneGUI() {
        if (gizmoTiles == null && grid != null)
            grid.GetGridTiles();
        if (gizmoTiles != null && gizmoTiles.Length > 0) {
            if (lastNode == null) lastNode = gizmoTiles[0, 0];  // Assing last node as the start if null
            int w = 0, h = 0;
            while (w < GridWidth) {
                try {
                    NodeRectangle_Gizmo cube = gizmoTiles[w, h] as NodeRectangle_Gizmo;
                    if (Handles.Button(cube.GetPosition, Quaternion.identity, cube.Size.x / 2, cube.Size.x / 2, cube.Draw) && cube.CanBeNavigated) {
                        List<NodeRectangle_Gizmo> path = Pathfinding<NodeRectangle_Gizmo>.GetPath(lastNode, gizmoTiles[cube.W, cube.H], gizmoTiles);
                        if (path != null) {
                            lastNode = path[path.Count - 1];
                            if (debugCoroutine != null)
                                EditorCoroutineUtility.StopCoroutine(debugCoroutine);
                            debugCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(SendDebugShpere(debugSphere.transform, path, 0.2f));
                        }
                    }
                    h++;
                    if (h >= GridHeight) { h = 0; w++; }
                }
                catch {
                    return;
                }

            }
            DrawSlider(ref widthSlider, Vector3.right, size: TileWidth * 4, step: 1f);
            DrawSlider(ref heightSlider, Vector3.up, size: TileHeight * 4, step: 1f);
        }
        HandleUtility.Repaint();
    }

    // Custom Inspector
    #region Custom Inspector
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        tilesParent = EditorGUILayout.ObjectField(tilesParent, typeof(Transform), true) as Transform;
        boarderSprite = EditorGUILayout.ObjectField(boarderSprite, typeof(Sprite), true) as Sprite;
        backgroundSprite = EditorGUILayout.ObjectField(backgroundSprite, typeof(Sprite), true) as Sprite;
        DrawInspectorButtons();
    }

    private void DrawInspectorButtons() {
        if (GUILayout.Button("Build Grid")) BuildGrid();
        if (GUILayout.Button("Clear nodes")) grid.ClearNodes();
        if (GUILayout.Button("Return sphere")) debugSphere.transform.position = tilesParent.position;
    }
    #endregion


    private void DrawSlider(ref Vector3 sliderValue, Vector3 direction, float size, float step) {

        Vector3 newValue = Handles.Slider(sliderValue, direction, size, Handles.ArrowHandleCap, step);
        if (direction.x > 0 && Mathf.Abs(newValue.x - sliderValue.x) > 0.9f && (int)sliderValue.x / TileWidth > 1
           || Mathf.Abs(newValue.y - sliderValue.y) > 0.9f && (int)sliderValue.y / TileHeight > 1) {
            sliderValue = newValue;
            if ((int)widthSlider.x / TileWidth > 0 || (int)heightSlider.y / TileHeight > 0)
                context.GridSize = new Vector2Int((int)widthSlider.x / TileWidth, (int)heightSlider.y / TileHeight);
            BuildGrid();
        }
    }
    private IEnumerator SendDebugShpere(Transform sphere, List<NodeRectangle_Gizmo> targets, float speed) {
        while (targets.Count > 0) {
            NodeRectangle_Gizmo target = targets[0];
            target.ChangeNodeColor(Color.red, 0.01f * speed);
            while (Vector3.Distance(sphere.position, target.Position) > 0.25f) {
                sphere.position = Vector3.MoveTowards(sphere.position, target.Position, speed);
                yield return new EditorWaitForSeconds(0.01f);
            }
            targets.Remove(target);
        }
    }
}
