using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;


public class NodeRectangle_Gizmo<TTile> : Pathfinding.IGridTile where TTile : Pathfinding.IGridTile {

    public static NodeRectangle_Gizmo<TTile> CreateNew(TTile targetTile, Vector2Int GridSize) {
        return new NodeRectangle_Gizmo<TTile>(
            targetTile,
            (targetTile.W == 0 || targetTile.W == GridSize.x - 1
            ||
            targetTile.H == 0 || targetTile.H == GridSize.y - 1) ?
            TileType.NonNavigatable : TileType.Navigatable,
            "X: " + targetTile.W + "\nY: " + targetTile.H,
            Color.white, Color.black);
    }

    // Variables       
    #region Variables

    public Vector3 LabelPosition = Vector3.zero;
    public Color Original = Color.white, TileGizmoColor = Color.white, LabelColor = Color.black;
    public string Label = "";
    private EditorCoroutine nodeCoroutine = null, lineCoroutine = null;
    TTile targetTile;
    public enum LineDirection { Upper, Right, }
    #endregion

    // INode Implementation
    #region INode implementation
    public int W { get => targetTile.W; set => targetTile.W = value; }
    public int H { get => targetTile.H; set => targetTile.H = value; }
    public Vector3 GetPosition => targetTile.GetPosition;
    public Vector3 GetSize => targetTile.GetSize;
    public int WalkingCost { get => targetTile.WalkingCost; set => targetTile.WalkingCost = value; }
    public int HeuristicCost { get => targetTile.HeuristicCost; set => targetTile.HeuristicCost = value; }
    public int TotalCost => targetTile.TotalCost;
    public Pathfinding.IGridTile CameFrom { get => targetTile.CameFrom; set => targetTile.CameFrom = value; }
    public bool CanBeNavigated { get => targetTile.CanBeNavigated; set => targetTile.CanBeNavigated = value; }
    public GameObject GameObject { get => targetTile.GameObject; set => targetTile.GameObject = value; }



    #endregion
    private NodeRectangle_Gizmo(TTile targetTile, TileType tileType, string label, Color tileGizmoColor = default, Color labelColor = default) {

        this.targetTile = targetTile;

        this.WalkingCost = int.MaxValue;
        this.HeuristicCost = 0;
        this.CameFrom = null;

        this.LabelPosition = this.GetPosition + new Vector3(-(this.GetSize.x / 10 + 0.2f), this.GetSize.y / 10 + 0.2f, 0);
        this.Label = label;

        this.Original = tileGizmoColor;
        this.TileGizmoColor = tileGizmoColor;

        if (tileType == TileType.Navigatable) this.LabelColor = labelColor;
        else this.LabelColor = Color.white;

        nodeCoroutine = null;
        lineCoroutine = null;
    }

    #region Drawing Functions
    public void Draw(int controlID, Vector3 pos, Quaternion rot, float size, EventType type) {
        Color prevColor = Handles.color;
        Handles.color = this.TileGizmoColor;
        if (Tools.current == Tool.View)
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Repaint);
        else {
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Repaint);
            Handles.RectangleHandleCap(controlID, pos, rot, size, EventType.Layout);
        }

        if (Camera.current != null && string.IsNullOrEmpty(Label) == false) {
            GUIStyle style = new GUIStyle(GUIStyle.none);
            style.normal.textColor = this.LabelColor;
            int fontSize = (int)(30 - Mathf.Abs(Camera.current.transform.position.z - GetPosition.z)); // a magic number who has the desired effect at 1920x1080 resolution
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
            this.TileGizmoColor = Original;
            EditorCoroutineUtility.StopCoroutine(nodeCoroutine);
        }
        nodeCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ChangeNodeColorCoroutine(color, duration));
    }
    public void ChangeLineColor(Color color, float duration, LineDirection direction) {
        // nodeCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(ChangeLineColorCoroutine(color, duration, direction));
    }
    private IEnumerator ChangeNodeColorCoroutine(Color color, float duration) {
        float timer = 0.0f;
        this.TileGizmoColor = color;
        while (timer < duration) {
            yield return new EditorWaitForSeconds(1);
            timer++;
        }
        if (nodeCoroutine != null) {
            this.TileGizmoColor = Original;
            EditorCoroutineUtility.StopCoroutine(nodeCoroutine);
        }
        this.TileGizmoColor = Original;
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
    #endregion
}
