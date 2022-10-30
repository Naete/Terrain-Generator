using UnityEngine;
using UnityEditor;
using NKStudios.VoxelMeshGeneration;
using System.Collections.Generic;
using NKStudios.ProceduralAlgorithm;
using SXNoise;

public class ExperimentalSyntaxScript : EditorWindow
{
    [MenuItem("NKStudios/Experimental")]
    static void ShowWindow() {
        GetWindow(typeof(ExperimentalSyntaxScript), true);
    }

    private void OnGUI() {
        if (GUILayout.Button("Experiment")) {
            // Put experimental code in here ...
            
        }
    }
}