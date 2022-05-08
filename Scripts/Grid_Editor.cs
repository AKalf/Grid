using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Grid))]
public class Grid_Editor : Editor {

    private static Transform myTrans = null;
    private static Grid grid = null;
    private NodeRectangle_Gizmo[,] nodes => grid.NodesOnGrid;

    private int Width => grid.Width;
    private int Height => grid.Height;
    private int Length => grid.Length;

    private Vector3 widthSlider = Vector3.zero, heightSlider = Vector3.zero;
    private NodeRectangle_Gizmo lastNode = null;
    private Pathfinding pathfinding = new Pathfinding();
    private SphereController debugSphere = null;
    private EditorCoroutine debugCoroutine = null;

    private void OnEnable() {
        if (debugSphere == null)
            debugSphere = GameObject.FindObjectOfType<SphereController>();
        if (grid == null)
            grid = (Grid)target;
        if (myTrans == null)
            myTrans = grid.transform;
    }
    private void BuildGrid() {
        if (Width > 0 && Height > 0 && Length > 0) {
            grid.ClearNodes();
            grid.CreateGrid();

            Vector3 origin = myTrans.position, rectanglePosition = Vector3.zero, rectangleSize = Vector3.one * Length, labelPosition = Vector3.zero;
            for (int w = 0; w < Width; w++) {
                for (int h = 0; h < Height; h++) {
                    rectanglePosition = origin + new Vector3(w, h, 0) * Length;
                    labelPosition = rectanglePosition + new Vector3(-(Length / 4 + 0.4f), rectangleSize.y / 10 + 0.2f, 0);
                    grid.AssignNodeToGrid(w, h, rectanglePosition, rectangleSize, labelPosition, "X: " + w + "\nY: " + h, Color.white);
                }
            }
            widthSlider = myTrans.position + Vector3.right * (Width * Length);
            heightSlider = myTrans.position + Vector3.up * (Height * Length);
        }
    }
    protected virtual void OnSceneGUI() {
        if (nodes == null && grid != null)
            grid.GetNodesOnGrid();
        if (nodes != null && nodes.Length > 0) {
            if (lastNode == null) lastNode = nodes[0, 0];  // Assing last node as the start if null
            int w = 0, h = 0;
            while (w < Width) {
                try {
                    NodeRectangle_Gizmo cube = nodes[w, h];
                    if (Handles.Button(cube.Position, Quaternion.identity, cube.Size.x / 2, cube.Size.x / 2, cube.Draw) && cube.CanBeNavigated) {
                        List<NodeRectangle_Gizmo> path = pathfinding.GetPath(lastNode, nodes[cube.W, cube.H], nodes);
                        if (path != null) {
                            lastNode = path[path.Count - 1];
                            if (debugCoroutine != null)
                                EditorCoroutineUtility.StopCoroutine(debugCoroutine);
                            debugCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(SendDebugShpere(debugSphere.transform, path, 0.2f));
                        }
                    }
                    h++;
                    if (h >= Height) { h = 0; w++; }
                }
                catch {
                    return;
                }

            }
            DrawSlider(ref widthSlider, Vector3.right, size: Length * 4, step: 1f);
            DrawSlider(ref heightSlider, Vector3.up, size: Length * 4, step: 1f);
        }
        HandleUtility.Repaint();
    }

    // Custom Inspector
    #region Custom Inspector
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        AssignGridValuesFromInspector();
        DrawInspectorButtons();
    }
    private void AssignGridValuesFromInspector() {
        // Width
        int value = EditorGUILayout.IntField(Width);
        if (value != Width)
            grid.SetWidthFromEditor(value);
        // Height
        value = EditorGUILayout.IntField(Height);
        if (value != Height)
            grid.SetHeightFromEditor(value);
        // Length
        value = EditorGUILayout.IntField(Length);
        if (value != Length)
            grid.SetLengthFromEditor(value);
    }
    private void DrawInspectorButtons() {
        if (GUILayout.Button("Build Grid"))
            BuildGrid();
        if (GUILayout.Button("Clear nodes"))
            grid.ClearNodes();
        if (GUILayout.Button("Return sphere"))
            debugSphere.transform.position = myTrans.position;
    }
    #endregion


    private void DrawSlider(ref Vector3 sliderValue, Vector3 direction, float size, float step) {

        Vector3 newValue = Handles.Slider(sliderValue, direction, size, Handles.ArrowHandleCap, step);
        if (direction.x > 0 && Mathf.Abs(newValue.x - sliderValue.x) > 0.9f && (int)sliderValue.x / Length > 1) {
            sliderValue = newValue;
            if ((int)widthSlider.x / Length > 0)
                grid.SetWidthFromEditor((int)widthSlider.x / Length);
            BuildGrid();
        }
        else if (Mathf.Abs(newValue.y - sliderValue.y) > 0.9f && (int)sliderValue.y / Length > 1) {
            sliderValue = newValue;
            if ((int)heightSlider.y / Length > 0)
                grid.SetHeightFromEditor((int)heightSlider.y / Length);
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
