using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSpace : MonoBehaviour {
    [SerializeField] public Vector2Int GridSize, GridTileSize;
    [SerializeField] public Line[] Entrances = null, Exits = null;
    [SerializeField] public List<GridTile> Tiles = new List<GridTile>();
    [SerializeField] public Sprite BoarderSprite = null, BackgroundSprite = null;
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
