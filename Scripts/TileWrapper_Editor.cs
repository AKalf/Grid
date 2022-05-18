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
        Sprite s = EditorGUILayout.ObjectField(tile.Sprite, typeof(Sprite), false) as Sprite;
        if (s != tile.Sprite) {
            tile.Sprite = s;
            tile.gameObject.GetComponent<SpriteRenderer>().sprite = tile.Sprite;
        }

        tile.TyleType = (GridTile.TileType)EditorGUILayout.EnumPopup("Tyle type: ", tile.TyleType);
        tile.CanBeNavigated = EditorGUILayout.Toggle("Can be navigated: ", tile.CanBeNavigated);

    }
}
