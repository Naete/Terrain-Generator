using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NKStudios.ProceduralAlgorithm;

[CustomEditor(typeof(DiamondSquare))]
public class DiamondSquareInspectorEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Label("Diamond Square Algorithm");

        if (GUILayout.Button("Generate Terrain")) {
            Debug.Log("TODO: Add \"Execute Algorithm\"");
        }
    }
}