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

    private GameObject normalTilePrefab { get => context.NavigatableTilePrefab; set => context.NavigatableTilePrefab = value; }
    private GameObject boarderTilePrefab { get => context.NonNavigatableTilePrefab; set => context.NonNavigatableTilePrefab = value; }



    private void OnEnable() {
        context = target as GridSpace;

        if (debugSphere == null) debugSphere = GameObject.FindObjectOfType<SphereController>();
        if (context.Tiles.Count == 0) return;
        // Initialise Grids
        if (gizmoGrid == null || gameGrid == null) {
            gizmoGrid = new Grid(GridSize, TileSize);
            int index = 0;
            gizmoGrid.BuildGrid(GridSize, TileSize, (w, h) => {
                NodeRectangle_Gizmo<GridTile> newGizmo = NodeRectangle_Gizmo<GridTile>.CreateNew(context.Tiles[index], GridSize);
                index++;
                return newGizmo;
            });
        }


    }
    private void BuildGrid() {
        if (GridSize.x > 0 && GridSize.y > 0 && TileSize.x > 0 && TileSize.y > 0) {
            if (gameGrid == null) gameGrid = new Grid(GridSize, TileSize);
            foreach (GridTile tile in context.Tiles) {
                if (tile.thisGameObject != null) DestroyImmediate(tile.thisGameObject);
            }
            context.Tiles.Clear();

            gameGrid.BuildGrid(GridSize, TileSize, (w, h) => {
                bool isOnBoarder = w == 0 || h == 0 || w == GridSize.x - 1 || h == GridSize.y - 1;
                return new GridTile(w, h, context, isOnBoarder ? TileType.NonNavigatable : TileType.Navigatable, tilesParent.position + new Vector3(w * TileSize.x, h * TileSize.y, 0), (Vector3Int)TileSize);
            });
            //int index = 0;
            //gizmoGrid.BuildGrid(GridSize, TileSize, (w, h) => {
            //    NodeRectangle_Gizmo<GridTile> newGizmo = NodeRectangle_Gizmo<GridTile>.CreateNew(context.Tiles[index], GridSize);
            //    index++;
            //    return newGizmo;
            //});

        }
    }

    protected virtual void OnSceneGUI() {
        if (context.Tiles.Count == 0) return;
        if (gameGrid != null && gizmoTiles != null && gizmoTiles.Length > 0) {
            if (lastNode == null) lastNode = gizmoTiles[0, 0];  // Assing last node as the start if null
            int w = 0, h = 0;
            while (w < GridWidth) {
                try {
                    NodeRectangle_Gizmo<GridTile> gizmoTile = gizmoTiles[w, h] as NodeRectangle_Gizmo<GridTile>;
                    if (Handles.Button(gizmoTile.GetPosition, Quaternion.identity, gizmoTile.GetSize.x / 2, gizmoTile.GetSize.y / 2, gizmoTile.Draw) && gizmoTile.CanBeNavigated) {
                        List<IGridTile> path = Pathfinding.GetPath(lastNode, gizmoTile, gizmoTiles);
                        if (path != null) {
                            lastNode = path[path.Count - 1];
                            if (debugCoroutine != null) EditorCoroutineUtility.StopCoroutine(debugCoroutine);
                            debugCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(SendDebugShpere(debugSphere.transform, path, 0.2f));
                        }
                    }
                    h++;
                    if (h >= GridHeight) { h = 0; w++; }
                }
                catch (Exception ex) {
                    Debug.LogError(ex);
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
            NodeRectangle_Gizmo<GridTile> target = targets[0] as NodeRectangle_Gizmo<GridTile>;
            target.ChangeNodeColor(Color.red, 0.01f * speed);
            while (Vector3.Distance(sphere.position, target.GetPosition) > 0.25f) {
                sphere.position = Vector3.MoveTowards(sphere.position, target.GetPosition, speed);
                yield return new EditorWaitForSeconds(0.01f);
            }
            targets.Remove(target);
        }
    }
}
