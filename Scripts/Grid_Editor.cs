using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using IGridTile = Pathfinding.IGridTile;
[CustomEditor(typeof(GridSpace))]
public class Grid_Editor : Editor {

    private Grid gizmoGrid = null;
    private Grid gameGrid = null;

    private GridSpace context = null;

    private IGridTile lastNode = null;

    /* Grid Position and Size variables */
    #region Properties
    private Vector2Int GridSize => context.GridSize;
    private Vector2Int TileSize => context.GridTileSize;
    private Vector3 widthSlider => tilesParent.position + Vector3.right * GridWidth * TileWidth;
    private Vector3 heightSlider => tilesParent.position + Vector3.up * GridHeight * TileHeight;
    private int GridWidth => GridSize.x;
    private int GridHeight => GridSize.y;
    private int TileWidth => TileSize.x;
    private int TileHeight => TileSize.y;
    #endregion
    private IGridTile[,] gizmoTiles => gizmoGrid.NodesOnGrid;
    private Transform tilesParent => context.transform;

    #region Sphere Debugging variables
    private SphereController debugSphere = null;
    private EditorCoroutine debugCoroutine = null;
    #endregion

    private Sprite backgroundSprite { get => context.BackgroundSprite; set => context.BackgroundSprite = value; }
    private Sprite boarderSprite { get => context.BoarderSprite; set => context.BoarderSprite = value; }



    private void OnEnable() {
        context = target as GridSpace;

        if (debugSphere == null) debugSphere = GameObject.FindObjectOfType<SphereController>();

        // Initialise Grids
        if (gizmoGrid == null || gameGrid == null) {
            gizmoGrid = new Grid(GridSize, TileSize, tilesParent);
            gameGrid = new Grid(GridSize, TileSize, tilesParent, backgroundSprite, boarderSprite);
            gizmoGrid.ClearNodes();
            gizmoGrid.CreateGrid(GridSize, TileSize, (w, h) => NodeRectangle_Gizmo.CreateNew(w, h, tilesParent, GridSize, TileSize));
        }


    }
    private void BuildGrid() {
        if (GridSize.x > 0 && GridSize.y > 0 && TileSize.x > 0 && TileSize.y > 0) {
            gizmoGrid.ClearNodes();
            gizmoGrid.CreateGrid(GridSize, TileSize, (w, h) => NodeRectangle_Gizmo.CreateNew(w, h, tilesParent, GridSize, TileSize));

            gameGrid.ClearNodes();
            foreach (GridTile tile in context.Tiles) {
                if (tile.thisGameObject != null) DestroyImmediate(tile.thisGameObject);
            }
            context.Tiles.Clear();
            gameGrid.CreateGrid(GridSize, TileSize, (w, h) => GridTile.CreateNew(w, h, tilesParent, GridSize, TileSize, context, backgroundSprite, boarderSprite));

        }
    }

    protected virtual void OnSceneGUI() {
        if (gizmoTiles != null && gizmoTiles.Length > 0) {
            if (lastNode == null) lastNode = gizmoTiles[0, 0];  // Assing last node as the start if null
            int w = 0, h = 0;
            while (w < GridWidth) {
                try {
                    NodeRectangle_Gizmo cube = gizmoTiles[w, h] as NodeRectangle_Gizmo;
                    if (Handles.Button(cube.GetPosition, Quaternion.identity, cube.Size.x / 2, cube.Size.x / 2, cube.Draw) && cube.CanBeNavigated) {
                        List<IGridTile> path = Pathfinding.GetPath(lastNode, gizmoTiles[cube.W, cube.H], gizmoTiles);
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
            DrawSlider(widthSlider, Vector3.right, size: TileWidth * 4, step: 1f);
            DrawSlider(heightSlider, Vector3.up, size: TileHeight * 4, step: 1f);
        }
        HandleUtility.Repaint();
    }

    // Custom Inspector
    #region Custom Inspector
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        DrawInspectorButtons();
    }

    private void DrawInspectorButtons() {
        if (GUILayout.Button("Build Grid")) BuildGrid();
        if (GUILayout.Button("Return sphere")) debugSphere.transform.position = tilesParent.position;
    }
    #endregion


    private void DrawSlider(Vector3 sliderValue, Vector3 direction, float size, float step) {
        Vector3 newValue = Handles.Slider(sliderValue, direction, size, Handles.ArrowHandleCap, step);
        if (direction.x > 0 && Mathf.Abs(newValue.x - sliderValue.x) > 0.9f && (int)newValue.x / TileWidth > 1) {
            context.GridSize = new Vector2Int((int)newValue.x / TileWidth, (int)heightSlider.y / TileHeight);
            serializedObject.ApplyModifiedProperties();
            BuildGrid();
        }
        else if (Mathf.Abs(newValue.y - sliderValue.y) > 0.9f && (int)newValue.y / TileHeight > 1) {
            context.GridSize = new Vector2Int((int)widthSlider.x / TileWidth, (int)newValue.y / TileHeight);
            serializedObject.ApplyModifiedProperties();
            BuildGrid();
        }

    }
    private IEnumerator SendDebugShpere(Transform sphere, List<IGridTile> targets, float speed) {
        while (targets.Count > 0) {
            NodeRectangle_Gizmo target = targets[0] as NodeRectangle_Gizmo;
            target.ChangeNodeColor(Color.red, 0.01f * speed);
            while (Vector3.Distance(sphere.position, target.Position) > 0.25f) {
                sphere.position = Vector3.MoveTowards(sphere.position, target.Position, speed);
                yield return new EditorWaitForSeconds(0.01f);
            }
            targets.Remove(target);
        }
    }
}
