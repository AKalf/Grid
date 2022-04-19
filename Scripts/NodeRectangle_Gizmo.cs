using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;


public class NodeRectangle_Gizmo : Pathfinding.INode<NodeRectangle_Gizmo> {
    // Variables       
    #region Variables
    private int widthIndex = 0, heightIndex = 0;
    public Vector3 Position = Vector3.zero, Size = Vector3.zero, LabelPosition = Vector3.zero;
    public Color Original = Color.white, CubeColor = Color.white, LabelColor = Color.black;
    public string Label = "";
    [NonSerialized]
    private EditorCoroutine nodeCoroutine = null, lineCoroutine = null;

    public enum LineDirection { Upper, Right, }
    #endregion

    // INode Implementation
    #region INode implementation
    public int W { get => widthIndex; set => widthIndex = value; }
    public int H { get => heightIndex; set => heightIndex = value; }
    public Vector3 GetPosition => Position;
    public int WalkingCost { get; set; }
    public int HeuristicCost { get; set; }
    public int TotalCost => WalkingCost + HeuristicCost;
    public NodeRectangle_Gizmo CameFrom { get; set; }


    //public GameObject gameObject => Grid_Editor.BakedGrid[w, h];
    #endregion
    public NodeRectangle_Gizmo(int w, int h, Vector3 cubePos, Vector3 cubeSize, Vector3 labelPos, string label, Color cubeColor = default, Color labelColor = default) {
        this.widthIndex = w;
        this.heightIndex = h;

        this.WalkingCost = int.MaxValue;
        this.HeuristicCost = 0;
        this.CameFrom = null;

        this.Position = cubePos;
        this.Size = cubeSize;
        this.LabelPosition = labelPos;
        this.Label = label;
        this.Original = cubeColor;
        this.CubeColor = cubeColor;
        this.LabelColor = labelColor;

        nodeCoroutine = null;
        lineCoroutine = null;
    }
    public void Draw(int controlID, Vector3 pos, Quaternion rot, float size, EventType type) {
        Color prevColor = Handles.color;
        Handles.color = this.CubeColor;
        if (Tools.current == Tool.View)
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Repaint);
        else {
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Repaint);
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Layout);
        }

        Handles.color = this.LabelColor;
        if (Camera.current != null && string.IsNullOrEmpty(Label) == false) {
            GUIStyle style = new GUIStyle(GUIStyle.none);
            int fontSize = (int)(30 - Mathf.Abs(Camera.current.transform.position.z - Position.z)); // a magic number who has the desired effect at 1920x1080 resolution
            if (fontSize > 6) { // magic number
                if (fontSize > 16) // magic condition
                    fontSize = 16;
                style.fontSize = fontSize;
                Handles.Label(LabelPosition, Label, style);
            }
        }
        Handles.color = prevColor;
    }
    public void ChangeNodeColor(Color color, float duration) {
        if (nodeCoroutine != null) {
            this.CubeColor = Original;
            EditorCoroutineUtility.StopCoroutine(nodeCoroutine);
        }
        nodeCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ChangeNodeColorCoroutine(color, duration));
    }
    public void ChangeLineColor(Color color, float duration, LineDirection direction) {
        // nodeCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ChangeLineColorCoroutine(color, duration, direction));
    }
    private IEnumerator ChangeNodeColorCoroutine(Color color, float duration) {
        float timer = 0.0f;
        this.CubeColor = color;
        while (timer < duration) {
            yield return new EditorWaitForSeconds(1);
            timer++;
        }
        if (nodeCoroutine != null) {
            this.CubeColor = Original;
            EditorCoroutineUtility.StopCoroutine(nodeCoroutine);
        }
        this.CubeColor = Original;
    }
    //private IEnumerator ChangeLineColorCoroutine(Color color, float duration, LineDirection direction) {
    //    float timer = 0.0f;
    //    if (direction == LineDirection.Upper)
    //        Grid_Editor.LinesColors[w, h, w, h + 1] = color;
    //    else if (direction == LineDirection.Right)
    //        Grid_Editor.LinesColors[w, h, w + 1, h] = color;
    //    while (timer < duration) {
    //        yield return new EditorWaitForSeconds(1);
    //        timer++;
    //    }
    //    if (lineCoroutine != null)
    //        EditorCoroutineUtility.StopCoroutine(lineCoroutine);
    //    if (direction == LineDirection.Upper)
    //        Grid_Editor.LinesColors[w, h, w, h + 1] = Color.white;
    //    else if (direction == LineDirection.Right)
    //        Grid_Editor.LinesColors[w, h, w + 1, h] = Color.white;
    //}
}
