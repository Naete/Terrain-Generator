using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NKStudios.ProceduralAlgorithm;

namespace NKStudios.Editor {
    public class DiamondSquareEditor : EditorWindow
    {
        private static Gradient _gradient;

        private float[,] _heightMap = null;
        private int _seed = 0;
        private int _mapSize = 0;
        private static int _noiseIntensity = 5;
        private static int _exponentialMapSize = 4;
        
        private int _gaussRepeatTime = 1;

        private static GameObject _parentPrefab;
        private GameObject _parent;
        private static Mesh _mesh;
        
        private static GameObject _cubePrefab;
        private static Material _grassMat;
        private static Material _sandMat;
        private static Material _snowMat;
        private static Material _stoneMat;
        private static Material _terrainShaderMaterial;

        [MenuItem("NKStudios/Procedural Algorithm/Diamond Square")]
        public static void ShowWindow() {
            _grassMat = AssetDatabase.LoadAssetAtPath("Assets/_materials/Grass.mat", typeof(Material)) as Material;
            _sandMat = AssetDatabase.LoadAssetAtPath("Assets/_materials/Sand.mat", typeof(Material)) as Material;
            _snowMat = AssetDatabase.LoadAssetAtPath("Assets/_materials/Stone.mat", typeof(Material)) as Material;
            _stoneMat = AssetDatabase.LoadAssetAtPath("Assets/_materials/Stone.mat", typeof(Material)) as Material;
            _terrainShaderMaterial = AssetDatabase.LoadAssetAtPath("Assets/_materials/TerrainShaderColors.mat", typeof(Material)) as Material;

            _mesh = new Mesh();
            
            _cubePrefab = AssetDatabase.LoadAssetAtPath("Assets/_prefabs/Cube.prefab", typeof(GameObject)) as GameObject;
            
            _parentPrefab = AssetDatabase.LoadAssetAtPath("Assets/_prefabs/Terrain.prefab", typeof(GameObject)) as GameObject;
            _parentPrefab.GetComponent<MeshRenderer>().material = _terrainShaderMaterial;
            _parentPrefab.GetComponent<MeshFilter>().mesh = _mesh;
            
            _gradient = new Gradient();
            
            // Opens window
            GetWindow(typeof(DiamondSquareEditor), true, "Diamond Square Terrain Generator");
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("Diamond Square", EditorStyles.boldLabel);

            _gradient = EditorGUILayout.GradientField("Gradient", _gradient);
            _seed = EditorGUILayout.IntField("Seed", _seed);
            _exponentialMapSize = EditorGUILayout.IntSlider("Map Size (n-Factor)", _exponentialMapSize, 0, 10);
            _gaussRepeatTime = EditorGUILayout.IntSlider("Gauss Apply count", _gaussRepeatTime, 0, 20);
            _noiseIntensity = EditorGUILayout.IntField("Noise Intensity", _noiseIntensity);

            if (GUILayout.Button("Generate HeightMap")) {
                _heightMap = DiamondSquare.GenerateHeightMap(_exponentialMapSize, _noiseIntensity, _seed);
                _mapSize = _heightMap.GetLength(0) - 1;
            }

            if (GUILayout.Button("Apply Blur"))
                _heightMap = GaussianBlur.ApplyBlur(_heightMap, _gaussRepeatTime);
            
            if (GUILayout.Button("Generate Mesh"))
                GenerateMesh();

            if (GUILayout.Button("Generate Terrain"))
                GenerateTerrain();

            if (GUILayout.Button("Delete Grid"))
                DeleteGrid();
            
            if (GUILayout.Button("Add Octave"))
                AddOctave();
        }
        
        private void GenerateMesh() 
        {
            DeleteGrid();

            var verticesList = new List<Vector3>();
            var trianglesList = new List<int>();
            var colorList = new List<Color>();
            float minTerrainHeight = 0;
            float maxTerrainHeight = 0;

            #region New
            int scale = 0;
            for (int depth = 0; depth < _mapSize - 1; depth++) {
                for (int width = 0; width < _mapSize - 1; width++) 
                {
                    for (int z = depth; z < depth + 2; z++) {
                        for (int x = width; x < width + 2; x++)
                        {
                            float y = _heightMap[x, z];
                            verticesList.Add(new Vector3(x, y, z));
                            
                            // For gradients min and max value
                            if (y > maxTerrainHeight)
                                maxTerrainHeight = y;
                            if (y < minTerrainHeight)
                                minTerrainHeight = y;
                        }
                    }
                    
                    trianglesList.Add(0 + scale);
                    trianglesList.Add(2 + scale);
                    trianglesList.Add(3 + scale);
                    trianglesList.Add(0 + scale);
                    trianglesList.Add(3 + scale);
                    trianglesList.Add(1 + scale);

                    scale += 4;
                }
            }
            #endregion

            #region Old
            // for (int z = 0, i = 0; z <= _mapSize; z++) {
            //     for (int x = 0; x <= _mapSize; x++) {
            //         verticesList[i] = new Vector3(x, _heightMap[x, z], z);
            //         i++;
            //     }
            // }
            //
            // int vert = 0;
            // int tris = 0;
            // for (int z = 0; z < _mapSize; z++) {
            //     for (int i = 0; i < _mapSize; i++) {
            //         trianglesList[tris + 0] = vert + 0;
            //         trianglesList[tris + 1] = vert + _mapSize + 1;
            //         trianglesList[tris + 2] = vert + 1;
            //         trianglesList[tris + 3] = vert + 1;
            //         trianglesList[tris + 4] = vert + _mapSize + 1;
            //         trianglesList[tris + 5] = vert + _mapSize + 2;
            //
            //         vert++;
            //         tris += 6;
            //     }
            //     vert++;
            // }
            #endregion

            // Coloring mesh
            foreach (Vector3 vertex in verticesList)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertex.y);
                colorList.Add(_gradient.Evaluate(height));
            }
            
            _mesh.Clear();
            _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _mesh.vertices = verticesList.ToArray();
            _mesh.triangles = trianglesList.ToArray();
            _mesh.colors = colorList.ToArray();
            
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            _mesh.RecalculateTangents();

            _parent = Instantiate(_parentPrefab);
        }
        
        private void GenerateTerrain() 
        {
            DeleteGrid();

            _parent = new GameObject("Voxel-Terrain");
            
            for (int z = 0; z < _mapSize; z++) {
                for (int x = 0; x < _mapSize; x++) {
                    int y = (int)_heightMap[x, z];
                    
                    GameObject obj = Instantiate(_cubePrefab, new Vector3(x, y, z), Quaternion.identity);

                    if (obj.transform.position.y > 40)
                        obj.GetComponent<MeshRenderer>().material = _snowMat;
                    else if (obj.transform.position.y < 9)
                        obj.GetComponent<MeshRenderer>().material = _sandMat;
                    else if (obj.transform.position.y >= 9)
                        obj.GetComponent<MeshRenderer>().material = _grassMat;

                    obj.transform.SetParent(_parent.transform);
                }
            }
        }

        private void DeleteGrid() 
        {
            if (_heightMap == null) 
                throw new NullReferenceException("No height map instance existing");
            else if (_parent != null)
                DestroyImmediate(_parent);
        }
        
        private void AddOctave()
        {
            float [,] secondOctave = DiamondSquare.GenerateHeightMap(_exponentialMapSize, _noiseIntensity, _seed);

            for (int z = 0; z < _mapSize; z++) {
                for (int x = 0; x < _mapSize; x++)
                {
                    _heightMap[x, z] += secondOctave[x, z];
                }
            }
        }
    }
}