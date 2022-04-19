using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    [SerializeField]
    public List<NodeRectangle_Gizmo> NodeCubes = new List<NodeRectangle_Gizmo>();

    [SerializeField, HideInInspector]
    private int width = 0, height = 0, length = 0;

    private NodeRectangle_Gizmo[,] nodesOnGrid = null;
    public NodeRectangle_Gizmo[,] NodesOnGrid { get => nodesOnGrid; private set => nodesOnGrid = value; }

    public int Width => width;
    public int Height => height;
    public int Length => length;

    public void ClearNodes() {
        NodeCubes.Clear();
        NodesOnGrid = null;
    }
    public void CreateGrid() {
        ClearNodes();
        NodesOnGrid = new NodeRectangle_Gizmo[width, height];
    }

    public void AssignNodeToGrid(int w, int h, Vector3 pos, Vector3 size, Vector3 labelPos, string label, Color color) {
        if (NodesOnGrid == null)
            return;
        NodeRectangle_Gizmo newGizmo = new NodeRectangle_Gizmo(w, h, pos, size, labelPos, label, color);
        NodeCubes.Add(newGizmo);
        NodesOnGrid[w, h] = newGizmo;
    }

    public NodeRectangle_Gizmo[,] GetNodesOnGrid() {
        if (NodeCubes == null || NodeCubes.Count == 0)
            return null;
        if (NodesOnGrid == null && NodeCubes != null && NodeCubes.Count > 0) {
            foreach (NodeRectangle_Gizmo cube in NodeCubes)
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
        this.length = length;
    }
#endif
    #endregion

}
