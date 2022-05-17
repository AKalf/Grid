using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSpace : MonoBehaviour {
    [SerializeField] public int GridWidth = 10, GridHeight = 10, TileLength = 2;
    [SerializeField] public Line[] Entrances = null, Exits = null;

    [SerializeField] public Grid<GridTile> Grid = new Grid<GridTile>(10, 10, 2);

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    [Serializable]
    public struct Line {
        public Vector3 start, end;

        public Line(Vector3 start, Vector3 end) {
            this.start = start;
            this.end = end;
        }
    }
}
