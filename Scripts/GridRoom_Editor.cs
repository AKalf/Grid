using System;
using UnityEngine;
using UnityEditor;
using static GridSpace;
using System.Collections.Generic;

[CustomEditor(typeof(GridSpace))]
public class GridRoom_Editor : Editor {

    private GridSpace gridRoom = null;
    private Transform gridTransform = null;
    private List<Transform> generatedRoomObjects = new List<Transform>();

    private List<Vector3> verticiesList = new List<Vector3>(), normalsList = new List<Vector3>();
    private List<int> trianglesList = new List<int>();
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (gridRoom == null) {
            gridRoom = (GridSpace)target;
            gridTransform = gridRoom.transform;
        }
        if (GUILayout.Button("Create new room")) {
            generatedRoomObjects.Clear();
            gridTransform.localScale = new Vector3(gridRoom.roomLength, gridRoom.roomHeight, 1);
            GameObject newG = new GameObject();
            PolyMesh(newG, 20, 50);
        }
    }

    protected virtual void OnSceneGUI() {
        if (gridRoom == null)
            return;
        Color original = Handles.color;
        if (gridRoom.entrances != null) {
            Handles.color = Color.green;
            for (int i = 0; i < gridRoom.entrances.Length; i++) {
                Line line = gridRoom.entrances[i];
                Handles.color = Color.magenta;
                Vector3 newStartVec = Handles.FreeMoveHandle(line.start, Quaternion.identity, 1, Vector3.one, Handles.SphereHandleCap);
                Vector3 newEndVec = Handles.FreeMoveHandle(line.end, Quaternion.identity, 1, Vector3.one, Handles.SphereHandleCap);
                Handles.color = Color.green;
                Handles.DrawLine(newStartVec, newEndVec);
                if (newStartVec != line.start || newEndVec != line.end)
                    gridRoom.entrances[i] = new Line(
                        new Vector3Int((int)newStartVec.x, (int)newStartVec.y, (int)newStartVec.z),
                        new Vector3Int((int)newEndVec.x, (int)newEndVec.y, (int)newEndVec.z)
                    );
            }
        }
        if (verticiesList.Count > 0) {
            int index = 0;
            Handles.color = Color.blue;
            for (int i = 0; i < this.verticiesList.Count; i++) {
                Vector3 vertex = this.verticiesList[i];
                Handles.DrawWireCube(vertex, Vector3.one / 2);
                Handles.Label(vertex, i.ToString());
                index++;

                if (index > 5) {
                    Handles.color = Color.blue;
                    index = 0;
                }
                if (index > 2)
                    Handles.color = Color.red;
            }
        }
        //if (trianglesList.Count > 0)
        Handles.color = original;

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    void PolyMesh(GameObject newGameobject, float radius, int n) {

        MeshFilter mf = newGameobject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;


        float outerX, innerX, outerY, innerY;
        for (int i = 0; i < n; i++) {
            outerX = radius * Mathf.Sin(2 * Mathf.PI * i / n);
            outerY = radius * Mathf.Cos(2 * Mathf.PI * i / n);
            verticiesList.Add(new Vector3(outerX, outerY, 0f));
        }
        for (int i = 0; i < n; i++) {

            outerX = (radius + 1) * Mathf.Sin(2 * Mathf.PI * i / n);
            outerY = (radius + 1) * Mathf.Cos(2 * Mathf.PI * i / n);
            verticiesList.Add(new Vector3(outerX, outerY, 0f));
        }


        mesh.SetVertices(verticiesList);
        n = verticiesList.Count;

        for (int i = 0; i < n; i++) {
            trianglesList.Add(i);
            if (i + 1 >= n)
                trianglesList.Add(i + 1 - n);
            else
                trianglesList.Add(i + 1);
            if (i + 2 >= n)
                trianglesList.Add(i + 2 - n);
            else
                trianglesList.Add(i + 2);
        }

        mesh.SetTriangles(trianglesList.ToArray(), 0);

        for (int i = 0; i < n; i++) {
            normalsList.Add(-Vector3.forward);
        }
        mesh.SetNormals(normalsList);

        PolygonCollider2D polyCollider = newGameobject.AddComponent<PolygonCollider2D>();
        //polyCollider
        polyCollider.pathCount = 1;

        List<Vector2> pathList = new List<Vector2>();
        for (int i = 0; i < n; i++) {
            pathList.Add(new Vector2(verticiesList[i].x, verticiesList[i].y));
        }
        Vector2[] path = pathList.ToArray();

        polyCollider.SetPath(0, path);
        MeshRenderer renderer = newGameobject.AddComponent<MeshRenderer>();

    }


}
