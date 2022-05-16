using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TTile> {
    [SerializeField]
    public List<Pathfinding.INode<TTile>> NodeCubes = new List<Pathfinding.INode<TTile>>();
    [SerializeField, HideInInspector]
    private List<GameObject> gridObjects = new List<GameObject>();
    [SerializeField] private Transform nodesParent = null;
    [SerializeField] private Sprite backgroundSprite, borderSprite;
    [SerializeField, HideInInspector]
    private int width = 0, height = 0, length = 0;

    private Pathfinding.INode<TTile>[,] nodesOnGrid = null;
    public Pathfinding.INode<TTile>[,] NodesOnGrid { get => nodesOnGrid; private set => nodesOnGrid = value; }


    public int Width => width;
    public int Height => height;
    public int Length => length;

    public void ClearNodes() {

        NodeCubes.Clear();
        NodesOnGrid = null;
    }
    public void CreateGrid() {

        ClearNodes();
        foreach (var node in gridObjects)
            MonoBehaviour.DestroyImmediate(node);
        gridObjects.Clear();
        NodesOnGrid = new Pathfinding.INode<TTile>[width, height];
    }

    public void AssignNodeToGrid(int w, int h, Vector3 pos, Vector3 size, Vector3 labelPos, string label, Color color) {
        if (NodesOnGrid == null)
            return;
        Pathfinding.INode<TTile> newGizmo = new NodeRectangle_Gizmo(w, h, pos, size, labelPos, label, color);
        newGizmo.CanBeNavigated = true;
        NodeCubes.Add(newGizmo);
        NodesOnGrid[w, h] = newGizmo;
        GameObject newGamboject = new GameObject();
        newGamboject.name = "X: " + w + " H: " + h;
        newGamboject.transform.position = transform.position + new Vector3(w * size.x, h * size.y, 0);
        newGamboject.transform.parent = nodesParent;
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
