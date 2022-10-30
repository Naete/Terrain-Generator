using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NKStudios.VoxelMeshGeneration;
using NKStudios.ProceduralAlgorithm;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshingTest : MonoBehaviour
{
    private Mesh _mesh;
    private List<Vector3> _vertexesList;
    private List<int> _triangleList;

    private readonly Vector3Int[] _directions = {
        new Vector3Int(0, 0, 1), // North
        new Vector3Int(1, 0, 0), // East
        new Vector3Int(0, 0, -1), // South
        new Vector3Int(-1, 0, 0), // West
        new Vector3Int(0, 1, 0), // Up
        //new Vector3Int(0, -1, 0), // Down
    };

    private float[,] _heightMap;
    [SerializeField] private int gaussRepeatAmount = 1;
    
    private void Initialize() {
        _mesh = new Mesh();
        _vertexesList = new List<Vector3>();
        _triangleList = new List<int>();
        this.GetComponent<MeshFilter>().mesh = _mesh;
        
        _heightMap = DiamondSquare.GenerateHeightMap(8, 34);
        _heightMap = GaussianBlur.ApplyBlur(_heightMap, gaussRepeatAmount);
    }

    private void Start() {
        Initialize();
        
        #region SecondTestWithChunkClass
        var chunk1 = new Chunk<Voxel>(_heightMap.GetLength(0), 70, new Vector3(0,0,0));
        
        //Add voxels
        for (int z = 0; z < chunk1.Width; z++) {
            for (int x = 0; x < chunk1.Width; x++) {
                // Step 1: Create voxel and add it to a chunk
                int y = Mathf.Abs((int)(_heightMap[x, z]));
                Voxel voxel = new Voxel(x, y, z, BlockType.Grass);
                chunk1.AddAtPos(voxel, voxel.Position);
            }
        }
        
        int scale = 0;
        foreach (Voxel voxel in chunk1) {
            
            if (voxel.Type != BlockType.Air) {
                
                // Step 2: Get vertices of each voxel
                for (int i = 0; i < voxel.vertices.Length; i++) {
                    _vertexesList.Add(voxel.vertices[i]);
                }

                // Step 3: Get triangles of each voxel
                for (int direction = 0; direction < _directions.Length; direction++) {
                    if (DoesNeighbourExistOf(voxel.Position, direction, chunk1)) {
                        for (int i = 0; i < 6; i++) {
                            _triangleList.Add(voxel.faces[direction, i] + scale);
                        }
                    }
                }
                scale += 24; // 24 Vertexes per voxel
            }
        }
        #endregion

        UpdateMesh(_vertexesList, _triangleList);
    }
    
    // TODO: Needs clean approach.
    private bool DoesNeighbourExistOf(Vector3Int currentPosition, int direction, Chunk<Voxel> chunk) {
        Vector3Int neigbhourPos = currentPosition + _directions[direction];

        if (CheckIfOutsideBoundaries(neigbhourPos, chunk))
            return false;
        // If neighbour is an air block -> draw face
        else if (chunk.GetValueAtPos(neigbhourPos).Type == BlockType.Air) {
            #region Workaround. TODO: Find better solution
            Vector3Int aboveNeigbourPos = neigbhourPos + _directions[4];

            if (CheckIfOutsideBoundaries(aboveNeigbourPos, chunk)) {
                return true;
            }
            else if (chunk.GetValueAtPos(aboveNeigbourPos).Type == BlockType.Air)
                return true;
            #endregion
        }

        return false;
    }

    // TODO: Remove chunk as parameter if possible.
    private bool CheckIfOutsideBoundaries(Vector3Int positiontToCheck, Chunk<Voxel> chunk) {
        return positiontToCheck.x < 0 || positiontToCheck.x >= chunk.Width ||
               positiontToCheck.y < 0 || positiontToCheck.y >= chunk.Height ||
               positiontToCheck.z < 0 || positiontToCheck.z >= chunk.Width;
    }

    private void ExecuteGreedyMeshing(Voxel[,] chunk, int direction) {
        
        bool[,] isVisited = new bool[chunk.GetLength(0), chunk.GetLength(1)]; // Stores TRUE/FALSE at coordinates of visited voxels
        
        for (int z = 0; z < chunk.GetLength(0); z++) {
            for (int x = 0; x < chunk.GetLength(1); x++) {
                
                // Check if voxel at [z, x] is NOT visited
                if (!isVisited[z, x]) {
                    
                    Voxel start = chunk[z, x];
                    Voxel end = start;
                    
                    // Run along x-axis of the current voxel
                    for (int xPos = x + 1; xPos < chunk.GetLength(1); xPos++) {
                        
                        Voxel nextVoxel = chunk[z, xPos]; // Get the next voxel to check
                        
                        // Check if startVoxel and nextVoxel NOT have same height or type, ...
                        if (start.Position.y != nextVoxel.Position.y || start.Type != nextVoxel.Type) {
                            // ... when TRUE: Stop loop on x-axis 
                            break;
                        } else {
                            // ... when FALSE: Assign the new voxel as visited
                            isVisited[z, xPos] = true; // Voxel at [z, x] is visited
                            
                            end = nextVoxel;
                        }
                    }
                    
                    for (int zPos = z + 1; zPos < chunk.GetLength(0); zPos++) {
                        
                        Voxel nextVoxel = chunk[zPos, start.Position.x];
                        
                        bool isRowUsable = true;
                        
                        for (int xPos = start.Position.x; xPos <= end.Position.x; xPos++) {
                            
                            nextVoxel = chunk[zPos, xPos];
                            
                            if (start.Position.y != nextVoxel.Position.y || start.Type != nextVoxel.Type) {
                                isRowUsable = false;
                                break;
                            }
                        }
                        
                        if (!isRowUsable) {
                            break;
                        }
                        
                        for (int xPos = 0; xPos <= end.Position.x; xPos++) {
                            isVisited[zPos, xPos] = true;
                        }
                        
                        end = nextVoxel;
                    }
                    
                    Debug.Log("Start: " + start.Position + ", End: " + end.Position);
                    
                    int botLeft = (start.Position.x + start.Position.z * chunk.GetLength(0));
                    int topRight = (end.Position.x + end.Position.z * chunk.GetLength(0));
                    int botRight = botLeft + (end.Position.x - start.Position.x);
                    int topLeft = topRight - (end.Position.x - start.Position.x);

                    botLeft *= 24;
                    topRight *= 24;
                    botRight *= 24;
                    topLeft *= 24;
                    
                    Debug.Log("BotLeft: " + botLeft + ", BotRight: " + botRight + ", TopLeft: " + topLeft + ", TopRight: " + topRight);
                    
                    _triangleList.Add(start.faces[direction, 0] + botLeft);
                    _triangleList.Add(start.faces[direction, 1] + topLeft);
                    _triangleList.Add(end.faces[direction, 2] + topRight);
                    
                    _triangleList.Add(start.faces[direction, 0] + botLeft);
                    _triangleList.Add(end.faces[direction, 2] + topRight);
                    _triangleList.Add(end.faces[direction, 5] + botRight);
                }
            }
        }
    }
    
    private void UpdateMesh(List<Vector3> vertexesList, List<int> triangleList) {
        _mesh.Clear();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        _mesh.vertices = vertexesList.ToArray();
        _mesh.triangles = triangleList.ToArray();

        _mesh.Optimize();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }
}
