using System;
using UnityEngine;
using UnityEditor;
using static GridRoom;
using System.Collections.Generic;

[CustomEditor(typeof(GridRoom))]
public class GridRoom_Editor : Editor {

    private GridRoom gridRoom = null;
    private Transform gridTransform = null;
    private List<Transform> generatedRoomObjects = new List<Transform>();
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (gridRoom == null) {
            gridRoom = (GridRoom)target;
            gridTransform = gridRoom.transform;
        }
        if (GUILayout.Button("Build outer walls")) {
            generatedRoomObjects.Clear();
            gridTransform.localScale = new Vector3(gridRoom.roomLength, gridRoom.roomHeight, 1);
            GameObject newG = new GameObject();
            PolyMesh(newG, 20, 30);
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

        Handles.color = original;
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }

    void PolyMesh(GameObject newGameobject, float radius, int n) {

        MeshFilter mf = newGameobject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        //verticies
        List<Vector3> verticiesList = new List<Vector3> { };
        float outerX = 0, innerX, outerY = 0, innerY;
        for (int i = 0; i < n; i++) {
            outerX = radius * Mathf.Sin((2 * Mathf.PI * i) / n);
            outerY = radius * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticiesList.Add(new Vector3(outerX, outerY, 0f));
        }

        outerX = radius * Mathf.Sin((2 * Mathf.PI * 0) / n);
        outerY = radius * Mathf.Cos((2 * Mathf.PI * 0) / n);
        verticiesList.Add(new Vector3(outerX, outerY, 0f));

        for (int i = 0; i < n; i++) {
            innerX = (radius - 2) * Mathf.Sin((2 * Mathf.PI * i) / n);
            innerY = (radius - 2) * Mathf.Cos((2 * Mathf.PI * i) / n);
            verticiesList.Add(new Vector3(innerX, innerY, 0f));
        }

        innerX = (radius - 2) * Mathf.Sin((2 * Mathf.PI * 0) / n);
        innerY = (radius - 2) * Mathf.Cos((2 * Mathf.PI * 0) / n);
        verticiesList.Add(new Vector3(innerX, innerY, 0f));

        Vector3[] verticies = verticiesList.ToArray();
        mesh.SetVertices(verticiesList);
        n = n + n + 2;
        //triangles
        List<int> trianglesList = new List<int> { };
        for (int i = 0; i < (n - 2); i++) {
            trianglesList.Add(i);
            trianglesList.Add(i + 1);
            trianglesList.Add(i + 2);
        }

        mesh.SetTriangles(trianglesList.ToArray(), 0);

        ////normals
        List<Vector3> normalsList = new List<Vector3> { };
        for (int i = 0; i < n; i++) {
            normalsList.Add(-Vector3.forward);
        }
        mesh.SetNormals(normalsList);

        PolygonCollider2D polyCollider = newGameobject.AddComponent<PolygonCollider2D>();
        //polyCollider
        polyCollider.pathCount = 1;

        List<Vector2> pathList = new List<Vector2> { };
        for (int i = 0; i < n; i++) {
            pathList.Add(new Vector2(verticiesList[i].x, verticiesList[i].y));
        }
        Vector2[] path = pathList.ToArray();

        polyCollider.SetPath(0, path);
        MeshRenderer renderer = newGameobject.AddComponent<MeshRenderer>();

    }
}
