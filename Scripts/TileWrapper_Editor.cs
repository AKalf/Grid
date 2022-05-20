using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TileWrapper))]
public class TileWrapper_Editor : Editor {

    TileWrapper tile = null;

    private void OnEnable() {
        tile = (TileWrapper)target;
    }
    public override void OnInspectorGUI() {
        TileType newType = (TileType)EditorGUILayout.EnumPopup("Tyle type: ", tile.TileType);
        if (newType != tile.TileType) {
            tile.TileType = newType;
        }
    }
}
