using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGizmos : MonoBehaviour {
    //public void AssignNodeToGrid(int w, int h, Vector3 pos, Vector3 size, Vector3 labelPos, string label, Color color) {
    //    if (NodesOnGrid == null)
    //        return;
    //    Pathfinding.INode<TTile> newGizmo = NodesOnGrid[0].GetNewNode(new Vector3(w, h, 0), pos, size, labelPos, label, color);
    //    newGizmo.CanBeNavigated = true;
    //    NodeCubes.Add(newGizmo);
    //    NodesOnGrid[w, h] = newGizmo;
    //    GameObject newGamboject = new GameObject();
    //    newGamboject.name = "X: " + w + " H: " + h;
    //    newGamboject.transform.position = transform.position + new Vector3(w * size.x, h * size.y, 0);
    //    newGamboject.transform.parent = nodesParent;
    //    gridObjects.Add(newGamboject);
    //    SpriteRenderer renderer = newGamboject.AddComponent<SpriteRenderer>();
    //    if (w == 0 || h == 0 || w == NodesOnGrid.GetLength(0) - 1 || h == NodesOnGrid.GetLength(1) - 1 || (w == 5 && h != 2)) {
    //        newGizmo.CanBeNavigated = false;
    //        renderer.sprite = backgroundSprite;
    //        renderer.color = Color.black;
    //        newGamboject.AddComponent<PolygonCollider2D>();
    //    }
    //    else
    //        renderer.sprite = backgroundSprite;

    //}
}
