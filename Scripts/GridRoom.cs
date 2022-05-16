using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridRoom : MonoBehaviour {
    [SerializeField] public int roomLength = 1, roomHeight = 1;
    [SerializeField] public Line[] entrances = null, exits = null;

    private BoundsInt thisCollider = new BoundsInt();

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
